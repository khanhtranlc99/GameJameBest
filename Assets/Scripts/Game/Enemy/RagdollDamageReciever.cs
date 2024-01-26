using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class RagdollDamageReciever : HitEntity
	{
		private const float OnCollisionKnockingTreshold = 6f;

		private const float MaxVelocityCounter = 15f;

		private static int terrainLayerNumber = -1;

		private static int staticObjectLayerNumber = -1;

		private static int complexStaticObjectLayerNumber = -1;

		private static int WaterLayerNumber = -1;

		private static float AppliedForceReducer = 20f;

		public RagdollStatus RdStatus;

		public GameObject RecieverSensor;

		private Rigidbody rigidBody;

		public Transform rootParent;

		public float DamageMultiplier = 1f;

		public float Magnitude
		{
			get
			{
				if (rigidBody == null)
				{
					rigidBody = GetComponent<Rigidbody>();
				}
				return rigidBody.velocity.magnitude;
			}
		}

		public Rigidbody BodyPartRigidbody => rigidBody;

		private new void Awake()
		{
			if (terrainLayerNumber == -1)
			{
				terrainLayerNumber = LayerMask.NameToLayer("Terrain");
			}
			if (complexStaticObjectLayerNumber == -1)
			{
				complexStaticObjectLayerNumber = LayerMask.NameToLayer("ComplexStaticObject");
			}
			if (staticObjectLayerNumber == -1)
			{
				staticObjectLayerNumber = LayerMask.NameToLayer("SimpleStaticObject");
			}
			if (WaterLayerNumber == -1)
			{
				WaterLayerNumber = LayerMask.NameToLayer("Water");
			}
		}

		protected override void Start()
		{
			rigidBody = GetComponent<Rigidbody>();
			Health.Setup();
			Dead = false;
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (rigidBody != null && rigidBody.velocity.magnitude > 20)
			{
				rigidBody.velocity = rigidBody.velocity.normalized * 20;
			}
				
		}

		public void DeInit()
		{
			if ((bool)RecieverSensor)
			{
				PoolManager.Instance.ReturnToPool(RecieverSensor);
				RecieverSensor = null;
			}
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			if ((bool)RdStatus)
			{
				RdStatus.OnHit(damageType, owner, damage * DamageMultiplier, hitPos, hitVector, defenceReduction);
				if (rigidBody != null)
				{
					damage = Mathf.Min(damage, 15f);
					RdStatus.ApplyForce(rigidBody, hitVector.normalized * damage / AppliedForceReducer);
				}
				SpecifficEffect(hitPos);
			}
		}

		protected override void OnDie()
		{
		}

		private void OnCollisionEnter(Collision col)
		{
			if ((bool)RdStatus && !col.collider.transform.IsChildOf(rootParent) && ((col.collider.gameObject.layer != terrainLayerNumber && col.collider.gameObject.layer != staticObjectLayerNumber && col.collider.gameObject.layer != complexStaticObjectLayerNumber) || RdStatus.wakeUper.CurrentState.Equals(RagdollState.Ragdolled)))
			{
				float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
				if (!(Mathf.Abs(num) < 6f) && (RdStatus.Faction != Faction.Player || col.collider.gameObject.layer != 13))
				{
					RdStatus.OnHit(DamageType.Collision, null, num, col.contacts[0].point, col.contacts[0].normal.normalized, 0f);
				}
			}
		}
	}
}
