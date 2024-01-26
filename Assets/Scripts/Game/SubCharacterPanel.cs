using Game.GlobalComponent;
using UnityEngine;

namespace Game
{
	[RequireComponent(typeof(Animator))]
	public class SubCharacterPanel : SubPanel
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
