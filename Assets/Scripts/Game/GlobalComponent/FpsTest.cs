using UnityEngine;

namespace Game.GlobalComponent
{
	public class FpsTest : PerformanceTest
	{
		public float MaxFps = 58f;

		private float timer;

		private float sumFps;

		private float fpsCounter;

		private float fps;

		private float absFps;

		private bool isInit;

		public override void Init()
		{
			isInit = true;
			timer = DetectingTime;
		}

		private void ChangeFps()
		{
			fps = 1f / Time.deltaTime;
			sumFps += fps;
			fpsCounter += 1f;
			absFps = sumFps / fpsCounter;
		}

		private void Update()
		{
			if (isInit)
			{
				Timer();
				ChangeFps();
			}
		}

		private void Timer()
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				isInit = false;
				absFps = fpsCounter / DetectingTime;
				EndTesting();
			}
		}

		public override void EndTesting()
		{
			Result = absFps * (100f / MaxFps);
			CallEndTestingEvent(Result, this);
		}
	}
}
