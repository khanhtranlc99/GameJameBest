using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class MechControlPanel : ControlPanel
	{
		private static MechControlPanel instance;

		public JoyPad AimJoystick;

		public TouchPad RotatePad;

		public Joystick MoveJoystick;

		public Image ReloadIndicator;

		public GameObject GetOutButton;

		public GameObject RotateRight;

		public GameObject RotateLeft;

		public bool isBigFoot;

		public float GetInAnimationLength;

		public static MechControlPanel Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("MechControlPanel is not initialized");
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		public override void OnOpen()
		{
			AimJoystick.enabled = false;
			RotatePad.enabled = true;
			MoveJoystick.enabled = true;
			Invoke("DefferedGetOutButtonActivate", GetInAnimationLength);
		}

		public override void OnClose()
		{
			AimJoystick.enabled = false;
			RotatePad.enabled = false;
			MoveJoystick.enabled = false;
			GetOutButton.SetActive(value: false);
			RotateLeft.SetActive(value: false);
			RotateRight.SetActive(value: false);
		}

		public void EnableToSideRotationButtons()
		{
			RotateLeft.SetActive(value: true);
			RotateRight.SetActive(value: true);
		}

		private void DefferedGetOutButtonActivate()
		{
			GetOutButton.SetActive(value: true);
		}
	}
}
