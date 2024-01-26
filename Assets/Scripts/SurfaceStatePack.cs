using System;

namespace Game.Character
{
	[Serializable]
	public struct SurfaceStatePack
	{
		public bool AboveGround;

		public bool AboveWater;

		public bool InWater;

		public SurfaceStatePack(bool aboveGround, bool aboveWater, bool inWater)
		{
			AboveGround = aboveGround;
			AboveWater = aboveWater;
			InWater = inWater;
		}

		public void SetTypePack(bool aboveGround, bool aboveWater, bool inWater)
		{
			AboveGround = aboveGround;
			AboveWater = aboveWater;
			InWater = inWater;
		}
	}
}
