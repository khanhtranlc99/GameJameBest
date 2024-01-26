using UnityEngine;
using UnityEngine.UI;

public class LevelButtonController : MonoBehaviour
{
	public delegate void LevelSelected(int level);

	public GameObject[] Stars;

	public Text LevelText;

	public Button Button;

	private LevelSelected _levelSelected;

	private int _level;

	private LevelInfo _levelInfo;

	public void Init(LevelSelected levelSelected, LevelInfo levelInfo, int level)
	{
		_level = level;
		_levelSelected = levelSelected;
		_levelInfo = levelInfo;
		if (_levelInfo.IsAvailable)
		{
			int num = _levelInfo.StarsCount;
			GameObject[] stars = Stars;
			foreach (GameObject gameObject in stars)
			{
				if (--num < 0)
				{
					break;
				}
				gameObject.SetActive(value: true);
			}
		}
		else
		{
			Button.interactable = false;
		}
		LevelText.text = level.ToString();
	}

	public void SelectLevel()
	{
		if (_levelSelected != null)
		{
			_levelSelected(_level);
		}
	}
}
