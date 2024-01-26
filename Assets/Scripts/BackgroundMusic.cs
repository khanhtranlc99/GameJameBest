using Game.Character;
using Game.GlobalComponent;
using Game.Managers;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
	public static BackgroundMusic instance;

	public AudioClip[] Clips;

	public AudioClip TimeQuestClip;

	public bool PlayBackgroundInGameScene;

	public float MusicVolumeFactor = 0.5f;

	private AudioSource _thisAudio;

	private bool playClips;

	public AudioSource GetAudioSource()
	{
		return (!_thisAudio) ? GetComponent<AudioSource>() : _thisAudio;
	}

	public void PlayTimeQuestClip()
	{
		RadioManager.Instance.DisableRadio();
		RadioManager.Instance.BlockRadio = true;
		_thisAudio.clip = TimeQuestClip;
		_thisAudio.loop = true;
		if (!_thisAudio.isPlaying)
		{
			_thisAudio.Play();
		}
	}

	public void StopTimeQuestClip()
	{
		RadioManager.Instance.BlockRadio = false;
		_thisAudio.Stop();
		_thisAudio.loop = false;
		if (PlayerInteractionsManager.Instance.inVehicle && PlayerInteractionsManager.Instance.LastDrivableVehicle.VehicleSpecificPrefab.HasRadio)
		{
			RadioManager.Instance.EnableRadio();
		}
	}

	private void PlayOther()
	{
		int num = UnityEngine.Random.Range(0, Clips.Length);
		if (_thisAudio.clip.Equals(Clips[num]))
		{
			num = (num + 1) % Clips.Length;
		}
		_thisAudio.clip = Clips[num];
		_thisAudio.Play();
	}

	private void Awake()
	{
		HasPlayClips();
		if (instance == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			_thisAudio = GetComponent<AudioSource>();
			if (_thisAudio != null)
			{
				_thisAudio.volume = SoundManager.instance.GetMusicValue();
			}
			SoundManager soundManager = SoundManager.instance;
			soundManager.MusicChanged = (SoundManager.ValueChanged)Delegate.Combine(soundManager.MusicChanged, (SoundManager.ValueChanged)delegate(float value)
			{
				if (_thisAudio != null)
				{
					_thisAudio.volume = value * MusicVolumeFactor;
				}
			});
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		HasPlayClips();
		if ((bool)_thisAudio)
		{
			_thisAudio.Stop();
		}
	}

	private bool HasPlayClips()
	{
		playClips = (SceneManager.GetActiveScene().name.Equals(Constants.Scenes.Menu.ToString()) && BaseProfile.ResolveValue(SystemSettingsList.PerformanceDetected.ToString(), defaultValue: false));
		return playClips;
	}

	private void FixedUpdate()
	{
		if (playClips && !_thisAudio.isPlaying)
		{
			PlayOther();
		}
	}
}
