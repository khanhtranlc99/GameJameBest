using UnityEngine;

public class WinnerStarsController : MonoBehaviour
{
	public GameObject[] Stars;

	private void Awake()
	{
		GameObject[] stars = Stars;
		foreach (GameObject gameObject in stars)
		{
			gameObject.SetActive(value: false);
		}
	}

	public void SetStarts(int starsCount)
	{
		GameObject[] stars = Stars;
		foreach (GameObject gameObject in stars)
		{
			if (--starsCount < 0)
			{
				break;
			}
			gameObject.SetActive(value: true);
		}
	}
}
