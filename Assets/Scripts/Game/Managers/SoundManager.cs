using System;
using UnityEngine;

namespace Game.Managers
{
	public class SoundManager
	{
		public enum SoundType
		{
			Sound,
			Music,
			GameSound
		}

		public delegate void ValueChanged(float newValue);

		private static SoundManager _instance;

		public ValueChanged SoundChanged;

		public ValueChanged GameSoundChanged;

		public ValueChanged MusicChanged;

		private bool _soundMuted;

		private bool _gameSoundMuted;

		private bool _musicMuted;

		private float _soundValue = -1f;

		private float _musicValue = -1f;

		public static SoundManager instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SoundManager();
				}
				return _instance;
			}
		}

		public bool SoundMuted
		{
			get
			{
				return _soundMuted;
			}
			set
			{
				_soundMuted = value;
				instance.SetSoundValue(_soundValue);
			}
		}

		public bool GameSoundMuted
		{
			get
			{
				return _gameSoundMuted;
			}
			set
			{
				_gameSoundMuted = value;
				instance.SetSoundValue(_soundValue);
			}
		}

		public bool MusicMuted
		{
			get
			{
				return _musicMuted;
			}
			set
			{
				_musicMuted = value;
				instance.SetMusicValue(_musicValue);
			}
		}

		private SoundManager()
		{
			_soundValue = BaseProfile.SoundVolume;
			_musicValue = BaseProfile.MusicVolume;
		}

		public static void PlaySoundAtPosition(AudioClip clip, Vector3 position, SoundType soundType)
		{
			AudioSource.PlayClipAtPoint(clip, position, instance.GetValueByType(soundType));
		}

		public ValueChanged AddValueChangeByType(SoundType type, ValueChanged valueChanged)
		{
			if (type == SoundType.Sound)
			{
				SoundChanged = (ValueChanged)Delegate.Combine(SoundChanged, valueChanged);
			}
			if (type == SoundType.Music)
			{
				MusicChanged = (ValueChanged)Delegate.Combine(MusicChanged, valueChanged);
			}
			if (type == SoundType.GameSound)
			{
				GameSoundChanged = (ValueChanged)Delegate.Combine(GameSoundChanged, valueChanged);
			}
			return SoundChanged;
		}

		public void SetSoundValue(float value)
		{
			if (SoundChanged != null)
			{
				SoundChanged((!_soundMuted) ? value : 0f);
			}
			if (GameSoundChanged != null)
			{
				GameSoundChanged((!_gameSoundMuted) ? value : 0f);
			}
			_soundValue = value;
			BaseProfile.SoundVolume = value;
		}

		public void SetMusicValue(float value)
		{
			if (MusicChanged != null)
			{
				MusicChanged((!_musicMuted) ? value : 0f);
			}
			_musicValue = value;
			BaseProfile.MusicVolume = value;
		}

		public float GetSoundValue()
		{
			return _soundValue;
		}

		public float GetMusicValue()
		{
			return _musicValue;
		}

		public float GetValueByType(SoundType type)
		{
			switch (type)
			{
			case SoundType.Sound:
				return GetSoundValue();
			case SoundType.Music:
				return GetMusicValue();
			case SoundType.GameSound:
				return GetSoundValue();
			default:
				return GetSoundValue();
			}
		}

		public void SetValueByType(float value, SoundType type)
		{
			switch (type)
			{
			case SoundType.Sound:
				SetSoundValue(value);
				break;
			case SoundType.Music:
				SetMusicValue(value);
				break;
			case SoundType.GameSound:
				SetSoundValue(value);
				break;
			default:
				SetSoundValue(value);
				break;
			}
		}
	}
}
