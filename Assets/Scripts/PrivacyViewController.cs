using Common.Analytics;
using Game.GlobalComponent;
using Game.Managers;
using System.Collections;
using UnityEngine;

public class PrivacyViewController : MonoBehaviour
{
	[SerializeField]
	private string m_Link;

	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[SerializeField]
	private AudioClip m_AcceptSound;

	private void Awake()
	{
		ShowPanel(value: false);
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		if (SoundManager.instance.MusicMuted)
		{
			UnmuteMusic();
		}
	}

	private void Start()
	{
		bool flag = BaseProfile.ResolveValue(SystemSettingsList.PerformanceDetected.ToString(), defaultValue: false);
		bool flag2 = BaseProfile.ResolveValue("PPConfirmed", defaultValue: false);
		flag2 = true;
		if (flag && !flag2)
		{
			MuteMusic();
			ShowPanel(value: true);
			BackButton.BackButtonsActive = false;
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator BehaviorOnAccept()
	{
		SoundManager.PlaySoundAtPosition(m_AcceptSound, base.transform.position, SoundManager.SoundType.Sound);
		yield return new WaitForSecondsRealtime(m_AcceptSound.length);
		BackButton.BackButtonsActive = true;
		UnmuteMusic();
		base.gameObject.SetActive(value: false);
	}

	private void MuteMusic()
	{
		SoundManager.instance.MusicMuted = true;
	}

	private void UnmuteMusic()
	{
		SoundManager.instance.MusicMuted = false;
	}

	public void OpenLink()
	{
		Application.OpenURL(m_Link);
		//BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.AM, "PPolicy", "OpenLink");
	}

	public void Confirm()
	{
		BaseProfile.StoreValue(true, "PPConfirmed");
		ShowPanel(value: false);
		StartCoroutine(BehaviorOnAccept());
		Debug.Log("Old Log Show ");
		//BaseAnalyticsManager.Instance.SendEvent(EventsActionsTypes.AM, "PPolicy", "Confirmed");
	}

	private void ShowPanel(bool value)
	{
		m_CanvasGroup.alpha = ((!value) ? 0f : 1f);
		m_CanvasGroup.interactable = value;
	}
}
