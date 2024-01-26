using Game.Character.CharacterController;
using Game.GlobalComponent;
using System.Collections;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableCar : DrivableVehicle
	{
		private SteeringWheels steeringWheels;

		private VehicleSpecific vehicleSpecific;

		private bool whileDoorOpenLock;

		public WheelCollider[] wheels;

		public bool HasEnterAnim = true;

		public bool HasExitAnim = true;

		public override bool HasEnterAnimation()
		{
			return HasEnterAnim;
		}

		public override bool HasExitAnimation()
		{
			return HasExitAnim;
		}

		public override VehicleType GetVehicleType()
		{
			return VehicleType.Car;
		}

		public override void ApplyStabilization(float force)
		{
			if (!OnGround)
			{
				return;
			}
			Vector3 position = base.MainRigidbody.transform.position;
			if (VehiclePoints.CenterOfMass != null)
			{
				position = VehiclePoints.CenterOfMass.position;
			}
			float num = 0f;
			if (wheels != null)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					Vector3 pos;
					Quaternion _;
					wheelCollider.GetWorldPose(out pos, out _);
					float num2 = (pos - wheelCollider.transform.position).magnitude / wheelCollider.suspensionDistance;
					num += num2 * Vector3.Dot(position - wheelCollider.transform.position, base.MainRigidbody.transform.right);
				}
			}
			base.MainRigidbody.AddRelativeTorque(Vector3.forward * num * force);
		}

		public override void CheckLocationOnGround()
		{
			if (wheels == null)
			{
				return;
			}
			bool onGround = false;
			for (int i = 0; i < wheels.Length; i++)
			{
				WheelCollider wheelCollider = wheels[i];
				if (wheelCollider.isGrounded)
				{
					onGround = true;
					break;
				}
			}
			OnGround = onGround;
		}

		protected override void Awake()
		{
			base.Awake();
			steeringWheels = GetComponent<SteeringWheels>();
			if (steeringWheels == null)
			{
				UnityEngine.Debug.LogWarning("Drivable car without steeringWheels");
			}
			wheels = GetComponentsInChildren<WheelCollider>();
			if (wheels == null)
			{
				UnityEngine.Debug.LogError("Drivable car without wheels");
			}
		}

		public override void OpenVehicleDoor(VehicleDoor door, bool isGettingIn)
		{
			base.OpenVehicleDoor(door, isGettingIn);
			if (controller != null || vehicleSpecific != null || !(VehicleSpecificPrefab != null))
			{
				return;
			}
			vehicleSpecific = PoolManager.Instance.GetFromPool(VehicleSpecificPrefab);
			whileDoorOpenLock = true;
			PoolManager.Instance.AddBeforeReturnEvent(vehicleSpecific, delegate
			{
				whileDoorOpenLock = false;
				vehicleSpecific = null;
				if (SimpleModel != null && controller == null)
				{
					SimpleModel.SetActive(value: true);
				}
			});
			if (SimpleModel != null)
			{
				SimpleModel.SetActive(value: false);
			}
			GameObject gameObject = vehicleSpecific.gameObject;
			gameObject.transform.parent = base.MainRigidbody.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			CarSpecific carSpecific = vehicleSpecific as CarSpecific;
			if (carSpecific != null && carSpecific.CarAnimator != null)
			{
				carSpecific.CarAnimator.SetBool("EnterInCar", isGettingIn);
				string trigger = string.Empty;
				switch (door)
				{
				case VehicleDoor.LeftDoor:
					trigger = "LeftOpen";
					break;
				case VehicleDoor.RightDoor:
					trigger = "RightOpen";
					break;
				}
				carSpecific.CarAnimator.SetTrigger(trigger);
			}
			float timeDelay = (!(carSpecific != null)) ? 1f : carSpecific.GetOutAnimationTime;
			StartCoroutine(ReturnToPoolVehicleSpecific(timeDelay));
		}

		public override void Drive(Player driver)
		{
			if (vehicleSpecific != null)
			{
				PoolManager.Instance.ReturnToPool(vehicleSpecific);
			}
			base.Drive(driver);
		}

		public override void DeInit()
		{
			if (vehicleSpecific != null)
			{
				PoolManager.Instance.ReturnToPool(vehicleSpecific);
			}
			base.DeInit();
		}

		public override bool IsAbleToEnter()
		{
			return !whileDoorOpenLock && base.IsAbleToEnter() && !DeepInWater;
		}

		private IEnumerator ReturnToPoolVehicleSpecific(float timeDelay)
		{
			yield return new WaitForSeconds(timeDelay * 0.5f);
			if (vehicleSpecific != null)
			{
				whileDoorOpenLock = false;
			}
			yield return new WaitForSeconds(timeDelay * 0.5f);
			if (vehicleSpecific != null)
			{
				PoolManager.Instance.ReturnToPool(vehicleSpecific);
			}
		}
	}
}
