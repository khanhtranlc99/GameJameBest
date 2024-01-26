using Game.Character;
using Game.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.GlobalComponent
{
	public class AmbientSoundPoint : MonoBehaviour
	{
		public enum ZoneType
		{
			Sphere,
			Box
		}

		[Serializable]
		public class AmbientPoint
		{
			public string Name;

			[HideInInspector]
			public AudioSource AudioSource;

			[HideInInspector]
			public AudioMixer AudioMixer;

			[HideInInspector]
			public float CurrentVolume;

			public AudioClip AmbientSound;

			[Range(0.1f, 1f)]
			public float MaxVolume;

			[Range(0f, 1f)]
			public float MinVolume;
		}

		private const string AmbiantSoundLayerName = "AmbiantSound";

		private const string PointsParetName = "AudioSorses";

		private const float MuteLerpSpeed = 0.5f;

		private const float DetectTimeOut = 0.5f;

		private const float MuteLerpSensitivity = 0.9f;

		private const float SwithOfset = 20f;

		private static int ambiantSoundLayer = -1;

		public static bool AllGizmos;

		[Tooltip("квадратные жрут больше чем круглые")]
		public ZoneType CurrZoneType;

		private BoxCollider BoxTrigger;

		[Range(0f, 1000f)]
		[HideInInspector]
		public float SwitchingDistance = 10f;

		[HideInInspector]
		public Vector3 BoxRect;

		public GameObject AmbiantPointPrefab;

		public AmbientPoint[] AmbientPoints;

		[HideInInspector]
		public bool IsPlaing;

		public Color GizmosColor = Color.cyan;

		private Transform player;

		private Transform pointsParent;

		private bool muting;

		private AmbientSoundPoint[] ambientSoundPoints;

		private float currentVolume;

		private void Start()
		{
			ambiantSoundLayer = LayerMask.NameToLayer("AmbiantSound");
			Invoke("Init", 0.5f);
			currentVolume = SoundManager.instance.GetSoundValue();
			SoundManager.ValueChanged b = delegate(float value)
			{
				currentVolume = SoundManager.instance.GetSoundValue();
				currentVolume *= value;
			};
			SoundManager instance = SoundManager.instance;
			instance.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(instance.GameSoundChanged, b);
		}

		private void Init()
		{
			player = PlayerInteractionsManager.Instance.Player.transform;
			pointsParent = base.transform.root.Find("AudioSorses");
			ambientSoundPoints = GetComponentsInChildren<AmbientSoundPoint>();
			base.gameObject.layer = ambiantSoundLayer;
			for (int i = 0; i < AmbientPoints.Length; i++)
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool(AmbiantPointPrefab);
				fromPool.transform.parent = pointsParent;
				fromPool.transform.position = base.transform.position;
				fromPool.name = base.name + "_" + i.ToString();
				AmbientPoints[i].AudioSource = fromPool.GetComponent<AudioSource>();
				AmbientPoints[i].AudioSource.clip = AmbientPoints[i].AmbientSound;
				AmbientPoints[i].CurrentVolume = AmbientPoints[i].MinVolume;
				AmbientPoints[i].AudioSource.volume = AmbientPoints[i].CurrentVolume * currentVolume;
				AmbientPoints[i].AudioSource.maxDistance = SwitchingDistance;
				AmbientPoints[i].AudioMixer = AmbientPoints[i].AudioSource.outputAudioMixerGroup.audioMixer;
			}
			if (CurrZoneType.Equals(ZoneType.Box))
			{
				BoxTrigger = base.gameObject.AddComponent<BoxCollider>();
				base.gameObject.AddComponent<ActivateOnTriggerStayHack>();
				BoxTrigger.size = BoxRect;
				BoxTrigger.isTrigger = true;
				SwitchingDistance = (BoxRect.x + BoxRect.y + BoxRect.z) / 3f;
			}
			StartCoroutine(Detect());
		}

		private bool CheckChildsPoints()
		{
			if (player != null)
			{
				if (Vector3.Distance(base.transform.position, player.position) > SwitchingDistance * 0.9f)
				{
					return true;
				}
				for (int i = 0; i < ambientSoundPoints.Length; i++)
				{
					if (ambientSoundPoints[i].IsPlaing && !ambientSoundPoints[i].Equals(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void PlayStopSounds(bool play = true)
		{
			for (int i = 0; i < AmbientPoints.Length; i++)
			{
				if (play)
				{
					AmbientPoints[i].AudioSource.Play();
				}
				else
				{
					AmbientPoints[i].AudioSource.Stop();
				}
			}
		}

		private bool Mute(bool mute)
		{
			if (mute)
			{
				for (int i = 0; i < AmbientPoints.Length; i++)
				{
					AmbientPoints[i].CurrentVolume = Mathf.Lerp(AmbientPoints[i].CurrentVolume, AmbientPoints[i].MinVolume, Time.deltaTime * 0.5f);
					AmbientPoints[i].AudioSource.volume = AmbientPoints[i].CurrentVolume * currentVolume;
					if (AmbientPoints[i].AudioSource.volume <= AmbientPoints[i].MinVolume + 0.001f)
					{
						return false;
					}
				}
			}
			else
			{
				for (int j = 0; j < AmbientPoints.Length; j++)
				{
					AmbientPoints[j].CurrentVolume = Mathf.Lerp(AmbientPoints[j].CurrentVolume, AmbientPoints[j].MaxVolume, Time.deltaTime * 0.5f);
					AmbientPoints[j].AudioSource.volume = AmbientPoints[j].CurrentVolume * currentVolume;
					if (AmbientPoints[j].AudioSource.volume >= AmbientPoints[j].MaxVolume - 0.001f)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void Update()
		{
			if (IsPlaing)
			{
				Mute(muting);
			}
		}

		private IEnumerator Detect()
		{
			WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
			while (true)
			{
				if (player == null)
				{
					yield return waitForSeconds;
				}
				if (player != null)
				{
					if (CurrZoneType.Equals(ZoneType.Sphere))
					{
						if (Vector3.Distance(base.transform.position, player.position) < SwitchingDistance && !IsPlaing)
						{
							PlayStopSounds();
							IsPlaing = true;
						}
						if (Vector3.Distance(base.transform.position, player.position) > SwitchingDistance && IsPlaing)
						{
							PlayStopSounds(play: false);
							IsPlaing = false;
						}
					}
					if (CurrZoneType.Equals(ZoneType.Box))
					{
						if (Vector3.Distance(base.transform.position, player.position) < SwitchingDistance + 20f)
						{
							if ((bool)BoxTrigger)
							{
								BoxTrigger.enabled = true;
							}
						}
						else if ((bool)BoxTrigger)
						{
							BoxTrigger.enabled = false;
						}
					}
				}
				muting = CheckChildsPoints();
				yield return waitForSeconds;
			}
		}

		public void OnDrawGizmosSelected()
		{
			if (!AllGizmos)
			{
				if (CurrZoneType.Equals(ZoneType.Box))
				{
					Gizmos.matrix = base.transform.localToWorldMatrix;
					Gizmos.color = GizmosColor;
					Gizmos.DrawCube(Vector3.zero, BoxRect);
				}
				if (CurrZoneType.Equals(ZoneType.Sphere))
				{
					Gizmos.DrawSphere(base.transform.position, SwitchingDistance);
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (AllGizmos)
			{
				Gizmos.color = GizmosColor;
				if (CurrZoneType.Equals(ZoneType.Box))
				{
					Gizmos.matrix = base.transform.localToWorldMatrix;
					Gizmos.color = GizmosColor;
					Gizmos.DrawCube(Vector3.zero, BoxRect);
				}
				if (CurrZoneType.Equals(ZoneType.Sphere))
				{
					Gizmos.DrawSphere(base.transform.position, SwitchingDistance);
				}
			}
		}

		private IEnumerator SoftStop()
		{
			while (Mute(mute: true))
			{
				yield return new WaitForSeconds(Time.deltaTime);
			}
			PlayStopSounds(play: false);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!IsPlaing)
			{
				PlayStopSounds();
				IsPlaing = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (IsPlaing)
			{
				IsPlaing = false;
				StartCoroutine(SoftStop());
			}
		}
	}
}
