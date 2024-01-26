using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CameraEffects
{
	public class Fall : CameraEffect
	{
		public float Mass;

		public float Distance;

		public float Strength;

		public float Damping;

		public float Force;

		public int ForceFrames;

		public float ImpactVelocity;

		private Spring spring;

		private float frameCounter;

		private float DistanceMax;

		public override void Init()
		{
			base.Init();
			spring = new Spring();
			spring.Setup(Mass, Distance, Strength, Damping);
		}

		public override void OnPlay()
		{
			frameCounter = ForceFrames;
			spring.Setup(Mass, Distance, Strength, Damping);
		}

		public override void OnUpdate()
		{
			if (frameCounter > 0f)
			{
				spring.AddForce(Force);
				frameCounter -= 1f;
			}
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
			float num2 = Mathf.Clamp01(ImpactVelocity / 10f);
			DistanceMax = num2 * 2f;
			if (num > DistanceMax)
			{
				num = DistanceMax;
			}
			unityCamera.transform.position += Vector3.up * num * d * -1f;
		}
	}
}
