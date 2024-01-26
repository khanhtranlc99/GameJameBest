namespace Common.Analytics
{
	public class FPSCounter
	{
		public int HitsCount;

		public float SecodsCount;

		public void Count(float sec)
		{
			HitsCount++;
			SecodsCount += sec;
		}

		public float GetAverageOfSeconds()
		{
			return SecodsCount / (float)HitsCount;
		}

		public int GetFPS()
		{
			return (int)(1f / GetAverageOfSeconds());
		}
	}
}
