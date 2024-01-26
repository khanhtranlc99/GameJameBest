using Game.GlobalComponent;
using UnityEngine;

namespace Game.Character.Extras
{
	public class BloodSFX : MonoBehaviour
	{
		private const float DestructPrefabTime = 5f;

		public static BloodSFX Instance;

		public GameObject[] BloodSFXPrefabs;

		private ParticleSystem[] emmiters;

		private void Awake()
		{
			emmiters = GetComponentsInChildren<ParticleSystem>();
			Instance = this;
		}

		public void Emit(Vector3 pos)
		{
			base.transform.position = pos;
			if (BloodSFXPrefabs.Length != 0)
			{
				int num = UnityEngine.Random.Range(0, BloodSFXPrefabs.Length);
				GameObject fromPool = PoolManager.Instance.GetFromPool(BloodSFXPrefabs[num]);
				fromPool.transform.position = pos;
				PoolManager.Instance.ReturnToPoolWithDelay(fromPool, 5f);
				return;
			}
			ParticleSystem[] array = emmiters;
			foreach (ParticleSystem particleEmitter in array)
			{
				var emit = particleEmitter.emission;
				emit.enabled = true;
				//particleEmitter.Emit();
			}
		}
	}
}
