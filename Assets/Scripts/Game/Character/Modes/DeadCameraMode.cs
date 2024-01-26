using Game.Character.Config;
using Game.Character.Utils;
using System;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(DeadConfig))]
	public class DeadCameraMode : CameraMode
	{
		private float rotX;

		private float rotY;

		private float angle;

		public override Type Type => Type.Dead;

		public override void Init()
		{
			base.Init();
			UnityCamera.transform.LookAt(cameraTarget);
			config = GetComponent<DeadConfig>();
		}

		public override void OnActivate()
		{
			base.OnActivate();
			targetDistance = (cameraTarget - UnityCamera.transform.position).magnitude;
		}

		private void RotateCamera()
		{
			Game.Character.Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
			angle = config.GetFloat("RotationSpeed") * Time.deltaTime;
			rotY = (0f - config.GetFloat("Angle")) * ((float)System.Math.PI / 180f);
			rotX += angle;
		}

		private void UpdateFOV()
		{
			UnityCamera.fieldOfView = config.GetFloat("FOV");
		}

		private Vector3 GetOffsetPos()
		{
			Vector3 vector = config.GetVector3("TargetOffset");
			Vector3 a = Game.Character.Utils.Math.VectorXZ(UnityCamera.transform.forward);
			Vector3 a2 = Game.Character.Utils.Math.VectorXZ(UnityCamera.transform.right);
			Vector3 up = Vector3.up;
			return a * vector.z + a2 * vector.x + up * vector.y;
		}

		public override void PostUpdate()
		{
			UpdateFOV();
			RotateCamera();
			if (config.GetBool("Collision"))
			{
				UpdateCollision();
			}
			UpdateDir();
		}

		private void UpdateDir()
		{
			Vector3 dir;
			Game.Character.Utils.Math.ToCartesian(rotX, rotY, out dir);
			UnityCamera.transform.forward = dir;
			cameraTarget = Target.position + GetOffsetPos();
			UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
		}

		private void UpdateCollision()
		{
			float @float = config.GetFloat("Distance");
			float _; 
			float collisionDistance;
			collision.ProcessCollision(cameraTarget, GetTargetHeadPos(), UnityCamera.transform.forward, @float, out _, out collisionDistance);
			targetDistance = Interpolation.Lerp(targetDistance, collisionDistance, (!(targetDistance > collisionDistance)) ? collision.GetReturnSpeed() : collision.GetClipSpeed());
		}
	}
}
