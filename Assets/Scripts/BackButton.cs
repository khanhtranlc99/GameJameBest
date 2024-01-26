using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
	public static bool BackButtonsActive = true;

	private Button _button;

	public static void ChangeBackButtonsStatus(bool active)
	{
		BackButtonsActive = active;
	}

	private void Awake()
	{
		_button = GetComponent<Button>();
	}

	private void Update()
	{
		if (BackButtonsActive && _button != null && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			_button.onClick.Invoke();
		}
	}
}
