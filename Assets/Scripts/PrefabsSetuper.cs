using Game.GlobalComponent;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabsSetuper : MonoBehaviour
{
	public GameObject Prefab;

	public Transform[] Holders;

	[InspectorButton("SetMeUp")]
	public bool SetUp;

	[InspectorButton("DeleteLastCreated")]
	public bool Clear;

	private List<GameObject> lastCreated = new List<GameObject>();

	public void SetMeUp()
	{
		if (lastCreated.Count != 0)
		{
			lastCreated.Clear();
		}
		Transform[] holders = Holders;
		foreach (Transform transform in holders)
		{
			GameObject gameObject = Prefab;
			ObjectRespawner component = transform.GetComponent<ObjectRespawner>();
			if (component != null)
			{
				gameObject = component.ObjectPrefab;
			}
			if (!(gameObject == null))
			{
				GameObject item = UnityEngine.Object.Instantiate(gameObject, transform.position, transform.rotation, transform) as GameObject;
				lastCreated.Add(item);
			}
		}
	}

	public void DeleteLastCreated()
	{
		foreach (GameObject item in lastCreated)
		{
			if (item != null)
			{
				UnityEngine.Object.DestroyImmediate(item);
			}
		}
		lastCreated.Clear();
	}
}
