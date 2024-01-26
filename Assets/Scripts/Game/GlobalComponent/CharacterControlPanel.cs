using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Animator))]
	public class CharacterControlPanel : ControlPanel
	{
		public JoyPad AimJoystick;

		public TouchPad RotatePad;

		public Joystick MoveJoystick;

		public override void OnOpen()
		{
			AimJoystick.enabled = false;
			RotatePad.enabled = true;
			MoveJoystick.enabled = true;
		}

		public override void OnClose()
		{
			AimJoystick.enabled = false;
			RotatePad.enabled = false;
			MoveJoystick.enabled = false;
		}
	}
}
