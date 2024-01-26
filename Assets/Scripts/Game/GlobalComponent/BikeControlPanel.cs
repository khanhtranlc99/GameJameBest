using UnityEngine;

namespace Game.GlobalComponent
{
	public class BikeControlPanel : ControlPanel
	{
		public Joystick[] Joysticks;

		public ControlButtonManager ButtonManager;

		public GameObject GetOutButton;

		public float GetInAnimationLength;

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
