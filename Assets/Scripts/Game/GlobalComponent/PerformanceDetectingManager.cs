using UnityEngine;

namespace Game.GlobalComponent
{
	public class PerformanceDetectingManager : MonoBehaviour
	{
		//public MenuPanelManager MenuPanelManager;

		private void Start()
		{
			if (!BaseProfile.ResolveValue(SystemSettingsList.PerformanceDetected.ToString(), defaultValue: false))
			{
				// MenuPanelManager.FirstOpen.gameObject.SetActive(value: false);
				// MenuPanelManager.FirstOpen = null;
				GetComponent<LoadSceneController>().Load();
			}
		}
	}
}
