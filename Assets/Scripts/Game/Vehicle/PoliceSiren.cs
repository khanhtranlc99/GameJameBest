using Game.GlobalComponent;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Vehicle
{
	public class PoliceSiren : MonoBehaviour, IInitable
	{
		private const int ChangeMarkerWaitingFrame = 15;

		public GameObject BlueLight;

		public GameObject RedLight;

		public GameObject BlueMark;

		public GameObject RedMark;

		public float ChangeLightTime = 0.5f;

		public AudioSource SirenAudioSource;

		public AudioClip SirenSound;

		private bool working;

		private bool activeRedLight;

		private bool activeRedMark;

		private SlowUpdateProc slowUpdateProc;

		private DummyDriver currentDummyDriver;

		public void Init()
		{
			if ((bool)SirenSound)
			{
				SirenAudioSource.clip = SirenSound;
			}
			Invoke("InitDummyDriver", 0.2f);
		}

		public void DeInit()
		{
			if ((bool)SirenAudioSource)
			{
				SirenAudioSource.Stop();
				SirenAudioSource.clip = null;
			}
			BlueLight.SetActive(value: false);
			RedLight.SetActive(value: false);
			RedMark.SetActive(value: false);
			BlueMark.SetActive(value: false);
			currentDummyDriver = null;
			working = false;
			StopAllCoroutines();
		}

		private void Awake()
		{
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, ChangeLightTime);
			DeInit();
		}

		private void FixedUpdate()
		{
			if (working)
			{
				slowUpdateProc.ProceedOnFixedUpdate();
			}
		}

		private void InitDummyDriver()
		{
			currentDummyDriver = GetComponentInChildren<DummyDriver>();
			if ((bool)currentDummyDriver)
			{
				DummyDriver dummyDriver = currentDummyDriver;
				dummyDriver.DummyExitEvent = (DummyDriver.DummyEvent)Delegate.Combine(dummyDriver.DummyExitEvent, new DummyDriver.DummyEvent(DeInit));
				SirenAudioSource.Play();
				StartCoroutine(MiniMapMarkerChange());
				working = true;
			}
		}

		private void SlowUpdate()
		{
			activeRedLight = !activeRedLight;
			RedLight.SetActive(activeRedLight);
			BlueLight.SetActive(!activeRedLight);
		}

		private IEnumerator MiniMapMarkerChange()
		{
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while (true)
			{
				activeRedMark = !activeRedMark;
				RedMark.SetActive(activeRedMark);
				BlueMark.SetActive(!activeRedMark);
				for (int i = 0; i < 15; i++)
				{
					yield return waitForEndOfFrame;
				}
			}
		}
	}
}
