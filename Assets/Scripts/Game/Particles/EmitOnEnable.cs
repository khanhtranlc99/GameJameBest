using UnityEngine;

namespace Game.Particles
{
	public class EmitOnEnable : MonoBehaviour
	{
		private ParticleSystem emitter;

		private void Awake()
		{
			emitter = GetComponent<ParticleSystem>();
		}

		private void OnEnable()
		{
			var emit = emitter.emission;
			emit.enabled = true;
		}

		private void OnDisable()
		{
			var emit = emitter.emission;
			emit.enabled = false;
		}
	}
}
