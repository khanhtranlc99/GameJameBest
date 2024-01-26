using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Extras
{
	public class BloodScreenSplash : MonoBehaviour
	{
		public static BloodScreenSplash Instance;

		public float FadeoutTimer = 1f;

		public float MaxAlpha = 0.5f;

		private Image bloodTexture;

		private float timeout;

		private void Awake()
		{
			Instance = this;
			bloodTexture = GetComponent<Image>();
		}

		public void Hit()
		{
			timeout = FadeoutTimer;
		}

		private void Update()
		{
			Color color = bloodTexture.color;
			color.a = Mathf.Clamp01(timeout / FadeoutTimer) * MaxAlpha;
			bloodTexture.color = color;
			timeout -= Time.deltaTime;
		}
	}
}
