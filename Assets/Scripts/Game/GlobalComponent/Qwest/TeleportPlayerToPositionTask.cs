using Game.Character;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class TeleportPlayerToPositionTask : BaseTask
	{
		public Vector3 targetPosition;

		public Quaternion targetRotarion;

		public override void TaskStart()
		{
			base.TaskStart();
			if (PlayerInteractionsManager.Instance != null && PlayerInteractionsManager.Instance.TeleportPlayerToPosition(targetPosition, targetRotarion))
			{
				CurrentQwest.MoveToTask(NextTask);
			}
		}

		public override void Finished()
		{
			base.Finished();
		}
	}
}
