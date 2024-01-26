using UnityEngine;

namespace Game.Vehicle
{
	public class CollisionTrigger : MonoBehaviour
	{
		public enum Sensor
		{
			Front,
			Right,
			Left,
			RightBlocking,
			LeftBlocking
		}

		public Sensor SensorType;

		private Autopilot autopilot;

		private void Awake()
		{
			if (autopilot == null)
			{
				autopilot = GetComponentInParent<Autopilot>();
			}
		}

		private void Update()
		{
			if (autopilot == null)
			{
				autopilot = GetComponentInParent<Autopilot>();
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (autopilot != null)
			{
				autopilot.OnSensorStay(other, SensorType);
			}
		}
	}
}
