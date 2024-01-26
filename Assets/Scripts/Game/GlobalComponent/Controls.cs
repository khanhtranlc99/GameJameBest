using Game.Vehicle;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Game.GlobalComponent
{
	public class Controls : MonoBehaviour
	{
		public static void SetControlsByVehicle(VehicleType vehicleType)
		{
			switch (vehicleType)
			{
			case VehicleType.Any:
			case VehicleType.Boat:
			case VehicleType.Plane:
				break;
			case VehicleType.Car:
				SetControlsByType(ControlsType.Car);
				break;
			case VehicleType.Bicycle:
				SetControlsByType(ControlsType.Bike);
				break;
			case VehicleType.Motorbike:
				SetControlsByType(ControlsType.Moto);
				break;
			case VehicleType.Copter:
				SetControlsByType(ControlsType.Copter);
				break;
			case VehicleType.Tank:
				SetControlsByType(ControlsType.Tank);
				break;
			case VehicleType.Mech:
				SetControlsByType(ControlsType.Mech);
				break;
			}
		}

		public static void SetControlsByType(ControlsType controlsType)
		{
			ControlsPanelManager.Instance.SwitchPanel(controlsType);
		}

		public static void SetControlsSubPanel(ControlsType controlsType, int index = 0)
		{
			ControlsPanelManager.Instance.SwitchSubPanel(controlsType, index);
		}

		public static float GetAxis(string axeName)
		{
			return CrossPlatformInputManager.GetAxis(axeName);
		}

		public static bool GetButton(string btnName)
		{
			return CrossPlatformInputManager.GetButton(btnName);
		}

		public static bool GetButtonUp(string btnName)
		{
			return CrossPlatformInputManager.GetButtonUp(btnName);
		}

		public static bool GetButtonDown(string btnName)
		{
			return CrossPlatformInputManager.GetButtonDown(btnName);
		}
	}
}
