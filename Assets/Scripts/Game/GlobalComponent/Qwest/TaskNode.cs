using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class TaskNode : MonoBehaviour
	{
		public string TaskText;

		public TaskNode PrevTask;

		public TaskNode NextTask;

		public float AdditionalTimer;

		[TextArea]
		public string DialogData;

		[TextArea]
		public string EndDialogData;

		[Separator("Debug")]
		public bool IsDebug;

		public virtual BaseTask ToPo()
		{
			return new BaseTask();
		}

		public virtual void ProceedTaskLinks(BaseTask task, List<BaseTask> tasks, List<TaskNode> nodes)
		{
			task.NextTask = GetBaseTask(NextTask, tasks, nodes);
			task.PrevTask = GetBaseTask(PrevTask, tasks, nodes);
		}

		protected BaseTask GetBaseTask(TaskNode node, List<BaseTask> tasks, List<TaskNode> nodes)
		{
			if (node == null)
			{
				return null;
			}
			for (int i = 0; i < nodes.Count; i++)
			{
				TaskNode taskNode = nodes[i];
				if (taskNode.Equals(node))
				{
					return tasks[i];
				}
			}
			throw new Exception(string.Format("TaskNode-Task corresponding failed {4}->{0}({1})-{2}({3}) ", base.name, GetInstanceID(), NextTask.name, NextTask.GetInstanceID(), base.transform.parent.name));
		}

		protected void ToPoBase(BaseTask task)
		{
			task.TaskText = TaskText;
			task.DialogData = DialogData;
			task.EndDialogData = EndDialogData;
			task.AdditionalTimer = AdditionalTimer;
		}
	}
}
