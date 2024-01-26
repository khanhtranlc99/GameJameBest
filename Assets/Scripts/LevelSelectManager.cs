using Game.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
	public MenuPanelManager MenuPanelManager;

	public Animator LoadingPanel;

	public LevelButtonController LevelButtonPrefab;

	public int LevelsCount;

	private void Awake()
	{
		LevelInfo levelInfo = LevelsProfile.GetLevelInfo(1);
		if (!levelInfo.IsAvailable)
		{
			levelInfo.IsAvailable = true;
			LevelsProfile.SetLevelInfo(1, levelInfo);
		}
		if (LevelButtonPrefab != null)
		{
			for (int i = 0; i < LevelsCount; i++)
			{
				LevelButtonController levelButtonController = UnityEngine.Object.Instantiate(LevelButtonPrefab);
				levelButtonController.transform.SetParent(base.transform);
				levelButtonController.transform.localScale = new Vector3(1f, 1f, 1f);
				LevelInfo levelInfo2 = LevelsProfile.GetLevelInfo(i + 1);
				levelButtonController.Init(SelectLevel, levelInfo2, i + 1);
				levelButtonController.gameObject.SetActive(value: true);
			}
		}
	}

	public void SelectLevel(int level)
	{
		if (GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log("Selected level = " + level);
		}
		MApplication.CurrentLevel = level;
		MenuPanelManager.OpenPanel(LoadingPanel);
		//AdsManager.ShowFullscreenAd();
		Invoke("LoadLevel", 0.5f);
	}

	private void LoadLevel()
	{
		SceneManager.LoadScene(Constants.Scenes.Game.ToString());
	}
}
