using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class ExplosionCFX : CameraEffect
	{
		public float Mass;

		public float Distance;

		public float Strength;

		public float Damping;

		public Vector3 position;

		public float Size = 1f;

		public float Speed = 10f;

		private float size;

		private Spring posSpring;

		private Vector3 v0;

		private Vector3 diff;

		public override void Init()
		{
			base.Init();
			posSpring = new Spring();
		}

		public override void OnPlay()
		{
			posSpring.Setup(Mass, Distance, Strength, Damping);
			v0 = (position - unityCamera.transform.position).normalized;
			diff = Vector3.zero;
		}

		public override void OnUpdate()
		{
			Vector3 eulerAngles = unityCamera.transform.rotation.eulerAngles;
			size = Size;
			float num = posSpring.Calculate(Time.deltaTime);
			float d = 1f;
			switch (fadeState)
			{
			case FadeState.FadeIn:
				d = Interpolation.LerpS3(0f, num, 1f - fadeInNormalized);
				size = Interpolation.LerpS3(0f, Size, 1f - fadeInNormalized);
				break;
			case FadeState.FadeOut:
				d = Interpolation.LerpS2(num, 0f, fadeOutNormalized);
				size = Interpolation.LerpS2(Size, 0f, fadeOutNormalized);
				break;
			}
			Vector3 b = SmoothRandom.GetVector3(Speed) * size;
			Vector3 euler = eulerAngles - diff + b;
			diff = b;
			unityCamera.transform.rotation = Quaternion.Euler(euler);
			unityCamera.transform.position += v0 * num * d;
		}
	}
}
