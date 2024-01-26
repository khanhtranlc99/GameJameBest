using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Shop;
using UnityEngine;

namespace Game.Weapons
{
	public abstract class Weapon : MonoBehaviour
	{
		public delegate void AttackEvent(Weapon weapon);

		public delegate void DamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f);

		public bool DebugLog;

		public WeaponArchetype Archetype;

		public WeaponTypes Type;

		public AmmoTypes AmmoType;

		public WeaponNameList WeaponNameInList;

		public string Name;

		public float AttackDistance;

		public float Damage;

		public DamageType WeaponDamageType = DamageType.Bullet;

		public float DefenceIgnorence;

		public float AttackDelay;

		public AudioClip SoundAttack;

		public AudioClip SoundHit;

		public Sprite image;

		public bool Unique;

		public GameObject[] WeaponObjects;

		public Vector2 AimOffset;

		[SerializeField]
		protected float lastAttackTime;

		public AttackEvent PerformAttackEvent;

		public DamageEvent InflictDamageEvent;

		public HitEntity WeaponOwner
		{
			get;
			protected set;
		}

		public bool IsOnCooldown => Time.time - lastAttackTime < AttackDelay;

		protected virtual void Start()
		{
			if (AttackDistance <= 0f)
			{
				if (Archetype == WeaponArchetype.Melee)
				{
					AttackDistance = 1f;
				}
				if (Archetype == WeaponArchetype.Ranged || Archetype == WeaponArchetype.Grenade)
				{
					AttackDistance = CameraManager.Instance.UnityCamera.farClipPlane;
				}
			}
			if (Damage <= 0f)
			{
				Damage = 1f;
			}
			if (AttackDelay <= 0f)
			{
				AttackDelay = 1f;
			}
		}

		public virtual void Init()
		{
			WeaponOwner = GetComponentInParent<HitEntity>();
			BodyPartDamageReciever bodyPartDamageReciever = WeaponOwner as BodyPartDamageReciever;
			if (bodyPartDamageReciever != null)
			{
				WeaponOwner = bodyPartDamageReciever.RerouteEntity;
			}
		}

		public virtual void DeInit()
		{
		}

		public void SetWeaponOwner(HitEntity newOwner)
		{
			WeaponOwner = newOwner;
		}

		public abstract void Attack(HitEntity owner);

		public abstract void Attack(HitEntity owner, Vector3 direction);

		public abstract void Attack(HitEntity owner, HitEntity victim);

		protected void PlaySound(AudioSource audioSource, AudioClip audioClip)
		{
			if (!(audioClip == null) && !(audioSource == null))
			{
				audioSource.PlayOneShot(audioClip);
			}
		}

		public void PlayAttackSound(AudioSource audioSource)
		{
			PlaySound(audioSource, SoundAttack);
		}

		public void PlayHitSound(Vector3 hitPosition)
		{
			PointSoundManager.Instance.Play3DSoundOnPoint(hitPosition, SoundHit);
		}
	}
}
