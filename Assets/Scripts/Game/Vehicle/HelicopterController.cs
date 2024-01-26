using Game.Character;
using Game.GlobalComponent;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Vehicle
{
	public class HelicopterController : VehicleController
	{
		private const string XAccelerationAxisName = "Vertical";

		private const string ZAccelerationAxisName = "Horizontal";

		private const string VerticalMovementAxisName = "Vertical_Heli";

		private const int GroundCheckRayDistance = 1;

		private const float JumpOutFromHelicopterAnimationLenght = 2.7f;

		private const float EnterInAnimationLength = 5f;

		private const int EvacuationForce = 2;

		[Separator("HelicopterSpecificParametrs")]
		public LayerMask GroundLayerMask;

		public float TiltAngleSpeed = 1f;

		public float MaxTiltAngle = 30f;

		public float VerticalMovementVelocity = 0.2f;

		public float MaxVerticalVelocity = 20f;

		public int VerticalMovementBrakeForce = 6;

		public float SideMovementForce = 0.5f;

		public float MaximumSideForce = 25f;

		public float TorqueForce = 200000f;

		public float MaxBladesRotateSpeed = 20f;

		public float AvaibleToFlyBladesRotateSpeed = 10f;

		public float MaximumHelicopterY = 200f;

		public float RigidbodyAngularDrag = 2f;

		public AudioClip OpenCabineSound;

		private bool isGrounded;

		private bool avaibleToFly;

		private DrivableHelicopter currentHelicopter;

		private float verticalMovementInput;

		private float xAccelerationInput;

		private float zAccelerationInput;

		private Transform cameraTransform;

		private float verticalForce;

		private float xTiltAngle;

		private float zTiltAngle;

		private float zForce;

		private float xForce;

		private Action deInitAction;

		public bool IsGrounded => isGrounded;

		public bool InitializedGetter => IsInitialized;

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Copter;
		}

		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			IsInitialized = false;
			DrivableHelicopter x = drivableVehicle as DrivableHelicopter;
			if (x != null)
			{
				currentHelicopter = x;
			}
			cameraTransform = CameraManager.Instance.UnityCamera.transform;
			OpenCabine(getIn: true);
			StartCoroutine(DeferredInit());
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			if (!IsGrounded)
			{
				Invoke("EvacuationTilt", 2.7f);
			}
			else
			{
				MainRigidbody.useGravity = true;
				MainRigidbody.angularDrag = 0.05f;
				currentHelicopter.CurrentBladesRotateSpeed = Mathf.Min(currentHelicopter.CurrentBladesRotateSpeed, AvaibleToFlyBladesRotateSpeed);
			}
			IsInitialized = false;
			xTiltAngle = 0f;
			zTiltAngle = 0f;
			xForce = 0f;
			zForce = 0f;
			OpenCabine(getIn: false);
			deInitAction = callbackAfterDeInit;
			HelicopterSpecific helicopterSpecific = VehicleSpecific as HelicopterSpecific;
			if (helicopterSpecific != null)
			{
				Invoke("DeferredDeInit", helicopterSpecific.GetOutAnimationTime);
			}
		}

		public override void StopVehicle(bool inMoment = false)
		{
			base.StopVehicle(inMoment);
			IsInitialized = false;
			currentHelicopter.CurrentBladesRotateSpeed = Mathf.Min(currentHelicopter.CurrentBladesRotateSpeed, AvaibleToFlyBladesRotateSpeed);
		}

		protected override void Drowning()
		{
			base.Drowning();
			IsInitialized = false;
			if (currentHelicopter.CurrentBladesRotateSpeed > AvaibleToFlyBladesRotateSpeed)
			{
				currentHelicopter.CurrentBladesRotateSpeed = AvaibleToFlyBladesRotateSpeed;
			}
			if (currentHelicopter.CurrentBladesRotateSpeed > 0f)
			{
				currentHelicopter.CurrentBladesRotateSpeed -= Time.deltaTime;
			}
			xAccelerationInput = 0f;
			zAccelerationInput = 0f;
			TiltControl();
			verticalMovementInput = -1f;
			VerticalControl();
		}

		protected override void SetCameraFollowTarget(DrivableVehicle drivableVehicle)
		{
			StartCoroutine(DefferedSetCameraTerget(drivableVehicle));
		}

		private IEnumerator DeferredInit()
		{
			yield return new WaitForSeconds(5f);
			IsInitialized = true;
			MainRigidbody.angularDrag = RigidbodyAngularDrag;
		}

		private IEnumerator DefferedSetCameraTerget(DrivableVehicle drivableVehicle)
		{
			yield return new WaitForSeconds(5f);
			base.SetCameraFollowTarget(drivableVehicle);
		}

		private void DeferredDeInit()
		{
			base.DeInit(deInitAction);
		}

		protected override void FixedUpdate()
		{
			if (DrivableVehicle.WaterSensor.InWater)
			{
				Drowning();
			}
			if (!IsInitialized)
			{
				return;
			}
			base.FixedUpdate();
			Inputs();
			BladesControl();
			GroundCheck();
			MainRigidbody.useGravity = !avaibleToFly;
			if (avaibleToFly)
			{
				VerticalControl();
				if (!isGrounded)
				{
					TiltControl();
					HorizontralRotateControl();
					return;
				}
				xTiltAngle = 0f;
				zTiltAngle = 0f;
				xForce = 0f;
				zForce = 0f;
			}
		}

		private void OpenCabine(bool getIn)
		{
			HelicopterSpecific helicopterSpecific = VehicleSpecific as HelicopterSpecific;
			if (helicopterSpecific != null && helicopterSpecific.CabinAnimator != null && currentHelicopter.AnimateGetInOut)
			{
				helicopterSpecific.CabinAnimator.SetBool("EnterIn", getIn);
				helicopterSpecific.CabinAnimator.SetTrigger("Open");
			}
		}

		private void Inputs()
		{
			xAccelerationInput = Controls.GetAxis("Vertical");
			zAccelerationInput = 0f - Controls.GetAxis("Horizontal");
			verticalMovementInput = Controls.GetAxis("Vertical_Heli");
		}

		private void GroundCheck()
		{
			isGrounded = Physics.Raycast(MainRigidbody.transform.position + MainRigidbody.transform.up * 0.2f, -MainRigidbody.transform.up, 1f, GroundLayerMask);
		}

		private void VerticalControl()
		{
			if (verticalMovementInput != 0f && (Math.Sign(verticalMovementInput) == Math.Sign(verticalForce) || Math.Round(verticalForce, 1) == 0.0))
			{
				verticalForce += VerticalMovementVelocity * verticalMovementInput;
			}
			else if (verticalMovementInput != 0f && Math.Sign(verticalMovementInput) != Math.Sign(verticalForce))
			{
				verticalForce = Mathf.Lerp(verticalForce, 0f, Time.deltaTime * (float)VerticalMovementBrakeForce);
			}
			else
			{
				verticalForce = Mathf.Lerp(verticalForce, 0f, Time.deltaTime);
			}
			verticalForce = Mathf.Clamp(verticalForce, 0f - MaxVerticalVelocity, MaxVerticalVelocity);
			Vector3 position = MainRigidbody.transform.position;
			if (position.y >= MaximumHelicopterY)
			{
				verticalForce = 0f - MaxVerticalVelocity;
			}
			MainRigidbody.AddForce(MainRigidbody.transform.up * verticalForce, ForceMode.Acceleration);
		}

		private void TiltControl()
		{
			xTiltAngle += TiltAngleSpeed * xAccelerationInput;
			zTiltAngle += TiltAngleSpeed * zAccelerationInput;
			xTiltAngle = Mathf.Clamp(xTiltAngle, 0f - MaxTiltAngle, MaxTiltAngle);
			zTiltAngle = Mathf.Clamp(zTiltAngle, 0f - MaxTiltAngle, MaxTiltAngle);
			xForce += 0f - SideMovementForce * zAccelerationInput;
			zForce += SideMovementForce * xAccelerationInput;
			xForce = Mathf.Clamp(xForce, 0f - MaximumSideForce, MaximumSideForce);
			zForce = Mathf.Clamp(zForce, 0f - MaximumSideForce, MaximumSideForce);
			if (xAccelerationInput == 0f)
			{
				xTiltAngle = Mathf.LerpAngle(xTiltAngle, 0f, Time.deltaTime);
				zForce = Mathf.Lerp(zForce, 0f, Time.deltaTime);
			}
			if (zAccelerationInput == 0f)
			{
				zTiltAngle = Mathf.LerpAngle(zTiltAngle, 0f, Time.deltaTime);
				xForce = Mathf.Lerp(xForce, 0f, Time.deltaTime);
			}
			Transform transform = MainRigidbody.transform;
			float x = xTiltAngle;
			Vector3 localEulerAngles = MainRigidbody.transform.localEulerAngles;
			transform.localEulerAngles = new Vector3(x, localEulerAngles.y, zTiltAngle);
			Vector3 force = MainRigidbody.transform.TransformDirection(xForce, 0f, zForce);
			force.y = 0f;
			MainRigidbody.AddForce(force, ForceMode.Acceleration);
		}

		private void HorizontralRotateControl()
		{
			Vector3 vector = MainRigidbody.transform.InverseTransformPoint(cameraTransform.position + cameraTransform.transform.forward * 100f);
			float num = vector.x / vector.magnitude;
			float num2 = (!(num > 0f)) ? (0f - TorqueForce) : TorqueForce;
			MainRigidbody.AddRelativeTorque(0f, num2 * Mathf.Abs(num), 0f, ForceMode.Force);
		}

		private void BladesControl()
		{
			currentHelicopter.CurrentBladesRotateSpeed += Time.deltaTime;
			currentHelicopter.CurrentBladesRotateSpeed = Mathf.Clamp(currentHelicopter.CurrentBladesRotateSpeed, 0f, MaxBladesRotateSpeed);
			avaibleToFly = (currentHelicopter.CurrentBladesRotateSpeed >= AvaibleToFlyBladesRotateSpeed);
		}

		private void EvacuationTilt()
		{
			MainRigidbody.useGravity = true;
			MainRigidbody.angularDrag = 0.05f;
			Vector3 a = MainRigidbody.transform.TransformDirection(Vector3.right);
			Vector3 a2 = MainRigidbody.transform.TransformDirection(-Vector3.forward);
			MainRigidbody.AddForce(a * 2f, ForceMode.VelocityChange);
			MainRigidbody.AddTorque(a2 * 2f, ForceMode.Acceleration);
		}
	}
}
