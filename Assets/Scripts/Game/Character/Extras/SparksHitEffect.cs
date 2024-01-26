using UnityEngine;

namespace Game.Character.Extras
{
	public class SparksHitEffect : BaseHitEffect
	{
		public static SparksHitEffect Instance;
		protected override void Awake()
		{
			emmiters = GetComponentsInChildren<ParticleSystem>();
			Instance = this;
		}

		public override void Emit(Vector3 pos)
		{
			base.transform.position = pos;
			foreach (var particleEmitter in emmiters)
			{
				particleEmitter.Play();
			}
		}
	}
}
