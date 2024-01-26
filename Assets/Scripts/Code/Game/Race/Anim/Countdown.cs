using UnityEngine;
using UnityEngine.UI;

namespace Code.Game.Race.Anim
{
	public class Countdown : MonoBehaviour
	{
		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private Text text;

		public void SetAudio(AudioClip audioClip)
		{
			audioSource.clip = audioClip;
			audioSource.Play();
		}

		public void SetText(string text)
		{
			this.text.text = text;
		}
	}
}
