using Game.Character;
using Game.Vehicle;
using UnityEngine;

namespace MiamiGames.Mech
{
	public class MechSpiderController : MechController
	{
		private const float RayDistance = 2f;

		private const int CameraOffset = -100;

		private Transform cameraTransform;

		private Quaternion targetRotation;

		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			cameraTransform = CameraManager.Instance.UnityCamera.transform;
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (IsInitialized)
			{
				DetermineTargetRotation();
			}
		}

		private void Update()
		{
			if (IsInitialized)
			{
				StabilizationOnSurface();
				HorizontalRotateControl();
			}
		}

		private void HorizontalRotateControl()
		{
			Vector3 worldPosition = cameraTransform.position + cameraTransform.transform.forward * -100f;
			drivableMech.rotatedPart.transform.LookAt(worldPosition);
		}

		private void StabilizationOnSurface()
		{
			MainRigidbody.transform.rotation = Quaternion.Lerp(MainRigidbody.transform.rotation, targetRotation, Time.deltaTime * 5f);
		}

		private void DetermineTargetRotation()
		{
			Ray ray = new Ray(MainRigidbody.transform.position, -MainRigidbody.transform.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 2f, GroundMask))
			{
				targetRotation = Quaternion.FromToRotation(MainRigidbody.transform.up, hitInfo.normal) * MainRigidbody.transform.rotation;
			}
			else
			{
				targetRotation = Quaternion.identity;
			}
		}
	}
}
