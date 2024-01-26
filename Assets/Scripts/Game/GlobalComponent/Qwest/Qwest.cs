using Game.DialogSystem;
using Game.MiniMap;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class Qwest
	{
		public static int MinCharDialogLength = 2;

		public string Name;

		public string QwestTitle;

		public UniversalReward Rewards;

		public bool ShowQwestCompletePanel;

		public float TimerValue;

		public bool RepeatableQuest;

		public int MMMarkId;

		public MarkForMiniMap MarkForMiniMap;

		public BaseTask[] TasksList;

		public Qwest ParentQwest;

		public List<Qwest> QwestTree = new List<Qwest>();

		public Vector3 StartPosition;

		public float AdditionalStartPointRadius;

		public string StartDialog;

		public string EndDialog;

		private BaseTask currentTask;

		public bool IsTimeQwest => TimerValue > 0f;

		public Qwest()
		{
		}

		public Qwest(BaseTask[] tasksList)
		{
			TasksList = tasksList;
			currentTask = tasksList[0];
		}

		public void Init()
		{
			StartSelectedDialog(StartDialog);
			BaseTask[] tasksList = TasksList;
			foreach (BaseTask baseTask in tasksList)
			{
				baseTask.Init(this);
			}
			if (TimerValue > 0f)
			{
				QwestTimerManager.Instance.StartCountdown(TimerValue, this);
			}
			InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestStart, QwestTitle);
			currentTask = TasksList[0];
			currentTask.TaskStart();
		}

		public void MoveToTask(BaseTask task)
		{
			currentTask.Finished();
			if (task == null)
			{
				StartSelectedDialog(EndDialog);
				InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestFinished, QwestTitle);
				if (TimerValue > 0f)
				{
					QwestTimerManager.Instance.EndCountdown();
				}
				Rewards.GiveReward();
				if (ShowQwestCompletePanel)
				{
					QwestCompletePanel.Instance.ShowCompletedQwestInfo("Mission complete", Rewards);
				}
				GameEventManager.Instance.QwestResolved(this);
			}
			else
			{
				currentTask = task;
				if (TimerValue > 0f && task.AdditionalTimer > 0f)
				{
					QwestTimerManager.Instance.AddAdditionalTime(task.AdditionalTimer);
					InGameLogManager.Instance.RegisterNewMessage(MessageType.AddQuestTime, ((int)task.AdditionalTimer).ToString());
				}
				task.TaskStart();
				GameEventManager.Instance.RefreshQwestArrow();
			}
		}

		public BaseTask GetCurrentTask()
		{
			return currentTask;
		}

		public string GetQwestStatus()
		{
			return QwestTitle + ": " + currentTask.TaskStatus();
		}

		public Transform GetQwestTarget()
		{
			if (currentTask != null)
			{
				return currentTask.TaskTarget();
			}
			return null;
		}

		private void StartSelectedDialog(string dialog)
		{
			if (!string.IsNullOrEmpty(dialog) && dialog.Length > MinCharDialogLength)
			{
				DialogManager.Instance.StartDialog(dialog);
			}
		}
	}
}
