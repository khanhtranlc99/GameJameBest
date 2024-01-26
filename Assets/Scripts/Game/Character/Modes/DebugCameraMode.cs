using Game.Character.Config;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(DebugConfig))]
	public class DebugCameraMode : CameraMode
	{
		private float rotX;

		private float rotY;

		public override Type Type => Type.Debug;

		public override void OnActivate()
		{
			base.OnActivate();
			if ((bool)Target)
			{
				cameraTarget = Target.position;
				RotateCamera(Vector2.zero);
			}
		}

		public override void Init()
		{
			base.Init();
			UnityCamera.transform.LookAt(cameraTarget);
			config = GetComponent<DebugConfig>();
		}

		private void UpdateFOV()
		{
			UnityCamera.fieldOfView = config.GetFloat("FOV");
		}

		public void RotateCamera(Vector2 mousePosition)
		{
			Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
			rotY += config.GetFloat("RotationSpeedY") * mousePosition.y * 0.01f;
			rotX += config.GetFloat("RotationSpeedX") * mousePosition.x * 0.01f;
		}

		public void MoveCamera()
		{
			Vector3 a = Vector3.zero;
			if (UnityEngine.Input.GetKey(KeyCode.W))
			{
				a += UnityCamera.transform.forward;
			}
			if (UnityEngine.Input.GetKey(KeyCode.S))
			{
				a += -UnityCamera.transform.forward;
			}
			if (UnityEngine.Input.GetKey(KeyCode.A))
			{
				a += -UnityCamera.transform.right;
			}
			if (UnityEngine.Input.GetKey(KeyCode.D))
			{
				a += UnityCamera.transform.right;
			}
			a.Normalize();
			UnityCamera.transform.position += a * config.GetFloat("MoveSpeed") * Time.deltaTime * 10f;
		}

		private void UpdateDir()
		{
			Vector3 dir;
			Math.ToCartesian(rotX, rotY, out dir);
			UnityCamera.transform.forward = dir;
		}

		public override void PostUpdate()
		{
			UpdateFOV();
			if (CursorLocking.IsLocked)
			{
				RotateCamera(new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")));
			}
			MoveCamera();
			UpdateDir();
		}
	}
}
