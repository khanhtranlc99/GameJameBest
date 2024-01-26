using Game.Managers;
using UnityEngine;

public class LocalSoundManager : MonoBehaviour
{
	private void Start()
	{
		SetSoundVolume(SoundManager.instance.GetSoundValue());
		SetMusicVolume(SoundManager.instance.GetMusicValue());
	}

	public void SetSoundVolume(float value)
	{
		SoundManager.instance.SetSoundValue(value);
	}

	public void SetMusicVolume(float value)
	{
		SoundManager.instance.SetMusicValue(value);
	}
}
