using Game.Managers;
using System;
using UnityEngine;

public class GameplayUtils
{
	private static float _currentTimeScale = 1f;

	public static bool OnPause => Time.timeScale == 0f;

	public static void PauseGame()
	{
		if (Time.timeScale > 0f)
		{
			_currentTimeScale = Time.timeScale;
		}
		Time.timeScale = 0f;
		SoundManager.instance.GameSoundMuted = true;
		GC.Collect();
	}

	public static void ResumeGame()
	{
		SoundManager.instance.GameSoundMuted = false;
		Time.timeScale = _currentTimeScale;
	}

	public static void PerformScreenshot()
	{
		PerformScreenshot(0);
	}

	public static void PerformScreenshot(int superSize)
	{
		if (superSize < 1)
		{
			superSize = 1;
		}
		string text = Application.persistentDataPath + "/screen-" + superSize * Screen.width + "x" + superSize * Screen.height + "-" + DateTime.Now.ToString("yyMMdd-hhmmss") + ".png";
		UnityEngine.ScreenCapture.CaptureScreenshot(text, superSize);
		UnityEngine.Debug.Log("Screenshot saved to location: " + text);
	}
}
