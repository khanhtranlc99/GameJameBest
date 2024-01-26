using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class TeleportPlayerToPositionTaskNode : TaskNode
	{
		[Separator("Specific")]
		public bool m_LoadPositionFromTargetTransform;

		public Transform m_TargetPoint;

		[SerializeField]
		private Vector3 m_TargetPositon;

		[SerializeField]
		private Quaternion m_TargetRotation;

		[InspectorButton("UpdatePostions")]
		public bool UpdatePosition;

		public override BaseTask ToPo()
		{
			if (m_LoadPositionFromTargetTransform)
			{
				m_TargetPositon = m_TargetPoint.position;
				m_TargetRotation = m_TargetPoint.rotation;
			}
			TeleportPlayerToPositionTask teleportPlayerToPositionTask = new TeleportPlayerToPositionTask();
			teleportPlayerToPositionTask.targetPosition = m_TargetPositon;
			teleportPlayerToPositionTask.targetRotarion = m_TargetRotation;
			TeleportPlayerToPositionTask teleportPlayerToPositionTask2 = teleportPlayerToPositionTask;
			ToPoBase(teleportPlayerToPositionTask2);
			return teleportPlayerToPositionTask2;
		}

		private void UpdatePostions()
		{
			if (m_LoadPositionFromTargetTransform)
			{
				m_TargetPositon = m_TargetPoint.position;
				m_TargetRotation = m_TargetPoint.rotation;
			}
		}
	}
}
