using Game.GlobalComponent.Qwest;
using Game.PickUps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class CollectionPickUpsManager : MonoBehaviour
	{
		public class Reward
		{
			public delegate void RewardAction();

			public CollectionTypes collectionType;

			public int count;

			public RewardAction Action;

			public Reward(CollectionTypes type, int countForStart, RewardAction actionForRewardDelegate)
			{
				collectionType = type;
				count = countForStart;
				Action = actionForRewardDelegate;
			}
		}

		public enum CollectionTypes
		{
			Biceps,
			Rope,
			Hook
		}

		public delegate void OnElementTaken(CollectionTypes type);

		private static CollectionPickUpsManager _instance;

		public bool restart;

		public Transform collectionsContainer;

		public ObjectRespawner[] collectionRespauners;

		private bool[] collectinBools;

		public Dictionary<GameObject, ObjectRespawner> pickUpRespawnerDict = new Dictionary<GameObject, ObjectRespawner>();

		public Dictionary<ObjectRespawner, GameObject> respawnerPickUpDict = new Dictionary<ObjectRespawner, GameObject>();

		public Dictionary<CollectionTypes, int> totalAmmount = new Dictionary<CollectionTypes, int>();

		public Dictionary<CollectionTypes, int> countAmmount = new Dictionary<CollectionTypes, int>();

		public Action OnManagerInitAction;

		public OnElementTaken OnElementTakenEvent;

		private bool initialized;

		private List<Reward> RewardList = new List<Reward>();

		public static CollectionPickUpsManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.Object.FindObjectOfType<CollectionPickUpsManager>();
				}
				return _instance;
			}
		}

		public void OnPickupCreate(ObjectRespawner respawner, GameObject pickupObject)
		{
			if ((bool)pickupObject)
			{
				if (pickUpRespawnerDict.ContainsKey(pickupObject))
				{
					pickUpRespawnerDict.Remove(pickupObject);
				}
				pickUpRespawnerDict.Add(pickupObject, respawner);
				if (respawnerPickUpDict.ContainsKey(respawner))
				{
					respawnerPickUpDict.Remove(respawner);
				}
				respawnerPickUpDict.Add(respawner, pickupObject);
			}
		}

		public void SaveCollections()
		{
			if (collectinBools != null)
			{
				BaseProfile.StoreArray(collectinBools, "Collections");
			}
		}

		public void ElementWasTaken(GameObject elem)
		{
			ObjectRespawner objectRespawner = pickUpRespawnerDict[elem];
			for (int i = 0; i < collectionRespauners.Length; i++)
			{
				if (collectionRespauners[i] == objectRespawner)
				{
					collectinBools[i] = true;
					break;
				}
			}
			RefreshAmmount();
			CollectionPickup component = objectRespawner.ObjectPrefab.GetComponent<CollectionPickup>();
			if (!(component == null))
			{
				CollectionTypes collectionType = component.CollectionType;
				string specificString = "You have collected " + countAmmount[collectionType] + " / " + totalAmmount[collectionType] + " of " + collectionType.ToString();
				InGameLogManager.Instance.RegisterNewMessage(MessageType.Collect, specificString);
				if (OnElementTakenEvent != null)
				{
					OnElementTakenEvent(collectionType);
				}
				foreach (Reward reward in RewardList)
				{
					if (collectionType == reward.collectionType && countAmmount[reward.collectionType] == reward.count)
					{
						reward.Action();
					}
				}
			}
		}

		private void LoadCollections()
		{
			try
			{
				bool[] array = BaseProfile.ResolveArray<bool>("Collections");
				if (array.Length == collectionRespauners.Length)
				{
					collectinBools = array;
				}
				else
				{
					BaseProfile.StoreArray(new bool[collectionRespauners.Length], "Collections");
					LoadCollections();
				}
			}
			catch (Exception)
			{
				BaseProfile.StoreArray(collectinBools, "Collections");
				LoadCollections();
			}
		}

		private void ResetCollections()
		{
			bool[] values = new bool[collectionRespauners.Length];
			BaseProfile.StoreArray(values, "Collections");
		}

		private void Awake()
		{
			if (initialized)
			{
				return;
			}
			initialized = true;
			collectionRespauners = collectionsContainer.GetComponentsInChildren<ObjectRespawner>();
			if (restart)
			{
				ResetCollections();
			}
			collectinBools = new bool[collectionRespauners.Length];
			LoadCollections();
			for (int i = 0; i < collectionRespauners.Length; i++)
			{
				collectionRespauners[i].SetCollectionRespawner();
				collectionRespauners[i].SetIsTaken(collectinBools[i]);
				if (!collectinBools[i] && respawnerPickUpDict.ContainsKey(collectionRespauners[i]))
				{
					PoolManager.Instance.ReturnToPool(respawnerPickUpDict[collectionRespauners[i]]);
					respawnerPickUpDict[collectionRespauners[i]].SetActive(value: false);
				}
			}
			RefreshAmmount();
			AddRevard();
			if (OnManagerInitAction != null)
			{
				OnManagerInitAction();
			}
		}

		private void RefreshAmmount()
		{
			foreach (int value in Enum.GetValues(typeof(CollectionTypes)))
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < collectionRespauners.Length; i++)
				{
					if (collectinBools[i])
					{
						collectionRespauners[i].SetIsTaken(val: true);
					}
					CollectionPickup component = collectionRespauners[i].ObjectPrefab.GetComponent<CollectionPickup>();
					if (!(component == null) && component.CollectionType == (CollectionTypes)value)
					{
						num++;
						if (collectinBools[i])
						{
							num2++;
						}
					}
				}
				totalAmmount[(CollectionTypes)value] = num;
				countAmmount[(CollectionTypes)value] = num2;
			}
		}

		private void AddRevard()
		{
			foreach (int value in Enum.GetValues(typeof(CollectionTypes)))
			{
				CollectionTypes ct = (CollectionTypes)value;
				Reward item = new Reward((CollectionTypes)value, totalAmmount[(CollectionTypes)value], delegate
				{
					GameEventManager.Instance.Event.PickUpCollectionEvent(ct.ToString());
				});
				RewardList.Add(item);
			}
		}
	}
}
