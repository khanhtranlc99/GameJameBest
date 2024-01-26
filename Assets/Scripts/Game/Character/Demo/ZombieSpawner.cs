using UnityEngine;

namespace Game.Character.Demo
{
	public class ZombieSpawner : MonoBehaviour
	{
		public GameObject ZombiePrefab;

		public GameObject WormPrefab;

		private bool spawnHell;

		public static ZombieSpawner Instance
		{
			get;
			private set;
		}

		public void SpawnZombieAt(Vector3 position)
		{
			Object.Instantiate(ZombiePrefab, position, Quaternion.identity);
		}

		public void SpawnWormAt(Vector3 position)
		{
			Object.Instantiate(WormPrefab, position, Quaternion.identity);
		}

		public void SpawnZombie()
		{
			SpawnZombieAt(base.gameObject.transform.position);
		}

		public void SpawnWorm()
		{
			SpawnWormAt(base.gameObject.transform.position);
		}

		public void SpawnHell()
		{
			for (int i = 0; i < 10; i++)
			{
				SpawnZombie();
				SpawnWorm();
			}
		}

		public void SpawnHellOnce()
		{
			if (!spawnHell)
			{
				SpawnHell();
				spawnHell = true;
			}
		}

		private void Awake()
		{
			Instance = this;
		}
	}
}
