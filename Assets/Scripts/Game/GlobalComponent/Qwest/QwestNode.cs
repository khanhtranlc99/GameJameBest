using Game.MiniMap;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestNode : MonoBehaviour
	{
		public string QwestTitle;

		public UniversalReward Rewards;

		public bool ShowQwestCompletePanel;

		[Header("If timer not used, value must be zero.")]
		public float TimerValue;

		[Header("After finishing quest, it will appear again.")]
		public bool RepeatableQuest;

		[Header("Mark from a GameEventManager.MapMarksList")]
		public MarkForMiniMap MMMark;

		[TextArea]
		public string StartDialog;

		[TextArea]
		public string EndDialog;

		[Header("Will use its own position if null")]
		public Transform StartPosition;

		public float AdditionalPointRadius;

		[Separator("Debug")]
		public bool IsDebug;

		public Qwest ToPo()
		{
			List<BaseTask> list = new List<BaseTask>();
			List<TaskNode> list2 = new List<TaskNode>();
			BaseTask baseTask = null;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				TaskNode component = base.transform.GetChild(i).GetComponent<TaskNode>();
				if (component != null)
				{
					BaseTask baseTask2 = component.ToPo();
					if (baseTask != null)
					{
						baseTask.NextTask = baseTask2;
						baseTask2.PrevTask = baseTask;
					}
					list.Add(baseTask2);
					list2.Add(component);
					baseTask = baseTask2;
				}
			}
			for (int j = 0; j < list2.Count; j++)
			{
				list2[j].ProceedTaskLinks(list[j], list, list2);
			}
			int mMMarkId = -1;
			if (MMMark != null)
			{
				mMMarkId = MMMark.transform.GetSiblingIndex();
			}
			Qwest qwest = new Qwest(list.ToArray());
			qwest.QwestTitle = QwestTitle;
			qwest.Rewards = Rewards;
			qwest.ShowQwestCompletePanel = ShowQwestCompletePanel;
			qwest.TimerValue = TimerValue;
			qwest.RepeatableQuest = RepeatableQuest;
			qwest.StartDialog = StartDialog;
			qwest.EndDialog = EndDialog;
			qwest.MMMarkId = mMMarkId;
			qwest.StartPosition = ((!(StartPosition == null)) ? StartPosition.transform.position : base.transform.position);
			qwest.AdditionalStartPointRadius = AdditionalPointRadius;
			return qwest;
		}

		private void OnDrawGizmos()
		{
			if (IsDebug)
			{
				Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere((!(StartPosition == null)) ? StartPosition.transform.position : base.transform.position, 2f);
			}
		}
	}
}
