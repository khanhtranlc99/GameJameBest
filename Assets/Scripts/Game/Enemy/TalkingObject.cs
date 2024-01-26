using System;
using UnityEngine;

namespace Game.Enemy
{
	[RequireComponent(typeof(AudioSource), typeof(BaseSoundController))]
	public class TalkingObject : MonoBehaviour
	{
		public enum PhraseType
		{
			Greeting,
			Alarm,
			RunOut,
			Attack
		}

		[Serializable]
		public class Phrase
		{
			public PhraseType Type;

			public AudioClip[] Clips;

			[Range(0f, 1f)]
			public float ChanceToProc;
		}

		private const int SuccessfulPhraseCooldownCounter = 2;

		public Phrase[] Phrases;

		private float nextAvaibleTalkTime;

		private AudioSource audioSource;

		private BaseNPC parentNPC;

		public bool IsGreetingAvaible
		{
			get
			{
				if (parentNPC == null)
				{
					parentNPC = base.gameObject.GetComponentInParent<BaseNPC>();
				}
				return parentNPC.CurrentControllerType == BaseNPC.NPCControllerType.Simple;
			}
		}

		public void TalkPhraseOfType(PhraseType type)
		{
			if (Time.time < nextAvaibleTalkTime)
			{
				return;
			}
			Phrase phrase = null;
			Phrase[] phrases = Phrases;
			foreach (Phrase phrase2 in phrases)
			{
				if (phrase2.Type == type)
				{
					phrase = phrase2;
					break;
				}
			}
			if (phrase == null || phrase.Clips.Length == 0)
			{
				return;
			}
			AudioClip audioClip = phrase.Clips[UnityEngine.Random.Range(0, phrase.Clips.Length)];
			if (!(audioClip == null))
			{
				float num = audioClip.length;
				if (UnityEngine.Random.value <= phrase.ChanceToProc)
				{
					num *= 2f;
					audioSource.PlayOneShot(audioClip);
				}
				nextAvaibleTalkTime = Time.time + num;
			}
		}

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			var boxCol = GetComponentInChildren<Collider>();
			if (boxCol != null && !boxCol.isTrigger)
				boxCol.isTrigger = true;
		}
	}
}
