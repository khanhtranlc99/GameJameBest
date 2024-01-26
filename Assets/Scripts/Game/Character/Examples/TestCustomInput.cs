using Game.Character.Input;
using UnityEngine;

namespace Game.Character.Examples
{
	public class TestCustomInput : MonoBehaviour
	{
		private void Update()
		{
			InputManager.Instance.ResetInputArray();
			CustomInput customInput = InputManager.Instance.GetInputPresetCurrent() as CustomInput;
			if ((bool)customInput)
			{
				customInput.OnZoom(UnityEngine.Input.GetAxis("Mouse ScrollWheel"));
				Vector3 pos;
				if (UnityEngine.Input.GetMouseButtonUp(0) && GameInput.FindWaypointPosition(UnityEngine.Input.mousePosition, out pos))
				{
					customInput.OnWaypoint(pos);
				}
				if (UnityEngine.Input.GetMouseButton(0))
				{
					customInput.OnPan(UnityEngine.Input.mousePosition);
				}
			}
		}
	}
}
