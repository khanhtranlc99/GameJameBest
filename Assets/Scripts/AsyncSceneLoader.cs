using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncSceneLoader : MonoBehaviour
{
	//public Sprite Spinner;
	public Image imgFiller;
	// public Sprite Back;
	//
	// public Sprite Bar;
	

	//private Texture2D back;

	//private Texture2D bar;
	

	// private bool loading;
	//
	// private float angle;

	public void LoadScene(string scene)
	{
		//loading = true;
		StartCoroutine(LoadNewScene(scene));
	}

	// private void OnGUI()
	// {
	// 	if (loading)
	// 	{
	// 		ShowElements();
	// 	}
	// }

	// private void SetTexturesFromAtlas()
	// {
	// 	spinner = new Texture2D((int)Spinner.rect.width, (int)Spinner.rect.height);
	// 	Color[] pixels = Spinner.texture.GetPixels((int)Spinner.rect.x, (int)Spinner.rect.y, (int)Spinner.rect.width, (int)Spinner.rect.height);
	// 	spinner.SetPixels(pixels);
	// 	spinner.Apply();
	// 	back = new Texture2D((int)Back.rect.width, (int)Back.rect.height);
	// 	pixels = Back.texture.GetPixels((int)Back.rect.x, (int)Back.rect.y, (int)Back.rect.width, (int)Back.rect.height);
	// 	back.SetPixels(pixels);
	// 	back.Apply();
	// 	bar = new Texture2D((int)Bar.rect.width, (int)Bar.rect.height);
	// 	pixels = Bar.texture.GetPixels((int)Bar.rect.x, (int)Bar.rect.y, (int)Bar.rect.width, (int)Bar.rect.height);
	// 	bar.SetPixels(pixels);
	// 	bar.Apply();
	// }
	//
	// private void ShowElements()
	// {
	// 	//Rect position = new Rect(0f, 0f, Screen.width, Screen.height);
	// 	//Rect position2 = new Rect(position.xMin + position.width * 0.1f, position.yMax - position.height * 0.2f, position.xMax - position.width * 0.2f, position.height * 0.05f);
	// 	//Rect position3 = new Rect(position.xMin + position.width * 0.1f, position.yMax - position.height * 0.2f, (position.xMax - position.width * 0.2f) * progress, position.height * 0.05f);
	// 	//Rect position4 = new Rect(Screen.width / 2 - Screen.width / 20, Screen.height / 6, Screen.width / 10, Screen.width / 10);
	// 	//GUI.BeginGroup(position);
	// 	//GUI.DrawTexture(position2, back, ScaleMode.StretchToFill);
	// 	//GUI.DrawTexture(position3, bar, ScaleMode.StretchToFill);
	// 	//GUI.EndGroup();
	// 	//GUIUtility.RotateAroundPivot(pivotPoint: new Vector2(position4.xMin + position4.width * 0.5f, position4.yMin + position4.height * 0.5f), angle: angle);
	// 	//GUI.DrawTexture(position4, spinner);
	// }

	private IEnumerator LoadNewScene(string scene)
	{
		if (imgFiller != null)
		{
			imgFiller.gameObject.SetActive(true);
			imgFiller.fillAmount = 0;
		}

		yield return new WaitForSeconds(0.5f);
		AsyncOperation async = SceneManager.LoadSceneAsync(scene);
		while (!async.isDone)
		{
			//progress = async.progress;
			if(imgFiller!=null)
				imgFiller.fillAmount = async.progress;
			yield return null;
		}
		//loading = false;
	}
}
