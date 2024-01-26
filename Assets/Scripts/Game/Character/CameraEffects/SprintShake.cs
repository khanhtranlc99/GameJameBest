using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class SprintShake : CameraEffect
	{
		public float Size = 1f;

		public float Speed = 10f;

		private Vector3 diff;

		private float size;

		public override void OnPlay()
		{
			diff = Vector2.zero;
		}

		public override void OnUpdate()
		{
			Vector3 eulerAngles = unityCamera.transform.rotation.eulerAngles;
			switch (fadeState)
			{
			case FadeState.FadeIn:
				size = Interpolation.LerpS3(0f, Size, 1f - fadeInNormalized);
				break;
			case FadeState.FadeOut:
				size = Interpolation.LerpS2(Size, 0f, fadeOutNormalized);
				break;
			case FadeState.Full:
				size = Size;
				break;
			}
			Vector3 b = SmoothRandom.GetVector3(Speed) * size;
			Vector3 euler = eulerAngles - diff + b;
			diff = b;
			unityCamera.transform.rotation = Quaternion.Euler(euler);
		}
	}
}
