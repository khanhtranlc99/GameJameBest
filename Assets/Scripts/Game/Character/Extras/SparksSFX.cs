using UnityEngine;

namespace Game.Character.Extras
{
	public class SparksSFX : MonoBehaviour
	{
		public static SparksSFX Instance;

		private ParticleSystem[] emmiters;

		private void Awake()
		{
			emmiters = GetComponentsInChildren<ParticleSystem>();
			Instance = this;
		}

		public void Emit(Vector3 pos)
		{
			base.transform.position = pos;
			foreach (var particleEmitter in emmiters)
			{
				var emit = particleEmitter.emission;
				emit.enabled = true;
			}
		}
	}
}
