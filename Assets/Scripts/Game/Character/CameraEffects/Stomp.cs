using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class Stomp : CameraEffect
	{
		public float Mass;

		public float Distance;

		public float Strength;

		public float Damping;

		private Spring spring;

		public override void Init()
		{
			base.Init();
			spring = new Spring();
		}

		public override void OnPlay()
		{
			spring.Setup(Mass, Distance, Strength, Damping);
		}

		public override void OnUpdate()
		{
			float num = spring.Calculate(Time.deltaTime);
			float d = 1f;
			switch (fadeState)
			{
			case FadeState.FadeIn:
				d = Interpolation.LerpS3(0f, num, 1f - fadeInNormalized);
				break;
			case FadeState.FadeOut:
				d = Interpolation.LerpS2(num, 0f, fadeOutNormalized);
				break;
			}
			unityCamera.transform.position += Vector3.up * num * d;
		}
	}
}
