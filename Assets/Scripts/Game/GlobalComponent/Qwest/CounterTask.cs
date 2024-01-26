namespace Game.GlobalComponent.Qwest
{
	public class CounterTask : BaseTask
	{
		public int TaskChecksCount;

		public BaseTask ReturnTask;

		private int checkedCount;

		public override void Init(Qwest qwest)
		{
			base.Init(qwest);
			checkedCount = 0;
		}

		public override void TaskStart()
		{
			checkedCount++;
			if (checkedCount >= TaskChecksCount)
			{
				CurrentQwest.MoveToTask(NextTask);
				return;
			}
			base.TaskStart();
			CurrentQwest.MoveToTask(ReturnTask);
		}
	}
}
