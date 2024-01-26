using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Traffic;
using System.Collections;
using UnityEngine;

namespace Game.Vehicle
{
	public class DummyDriver : MonoBehaviour
	{
		public delegate void DummyEvent();

		private const float CorpseFallAnimationLength = 1.7f;

		private const float GetOutAlarmRange = 20f;

		private const float DropForce = 20000f;

		public GameObject DriverModel;

		public GameObject DriverNPC;

		public GameObject DriverVehStatusPrefab;

		public float forceGetOutAnimationLength = 3f;

		public float getOutAnimationLength = 2.8f;

		[HideInInspector]
		public bool DriverDead;

		public DriverStatus DriverStatus;

		private Transform driverIdeal;

		private GameObject initedDriverModel;

		private GameObject driverStatusGO;

		private DrivableVehicle localDrivableVehicle;

		private HumanoidStatusNPC initedStatusNpc;

		private bool driverOnVehicle;

		public DummyEvent DummyExitEvent;

		public bool HaveDriver => initedDriverModel;

		public HumanoidStatusNPC InitedStatusNPC => initedStatusNpc;

		public void InitDriver(DrivableVehicle drivableVehicle)
		{
			if ((bool)DriverModel)
			{
				driverOnVehicle = true;
				initedDriverModel = PoolManager.Instance.GetFromPool(DriverModel, drivableVehicle.transform.position, drivableVehicle.transform.rotation);
				initedDriverModel.transform.parent = base.transform;
				localDrivableVehicle = drivableVehicle;
				if (!driverStatusGO)
				{
					driverStatusGO = PoolManager.Instance.GetFromPool(DriverVehStatusPrefab);
					driverStatusGO.transform.parent = initedDriverModel.transform.Find("metarig").Find("hips").transform;
					driverStatusGO.transform.localPosition = localDrivableVehicle.VehicleSpecificPrefab.DriverStatusPosition;
					driverStatusGO.transform.localEulerAngles = localDrivableVehicle.VehicleSpecificPrefab.DriverStatusRotation;
					PoolManager.Instance.AddBeforeReturnEvent(driverStatusGO, delegate
					{
						driverStatusGO = null;
					});
				}
				DriverStatus = driverStatusGO.GetComponent<DriverStatus>();
				DriverStatus.Init(base.gameObject, drivableVehicle.DriverIsVulnerable);
				localDrivableVehicle.SetDummyDriver(this);
				PoolManager.Instance.AddBeforeReturnEvent(this, delegate
				{
					if (driverStatusGO != null)
					{
						PoolManager.Instance.ReturnToPool(driverStatusGO);
					}
				});
				DriverDead = false;
				driverIdeal = localDrivableVehicle.VehicleSpecificPrefab.Skeleton;
				CopyTransformRecurse(driverIdeal, initedDriverModel);
			}
		}

		public void DeInitDriver()
		{
			if ((bool)initedDriverModel)
			{
				PoolManager.Instance.ReturnToPool(initedDriverModel);
				initedDriverModel = null;
				driverIdeal = null;
				initedStatusNpc = null;
				if (driverStatusGO != null)
				{
					PoolManager.Instance.ReturnToPool(driverStatusGO);
				}
			}
		}

		private void CopyTransformRecurse(Transform idealModelTransform, GameObject newModel)
		{
			if (idealModelTransform != null && newModel != null)
			{
				newModel.transform.localPosition = idealModelTransform.localPosition;
				newModel.transform.localRotation = idealModelTransform.localRotation;
				newModel.transform.localScale = idealModelTransform.localScale;
				foreach (Transform item in newModel.transform)
				{
					Transform transform2 = idealModelTransform.Find(item.name);
					if ((bool)transform2)
					{
						CopyTransformRecurse(transform2, item.gameObject);
					}
				}
			}
		}

		public void DeInitTrafficDriver()
		{
			TrafficDriver componentInChildren = localDrivableVehicle.gameObject.GetComponentInChildren<TrafficDriver>();
			if (componentInChildren != null)
			{
				PoolManager.Instance.ReturnToPool(componentInChildren);
			}
		}

		public void DriverDie()
		{
			CopyTransformRecurse(localDrivableVehicle.VehicleSpecificPrefab.SkeletonDead, initedDriverModel);
			DriverDead = true;
			DeInitTrafficDriver();
			Autopilot componentInChildren = localDrivableVehicle.gameObject.GetComponentInChildren<Autopilot>();
			if (componentInChildren != null)
			{
				componentInChildren.DropPassangers();
			}
			localDrivableVehicle.StopVehicle();
		}

		public void InitOutOfVehicle(bool force, HitEntity disturber, bool isReplaceOnRagdoll)
		{
			if ((bool)initedDriverModel && driverOnVehicle)
			{
				StartCoroutine(InitOutOfVehicleEnumerator(force, disturber, isReplaceOnRagdoll));
			}
		}

		public void DropRagdoll(HitEntity disturber, Vector3 direction, bool canWakeUp = true, bool isOnWater = false, float waterHight = 0f)
		{
			if (!initedDriverModel || !driverOnVehicle)
			{
				return;
			}
			BaseNPC component = PoolManager.Instance.GetFromPool(DriverNPC).GetComponent<BaseNPC>();
			HumanoidStatusNPC component2 = component.GetComponent<HumanoidStatusNPC>();
			if (driverStatusGO != null)
			{
				component2.Health.Current = driverStatusGO.GetComponent<DriverStatus>().Health.Current;
			}
			CopyTransformRecurse(localDrivableVehicle.VehicleSpecificPrefab.Skeleton, component.RootModel);
			component.transform.position = initedDriverModel.transform.position;
			component.transform.rotation = initedDriverModel.transform.rotation;
			component.RootModel.transform.localPosition = Vector3.zero;
			component.QuietControllerType = BaseNPC.NPCControllerType.Pedestrian;
			if (!DriverDead)
			{
				TrafficManager.Instance.TakePedestrianSlot(component);
			}
			PoolManager.Instance.AddBeforeReturnEvent(component, delegate
			{
				TrafficManager.Instance.FreePedestrianSlot();
			});
			if ((bool)disturber)
			{
				component.ChangeController(BaseNPC.NPCControllerType.Smart);
				SmartHumanoidController smartHumanoidController = component.CurrentController as SmartHumanoidController;
				component2.OnStatusAlarm(disturber);
				smartHumanoidController.AddPersonalTarget(disturber);
				smartHumanoidController.InitBackToDummyLogic();
				if (disturber.Faction == Faction.Player)
				{
					if (canWakeUp)
					{
						FactionsManager.Instance.PlayerAttackHuman(DriverStatus);
					}
					else
					{
						FactionsManager.Instance.CommitedACrime();
					}
				}
			}
			component2.LastWaterHeight = waterHight - 1.6f;
			component2.IsInWater = isOnWater;
			component2.ReplaceOnRagdoll(canWakeUp);
			component2.GetRagdollHips().GetComponent<Rigidbody>().AddForce(direction * 20000f);
			localDrivableVehicle.StopVehicle();
			DeInitTrafficDriver();
			DeInitDriver();
			PoolManager.Instance.ReturnToPool(this);
		}

		private IEnumerator InitOutOfVehicleEnumerator(bool force, HitEntity disturber, bool isReplaceOnRagdoll)
		{
			driverOnVehicle = false;
			if (localDrivableVehicle.CurrentDriver == null)
			{
				localDrivableVehicle.GetVehicleStatus().Faction = Faction.NoneFaction;
			}
			BaseNPC initNPC = PoolManager.Instance.GetFromPool(DriverNPC).GetComponent<BaseNPC>();
			initNPC.WaterSensor.Reset();
			if (!DriverDead)
			{
				TrafficManager.Instance.TakePedestrianSlot(initNPC);
			}

			BaseControllerNPC npcController;
			initNPC.ChangeController(BaseNPC.NPCControllerType.Smart, out npcController);
			PoolManager.Instance.AddBeforeReturnEvent(initNPC, delegate
			{
				//base._003CinitNPC_003E__0.WaterSensor.Reset();
				TrafficManager.Instance.FreePedestrianSlot();
			});
			SmartHumanoidController smartHumanoidController = npcController as SmartHumanoidController;
			smartHumanoidController.InitBackToDummyLogic();
			Collider[] npcColliders = initNPC.SpecificNpcLinks.ModelColliders;
			Rigidbody npcRigidbody = initNPC.NPCRigidbody;
			initedStatusNpc = (initNPC.StatusNpc as HumanoidStatusNPC);
			initNPC.transform.parent = base.transform.parent;
			Collider[] array = npcColliders;
			foreach (Collider npcCollider in array)
			{
				npcCollider.isTrigger = true;
			}
			npcRigidbody.isKinematic = true;
			if (driverStatusGO != null)
			{
				initedStatusNpc.Health.Current = driverStatusGO.GetComponent<DriverStatus>().Health.Current;
			}
			smartHumanoidController.AnimationController.StartInCar(localDrivableVehicle.GetVehicleType(), force, DriverDead);
			yield return new WaitForEndOfFrame();
			smartHumanoidController.WeaponController.HideWeapon();
			npcController.gameObject.SetActive(value: false);
			Transform exitPoint = force ? localDrivableVehicle.VehiclePoints.EnterFromPositions[2] : (DriverDead ? localDrivableVehicle.VehiclePoints.EnterFromPositions[4] : localDrivableVehicle.VehiclePoints.EnterFromPositions[0]);
			initNPC.transform.position = exitPoint.position;
			initNPC.transform.rotation = exitPoint.rotation;
			PoolManager.Instance.ReturnToPool(initedDriverModel);
			initedDriverModel = null;
			float waitTime = DriverDead ? 1.7f : ((!force) ? getOutAnimationLength : localDrivableVehicle.VehicleSpecificPrefab.forceGetOutAnimationLength);
			yield return new WaitForSeconds(waitTime);
			smartHumanoidController.WeaponController.ShowWeapon();
			npcController.gameObject.SetActive(value: true);
			Collider[] array2 = npcColliders;
			foreach (Collider npcCollider2 in array2)
			{
				npcCollider2.isTrigger = false;
			}
			npcRigidbody.isKinematic = false;
			initNPC.transform.parent = null;
			if (!DriverDead)
			{
				initNPC.QuietControllerType = BaseNPC.NPCControllerType.Pedestrian;
				smartHumanoidController.InitBackToDummyLogic();
				if (isReplaceOnRagdoll)
				{
					initedStatusNpc.ReplaceOnRagdoll(canWakeUp: true);
					initedStatusNpc.GetRagdollHips().GetComponent<Rigidbody>().AddForce(-base.transform.forward * 20000f);
				}
				else
				{
					Transform transform = initNPC.transform;
					Vector3 eulerAngles = initNPC.transform.eulerAngles;
					transform.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
				}
			}
			else
			{
				initedStatusNpc.ReplaceOnRagdoll(canWakeUp: false);
			}
			if ((bool)disturber)
			{
				if (!(driverStatusGO == null))
				{
					driverStatusGO.GetComponent<DriverStatus>();
				}
				initedStatusNpc.OnStatusAlarm(disturber);
				smartHumanoidController.AddPersonalTarget(disturber);
				if (disturber.Faction == Faction.Player && force)
				{
					FactionsManager.Instance.CommitedACrime();
				}
			}
			if (DummyExitEvent != null)
			{
				DummyExitEvent();
				DummyExitEvent = null;
			}
			localDrivableVehicle.StopVehicle();
			DeInitTrafficDriver();
			DeInitDriver();
			PoolManager.Instance.ReturnToPool(this);
		}
	}
}
