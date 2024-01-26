using Game.Traffic;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class StashManager : MonoBehaviour
	{
		private static StashManager instance;

		public GameObject RootCity;

		public GameObject[] ObjectsToManage;

		private IDictionary<int, List<GameObject>> objectsMap = new Dictionary<int, List<GameObject>>();

		public static StashManager Instance => instance ?? (instance = UnityEngine.Object.FindObjectOfType<StashManager>());

		public void UpdateObjects()
		{
			PseudoDynamicObject[] componentsInChildren = RootCity.GetComponentsInChildren<PseudoDynamicObject>();
			ObjectRespawner[] array = UnityEngine.Object.FindObjectsOfType<ObjectRespawner>();
			TrafficLight[] array2 = UnityEngine.Object.FindObjectsOfType<TrafficLight>();
			EffectArea[] array3 = UnityEngine.Object.FindObjectsOfType<EffectArea>();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				list.Add(componentsInChildren[i].gameObject);
			}
			for (int j = 0; j < array.Length; j++)
			{
				list.Add(array[j].gameObject);
			}
			for (int k = 0; k < array2.Length; k++)
			{
				list.Add(array2[k].gameObject);
			}
			for (int l = 0; l < array3.Length; l++)
			{
				list.Add(array3[l].gameObject);
			}
			List<GameObject> list2 = new List<GameObject>();
			for (int m = 0; m < ObjectsToManage.Length; m++)
			{
				if (list.Contains(ObjectsToManage[m]))
				{
					list.Remove(ObjectsToManage[m]);
				}
				if (ObjectsToManage[m] != null)
				{
					list2.Add(ObjectsToManage[m]);
				}
			}
			list2.AddRange(list);
			ObjectsToManage = list2.ToArray();
			UnityEngine.Debug.LogFormat("Added {0}/{1} Managing objects found. \nManaging objects: {2}", list.Count, componentsInChildren.Length, ObjectsToManage.Length);
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		private void Start()
		{
			HashSet<int> hashSet = new HashSet<int>(SectorManager.Instance.GetAllActiveSectors());
			for (int i = 0; i < ObjectsToManage.Length; i++)
			{
				if (!(ObjectsToManage[i] == null))
				{
					GameObject gameObject = ObjectsToManage[i];
					int sector = SectorManager.Instance.GetSector(gameObject.transform.position);
					List<GameObject> value;
					if (!objectsMap.TryGetValue(sector, out  value))
					{
						value = new List<GameObject>();
						objectsMap.Add(sector, value);
					}
					value.Add(gameObject);
					gameObject.SetActive(hashSet.Contains(sector));
				}
			}
			SectorManager.Instance.AddOnActivateListener(SectorsToUnstash);
			SectorManager.Instance.AddOnDeactivateListener(SectorsToStash);
			ObjectsToManage = null;
		}

		private void ToggleSectorsObjects(int[] sectors, bool toggle)
		{
			for (int i = 0; i < sectors.Length; i++)
			{
				List<GameObject> value;
				if (objectsMap.TryGetValue(sectors[i], out  value))
				{
					for (int j = 0; j < value.Count; j++)
					{
						value[j].SetActive(toggle);
					}
				}
			}
		}

		private void SectorsToUnstash(int[] sectors)
		{
			ToggleSectorsObjects(sectors, toggle: true);
		}

		private void SectorsToStash(int[] sectors)
		{
			ToggleSectorsObjects(sectors, toggle: false);
		}
	}
}
