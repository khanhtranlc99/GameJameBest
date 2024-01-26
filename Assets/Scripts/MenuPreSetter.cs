using UnityEngine;

public class MenuPreSetter : MonoBehaviour
{
	public MenuPanelManager MenuPanelManager;

	public Animator LevelsPanel;

	private void Awake()
	{
		Time.timeScale = 1f;
		if (MApplication.MenuState == Constants.MenuState.Levels)
		{
			MenuPanelManager.FirstOpen = LevelsPanel;
			MApplication.MenuState = Constants.MenuState.None;
		}
	}
}
