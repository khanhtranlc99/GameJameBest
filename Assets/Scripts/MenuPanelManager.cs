using Root.Scripts.Helper;
using UnityEngine;

public class MenuPanelManager : MonoBehaviour
{
	private const string AnimKey = "Open";

	public Animator FirstOpen;

	private Animator _currentPanelAnimator;

	public void Start()
	{
		if (FirstOpen != null && _currentPanelAnimator == null)
		{
			OpenPanel(FirstOpen);
		}
		else
		{
			OpenPanel(_currentPanelAnimator);
		}
	}

	public void OpenPanel(Animator panelAnimator)
	{
		if (_currentPanelAnimator == panelAnimator)
		{
			return;
		}
		if (_currentPanelAnimator != null)
		{
			if (_currentPanelAnimator.isInitialized)
			{
				_currentPanelAnimator.SetBool("Open", value: false);
			}
			_currentPanelAnimator.gameObject.SetActive(value: false);
		}
		panelAnimator.gameObject.SetActive(value: true);
		if (panelAnimator.isInitialized)
		{
			panelAnimator.SetBool("Open", value: true);
		}
		_currentPanelAnimator = panelAnimator;
		AudioSource component = GetComponent<AudioSource>();
		if (component != null && component.isActiveAndEnabled)
		{
			component.Play();
		}
		//AdManager.Instance.ShowInterstitial();
	}

	public void SwitchPanel(Animator switchPanel)
	{
		if (_currentPanelAnimator == switchPanel)
		{
			if (_currentPanelAnimator.isInitialized)
			{
				_currentPanelAnimator.SetBool("Open", value: false);
			}
			_currentPanelAnimator.gameObject.SetActive(value: false);
			if (FirstOpen != null)
			{
				FirstOpen.gameObject.SetActive(value: true);
				if (FirstOpen.isInitialized)
				{
					FirstOpen.SetBool("Open", value: false);
				}
				_currentPanelAnimator = FirstOpen;
			}
			else
			{
				_currentPanelAnimator = null;
			}

		}
		else if (_currentPanelAnimator != switchPanel)
		{
			if (_currentPanelAnimator != null)
			{
				if (_currentPanelAnimator.isInitialized)
				{
					_currentPanelAnimator.SetBool("Open", value: false);
				}
				_currentPanelAnimator.gameObject.SetActive(value: false);
			}
			if (switchPanel.isInitialized)
			{
				switchPanel.SetBool("Open", value: false);
			}
			switchPanel.gameObject.SetActive(value: true);
			_currentPanelAnimator = switchPanel;
		}
		AudioSource component = GetComponent<AudioSource>();
		if (component != null && component.isActiveAndEnabled)
		{
			component.Play();
		}
		//AdManager.Instance.ShowInterstitial();
	}

	public Animator GetCurrentPanel()
	{
		return _currentPanelAnimator;
	}

	public void ResetSaves()
	{
		BaseProfile.ClearBaseProfileWithoutSystemSettings();
	}

	public void ExitApplication()
	{
		Application.Quit();
	}
}
