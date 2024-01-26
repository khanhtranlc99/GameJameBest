using Game.Character;
using Game.Character.Stats;
using Game.GlobalComponent;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Vehicle
{
	public class BikeController : VehicleController
	{
		private const string SteerAxeName = "Horizontal";

		private const string ThrottleAxeName = "Vertical";

		private const string LeanAxisName = "Lean";

		private const string JumpAxisName = "Jump";

		private const float BikeMass = 500f;

		private const float DropSpeed = 14.8f;

		private const float ForcePopRagdoll = 400f;

		private const float DropFromTimeOut = 0.05f;

		private const float DropFromOffsetRate = 2f;

		private const float StaminaPerSecond = 1f;

		private const float StaminaPerJump = 3f;

		private const float jumpForce = 160000f;

		private const float SteerSpeedRate = 0.1f;

		public WheelCollider FrontWheelCollider;

		public WheelCollider RearWheelCollider;

		public float EngineTorque = 1500f;

		public float MaxEngineRPM = 6000f;

		public float MinEngineRPM = 1000f;

		public float SteerAngle = 40f;

		[HideInInspector]
		public float Speed;

		public float maxSpeed = 180f;

		public float Brake = 2500f;

		[HideInInspector]
		public float MotorInput;

		[HideInInspector]
		public bool brakingNow;

		[HideInInspector]
		public float steerInput;

		[HideInInspector]
		public bool crashed;

		[HideInInspector]
		public bool reversing;

		[HideInInspector]
		public DrivableVehicle mainDrivableVehicle;

		private float leanInput;

		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			MainRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
			MainRigidbody.mass = 500f;
			MainRigidbody.maxAngularVelocity = 2f;
			mainDrivableVehicle = drivableVehicle;
			WheelCollider[] componentsInChildren = MainRigidbody.GetComponentsInChildren<WheelCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].Equals(drivableVehicle.GetComponent<SteeringWheels>().Wheels[0]))
				{
					FrontWheelCollider = componentsInChildren[i];
				}
				else
				{
					RearWheelCollider = componentsInChildren[i];
				}
			}
			GetComponent<BikeAnimatorController>().Init(drivableVehicle.GetComponent<DrivableBike>().animator);
			maxSpeed = drivableVehicle.MaxSpeed * player.stats.GetPlayerStat(StatsList.DrivingMaxSpeed);
			IsInitialized = true;
		}

		public void DropFromSpeed(float relativeSpeed)
		{
			if (Mathf.Abs(relativeSpeed) >= 14.8f || (!RearWheelCollider.isGrounded && !FrontWheelCollider.isGrounded))
			{
				DropFrom();
			}
		}

		public override void DropFrom()
		{
			StartCoroutine(DropFromDelay());
		}

		public IEnumerator DropFromDelay()
		{
			crashed = true;
			yield return new WaitForSeconds(0.05f);
			if (IsInitialized)
			{
				Vector3 up = base.transform.up;
				bool inverted = up.y < 0.4f;
				Transform playerTransform = PlayerInteractionsManager.Instance.Player.transform;
				playerTransform.localPosition = Vector3.up * 2f;
				if (inverted)
				{
					playerTransform.position += Vector3.up * 2f;
				}
				playerTransform.parent = null;
				PlayerInteractionsManager.Instance.SwitchSkeletons(false, false);
				player.ResetRotation();
				GameObject ragdoll;
				player.ReplaceOnRagdoll(true, out ragdoll);
				MainRigidbody.constraints = RigidbodyConstraints.None;
				Transform transform = ragdoll.transform;
				Vector3 eulerAngles = ragdoll.transform.eulerAngles;
				float x = eulerAngles.x;
				Vector3 eulerAngles2 = ragdoll.transform.eulerAngles;
				transform.eulerAngles = new Vector3(x, eulerAngles2.y, 0f);
				PlayerInteractionsManager.Instance.GetOutFromVehicle(isRaggdol: true);
			}
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			MainRigidbody.constraints = RigidbodyConstraints.None;
			MainRigidbody.centerOfMass = primordialCenterOfMass;
			MainRigidbody.ResetInertiaTensor();
			MotorInput = 0f;
			RearWheelCollider.motorTorque = 0f;
			FrontWheelCollider.brakeTorque = Brake;
			RearWheelCollider.brakeTorque = Brake;
			FrontWheelCollider = null;
			RearWheelCollider = null;
			GetComponent<BikeAnimatorController>().DeInit();
			base.DeInit(callbackAfterDeInit);
		}

		public void Braking()
		{
			if (Mathf.Abs(MotorInput) <= 0.05f)
			{
				brakingNow = false;
				FrontWheelCollider.brakeTorque = Brake / 25f;
				RearWheelCollider.brakeTorque = Brake / 25f;
			}
			else if (MotorInput < 0f && !reversing)
			{
				brakingNow = true;
				FrontWheelCollider.brakeTorque = Brake * (Mathf.Abs(MotorInput) / 5f);
				RearWheelCollider.brakeTorque = Brake * Mathf.Abs(MotorInput);
			}
			else
			{
				brakingNow = false;
				FrontWheelCollider.brakeTorque = 0f;
				RearWheelCollider.brakeTorque = 0f;
			}
		}

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Bicycle;
		}

		private void Update()
		{
			Inputs();
			ProceedStamina();
			Lean();
			Drive();
			Braking();
		}

		public void ProceedStamina()
		{
			if (MotorInput <= 0f)
			{
				player.stats.stamina.DoFixedUpdate();
			}
			else
			{
				player.stats.stamina.SetAmount(-1f * Time.deltaTime);
			}
			if (player.stats.stamina.Current <= 10f && MotorInput > 0f)
			{
				MotorInput = 0f;
			}
		}

		private void Inputs()
		{
			leanInput = Controls.GetAxis("Lean");
			if (Controls.GetButtonDown("Jump"))
			{
				Jump();
			}
			if (!crashed)
			{
				MotorInput = Controls.GetAxis("Vertical");
				steerInput = Controls.GetAxis("Horizontal");
			}
			else
			{
				MotorInput = 0f;
				steerInput = 0f;
			}
			if (MotorInput < 0f)
			{
				Vector3 vector = base.transform.InverseTransformDirection(MainRigidbody.velocity);
				if (vector.z < 0f)
				{
					reversing = true;
					return;
				}
			}
			reversing = false;
		}

		private void Jump()
		{
			WheelHit _;
			if (RearWheelCollider.GetGroundHit(out _))
			{
				player.stats.stamina.SetAmount(-3f);
				MainRigidbody.AddForce(Vector3.up * 160000f);
			}
		}

		private void Drive()
		{
			Speed = CalcSpeedKmh();
			FrontWheelCollider.steerAngle = SteerAngle * steerInput;
			if (Speed > maxSpeed)
			{
				RearWheelCollider.motorTorque = 0f;
			}
			else if (!reversing)
			{
				RearWheelCollider.motorTorque = EngineTorque * Mathf.Clamp(MotorInput, 0f, 1f);
			}
			if (reversing)
			{
				if (Speed < 10f)
				{
					RearWheelCollider.motorTorque = EngineTorque * MotorInput / 5f;
				}
				else
				{
					RearWheelCollider.motorTorque = 0f;
				}
			}
		}

		private void LateUpdate()
		{
			if (FrontWheelCollider.isGrounded || RearWheelCollider.isGrounded)
			{
				Transform transform = MainRigidbody.transform;
				Vector3 eulerAngles = base.transform.eulerAngles;
				float x = eulerAngles.x;
				Vector3 eulerAngles2 = base.transform.eulerAngles;
				transform.eulerAngles = new Vector3(x, eulerAngles2.y, 0f);
			}
		}

		private void Lean()
		{
			if (Mathf.Abs(leanInput) > 0.3f && MotorInput > 0f)
			{
				MainRigidbody.AddRelativeTorque(new Vector3(5000f * leanInput, 0f, 0f));
			}
		}

		protected void ShiftGears()
		{
		}

		private float CalcSpeedKmh()
		{
			return Vector3.Dot(MainRigidbody.velocity, MainRigidbody.transform.forward) * 3.6f;
		}
	}
}
