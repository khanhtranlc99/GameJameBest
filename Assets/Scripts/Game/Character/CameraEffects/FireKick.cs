using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class FireKick : CameraEffect
	{
		public float KickTime;

		public float KickAngle;

		private float diff;

		private float kickTimeout;

		public override void OnPlay()
		{
			diff = 0f;
			KickTime = Mathf.Clamp(KickTime, 0f, Length);
		}

		public override void OnUpdate()
		{
			Vector3 eulerAngles = unityCamera.transform.rotation.eulerAngles;
			float num = 0f;
			if (timeout < KickTime)
			{
				float t = timeout / KickTime;
				num = Interpolation.LerpS2(0f, KickAngle, t);
			}
			else
			{
				float t2 = (timeout - KickTime) / (Length - KickTime);
				num = Interpolation.LerpS(KickAngle, 0f, t2);
			}
			num = 0f - num;
			float x = eulerAngles.x - diff + num;
			diff = num;
			Vector3 euler = eulerAngles;
			euler.x = x;
			unityCamera.transform.rotation = Quaternion.Euler(euler);
		}
	}
}
