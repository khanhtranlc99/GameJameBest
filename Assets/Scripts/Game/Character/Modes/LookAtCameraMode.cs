using Game.Character.Config;
using Game.Character.Utils;
using System;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(LookAtConfig))]
	public class LookAtCameraMode : CameraMode
	{
		public delegate void OnLookAtFinished();

		private Vector3 newTarget;

		private Vector3 oldTarget;

		private Quaternion oldRot;

		private Quaternion newRot;

		private float targetTimeoutMax;

		private float targetTimeout;

		private OnLookAtFinished finishedCallback;

		public override Type Type => Type.LookAt;

		public override void Init()
		{
			base.Init();
			UnityCamera.transform.LookAt(cameraTarget);
			config = GetComponent<LookAtConfig>();
			targetTimeout = -1f;
			targetTimeoutMax = 1f;
		}

		public override void OnActivate()
		{
			ApplyCurrentCamera();
		}

		public void RegisterFinishCallback(OnLookAtFinished callback)
		{
			finishedCallback = (OnLookAtFinished)Delegate.Combine(finishedCallback, callback);
		}

		public void UnregisterFinishCallback(OnLookAtFinished callback)
		{
			finishedCallback = (OnLookAtFinished)Delegate.Remove(finishedCallback, callback);
		}

		public void ApplyCurrentCamera()
		{
			Ray ray = new Ray(UnityCamera.transform.position, UnityCamera.transform.forward);
			Vector3 cameraTarget = ray.origin + ray.direction * 100f;
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.MaxValue))
			{
				cameraTarget = hitInfo.point;
			}
			base.cameraTarget = cameraTarget;
			targetDistance = (UnityCamera.transform.position - base.cameraTarget).magnitude;
		}

		public void LookAt(Vector3 point, float timeout)
		{
			LookAt(UnityCamera.transform.position, point, timeout);
		}

		public void LookAt(Vector3 from, Vector3 point, float timeout)
		{
			oldTarget = cameraTarget;
			oldRot = UnityCamera.transform.rotation;
			newTarget = point;
			if (timeout < 0f)
			{
				timeout = 0f;
			}
			newRot = Quaternion.LookRotation(point - from);
			targetTimeoutMax = timeout;
			targetTimeout = timeout;
		}

		public void LookFrom(Vector3 from, float timeout)
		{
			LookAt(from, cameraTarget, timeout);
		}

		public override void FixedStepUpdate()
		{
			LookAt(Target.position + Vector3.up, 0.1f);
		}

		private void UpdateLookAt()
		{
			if (targetTimeout >= 0f)
			{
				targetTimeout -= Time.deltaTime;
				float t = (!(targetTimeoutMax < Mathf.Epsilon)) ? (1f - targetTimeout / targetTimeoutMax) : 1f;
				if (config.GetBool("InterpolateTarget"))
				{
					cameraTarget = Vector3.Lerp(oldTarget, newTarget, Time.deltaTime * 2f);
					UnityCamera.transform.LookAt(cameraTarget);
				}
				else
				{
					Quaternion b = Quaternion.Slerp(oldRot, newRot, Interpolation.LerpS(0f, 1f, t));
					UnityCamera.transform.rotation = Quaternion.Slerp(UnityCamera.transform.rotation, b, Time.deltaTime);
				}
				if (targetTimeout < 0f && finishedCallback != null)
				{
					finishedCallback();
				}
			}
		}

		private void UpdateFOV()
		{
			UnityCamera.fieldOfView = config.GetFloat("FOV");
		}

		public override void PostUpdate()
		{
			UpdateFOV();
			UpdateLookAt();
		}
	}
}
