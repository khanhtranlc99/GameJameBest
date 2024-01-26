using UnityEngine;

namespace Game.Character.Extras
{
	public class WaterSpraySFX : MonoBehaviour
	{

		private ParticleSystem[] emmiters;
		public static WaterSpraySFX Instance;
		private void Awake()
		{
			emmiters = GetComponentsInChildren<ParticleSystem>();
			Instance = this;
			foreach (var VARIABLE in emmiters)
			{
				VARIABLE.gameObject.SetActive(false);
			}
		}

		public void Emit(Vector3 pos)
		{

			base.transform.position = pos;
			foreach (var particleEmitter in emmiters)
			{
				particleEmitter.gameObject.SetActive(true);
			}
		}
	}
}
