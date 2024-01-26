using Game.GlobalComponent;
using Game.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PickUps
{
	public class PickUpManager : MonoBehaviour
	{
		public enum PickupType
		{
			Money,
			Bullets,
			HealthPack,
			QuestItem,
			EnegryPack,
			Weapon,
			Collection
		}

		public enum HealthPackType
		{
			Random = -1,
			Small,
			Medium,
			Large
		}

		private const int MaxPlacedPickups = 20;

		public static int MinMoney = 80;

		public static int MaxMoney = 120;

		private static PickUpManager instance;

		public PickUp[] Pickups;

		public AudioClip MoneyPickupSound;

		public AudioClip BulletsPickupSound;

		public AudioClip HealthPackPickupSound;

		public AudioClip QuestItemPickupSound;

		public AudioClip WeaponItemPickupSound;

		private readonly IDictionary<MoneyPickup, PickUp> moneyPickupPrefabs = new Dictionary<MoneyPickup, PickUp>();

		private readonly IDictionary<BulletsPickup, PickUp> bulletsPickupPrefabs = new Dictionary<BulletsPickup, PickUp>();

		private readonly IDictionary<HealthPickup, PickUp> healthPackPrefabs = new Dictionary<HealthPickup, PickUp>();

		private readonly IDictionary<EnergyPickup, PickUp> energyPackPrefabs = new Dictionary<EnergyPickup, PickUp>();

		private readonly IDictionary<WeaponPickup, PickUp> weaponPackPrefabs = new Dictionary<WeaponPickup, PickUp>();

		private readonly IDictionary<CollectionPickup, PickUp> CollectionPackPrefabs = new Dictionary<CollectionPickup, PickUp>();

		private readonly List<PickUp> dropedPickup = new List<PickUp>();

		private readonly HashSet<PickUp> takedPickup = new HashSet<PickUp>();

		public static PickUpManager Instance => instance ?? (instance = new PickUpManager());

		private void Awake()
		{
			instance = this;
			PickUp[] pickups = Pickups;
			foreach (PickUp pickUp in pickups)
			{
				BulletsPickup bulletsPickup = pickUp as BulletsPickup;
				if (bulletsPickup != null)
				{
					bulletsPickupPrefabs.Add(bulletsPickup, pickUp);
					continue;
				}
				HealthPickup healthPickup = pickUp as HealthPickup;
				if (healthPickup != null)
				{
					healthPackPrefabs.Add(healthPickup, pickUp);
					continue;
				}
				MoneyPickup moneyPickup = pickUp as MoneyPickup;
				if (moneyPickup != null)
				{
					moneyPickupPrefabs.Add(moneyPickup, pickUp);
				}
				EnergyPickup energyPickup = pickUp as EnergyPickup;
				if ((bool)energyPickup)
				{
					energyPackPrefabs.Add(energyPickup, pickUp);
				}
				WeaponPickup weaponPickup = pickUp as WeaponPickup;
				if ((bool)weaponPickup)
				{
					weaponPackPrefabs.Add(weaponPickup, pickUp);
				}
				CollectionPickup collectionPickup = pickUp as CollectionPickup;
				if ((bool)collectionPickup)
				{
					CollectionPackPrefabs.Add(collectionPickup, pickUp);
				}
			}
		}

		public void RegisterPickup(PickUp pickUp)
		{
			ControllPickupsCount();
			dropedPickup.Add(pickUp);
			takedPickup.Remove(pickUp);
		}

		public void OnTakedPickup(PickUp pickUp)
		{
			PickupSound(pickUp);
			takedPickup.Add(pickUp);
			dropedPickup.Remove(pickUp);
		}

		public bool PickupWasTaked(PickUp pickUp)
		{
			return takedPickup.Contains(pickUp);
		}

		public void GenerateMoneyOnPoint(Vector3 position)
		{
			GameObject gameObject = null;
			using (IEnumerator<KeyValuePair<MoneyPickup, PickUp>> enumerator = moneyPickupPrefabs.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					gameObject = enumerator.Current.Value.gameObject;
				}
			}
			if (gameObject != null)
			{
				PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Pickup Money not assigned!");
		}

		public void GenerateBulletsOnPoint(Vector3 position, AmmoTypes bulletsType)
		{
			GameObject gameObject = null;
			foreach (KeyValuePair<BulletsPickup, PickUp> bulletsPickupPrefab in bulletsPickupPrefabs)
			{
				if (bulletsPickupPrefab.Key.BulletType == bulletsType)
				{
					gameObject = bulletsPickupPrefab.Value.gameObject;
					break;
				}
			}
			if (gameObject != null)
			{
				PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Pickup bullets for weapon " + bulletsType + " not assigned!");
		}

		public void GenerateEnergyOnPoint(Vector3 position)
		{
			GameObject gameObject = null;
			foreach (KeyValuePair<EnergyPickup, PickUp> energyPackPrefab in energyPackPrefabs)
			{
				gameObject = energyPackPrefab.Value.gameObject;
			}
			if (gameObject != null)
			{
				PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Energy prefab not assigned!");
		}

		public void GenerateHealthPackOnPoint(Vector3 position, HealthPackType healthPackType)
		{
			HealthPackType healthPackType2 = healthPackType;
			if (healthPackType2 == HealthPackType.Random)
			{
				healthPackType2 = (HealthPackType)UnityEngine.Random.Range(0, 3);
			}
			GameObject gameObject = null;
			foreach (KeyValuePair<HealthPickup, PickUp> healthPackPrefab in healthPackPrefabs)
			{
				if (healthPackPrefab.Key.HealthPackType == healthPackType2)
				{
					gameObject = healthPackPrefab.Value.gameObject;
					break;
				}
			}
			if (gameObject != null)
			{
				PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Pickup for Helathpack type of " + healthPackType + " not assigned!");
		}

		private void PlacePickupPrefab(GameObject pickupPrefab, Vector3 position)
		{
			GameObject fromPool = PoolManager.Instance.GetFromPool(pickupPrefab);
			fromPool.transform.position = position;
			fromPool.transform.eulerAngles = Vector3.zero;
			RegisterPickup(fromPool.GetComponent<PickUp>());
		}

		private void ControllPickupsCount()
		{
			if (dropedPickup.Count >= 20)
			{
				PickUp pickUp = dropedPickup[0];
				PoolManager.Instance.ReturnToPool(pickUp);
				dropedPickup.Remove(pickUp);
				takedPickup.Add(pickUp);
			}
		}

		private void PickupSound(PickUp pickup)
		{
			PointSoundManager pointSoundManager = PointSoundManager.Instance;
			Vector3 position = pickup.transform.position;
			if (pickup is MoneyPickup)
			{
				pointSoundManager.PlayCustomClipAtPoint(position, MoneyPickupSound);
			}
			else if (pickup is BulletsPickup)
			{
				pointSoundManager.PlayCustomClipAtPoint(position, BulletsPickupSound);
			}
			else if (pickup is HealthPickup)
			{
				pointSoundManager.PlayCustomClipAtPoint(position, HealthPackPickupSound);
			}
			else if (pickup is QuestPickUp)
			{
				pointSoundManager.PlayCustomClipAtPoint(position, QuestItemPickupSound);
			}
		}
	}
}
