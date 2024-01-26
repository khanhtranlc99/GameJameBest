using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class TankControlPanel : ControlPanel
	{
		private static TankControlPanel instance;

		public Joystick[] Joysticks;

		public ControlButtonManager ButtonManager;

		public Image ReloadIndicator;

		public GameObject GetOutButton;

		public float GetInAnimationLength;

		public static TankControlPanel Instance => instance;

		private void Awake()
		{
			instance = this;
		}

		public override void OnOpen()
		{
			for (int i = 0; i < Joysticks.Length; i++)
			{
				Joysticks[i].enabled = true;
			}
			ButtonManager.enabled = true;
			Invoke("DefferedGetOutButtonActivate", GetInAnimationLength);
		}

		public override void OnClose()
		{
			for (int i = 0; i < Joysticks.Length; i++)
			{
				Joysticks[i].enabled = false;
			}
			ButtonManager.enabled = false;
			GetOutButton.SetActive(value: false);
		}

		private void DefferedGetOutButtonActivate()
		{
			GetOutButton.SetActive(value: true);
		}
	}
}
