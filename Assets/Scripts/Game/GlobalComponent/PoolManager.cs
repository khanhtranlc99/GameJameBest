using Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PoolManager : MonoBehaviour
	{
		[Serializable]
		public class PoolConfig
		{
			public GameObject Prefab;

			public int InitialCount = 2;

			public int MaximumCount = 100;

			public List<Type> InitableComponents = new List<Type>();

			private GameObject wrapper;

			private int itemsInUse;

			public GameObject Wrapper
			{
				get
				{
					return wrapper;
				}
				set
				{
					wrapper = value;
				}
			}

			public int ItemsInUse
			{
				get
				{
					return itemsInUse;
				}
				set
				{
					itemsInUse = value;
				}
			}
		}

		private class PoolFillRequest
		{
			public List<GameObject> VacantObjects;

			public PoolConfig Config;

			public int ToFillCount;
		}

		private static PoolManager instance;

		public bool IsDebug;

		[Separator("Debug components")]
		public GameObject[] DebugArray;

		public int TotalItemsCount;

		public int ItemInUseCount;

		private IDictionary<string, List<GameObject>> storage = new Dictionary<string, List<GameObject>>();

		private IDictionary<GameObject, string> itemsInUse = new Dictionary<GameObject, string>();

		private List<GameObject> activeItems = new List<GameObject>();

		private IDictionary<GameObject, float> debugItemsInUse = new Dictionary<GameObject, float>();

		private IDictionary<string, PoolConfig> poolConfigsByName = new Dictionary<string, PoolConfig>();

		private readonly List<PoolFillRequest> fillRequests = new List<PoolFillRequest>();

		private PoolEventProcessor eventProcessor = new PoolEventProcessor();

		private SlowUpdateProc slowUpdateProc;

		private SlowUpdateProc autoFillUpdateProc;

		public static PoolManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("PoolManager is not initialized");
				}
				return instance;
			}
		}

		public int GetItemInUse<T>(T[] items) where T : Component
		{
			int num = 0;
			int count = activeItems.Count;
			for (int i = 0; i < count; i++)
			{
				if (num >= items.Length)
				{
					break;
				}
				T component = activeItems[i].GetComponent<T>();
				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
				{
					items[num] = component;
					num++;
				}
			}
			return num;
		}

		public T GetFromPool<T>(T prefabComponent) where T : Component
		{
			return GetFromPool(prefabComponent.gameObject, Vector3.zero, Quaternion.identity).GetComponent<T>();
		}

		public T GetFromPool<T>(T prefabComponent, Vector3 position, Quaternion rotation) where T : Component
		{
			return GetFromPool(prefabComponent.gameObject, position, rotation).GetComponent<T>();
		}

		public GameObject GetFromPool(GameObject prefab)
		{
			return GetFromPool(prefab, Vector3.zero, Quaternion.identity);
		}

		public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			InitPoolingPrefab(prefab);
			return GetFromPoolNotInitable(prefab.name, position, rotation);
		}

		public void InitPoolingPrefab(Component prefab, int initialCount = 2)
		{
			InitPoolingPrefab(prefab.gameObject, initialCount);
		}

		public void InitPoolingPrefab(GameObject prefab, int initialCount = 2)
		{
			if (storage.ContainsKey(prefab.name))
			{
				return;
			}
			if (prefab.name.Contains("(Clone)"))
			{
				throw new Exception("Prefab based objects are only allowed");
			}
			PoolConfig poolConfig = new PoolConfig();
			poolConfig.Prefab = prefab;
			poolConfig.InitialCount = initialCount;
			Component[] components = prefab.GetComponents<Component>();
			Component[] array = components;
			foreach (Component component in array)
			{
				if (component is IInitable)
				{
					poolConfig.InitableComponents.Add(component.GetType());
				}
			}
			InitCustomConfig(poolConfig);
		}

		public GameObject GetFromPoolNotInitable(string poolName, Vector3 position, Quaternion rotation)
		{
			if (storage.ContainsKey(poolName))
			{
				List<GameObject> list = storage[poolName];
				PoolConfig poolConfig = poolConfigsByName[poolName];
				if (list.Count == 0)
				{
					PoolFillRequest poolFillRequest = new PoolFillRequest();
					poolFillRequest.Config = poolConfig;
					poolFillRequest.VacantObjects = list;
					poolFillRequest.ToFillCount = 1;
					PoolFillRequest request = poolFillRequest;
					CompleteRequest(request);
				}
				if (list.Count > 0)
				{
					GameObject gameObject = list[list.Count - 1];
					RectTransform rectTransform = gameObject.transform as RectTransform;
					if (rectTransform != null)
					{
						rectTransform.SetParent(null, worldPositionStays: false);
					}
					else
					{
						gameObject.transform.parent = null;
					}
					gameObject.transform.position = position;
					gameObject.transform.rotation = rotation;
					itemsInUse.Add(gameObject, poolName);
					activeItems.Add(gameObject);
					if (IsDebug)
					{
						debugItemsInUse.Add(gameObject, Time.timeSinceLevelLoad);
					}
					list.Remove(gameObject);
					if (list.Count == 0)
					{
						PoolFillRequest poolFillRequest = new PoolFillRequest();
						poolFillRequest.Config = poolConfig;
						poolFillRequest.VacantObjects = list;
						poolFillRequest.ToFillCount = 3;
						PoolFillRequest item = poolFillRequest;
						fillRequests.Add(item);
					}
					poolConfig.ItemsInUse++;
					gameObject.SetActive(value: true);
					foreach (Type initableComponent in poolConfig.InitableComponents)
					{
						Component[] components = gameObject.GetComponents(initableComponent);
						if (components != null)
						{
							Component[] array = components;
							foreach (Component component in array)
							{
								(component as IInitable)?.Init();
							}
						}
					}
					ItemInUseCount++;
					return gameObject;
				}
			}
			throw new Exception("No such pool named " + poolName);
		}

		public T GetFromPool<T>(GameObject prefab) where T : Component
		{
			return GetFromPool(prefab.GetComponent<T>());
		}

		public T GetFromPoolNotInitable<T>(string poolName, Vector3 position, Quaternion rotation) where T : Component
		{
			return GetFromPoolNotInitable(poolName, position, rotation).GetComponent<T>();
		}

		public bool AddBeforeReturnEvent(Component pooledObject, PoolEventProcessor.PoolEvent poolEvent)
		{
			return AddBeforeReturnEvent(pooledObject.gameObject, poolEvent);
		}

		public bool AddBeforeReturnEvent(GameObject gameObject, PoolEventProcessor.PoolEvent poolEvent)
		{
			if (gameObject == null)
			{
				return false;
			}
			if (itemsInUse.ContainsKey(gameObject))
			{
				eventProcessor.AddBeforeReturnToPoolEvent(gameObject, poolEvent);
				return true;
			}
			return false;
		}

		public bool ReturnToPool(GameObject o)
		{
			if (o != null && itemsInUse.ContainsKey(o))
			{
				eventProcessor.InvokeBeforeReturnToPoolEvents(o);
				o.SetActive(value: false);
				string key = itemsInUse[o];
				PoolConfig poolConfig = poolConfigsByName[key];
				poolConfig.ItemsInUse--;
				foreach (Type initableComponent in poolConfig.InitableComponents)
				{
					Component[] components = o.GetComponents(initableComponent);
					if (components != null)
					{
						Component[] array = components;
						foreach (Component component in array)
						{
							(component as IInitable)?.DeInit();
						}
					}
				}
				itemsInUse.Remove(o);
				activeItems.Remove(o);
				if (IsDebug)
				{
					debugItemsInUse.Remove(o);
				}
				storage[key].Add(o);
				RectTransform rectTransform = o.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.SetParent(poolConfig.Wrapper.transform, worldPositionStays: false);
				}
				else
				{
					o.transform.parent = poolConfig.Wrapper.transform;
				}
				o.transform.position = Vector3.zero;
				ItemInUseCount--;
				return true;
			}
			if (o != null && o.transform.IsChildOf(base.transform))
			{
				return true;
			}
			return false;
		}

		public PoolConfig GetConfig(GameObject prefab)
		{
			return GetConfig(prefab.name);
		}

		public PoolConfig GetConfig(string poolName)
		{
			PoolConfig _;
			poolConfigsByName.TryGetValue(poolName, out _);
			return null;
		}

		public bool ReturnToPool(Component o)
		{
			return ReturnToPool(o.gameObject);
		}

		public bool ReturnToPoolWithDelay(Component poolingObject, float timeDelay)
		{
			return ReturnToPoolWithDelay(poolingObject.gameObject, timeDelay);
		}

		public bool ReturnToPoolWithDelay(GameObject o, float timeDelay)
		{
			if (itemsInUse.ContainsKey(o))
			{
				StartCoroutine(ReturnToPoolEnumerator(o, timeDelay));
				return true;
			}
			return false;
		}

		public GameObject PrefabOf(GameObject pooledObject)
		{
			if (pooledObject != null && itemsInUse.ContainsKey(pooledObject))
			{
				string key = itemsInUse[pooledObject];
				PoolConfig poolConfig = poolConfigsByName[key];
				return poolConfig.Prefab;
			}
			return null;
		}

		private IEnumerator ReturnToPoolEnumerator(GameObject o, float timeDelay)
		{
			yield return new WaitForSeconds(timeDelay);
			ReturnToPool(o);
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			slowUpdateProc = new SlowUpdateProc(DebugSlowUpdate, 10f);
			autoFillUpdateProc = new SlowUpdateProc(AutoFillSlowUpdate, 0.6f);
		}

		private void DebugSlowUpdate()
		{
			if (IsDebug)
			{
				DebugArray = debugItemsInUse.Keys.ToArray();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<GameObject, float> item in debugItemsInUse)
				{
					stringBuilder.Append(item.Value).Append(" ").Append(item.Key.ToString())
						.Append("\n");
				}
				if (GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log(stringBuilder.ToString());
				}
			}
		}

		private void AutoFillSlowUpdate()
		{
			if (fillRequests.Count != 0)
			{
				PoolFillRequest poolFillRequest = fillRequests[0];
				int num = CompleteRequest(poolFillRequest);
				poolFillRequest.ToFillCount -= num;
				if (poolFillRequest.ToFillCount <= 0)
				{
					fillRequests.Remove(poolFillRequest);
				}
			}
		}

		private int CompleteRequest(PoolFillRequest request)
		{
			List<GameObject> vacantObjects = request.VacantObjects;
			PoolConfig config = request.Config;
			if (config.ItemsInUse < config.MaximumCount)
			{
				int num = Mathf.Min(config.MaximumCount, vacantObjects.Count + config.ItemsInUse + 1);
				int num2 = 0;
				for (int i = 0; i < num - (config.ItemsInUse + vacantObjects.Count); i++)
				{
					GameObject item = InitObject(config);
					vacantObjects.Add(item);
					num2++;
				}
				return num2;
			}
			UnityEngine.Debug.LogWarning($"PoolManager: pool, named {config.Prefab.name} exceed maximum object count {config.MaximumCount}");
			return 0;
		}

		private void InitCustomConfig(PoolConfig config)
		{
			string name = config.Prefab.name;
			GameObject gameObject2 = config.Wrapper = new GameObject(name + "_Pool");
			RectTransform rectTransform = gameObject2.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(base.transform, worldPositionStays: false);
			}
			else
			{
				gameObject2.transform.parent = base.transform;
			}
			gameObject2.transform.position = Vector3.zero;
			List<GameObject> list = new List<GameObject>(config.InitialCount);
			storage.Add(name, list);
			for (int i = 0; i < config.InitialCount; i++)
			{
				GameObject item = InitObject(config);
				list.Add(item);
			}
			poolConfigsByName.Add(name, config);
		}

		private GameObject InitObject(PoolConfig config)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(config.Prefab);
			RectTransform rectTransform = gameObject.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetParent(config.Wrapper.transform, worldPositionStays: false);
			}
			else
			{
				gameObject.transform.parent = config.Wrapper.transform;
			}
			gameObject.transform.position = Vector3.zero;
			gameObject.SetActive(value: false);
			TotalItemsCount++;
			return gameObject;
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
			autoFillUpdateProc.ProceedOnFixedUpdate();
		}
	}
}
