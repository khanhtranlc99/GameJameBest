using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using System;
using UnityEngine;

namespace Game.Enemy
{
	public class HumanoidStatusNPC : BaseStatusNPC
	{
		public const float DeductedHpForDrowning = 12f;

		private const float VelocityTreshold = 30f;

		private const float VelocityReduction = 5f;

		private const string HipsPath = "metarig/hips";

		private static int SmallDynamicLayerNumber = -1;

		[Separator("Humanoid NPC Specific Parametrs")]
		public CharacterWaterSensor WaterSensor;

		public bool Ragdollable = true;

		public GameObject Ragdoll;

		public float RagdollDestroyTime;

		public RagdollWakeUper RagdollWakeUper;

		public bool IsTransformer;

		[HideInInspector]
		public float LastWaterHeight;

		private GameObject currentRagdoll;

		private RagdollWakeUper currentWakeUper;

		private Vector3 ragdollStartVelocity;

		public override void Init()
		{
			base.Init();
			WaterSensor.Init(this);
		}

		public override void DeInit()
		{
			base.DeInit();
			currentRagdoll = null;
			currentWakeUper = null;
			IsInWater = false;
		}

		protected override void Awake()
		{
			base.Awake();
			if (SmallDynamicLayerNumber == -1)
			{
				SmallDynamicLayerNumber = LayerMask.NameToLayer("SmallDynamic");
			}
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if (owner != null && owner.Faction == Faction.Player)
			{
				FactionsManager.Instance.PlayerAttackHuman(this);
			}
			base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
		}

		public override void Drowning(float waterHight, float drowningDamageCounter = 1f)
		{
			IsInWater = true;
			OnHit(DamageType.Water, null, 12f * drowningDamageCounter, Vector3.zero, Vector3.zero, 0f);
			LastWaterHeight = waterHight;
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp)
		{
			GameObject _;
			ReplaceOnRagdoll(canWakeUp, out _);
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp, Vector3 forceVector)
		{
			GameObject initedRagdoll = null;
			ReplaceOnRagdoll(canWakeUp, out initedRagdoll);
			Transform transform = null;
			if ((bool)initedRagdoll)
			{
				transform = initedRagdoll.transform.Find("metarig").Find("hips");
			}
			Rigidbody rigidbody = null;
			if ((bool)transform)
			{
				rigidbody = transform.GetComponent<Rigidbody>();
			}
			if ((bool)rigidbody)
			{
				rigidbody.AddForce(forceVector, ForceMode.Impulse);
			}
		}

		public virtual void ReplaceOnRagdoll(float health, out GameObject initedRagdoll)
		{
			bool flag = (health > 0f) ? true : false;
			initedRagdoll = null;
			if (!Ragdoll || currentRagdoll != null)
			{
				return;
			}
			GameObject prefab = Ragdoll;
			if (!flag && !IsTransformer)
			{
				GameObject specificRagdoll = PlayerDieManager.Instance.GetSpecificRagdoll(LastDamageType);
				if (specificRagdoll != null)
				{
					prefab = specificRagdoll;
				}
			}
			if (!currentRagdoll)
			{
				currentRagdoll = PoolManager.Instance.GetFromPool(prefab);
				PoolManager.Instance.AddBeforeReturnEvent(currentRagdoll, delegate
				{
					currentRagdoll = null;
				});
			}
			if (currentRagdoll == null)
			{
				throw new Exception("Current ragdoll for object " + base.gameObject.name + " not inited!");
			}
			currentRagdoll.transform.position = base.transform.position;
			currentRagdoll.transform.rotation = base.transform.rotation;
			ragdollStartVelocity = LastHitVector;
			if (LastDamage > 30f)
			{
				LastDamage = 30f;
			}
			ragdollStartVelocity *= LastDamage / 5f;
			currentRagdoll.SetActive(value: false);
			CopyTransformRecurse(BaseNPC.RootModel.transform, currentRagdoll);
			currentRagdoll.SetActive(value: true);
			currentRagdoll.transform.parent = null;
			initedRagdoll = currentRagdoll;
			base.gameObject.SetActive(value: false);
			if (IsInWater)
			{
				RagdollDrowning ragdollDrowning = currentRagdoll.AddComponent<RagdollDrowning>();
				ragdollDrowning.Init(GetRagdollHips(), LastWaterHeight);
				PoolManager.Instance.AddBeforeReturnEvent(currentRagdoll, delegate
				{
					UnityEngine.Object.Destroy(ragdollDrowning);
				});
			}
			if (!flag)
			{
				RagdollWakeUper.SetupRagdollMark(GetRagdollHips().gameObject);
				if (RagdollDestroyTime > 0f)
				{
					PoolManager.Instance.ReturnToPoolWithDelay(currentRagdoll, RagdollDestroyTime);
				}
			}
			else if ((bool)RagdollWakeUper)
			{
				currentWakeUper = currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
				if (currentWakeUper == null)
				{
					currentWakeUper = PoolManager.Instance.GetFromPool(RagdollWakeUper.gameObject).GetComponent<RagdollWakeUper>();
					currentWakeUper.transform.parent = GetRagdollHips();
					currentWakeUper.transform.localPosition = Vector3.zero;
					currentWakeUper.transform.localEulerAngles = Vector3.zero;
				}
				currentWakeUper.Init(base.gameObject, Health.Max, Health.Current, Defence, Faction);
			}
		}

		public virtual void ReplaceOnRagdoll(bool canWakeUp, out GameObject initedRagdoll)
		{
			initedRagdoll = null;
			if (!Ragdoll || currentRagdoll != null)
			{
				return;
			}
			GameObject prefab = Ragdoll;
			if (!canWakeUp && !IsTransformer)
			{
				GameObject specificRagdoll = PlayerDieManager.Instance.GetSpecificRagdoll(LastDamageType);
				if (specificRagdoll != null)
				{
					prefab = specificRagdoll;
				}
			}
			if (!currentRagdoll)
			{
				currentRagdoll = PoolManager.Instance.GetFromPool(prefab);
				PoolManager.Instance.AddBeforeReturnEvent(currentRagdoll, delegate
				{
					currentRagdoll = null;
				});
			}
			if (currentRagdoll == null)
			{
				throw new Exception("Current ragdoll for object " + base.gameObject.name + " not inited!");
			}
			currentRagdoll.transform.position = base.transform.position;
			currentRagdoll.transform.rotation = base.transform.rotation;
			ragdollStartVelocity = LastHitVector;
			if (LastDamage > 30f)
			{
				LastDamage = 30f;
			}
			ragdollStartVelocity *= LastDamage / 5f;
			currentRagdoll.SetActive(value: false);
			CopyTransformRecurse(BaseNPC.RootModel.transform, currentRagdoll);
			currentRagdoll.SetActive(value: true);
			currentRagdoll.transform.parent = null;
			initedRagdoll = currentRagdoll;
			base.gameObject.SetActive(value: false);
			if (IsInWater)
			{
				RagdollDrowning ragdollDrowning = currentRagdoll.AddComponent<RagdollDrowning>();
				ragdollDrowning.Init(GetRagdollHips(), LastWaterHeight);
				PoolManager.Instance.AddBeforeReturnEvent(currentRagdoll, delegate
				{
					UnityEngine.Object.Destroy(ragdollDrowning);
				});
			}
			if (!canWakeUp)
			{
				RagdollWakeUper.SetupRagdollMark(GetRagdollHips().gameObject);
				if (RagdollDestroyTime > 0f)
				{
					PoolManager.Instance.ReturnToPoolWithDelay(currentRagdoll, RagdollDestroyTime);
				}
			}
			else if ((bool)RagdollWakeUper)
			{
				currentWakeUper = currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
				if (currentWakeUper == null)
				{
					currentWakeUper = PoolManager.Instance.GetFromPool(RagdollWakeUper.gameObject).GetComponent<RagdollWakeUper>();
					currentWakeUper.transform.parent = GetRagdollHips();
					currentWakeUper.transform.localPosition = Vector3.zero;
					currentWakeUper.transform.localEulerAngles = Vector3.zero;
				}
				currentWakeUper.Init(base.gameObject, Health.Max, Health.Current, Defence, Faction);
			}
		}

		public Transform GetRagdollHips()
		{
			if (!currentRagdoll)
			{
				return null;
			}
			return currentRagdoll.transform.Find("metarig/hips");
		}

		protected override void OnDie()
		{
			if ((bool)LastHitOwner && LastHitOwner.Faction == Faction.Player)
			{
				FactionsManager.Instance.CommitedACrime();
			}
			if (currentRagdoll == null)
			{
				ReplaceOnRagdoll(canWakeUp: false);
			}
			else if (currentWakeUper != null)
			{
				currentWakeUper.DeInitRagdoll(mainObjectDead: true, callOnDieEvent: false);
			}
			base.OnDie();
		}

		protected void CopyTransformRecurse(Transform mainModelTransform, GameObject ragdoll)
		{
			ragdoll.transform.position = mainModelTransform.position;
			ragdoll.transform.rotation = mainModelTransform.rotation;
			ragdoll.transform.localScale = mainModelTransform.localScale;
			ragdoll.layer = SmallDynamicLayerNumber;
			if ((bool)ragdoll.GetComponent<Rigidbody>())
			{
				ragdoll.GetComponent<Rigidbody>().velocity = ragdollStartVelocity;
			}
			foreach (Transform item in ragdoll.transform)
			{
				Transform transform2 = mainModelTransform.Find(item.name);
				if ((bool)transform2)
				{
					CopyTransformRecurse(transform2, item.gameObject);
				}
			}
		}

		protected override void OnCollisionSpecific(Collision col)
		{
			if (Ragdollable)
			{
				ReplaceOnRagdoll(!Dead);
			}
		}
	}
}
