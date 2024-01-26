using UnityEngine;

namespace Game.Particles
{
	public class DelayedDisableObject : MonoBehaviour
	{
		public float Delay;

		private float delayTimer;

		private void OnEnable()
		{
			delayTimer = Delay;
		}

		private void Update()
		{
			if (delayTimer > 0f)
			{
				delayTimer -= Time.deltaTime;
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
