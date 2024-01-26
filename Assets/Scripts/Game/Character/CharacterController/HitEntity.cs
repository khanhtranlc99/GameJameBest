using Game.Character.Extras;
using Game.Character.Stats;
using Game.GlobalComponent.Qwest;
using Game.Shop;
using UI.DailyMission;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class HitEntity : MonoBehaviour
	{
		public delegate void ApplyDamage(float damage, HitEntity owner);

		public delegate void AliveStateChagedEvent();

		public delegate void HealthChagedEvent(HitEntity disturber);

		public Faction Faction;

		public Collider MainCollider;

		public HitEffectType HitEffect;

		public bool Immortal;

		[HideInInspector]
		public Vector3 LastHitVector;

		[HideInInspector]
		public float LastDamage;

		[HideInInspector]
		public HitEntity LastHitOwner;

		[HideInInspector]
		public DamageType LastDamageType;

		[HideInInspector]
		public bool IsInWater;

		public CharacterStat Health = new CharacterStat
		{
			Name = "Health",
			Max = 100f,
			RegenPerSecond = 0f
		};

		[HideInInspector]
		protected bool Dead;

		public WeaponNameList KilledByAbillity = WeaponNameList.None;

		public Defence Defence;

		public float ExperienceForAKill = 100f;

		[SerializeField]
		private Vector3 NpcShootVectorOffset = new Vector3(0f, 0f, 0f);

		public HealthChagedEvent OnHitEvent;

		public AliveStateChagedEvent DiedEvent;

		public AliveStateChagedEvent ResurrectedEvent;

		public bool IsDebug;

		public Vector3 NPCShootVectorOffset
		{
			get
			{
				return base.transform.right * NpcShootVectorOffset.x + base.transform.up * NpcShootVectorOffset.y + base.transform.forward * NpcShootVectorOffset.z;
			}
			set
			{
				NpcShootVectorOffset = value;
			}
		}

		public bool IsDead => Dead;

		public event ApplyDamage DamageEvent;

		protected virtual void Awake()
		{
			Defence.Init();
		}

		protected virtual void Start()
		{
		}

		public virtual void Initialization(bool setUpHealth = true)
		{
			Dead = false;
			DiedEvent = null;
			if (setUpHealth)
			{
				Health.Setup();
			}
			EntityManager.Instance.Register(this);
			LastDamageType = DamageType.Instant;
		}

		public virtual void Drowning(float waterHeight, float drowningDamageCounter = 1f)
		{
		}

		protected virtual void Update()
		{
		}

		protected virtual void FixedUpdate()
		{
		}

		public virtual void Resurrect()
		{
			if (Dead)
			{
				Health.Setup();
				Dead = false;
				EntityManager.Instance.Register(this);
				if (ResurrectedEvent != null)
				{
					ResurrectedEvent();
				}
			}
		}

		public virtual void Die()
		{
			OnHit(DamageType.Instant, null, Health.Current * 2f, Vector3.zero, Vector3.zero, 0f);
		}

		public virtual void Die(HitEntity lastHitOwner)
		{
			OnHit(DamageType.Instant, lastHitOwner, Health.Current * 2f, Vector3.zero, Vector3.zero, 0f);
		}

		public bool DeadByDamage(float damage)
		{
			if (Health.Current <= damage)
			{
				return true;
			}
			return false;
		}

		public virtual void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (Dead)
			{
				return;
			}
			float value = Defence.GetValue(damageType);
			value = ((!(value <= defenceReduction)) ? (value - defenceReduction) : 0f);
			damage -= damage * value;
			LastHitVector = hitVector;
			LastDamage = damage;
			LastHitOwner = owner;
			LastDamageType = damageType;
			if (!Immortal)
			{
				Health.Change(0f - damage);
				if (OnHitEvent != null)
				{
					OnHitEvent(owner);
				}
				if (Health.Current <= 0f)
				{
					OnDie();
				}
				if (this.DamageEvent != null)
				{
					this.DamageEvent(damage, owner);
				}
			}
			SpecifficEffect(hitPos);
		}

		protected virtual void SpecifficEffect(Vector3 hitPos)
		{
			if (HitEffect == HitEffectType.Blood)
			{
				BloodHitEffect.Instance.Emit(hitPos);
			}
			else if (HitEffect == HitEffectType.Sparks)
			{
				SparksHitEffect.Instance.Emit(hitPos);
			}
		}

		public virtual bool OnHealthPickUp(float hp)
		{
			if (!Dead && Health.Current < Health.Max)
			{
				Health.Change(hp);
			}
			return false;
		}

		public virtual void OnDieEventCaller()
		{
			OnDie();
		}

		protected virtual void OnDie()
		{
			if (!Dead)
			{
				Dead = true;
				EntityManager.Instance.OnDeath(this);
				if (DiedEvent != null)
				{
					DiedEvent();
				}
				GameEventManager.Instance.Event.NpcKilledEvent(base.transform.position, Faction, this, LastHitOwner);
			}
		}
	}
}
