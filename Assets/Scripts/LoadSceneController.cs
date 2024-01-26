using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
	public Constants.Scenes SceneToLoad;

	public Constants.MenuState MenuState;

	public AsyncSceneLoader Loader;

	public float Delay;

	public void Load()
	{
		Invoke("DelayLoad", Delay);
	}

	private void DelayLoad()
	{
		if ((bool)Loader)
		{
			Loader.LoadScene(SceneToLoad.ToString());
			return;
		}
		Thread.Sleep(500);
		if (MenuState != 0)
		{
			MApplication.MenuState = MenuState;
		}
		SceneManager.LoadScene(SceneToLoad.ToString());
	}
}
