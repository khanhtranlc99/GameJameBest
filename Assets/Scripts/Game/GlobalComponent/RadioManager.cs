using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class RadioManager : MonoBehaviour
	{
		[Serializable]
		public class RadioStation
		{
			public string Name;

			public Sprite LogoSprite;

			public AudioClip[] AudioClips;

			[HideInInspector]
			public int CurrentClipIndex;

			[HideInInspector]
			public bool StationEnabled;

			[HideInInspector]
			public float LastTime;

			[HideInInspector]
			public float LastClipTime;
		}

		private const float ClipOffset = 1.5f;

		public static RadioManager Instance;

		public GameObject RadioPanel;

		public Image RadioLogoImage;

		public Text RadioNameText;

		public List<RadioStation> Stations = new List<RadioStation>();

		[HideInInspector]
		public bool BlockRadio;

		private int currentRadioIndex;

		private AudioSource audioSource;

		private bool isPlay;

		private float prevRadioInput;

		private void Awake()
		{
			Instance = this;
		}

		public void EnableRadio()
		{
			if (!BlockRadio)
			{
				if ((bool)RadioPanel)
				{
					RadioPanel.SetActive(value: true);
				}
				isPlay = true;
				audioSource = BackgroundMusic.instance.GetAudioSource();
				currentRadioIndex = UnityEngine.Random.Range(0, Stations.Count);
				EnableStation();
			}
		}

		public void DisableRadio()
		{
			if (isPlay)
			{
				if ((bool)RadioPanel)
				{
					RadioPanel.SetActive(value: false);
				}
				isPlay = false;
				if (audioSource.isPlaying)
				{
					audioSource.Stop();
				}
			}
		}

		public void NextStation()
		{
			DisableStation();
			currentRadioIndex++;
			if (currentRadioIndex == Stations.Count)
			{
				currentRadioIndex = 0;
			}
			EnableStation();
		}

		public void PrevStation()
		{
			DisableStation();
			currentRadioIndex--;
			if (currentRadioIndex < 0)
			{
				currentRadioIndex = Stations.Count - 1;
			}
			EnableStation();
		}

		private void NextClip()
		{
			if (!audioSource.isPlaying && Stations[currentRadioIndex].AudioClips.Length != 0)
			{
				Stations[currentRadioIndex].CurrentClipIndex = GetRandomIndex(0, Stations[currentRadioIndex].AudioClips.Length, Stations[currentRadioIndex].CurrentClipIndex);
				audioSource.clip = Stations[currentRadioIndex].AudioClips[Stations[currentRadioIndex].CurrentClipIndex];
				audioSource.time = 0f;
				if (!audioSource.isPlaying)
				{
					audioSource.Play();
				}
			}
		}

		private void TurnClipFromPlace(float timeOffset = 0f, bool isStop = false)
		{
			if (isStop)
			{
				if (audioSource.isPlaying)
				{
					audioSource.Stop();
				}
				return;
			}
			RadioStation radioStation = Stations[currentRadioIndex];
			timeOffset = ((timeOffset != 0f) ? Mathf.Clamp(timeOffset, 0f, radioStation.AudioClips[radioStation.CurrentClipIndex].length - 1.5f) : UnityEngine.Random.Range(0f, radioStation.AudioClips[radioStation.CurrentClipIndex].length - 1.5f));
			audioSource.clip = radioStation.AudioClips[radioStation.CurrentClipIndex];
			audioSource.time = timeOffset;
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}

		private void EnableStation()
		{
			RadioStation radioStation = Stations[currentRadioIndex];
			if ((bool)RadioLogoImage)
			{
				RadioLogoImage.sprite = radioStation.LogoSprite;
			}
			if ((bool)RadioNameText)
			{
				RadioNameText.text = radioStation.Name;
			}
			InGameLogManager.Instance.RegisterNewMessage(MessageType.RadioName, radioStation.Name);
			if (radioStation.AudioClips.Length == 0 || (radioStation.AudioClips.Length>0 && radioStation.CurrentClipIndex>= radioStation.AudioClips.Length))
			{
				TurnClipFromPlace(0f, isStop: true);
				return;
			}
			float num = Time.fixedTime - radioStation.LastTime;
			bool flag = num + 1.5f < radioStation.AudioClips[radioStation.CurrentClipIndex].length - radioStation.LastClipTime;
			num = ((!flag) ? 0f : (num + radioStation.LastClipTime));
			if (!radioStation.StationEnabled || !flag)
			{
				radioStation.CurrentClipIndex = GetRandomIndex(0, radioStation.AudioClips.Length, radioStation.CurrentClipIndex);
			}
			TurnClipFromPlace(num);
			radioStation.StationEnabled = true;
		}

		private void DisableStation()
		{
			Stations[currentRadioIndex].LastTime = Time.fixedTime;
			Stations[currentRadioIndex].LastClipTime = audioSource.time;
		}

		private void FixedUpdate()
		{
			if (isPlay)
			{
				NextClip();
			}
		}

		private void Update()
		{
			if (!isPlay)
			{
				return;
			}
			float axis = Controls.GetAxis("Radio");
			if (axis == 0f)
			{
				if (prevRadioInput > 0f)
				{
					NextStation();
				}
				if (prevRadioInput < 0f)
				{
					PrevStation();
				}
			}
			prevRadioInput = axis;
		}

		private int GetRandomIndex(int start, int end, int exclusion)
		{
			if (start + 1 == end || start == end)
			{
				return 0;
			}
			int num;
			for (num = UnityEngine.Random.Range(start, end); num == exclusion; num = UnityEngine.Random.Range(start, end))
			{
			}
			return num;
		}
	}
}
