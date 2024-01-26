using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Animator))]
	public class CarControlPanel : ControlPanel
	{
		public override void OnOpen()
		{
			Joystick[] componentsInChildren = GetComponentsInChildren<Joystick>(includeInactive: true);
			foreach (Joystick joystick in componentsInChildren)
			{
				joystick.enabled = true;
			}
			JoyPad[] componentsInChildren2 = GetComponentsInChildren<JoyPad>(includeInactive: true);
			foreach (JoyPad joyPad in componentsInChildren2)
			{
				joyPad.enabled = true;
			}
			
		}

		public override void OnClose()
		{
			Joystick[] componentsInChildren = GetComponentsInChildren<Joystick>(includeInactive: true);
			foreach (Joystick joystick in componentsInChildren)
			{
				joystick.enabled = false;
			}
			JoyPad[] componentsInChildren2 = GetComponentsInChildren<JoyPad>(includeInactive: true);
			foreach (JoyPad joyPad in componentsInChildren2)
			{
				joyPad.enabled = false;
			}
		}
	}
}
