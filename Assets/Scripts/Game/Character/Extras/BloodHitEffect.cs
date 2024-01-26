using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character.Extras
{
	public class BloodHitEffect : BaseHitEffect
	{
		private const float DestructPrefabTime = 5f;

		public static BloodHitEffect Instance;

		public GameObject[] BloodSFXPrefabs;

		protected override void Awake()
		{
			emmiters = GetComponentsInChildren<ParticleSystem>();
			Instance = this;
		}

		public override void Emit(Vector3 pos)
		{
			base.transform.position = pos;
			if (BloodSFXPrefabs.Length != 0)
			{
				int num = Random.Range(0, BloodSFXPrefabs.Length);
				GameObject fromPool = PoolManager.Instance.GetFromPool(BloodSFXPrefabs[num]);
				fromPool.transform.position = pos;
				PoolManager.Instance.ReturnToPoolWithDelay(fromPool, 5f);
			}
		}
	}
}
