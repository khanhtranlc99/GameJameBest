using UnityEngine;

namespace Game.GlobalComponent
{
	public class SubPanelsController : ControlPanel
	{
		public GameObject[] ControlPanels;

		public int IndexForFirstPanel;

		private int currentPanelIndex;

		public override void OnOpen()
		{
		}

		public void OnOpen(int index)
		{
			if (index != currentPanelIndex && ControlPanels.Length > index)
			{
				GameObject gameObject = ControlPanels[currentPanelIndex];
				gameObject.SetActive(value: false);
				CheckControlPanel(gameObject, open: false);
				GameObject gameObject2 = ControlPanels[index];
				gameObject2.SetActive(value: true);
				CheckControlPanel(gameObject2);
				currentPanelIndex = index;
			}
		}

		public void OnOpen(ControlsType controlsType)
		{
		}

		private void CheckControlPanel(GameObject panel, bool open = true)
		{
			ControlPanel component = panel.GetComponent<ControlPanel>();
			if ((bool)component)
			{
				if (open)
				{
					component.OnOpen();
				}
				else
				{
					component.OnClose();
				}
			}
		}
	}
}
