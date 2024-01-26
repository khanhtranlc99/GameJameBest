	using Game.Character.CharacterController;
using Game.Enemy;
using Game.Managers;
using Game.Vehicle;
using UnityEngine;

	namespace Game.Character
{
	public class NPCSensor : MonoBehaviour
	{
		public bool DebugLog;

		public SmartHumanoidController SmartNpcController;

		private void OnTriggerEnter(Collider other)
		{
			if (SmartNpcController == null || !SmartNpcController.IsInited)
			{
				return;
			}
			HitEntity componentInParent = other.GetComponentInParent<HitEntity>();
			DrivableVehicle componentInParent2 = other.GetComponentInParent<DrivableVehicle>();
			if (componentInParent != null)
			{
				SmartNpcController.AddTarget(componentInParent);
			}
			if (componentInParent2 != null && componentInParent2.CurrentDriver != null)
			{
				if (componentInParent2.DriverIsVulnerable)
				{
					SmartNpcController.AddTarget(componentInParent2.CurrentDriver, toSecondary: true);
				}
				else
				{
					SmartNpcController.AddTarget(componentInParent2.GetVehicleStatus());
				}
			}
			if (SmartNpcController.VehiclesAsTargets && !(componentInParent2 == null))
			{
				componentInParent = componentInParent2.GetVehicleStatus();
				if (componentInParent == null && GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log("Какая-то херь");
				}
				SmartNpcController.AddTarget(componentInParent, toSecondary: true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (SmartNpcController == null || !SmartNpcController.IsInited)
			{
				return;
			}
			HitEntity componentInParent = other.GetComponentInParent<HitEntity>();
			if (componentInParent != null)
			{
				SmartNpcController.RemoveTarget(componentInParent, fromSighnLine: true);
				if (DebugLog && GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log(componentInParent.name);
				}
			}
		}
	}
}
