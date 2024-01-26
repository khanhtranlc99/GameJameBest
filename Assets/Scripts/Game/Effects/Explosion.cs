using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game.Effects
{
	public class Explosion : Effect
	{
		private const float MinMassToBeExploded = 4.5f;

		public LayerMask AffectedLayerMask;

		public LayerMask ExplosionBlockerLayerMask;

		public GameObject ExplosionFxPrefab;

		[Space(5f)]
		[SerializeField]
		protected float ExplosionRange = 10f;

		[SerializeField]
		protected float MaxDamage = 100f;

		public DamageType DamageType = DamageType.Explosion;

		public float MinDamageToReplaceHumanOnRagdoll = 10f;

		public bool ChangeHumanOnRagdoll;

		[Space(5f)]
		public float MaxForce = 1000f;

		public ForceMultipliers ForceMultipliers;

		private HitEntity explosionInitiator;

		private Vector3 currentTargetPos;

		private Vector3 currentDirection;

		private float currentMaxDamage;

		private float currentForce;

		private float currentDistance;

		private float currentDamage;

		private readonly List<int> oldLayersList = new List<int>();

		private readonly List<HitEntity> explodedEntities = new List<HitEntity>();

		public void Init(HitEntity initiator, GameObject[] ignoredGameObjects = null)
		{
			Init(initiator, MaxDamage, ExplosionRange, ignoredGameObjects);
		}

		public void Init(HitEntity initiator, float maxDamage, float explosionRange, GameObject[] ignoredGameObjects = null)
		{
			explosionInitiator = initiator;
			MaxDamage = (currentMaxDamage = maxDamage);
			ExplosionRange = explosionRange;
			if(ExplosionFxPrefab!=null)
				ExplosionSFX.Instance.Emit(base.transform.position, ExplosionFxPrefab);
			ExplosionProcess(ignoredGameObjects);
			oldLayersList.Clear();
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		public void Init(HitEntity initiator, float maxDamage, float explosionRange, float explosionForce, GameObject[] ignoredGameObjects = null)
		{
			MaxForce = explosionForce;
			Init(initiator, maxDamage, explosionRange, ignoredGameObjects);
		}

		private void ExplosionProcess(GameObject[] ignoredGameObjects)
		{
			explodedEntities.Capacity = 100;
			Collider[] array = Physics.OverlapSphere(transform.position, ExplosionRange, AffectedLayerMask);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if (ignoredGameObjects == null || !OneOfThem(ignoredGameObjects, collider.gameObject))
				{
					currentTargetPos = collider.ClosestPointOnBounds(base.transform.position);
					currentDirection = currentTargetPos - base.transform.position;
					currentDistance = currentDirection.magnitude;
					currentDirection = currentDirection.normalized;
					currentForce = MaxForce * (1f - currentDistance / ExplosionRange);
					currentDamage = currentMaxDamage * (1f - currentDistance / ExplosionRange);
					Component component;
					switch (GetExplodableType(collider, out component))
					{
					case ExplodableTypes.PDO:
						ExplodePDO(component);
						break;
					case ExplodableTypes.BodyPartDMGReceiver:
						ExplodeBodyPart(component);
						break;
					case ExplodableTypes.HitEntity:
						ExplodeHitEntity(component);
						break;
					case ExplodableTypes.Ragdoll:
						ExplodeRagdoll(component);
						break;
					}
				}
			}
			explodedEntities.Clear();
		}

		private void ExplodeRigidbody(Component component, float multiplier)
		{
			Vector3 force = currentDirection * currentForce * multiplier;
			ExplodeRigidbody(component, force);
		}

		private void ExplodeRigidbody(Component component, Vector3 force, [Optional] Vector3 torgue)
		{
			Rigidbody rigidbody = component as Rigidbody;
			if (!TargetIsBlocked(currentTargetPos) && !(rigidbody == null) && !(rigidbody.mass < 4.5f))
			{
				rigidbody.AddForce(force, ForceMode.Impulse);
				if (torgue != Vector3.zero)
				{
					rigidbody.AddTorque(torgue, ForceMode.Impulse);
				}
			}
		}

		private void ExplodePDO(Component component)
		{
			PseudoDynamicObject pseudoDynamicObject = component as PseudoDynamicObject;
			if (!(pseudoDynamicObject == null) && !TargetIsBlocked(currentTargetPos))
			{
				pseudoDynamicObject.ReplaceOnDynamic(currentDirection * currentForce * ForceMultipliers.PDO);
			}
		}

		private void ExplodeBodyPart(Component component)
		{
			BodyPartDamageReciever bodyPartDamageReciever = component as BodyPartDamageReciever;
			if ((bool)bodyPartDamageReciever)
			{
				ExplodeHitEntity(bodyPartDamageReciever.RerouteEntity);
			}
		}

		private void ExplodeRagdoll(Component component)
		{
			RagdollWakeUper componentInChildren = base.transform.GetComponentInChildren<RagdollWakeUper>();
			if (componentInChildren != null)
			{
				componentInChildren.SetRagdollWakeUpStatus(wakeUp: false);
			}
			ExplodeRigidbody(component.GetComponent<Rigidbody>(), ForceMultipliers.ragdoll);
		}

		private void ExplodeHitEntity(Component component)
		{
			HitEntity hitted = component as HitEntity;
			if (hitted == null || !hitted.enabled || explodedEntities.Any((HitEntity entity) => entity == hitted) || TargetIsBlocked(currentTargetPos))
			{
				return;
			}
			hitted.OnHit(DamageType, explosionInitiator, currentDamage, currentTargetPos, currentDirection, 0f);
			explodedEntities.Add(hitted);
			if (!ChangeHumanOnRagdoll)
			{
				return;
			}
			Human human = hitted as Human;
			if ((bool)human)
			{
				ExplodeHuman(human);
				return;
			}
			HumanoidStatusNPC humanoidStatusNPC = component as HumanoidStatusNPC;
			if (humanoidStatusNPC != null)
			{
				ExplodeNPC(humanoidStatusNPC);
				return;
			}
			RagdollStatus ragdollStatus = component as RagdollStatus;
			if ((bool)ragdollStatus)
			{
				ExplodeRigidbody(ragdollStatus.GetComponent<Rigidbody>(), ForceMultipliers.ragdoll);
				return;
			}
			VehicleStatus vehicleStatus = component as VehicleStatus;
			if ((bool)vehicleStatus)
			{
				Rigidbody component2 = vehicleStatus.rootDrivableVehicle.GetComponent<Rigidbody>();
				float d = (!(vehicleStatus.rootDrivableVehicle is DrivableTank)) ? ForceMultipliers.car : ForceMultipliers.hevyVehicle;
				Vector3 b = Vector3.up * Random.Range(0.3f, 1f);
				Vector3 a = Vector3.Cross(currentDirection, Vector3.up) * ((currentDirection.y < 0f) ? 1 : (-1));
				Vector3 force = (currentDirection + b) * currentForce * d;
				Vector3 a2 = a * currentForce * d;
				ExplodeRigidbody(component2, force, a2 * component2.mass);
			}
		}

		private void ExplodeHuman(Human human)
		{
			if (!human.IsDead && !human.RDExpInvul)
			{
				human.ReplaceOnRagdoll(true, currentDirection * currentForce * ForceMultipliers.human + Vector3.up);
			}
		}

		private void ExplodeNPC(HumanoidStatusNPC npc)
		{
			if (!npc.IsDead && npc.Ragdollable)
			{
				npc.ReplaceOnRagdoll(true, currentDirection * currentForce * ForceMultipliers.human + Vector3.up);
			}
		}

		private ExplodableTypes GetExplodableType(Collider col, out Component component)
		{
			component = col.GetComponent<PseudoDynamicObject>();
			if ((bool)component)
			{
				return ExplodableTypes.PDO;
			}
			component = col.GetComponent<BodyPartDamageReciever>();
			if ((bool)component)
			{
				return ExplodableTypes.BodyPartDMGReceiver;
			}
			component = col.GetComponent<HitEntity>();
			if ((bool)component)
			{
				return ExplodableTypes.HitEntity;
			}
			component = col.GetComponent<RagdollMark>();
			if ((bool)component)
			{
				return ExplodableTypes.Ragdoll;
			}
			component = null;
			return ExplodableTypes.None;
		}

		private static bool OneOfThem(GameObject[] ignoredGameObjects, GameObject gameObject)
		{
			return ignoredGameObjects.Any((GameObject currentGo) => gameObject.transform.IsChildOf(currentGo.transform));
		}

		private bool TargetIsBlocked(Vector3 targetPos)
		{
			return Physics.Linecast(base.transform.position, targetPos, ExplosionBlockerLayerMask);
		}
	}
}
