using UnityEngine;

namespace Game.Enemy
{
	public class DummyNPCSensor : MonoBehaviour
	{
		private float counter;

		public bool CanMove => counter <= 0f;

		private void Update()
		{
			if (counter > 0f)
			{
				counter -= Time.deltaTime;
			}
		}

		private void OnTriggerStay(Collider col)
		{
			counter = 1f;
		}
	}
}
