using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class ReachPointTask : BaseTask
	{
		public Vector3 PointPosition;

		public float AdditionalPointRadius;

		private QwestPoint point;

		public override void TaskStart()
		{
			base.TaskStart();
			point = PoolManager.Instance.GetFromPool<QwestPoint>(GameEventManager.Instance.QwestPointPrefab.gameObject);
			PoolManager.Instance.AddBeforeReturnEvent(point, delegate
			{
				Target = null;
				if (point != null)
				{
					point.Task = null;
					point = null;
				}
			});
			point.Task = this;
			point.transform.parent = GameEventManager.Instance.transform;
			point.transform.position = PointPosition;
			point.transform.localScale = new Vector3(1f + AdditionalPointRadius, 1f + AdditionalPointRadius, 1f + AdditionalPointRadius);
			Target = point.transform;
		}

		public override void Finished()
		{
			if (point != null)
			{
				PoolManager.Instance.ReturnToPool(point);
			}
			base.Finished();
		}

		public override void PointReachedEvent(Vector3 position, BaseTask task)
		{
			if (Equals(task))
			{
				point = null;
				CurrentQwest.MoveToTask(NextTask);
			}
		}
	}
}
