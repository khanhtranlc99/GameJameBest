using UnityEngine;

namespace Game.GlobalComponent
{
	public abstract class PerformanceTest : MonoBehaviour
	{
		public delegate void CalcOfResults(float result, PerformanceTest test);

		[HideInInspector]
		public bool IsEnd;

		public bool IsNotReturnResults;

		public float DetectingTime;

		[Range(0f, 100f)]
		protected float Result;

		public event CalcOfResults EndTestingEvent;

		public abstract void Init();

		public virtual float GetResult()
		{
			return Result;
		}

		public void CallEndTestingEvent(float result, PerformanceTest test)
		{
			IsEnd = true;
			if (this.EndTestingEvent != null)
			{
				this.EndTestingEvent(result, test);
			}
		}

		public abstract void EndTesting();
	}
}
