using Game.Traffic;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrafficPointClick : MonoBehaviour
{
	public GameObject CarsNode;

	public GameObject PedestrainNode;

	public static bool SelectCarTraffic;

	public static bool SelectPedestrianTraffic;

	public static bool SelectPedestrianNodeNet;

	public static int netLenth = 5;

	public static float distanceBetweenNodes = 10f;

	public static bool connectNextNode = true;

	public bool CarTrafficSelected;

	public bool PedestrianTrafficSelected;

	public bool PedestrianNodeNetSelected;

	public int NodeNetLenth = 5;

	public float DistanceBetweenNodes = 5f;

	public bool ConnectNextNode = true;

	[InspectorButton("CtrlZ")]
	public string DeleteLastPoint = string.Empty;

	[InspectorButton("ClearLastLinks")]
	public string ClearLastPoint = string.Empty;

	public List<GameObject> tmpArray = new List<GameObject>();

	public List<GameObject> pedestrianList = new List<GameObject>();

	private bool selectCarTrafficCurrState;

	private bool selectPedestrainTrafficCurrState;

	private bool selectPedestrianNodeNetCurrState;

	private static TrafficPointClick instance;

	public static TrafficPointClick Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<TrafficPointClick>();
				if (instance == null)
				{
					UnityEngine.Debug.LogError("TrafficManagerBCKP/TrafficPointClick отсутствует на сцене");
				}
			}
			return instance;
		}
	}

	public void CtrlZ()
	{
		if (tmpArray.Count <= 0)
		{
			return;
		}
		UnityEngine.Object.DestroyImmediate(tmpArray[tmpArray.Count - 1]);
		tmpArray.Remove(tmpArray[tmpArray.Count - 1]);
		if (tmpArray.Count > 0)
		{
			GameObject gameObject = tmpArray[tmpArray.Count - 1];
			if (gameObject.transform.parent.name == "VehicleNodes")
			{
				CarsNode = gameObject;
			}
			if (gameObject.transform.parent.name == "PedestrianNodes")
			{
				PedestrainNode = gameObject;
			}
		}
	}

	public void ClearLastLinks()
	{
		if (tmpArray.Count > 0)
		{
			tmpArray[tmpArray.Count - 1].GetComponent<Node>().NodeLinks.Clear();
		}
	}
}
