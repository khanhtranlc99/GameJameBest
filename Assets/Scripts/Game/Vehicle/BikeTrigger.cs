using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Vehicle
{
	public class BikeTrigger : MonoBehaviour
	{
		private static int collisionSensorLayerNumber = -1;

		private VehicleController controller;

		private void Awake()
		{
			if (collisionSensorLayerNumber == -1)
			{
				collisionSensorLayerNumber = LayerMask.NameToLayer("CollisionSensor");
			}
		}

		public void Init()
		{
			controller = base.transform.parent.GetComponent<DrivableVehicle>().controller;
		}

		private void OnTriggerEnter(Collider col)
		{
			if (!col.GetComponentInParent<Player>() && col.gameObject.layer != collisionSensorLayerNumber && PlayerInteractionsManager.Instance.inVehicle && (bool)controller && controller.isActiveAndEnabled)
			{
				controller.DropFrom();
			}
		}
	}
}
