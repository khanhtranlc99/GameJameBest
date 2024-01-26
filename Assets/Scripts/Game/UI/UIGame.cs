using System;
using System.Threading;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
	public class UIGame : MonoBehaviour
	{
		public KeyCode ScreenshotKey = KeyCode.Keypad0;

		public KeyCode PauseKey = KeyCode.KeypadPeriod;

		public MenuPanelManager PanelManager;

		public GameObject achievmentUI;

		public Animator PausePanel;

		public Animator LoosePanel;

		public Animator WinPanel;

		public Text Timer;

		public Text Score;

		public Slider ProgeressBar;

		public Slider HpBar;

		public bool HDScreenshots;

		// public Action OnPausePanelOpen;
		//
		// public Action OnExitInMenu;

		private static bool _isCompleted;

		public static bool DrawOnGUI = true;

		public static UIGame Instance;

		public static bool IsCompleted => _isCompleted;

		private void Awake()
		{
			GameplayUtils.ResumeGame();
			_isCompleted = false;
			Instance = this;
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(ScreenshotKey))
			{
				GameplayUtils.PerformScreenshot((!HDScreenshots) ? 1 : 2);
			}
			if (UnityEngine.Input.GetKeyDown(PauseKey))
			{
				UnityEngine.Debug.Break();
			}
		}

		public void SetProgress(float value)
		{
			if (ProgeressBar != null)
			{
				ProgeressBar.value = value;
			}
		}

		public void SetHP(float value)
		{
			if (HpBar != null)
			{
				HpBar.value = value;
			}
		}

		public void SetTime(DateTime dateTime)
		{
			if (Timer != null)
			{
				Timer.text = dateTime.ToString("mm:ss.f");
			}
		}

		public void SetScore(int score)
		{
			if (Score != null)
			{
				Score.text = score.ToString();
			}
		}

		public void Complete(bool won, int starsCount)
		{
			if (_isCompleted)
			{
				return;
			}
			_isCompleted = true;
			GameplayUtils.PauseGame();
			if (won)
			{
				PanelManager.OpenPanel(WinPanel);
				AudioSource component = WinPanel.gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.Play();
				}
				WinnerStarsController component2 = WinPanel.gameObject.GetComponent<WinnerStarsController>();
				if (component2 != null)
				{
					component2.SetStarts(starsCount);
				}
				LevelInfo levelInfo = LevelsProfile.GetLevelInfo(MApplication.CurrentLevel);
				if (levelInfo.StarsCount < starsCount)
				{
					levelInfo.StarsCount = starsCount;
					LevelsProfile.SetLevelInfo(MApplication.CurrentLevel, levelInfo);
				}
				LevelInfo levelInfo2 = LevelsProfile.GetLevelInfo(MApplication.CurrentLevel + 1);
				if (!levelInfo2.IsAvailable)
				{
					LevelsProfile.SetLevelInfo(MApplication.CurrentLevel + 1, new LevelInfo
					{
						IsAvailable = true
					});
				}
			}
			else
			{
				PanelManager.OpenPanel(LoosePanel);
				AudioSource component3 = LoosePanel.gameObject.GetComponent<AudioSource>();
				if (component3 != null)
				{
					component3.Play();
				}
			}
		}

		// public void Pause()
		// {
		// 	if (OnPausePanelOpen != null)
		// 	{
		// 		OnPausePanelOpen();
		// 	}
		// 	GameplayUtils.PauseGame();
		// }

		public void Resume()
		{
			GameplayUtils.ResumeGame();
		}

		public void Loose()
		{
			Complete(false, 0);
		}

		public void Win()
		{
			Complete(true, 2);
		}

		// public void ExitInMenu()
		// {
		// 	UI_GeneralPopup.ShowPopup("Exit game", "Are you sure?", delegate
		// 	{
		// 		if (OnExitInMenu != null)
		// 		{
		// 			OnExitInMenu();
		// 		}
		// 		Thread.Sleep(500);
		// 		SceneManager.LoadScene(Constants.Scenes.Menu.ToString());
		// 	});
		// }
	}
}
