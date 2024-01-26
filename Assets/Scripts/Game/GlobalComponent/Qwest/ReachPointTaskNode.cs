using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class ReachPointTaskNode : TaskNode
	{
		[Header("Will use its own position if null")]
		[Space]
		[Separator("Specific")]
		public Transform PointPosition;

		public float AdditionalPointRadius;

		public override BaseTask ToPo()
		{
			ReachPointTask reachPointTask = new ReachPointTask();
			reachPointTask.PointPosition = ((!(PointPosition == null)) ? PointPosition.position : base.transform.position);
			reachPointTask.AdditionalPointRadius = AdditionalPointRadius;
			ReachPointTask reachPointTask2 = reachPointTask;
			ToPoBase(reachPointTask2);
			return reachPointTask2;
		}

		private void OnDrawGizmos()
		{
			if (IsDebug)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere((!(PointPosition == null)) ? PointPosition.position : base.transform.position, 2f);
			}
		}
	}
}
