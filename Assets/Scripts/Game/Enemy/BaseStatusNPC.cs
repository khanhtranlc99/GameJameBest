using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.GlobalComponent;
using Game.Managers;
using Game.PickUps;
using Game.Vehicle;
using UnityEngine;

namespace Game.Enemy
{
	public class BaseStatusNPC : HitEntity, IInitable
	{
		private const float AlarmShoutRange = 20f;

		private const float OnBackCollisionRelativeForceCouter = 2f;

		private const float OnBigDynamicCollisionForceCounter = 3f;

		private const float OnCollisionKnockingTreshold = 10f;

		private const float OnCollisionDamageCounter = 1f;

		private const float MinVelocityToFall = 8f;

		private const float CheckSpeedTime = 2f;

		private const float MinPlayerHealthProcentForHealthPack = 0.25f;

		private static int bigDynamicLayerNumber = -1;

		[Separator("NPC Links")]
		public BaseNPC BaseNPC;

		[HideInInspector]
		public bool canDropEnergy;

		[HideInInspector]
		public bool isTransformer;

		private SlowUpdateProc slowUpdateProc;

		private Vector3 lastPosition;

		private float checkSpeedTimer;

		public virtual void Init()
		{
			Initialization();
		}

		public virtual void DeInit()
		{
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if ((bool)owner)
			{
				EntityManager.Instance.OverallAlarm(owner, this, base.transform.position, 20f);
			}
			base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
		}

		public virtual void OnStatusAlarm(HitEntity disturber)
		{
			BaseNPC.OnAlarm(disturber);
		}

		protected override void OnDie()
		{
			base.OnDie();
			DropPickup();
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		protected virtual void OnCollisionEnter(Collision col)
		{
			float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
			if (col.collider.gameObject.layer == bigDynamicLayerNumber)
			{
				num *= 3f;
			}
			if (Vector3.Dot(base.transform.forward, col.relativeVelocity.normalized) > 0.2f)
			{
				num *= 2f;
			}
			if (!(Mathf.Abs(num) < 10f))
			{
				HitEntity owner = FindCollisionDriver(col);
				OnHit(DamageType.Collision, owner, num * 1f, col.contacts[0].point, col.contacts[0].normal.normalized, 0f);
				OnCollisionSpecific(col);
			}
		}

		protected virtual void OnCollisionSpecific(Collision col)
		{
		}

		protected virtual void DropPickup()
		{
			if (PlayerInteractionsManager.Instance.Player.Health.Current > PlayerInteractionsManager.Instance.Player.Health.Max * 0.25f)
			{
				if (isTransformer && GameManager.Instance.IsTransformersGame)
				{
					PickUpManager.Instance.GenerateEnergyOnPoint(base.transform.position + base.transform.right);
				}
				else if (!GameManager.Instance.IsTransformersGame)
				{
					PickUpManager.Instance.GenerateMoneyOnPoint(base.transform.position + base.transform.right);
				}
			}
			else
			{
				PickUpManager.Instance.GenerateHealthPackOnPoint(base.transform.position + base.transform.right, PickUpManager.HealthPackType.Random);
			}
		}

		protected new virtual void Awake()
		{
			if (bigDynamicLayerNumber == -1)
			{
				bigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
			}
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 1f);
			isTransformer = GetComponent<Transformer>();
			BaseNPC = GetComponent<BaseNPC>();
		}

		private void OnEnable()
		{
			lastPosition = base.transform.position;
			checkSpeedTimer = 2f;
		}

		protected override void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			VelocityCheck();
		}

		private HitEntity FindCollisionDriver(Collision col)
		{
			VehicleStatus component = col.collider.gameObject.GetComponent<VehicleStatus>();
			if (component != null)
			{
				return component.GetVehicleDriver();
			}
			return null;
		}

		private void VelocityCheck()
		{
			lastPosition.y = 0f;
			Vector3 position = base.transform.position;
			position.y = 0f;
			float num = (lastPosition - position).magnitude / slowUpdateProc.DeltaTime;
			lastPosition = base.transform.position;
			if (checkSpeedTimer > 0f)
			{
				checkSpeedTimer -= slowUpdateProc.DeltaTime;
			}
			else if (num > 8f)
			{
				OnCollisionSpecific(null);
			}
		}
	}
}
