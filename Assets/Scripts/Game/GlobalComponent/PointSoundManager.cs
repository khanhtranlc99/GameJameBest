using Game.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PointSoundManager : MonoBehaviour
	{
		[Serializable]
		public class AudioClipConfig
		{
			public string name;

			public GameObject specificPointSound;

			public AudioClip clip;

			[Range(0f, 1f)]
			public float volume;

			public TypeOfSound type;
		}

		private static PointSoundManager instance;

		public GameObject pointSound;

		public GameObject SamplePointSound3D;

		[Range(0f, 1f)]
		public float Volume;

		public List<AudioClipConfig> AudioClips = new List<AudioClipConfig>();

		private float currentVolume;

		private AudioSource[] playingClips;

		public static PointSoundManager Instance
		{
			get
			{
				if (instance == null)
				{
					UnityEngine.Debug.LogError("PointSoundManager is no initialized");
				}
				return instance;
			}
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			currentVolume = SoundManager.instance.GetSoundValue() * Volume;
			SoundManager.ValueChanged b = delegate(float value)
			{
				currentVolume = SoundManager.instance.GetSoundValue() * Volume;
				currentVolume *= value;
			};
			SoundManager soundManager = SoundManager.instance;
			soundManager.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(soundManager.GameSoundChanged, b);
		}

		public void PlaySoundAtPoint(Vector3 position, TypeOfSound type)
		{
			AudioClipConfig[] array = new AudioClipConfig[AudioClips.Count];
			int num = 0;
			foreach (AudioClipConfig audioClip in AudioClips)
			{
				if (audioClip.type == type)
				{
					array[num] = audioClip;
					num++;
				}
			}
			if (array.Length > 0)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				float volume = currentVolume * array[num2].volume;
				GameObject specificPointSound = array[num2].specificPointSound;
				PlaySound(array[num2].clip, position, volume, specificPointSound);
			}
			else
			{
				UnityEngine.Debug.LogError("Wrong type");
			}
		}

		public void PlaySoundAtPoint(Vector3 position, AudioClip clip, float soundVolume = 1f)
		{
			soundVolume *= currentVolume;
			PlaySound(clip, position, soundVolume, null);
		}

		public void PlaySoundAtPoint(Vector3 position, string soundName)
		{
			AudioClip audioClip = null;
			float volume = 0f;
			GameObject specificPointSound = null;
			foreach (AudioClipConfig audioClip2 in AudioClips)
			{
				if (audioClip2.name == soundName)
				{
					volume = currentVolume * audioClip2.volume;
					audioClip = audioClip2.clip;
					specificPointSound = audioClip2.specificPointSound;
				}
			}
			if (audioClip != null)
			{
				PlaySound(audioClip, position, volume, specificPointSound);
			}
			else
			{
				UnityEngine.Debug.LogError("Wrong sound name");
			}
		}

		public void PlayCustomClipAtPoint(Vector3 position, AudioClip clip, GameObject specificPointSound = null)
		{
			PlaySound(clip, position, currentVolume, specificPointSound);
		}

		public void Play3DSoundOnPoint(Vector3 position, AudioClip clip)
		{
			PlaySound(clip, position, currentVolume, SamplePointSound3D);
		}

		private void PlaySound(AudioClip clip, Vector3 position, float volume, GameObject specificPointSound)
		{
			if (!(clip == null))
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool((!specificPointSound) ? pointSound : specificPointSound);
				fromPool.transform.position = position;
				AudioSource component = fromPool.GetComponent<AudioSource>();
				component.clip = clip;
				component.volume = volume;
				component.Play();
				PoolManager.Instance.ReturnToPoolWithDelay(fromPool, clip.length);
			}
		}
	}
}
