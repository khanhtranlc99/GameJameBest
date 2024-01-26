using UnityEngine;

namespace Game.Enemy
{
	public class SmartHumanoidCollisionTrigger : MonoBehaviour
	{
		public enum HumanoidSensorType
		{
			Front,
			Right,
			Left
		}

		public HumanoidSensorType SensorType;

		public SmartHumanoidController HumanoidController;

		private void OnTriggerStay(Collider col)
		{
			if (HumanoidController != null)
			{
				HumanoidController.UpdateSensorInfo(SensorType);
			}
		}
	}
}
