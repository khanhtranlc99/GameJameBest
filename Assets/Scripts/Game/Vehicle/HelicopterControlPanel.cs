using Game.GlobalComponent;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vehicle
{
	public class HelicopterControlPanel : ControlPanel
	{
		private const float GetInAnimationLength = 5f;

		private static HelicopterControlPanel instance;

		public JoyPad AimJoystick;

		public TouchPad RotatePad;

		public Joystick MoveJoystick;

		public Image ReloadIndicator;

		public GameObject GetOutButton;

		public static HelicopterControlPanel Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("HelicopterControlPanel is not initialized");
				}
				return instance;
			}
		}

		public override void OnOpen()
		{
			SetComponentActiveStatus(status: true);
			Invoke("DefferedGetOutButtonActivate", 5f);
		}

		public override void OnClose()
		{
			SetComponentActiveStatus(status: false);
		}

		private void SetComponentActiveStatus(bool status)
		{
			AimJoystick.enabled = false;
			GetOutButton.SetActive(value: false);
			RotatePad.enabled = status;
			MoveJoystick.enabled = status;
		}

		private void Awake()
		{
			instance = this;
		}

		private void DefferedGetOutButtonActivate()
		{
			GetOutButton.SetActive(value: true);
		}
	}
}
