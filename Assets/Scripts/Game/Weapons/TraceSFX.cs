using UnityEngine;

namespace Game.Weapons
{
	public class TraceSFX : MonoBehaviour
	{
		private ParticleSystem[] emmiters;

		public static TraceSFX Instance;

		protected void Awake()
		{
			emmiters = GetComponentsInChildren<ParticleSystem>();
			Instance = this;
		}

		public void Emit(Vector3 pos, Vector3 direction,GameObject obj)
		{
			transform.position = pos+direction*6f;
			//direction = transform.InverseTransformDirection(direction);
			transform.forward = direction;
			foreach (var particleEmitter in emmiters)
			{
				particleEmitter.Play();
			}
		}
	}
}
