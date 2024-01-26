using Game.Managers;
using System;
using UnityEngine;

public class BaseSoundController : MonoBehaviour
{
	public float VolumeEffect = 1f;

	public bool IsInGameSound;

	public bool IsMusicPlayer;

	private void Awake()
	{
		AudioSource audioSource = GetComponent<AudioSource>();
		if (audioSource != null)
		{
			audioSource.volume = SoundManager.instance.GetSoundValue() * VolumeEffect;
			if (IsMusicPlayer)
			{
				audioSource.volume *= SoundManager.instance.GetMusicValue();
			}
			SoundManager.ValueChanged b = delegate(float value)
			{
				if (audioSource != null)
				{
					audioSource.volume = value * VolumeEffect;
					if (IsMusicPlayer)
					{
						audioSource.volume *= SoundManager.instance.GetMusicValue();
					}
				}
			};
			if (IsInGameSound)
			{
				SoundManager instance = SoundManager.instance;
				instance.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(instance.GameSoundChanged, b);
			}
			else
			{
				SoundManager instance2 = SoundManager.instance;
				instance2.SoundChanged = (SoundManager.ValueChanged)Delegate.Combine(instance2.SoundChanged, b);
			}
		}
	}
}
