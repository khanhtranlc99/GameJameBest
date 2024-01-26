using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableMotorcycle : DrivableVehicle
	{
		private const float SpeedForDrop = 5f;

		private const float DamageForDropMultipler = 0.003f;

		public Transform DriverStatPoint;

		public BikeTrigger BikeTrigger;

		[HideInInspector]
		public Animator animator => GetComponent<Animator>();

		public override VehicleType GetVehicleType()
		{
			return VehicleType.Motorbike;
		}

		public override void Init()
		{
			if (vehStatus == null)
			{
				vehStatus = GetComponentInChildren<VehicleStatus>();
			}
			vehStatus.Initialization();
			ChangeBodyColor(BodyRenderers);
			PoolManager.Instance.AddBeforeReturnEvent(base.gameObject, delegate
			{
				base.MainRigidbody.ResetInertiaTensor();
				ConstraintsSetup(isIn: false);
			});
			RigidbodyConstraints constraints = base.MainRigidbody.constraints;
			base.MainRigidbody.constraints = RigidbodyConstraints.None;
			ApplyCenterOfMass(VehiclePoints.CenterOfMass);
			base.MainRigidbody.constraints = constraints;
		}

		public override bool IsControlsPlayerAnimations()
		{
			return false;
		}

		public override bool HasExitAnimation()
		{
			return true;
		}

		public override bool HasEnterAnimation()
		{
			return true;
		}

		public override void StopVehicle()
		{
			WheelCollider[] componentsInChildren = GetComponentsInChildren<WheelCollider>();
			WheelCollider[] array = componentsInChildren;
			foreach (WheelCollider wheelCollider in array)
			{
				wheelCollider.motorTorque = 0f;
				wheelCollider.brakeTorque = 400f;
			}
			base.MainRigidbody.velocity = Vector3.zero;
		}

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			ApplyCenterOfMass(VehiclePoints.CenterOfMass);
			ConstraintsSetup(isIn: true);
			if ((bool)BikeTrigger)
			{
				BikeTrigger.enabled = true;
				BikeTrigger.Init();
			}
		}

		public override void ConstraintsSetup(bool isIn)
		{
			base.MainRigidbody.constraints = (isIn ? RigidbodyConstraints.FreezeRotationZ : RigidbodyConstraints.None);
		}

		public override void SteerStabilization(float steer)
		{
			Vector3 eulerAngles = base.MainRigidbody.transform.eulerAngles;
			float z = eulerAngles.z;
			float b = (0f - steer) * 0.5f;
			z = Mathf.LerpAngle(z, b, Time.deltaTime * 15f);
			Transform transform = base.MainRigidbody.transform;
			Vector3 eulerAngles2 = base.transform.eulerAngles;
			float x = eulerAngles2.x;
			Vector3 eulerAngles3 = base.transform.eulerAngles;
			transform.eulerAngles = new Vector3(x, eulerAngles3.y, z);
		}

		public override void ApplyStabilization(float force)
		{
		}

		public override Vector3 GetExitPosition(bool toLeft)
		{
			return toLeft ? VehiclePoints.EnterFromPositions[0].position : VehiclePoints.EnterFromPositions[1].position;
		}

		public override bool IsDoorBlockedOffset(LayerMask blockedLayerMask, Transform driver, out Vector3 offset, bool horizontalCheckOnly = true)
		{
			offset = Vector3.zero;
			Vector3 vector = base.transform.position + base.transform.up * VehicleSpecificPrefab.MaxHeight * 0.5f;
			bool flag = false;
			bool flag2 = false;
			RaycastHit hitInfo;
			if (Physics.Raycast(vector, -base.transform.right, out hitInfo, ExitLeftMinDistance, blockedLayerMask))
			{
				offset = VehiclePoints.EnterFromPositions[0].position - driver.position;
				flag = true;
			}
			if (Physics.Raycast(vector, base.transform.right, out hitInfo, ExitRightMinDistance, blockedLayerMask))
			{
				offset = VehiclePoints.EnterFromPositions[1].position - driver.position;
				flag2 = true;
			}
			UnityEngine.Debug.DrawRay(vector, -base.transform.right * ExitLeftMinDistance, Color.cyan, 5f);
			UnityEngine.Debug.DrawRay(vector, base.transform.right * ExitLeftMinDistance, Color.cyan, 5f);
			UnityEngine.Debug.DrawRay(driver.position + Vector3.up * 0.1f, Vector3.up * 2f, Color.yellow, 5f);
			if (flag2 && flag)
			{
				offset = Vector3.up * ExitUpMinDistance;
			}
			return flag2 && flag;
		}

		public override void SetDummyDriver(DummyDriver driver)
		{
			base.SetDummyDriver(driver);
			if ((bool)DummyDriver)
			{
				DummyDriver.DriverStatus.DamageEvent += OnDriverStatusDamageEvent;
				PoolManager.Instance.AddBeforeReturnEvent(driver, delegate
				{
					DummyDriver.DriverStatus.DamageEvent -= OnDriverStatusDamageEvent;
					if (!controller)
					{
						ConstraintsSetup(isIn: false);
						base.MainRigidbody.ResetInertiaTensor();
					}
				});
			}
		}

		public override void OnDriverStatusDamageEvent(float damage, HitEntity owner)
		{
			if ((bool)DummyDriver && (bool)owner)
			{
				DummyDriver.DropRagdoll(owner, -(owner.transform.position - DummyDriver.transform.position).normalized * (damage * 0.003f), !DummyDriver.DriverDead, false, 0f);
				ConstraintsSetup(isIn: false);
				base.MainRigidbody.ResetInertiaTensor();
			}
		}

		public override void GetOut()
		{
			base.GetOut();
			if ((bool)BikeTrigger)
			{
				BikeTrigger.enabled = false;
			}
		}

		public override bool IsAbleToEnter()
		{
			return !DeepInWater || !WaterSensor.InWater;
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if ((bool)controller)
			{
				((MotorcycleController)controller).DropFromSpeed(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal));
			}
			float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
			HitEntity disturber = null;
			VehicleStatus component = col.collider.GetComponent<VehicleStatus>();
			if (component != null)
			{
				disturber = component.GetVehicleDriver();
			}
			if (num > 5f && (bool)DummyDriver)
			{
				DummyDriver.DropRagdoll(disturber, col.contacts[0].normal, !DummyDriver.DriverDead, false, 0f);
				ConstraintsSetup(isIn: false);
				base.MainRigidbody.ResetInertiaTensor();
			}
		}
	}
}
