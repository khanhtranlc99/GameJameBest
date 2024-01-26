using UnityEngine;

namespace Game.GlobalComponent
{
	public class SlowUpdateProc
	{
		public delegate void SlowUpdateDelegate();

		private readonly SlowUpdateDelegate slowUpdate;

		private float updateTime;

		private float updateCurrentTime;

		public float DeltaTime => updateTime - updateCurrentTime;

		public float UpdateTime
		{
			get
			{
				return updateTime;
			}
			set
			{
				updateTime = value;
			}
		}

		public SlowUpdateProc(SlowUpdateDelegate slowUpdate, float updateTime)
		{
			this.slowUpdate = slowUpdate;
			this.updateTime = updateTime;
		}

		public void ProceedOnFixedUpdate()
		{
			if (updateCurrentTime <= 0f)
			{
				slowUpdate();
				updateCurrentTime += updateTime;
			}
			updateCurrentTime -= Time.deltaTime;
		}
	}
}
