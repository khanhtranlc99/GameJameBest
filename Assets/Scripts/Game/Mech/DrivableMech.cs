using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.Mech
{
	public class DrivableMech : DrivableVehicle
	{
		[Separator("Mech Links")]
		public MechGunController GunControllerPrefab;

		[HideInInspector]
		public MechAnimationController AnimationController;

		public GameObject mainTarget;

		public CapsuleCollider mainCollider;

		[Tooltip("Part which looking in camera.")]
		public GameObject rotatedPart;

		public Transform fixCameraTarget;

		[Separator("Gun Links")]
		public GameObject LasergunPoint1;

		public GameObject LasergunPoint2;

		public GameObject LauncherPoint1;

		public GameObject testObject;

		private MechGunController gunControll;

		public MechGunController GunController => gunControll;

		public override VehicleType GetVehicleType()
		{
			return VehicleType.Mech;
		}

		public override bool HasExitAnimation()
		{
			return false;
		}

		public override bool HasEnterAnimation()
		{
			return false;
		}

		public override void GetOut()
		{
			base.GetOut();
			FreezeRotation(freeze: false);
			if (gunControll != null)
			{
				PoolManager.Instance.ReturnToPool(gunControll);
				gunControll = null;
			}
		}

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			FreezeRotation();
			if (GunControllerPrefab != null && gunControll == null)
			{
				gunControll = PoolManager.Instance.GetFromPool(GunControllerPrefab);
				MechGunController gunControllerLocal = gunControll;
				PoolManager.Instance.AddBeforeReturnEvent(gunControll, delegate
				{
					gunControllerLocal.DeInit();
				});
				gunControll.transform.parent = base.transform;
				gunControll.transform.localPosition = Vector3.zero;
				gunControll.transform.localEulerAngles = Vector3.zero;
				gunControll.Init(this, driver);
			}
		}

		public override void DeInit()
		{
			base.DeInit();
			base.MainRigidbody.ResetInertiaTensor();
		}

		public override void Init()
		{
			base.Init();
			base.MainRigidbody.ResetInertiaTensor();
			if (AnimationController == null)
			{
				AnimationController = GetComponent<MechAnimationController>();
			}
		}

		private void FreezeRotation(bool freeze = true)
		{
			if (freeze)
			{
				base.MainRigidbody.constraints = (RigidbodyConstraints)80;
			}
			else
			{
				base.MainRigidbody.freezeRotation = false;
			}
		}
	}
}
