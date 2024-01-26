using System.Collections.Generic;

namespace Game.GlobalComponent.Qwest
{
	public class CounterTaskNode : TaskNode
	{
		[Separator("Specific")]
		public int Counts;

		public TaskNode ReturnTask;

		public override BaseTask ToPo()
		{
			CounterTask counterTask = new CounterTask();
			counterTask.TaskChecksCount = Counts;
			CounterTask counterTask2 = counterTask;
			ToPoBase(counterTask2);
			return counterTask2;
		}

		public override void ProceedTaskLinks(BaseTask task, List<BaseTask> tasks, List<TaskNode> nodes)
		{
			base.ProceedTaskLinks(task, tasks, nodes);
			CounterTask counterTask = task as CounterTask;
			if (counterTask != null)
			{
				counterTask.ReturnTask = GetBaseTask(ReturnTask, tasks, nodes);
			}
		}
	}
}
