using System;
using System.Collections;
using UnityEngine;

namespace Common.Analytics
{
	public class FPSCountersController
	{
		private const float TimeThresholdToStopCountFPS = 40f;

		private const float BoundScale = 0.4f;

		private const float UpperScale = 1.4f;

		private const float LowerScale = 0.6f;

		public FPSCounter NormalFPS = new FPSCounter();

		public FPSCounter SpikeFPS = new FPSCounter();

		private int frameCount;

		private float movingAverageSec;

		private float upperBound;

		private float lowerBound;

		public void AddFrame(float deltaTime)
		{
			frameCount++;
			deltaTime = Time.deltaTime;
			movingAverageSec += (deltaTime - movingAverageSec) / (float)frameCount;
			upperBound = movingAverageSec * 1.4f;
			lowerBound = movingAverageSec * 0.6f;
			if (deltaTime >= lowerBound && deltaTime <= upperBound)
			{
				NormalFPS.Count(deltaTime);
			}
			else if (deltaTime > upperBound)
			{
				SpikeFPS.Count(deltaTime);
			}
		}

		public int GetPercentageFPSAtUpperBound()
		{
			float num = SpikeFPS.SecodsCount / (float)SpikeFPS.HitsCount;
			return (int)(100f * num / upperBound) - 100;
		}

		public int GetMovingAverageFPS()
		{
			return (int)(1f / movingAverageSec);
		}

		public IEnumerator StartCount(Action callback)
		{
			WaitForEndOfFrame waitEndFrame = new WaitForEndOfFrame();
			float startTime = Time.timeSinceLevelLoad;
			while (Time.timeSinceLevelLoad - startTime <= 40f)
			{
				AddFrame(Time.deltaTime);
				yield return waitEndFrame;
			}
			callback();
		}
	}
}
