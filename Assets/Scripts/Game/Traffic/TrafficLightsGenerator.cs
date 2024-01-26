using System;
using UnityEngine;

namespace Game.Traffic
{
	public class TrafficLightsGenerator : MonoBehaviour
	{
		private const string TrafficLightsGameObjectName = "Traffic Lights";

		public GameObject VehicleNodesContainer;

		[Separator("For Traffic Light On Nodes")]
		public GameObject FourthTrafficLights;

		public GameObject TripleTrafficLights;

		[Separator("For Traffic Lights On Workpieces")]
		public string WorkpieceName;

		public GameObject ConfigurateWorkpiecePrefab;

		private GameObject trafficLightsContainer;

		public void GenerateTrafficLightsPacks()
		{
			if (VehicleNodesContainer == null)
			{
				throw new Exception("Vehicle nodes container not found!");
			}
			InitTrafficLightsContainer();
			SpawnTrafficLightsPacks();
		}

		public void GenerateTrafficLightsOnWorkpiece()
		{
			InitTrafficLightsContainer();
			SpawnTrafficLightsWorkpieces();
		}

		public void ClearTrafficLightsLinks()
		{
			TrafficLight[] array = UnityEngine.Object.FindObjectsOfType<TrafficLight>();
			if (array == null)
			{
				return;
			}
			TrafficLight[] array2 = array;
			foreach (TrafficLight trafficLight in array2)
			{
				TrafficLight.LightObjects[] lightAndObjects = trafficLight.LightAndObjects;
				foreach (TrafficLight.LightObjects lightObjects in lightAndObjects)
				{
					lightObjects.Objects.RemoveAll((GameObject x) => x == null);
				}
			}
		}

		private void InitTrafficLightsContainer()
		{
			trafficLightsContainer = GameObject.Find("Traffic Lights");
			if (trafficLightsContainer != null)
			{
				int childCount = trafficLightsContainer.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					UnityEngine.Object.DestroyImmediate(trafficLightsContainer.transform.GetChild(0).gameObject);
				}
			}
			else
			{
				trafficLightsContainer = new GameObject
				{
					name = "Traffic Lights"
				};
			}
		}

		private void SpawnTrafficLightsPacks()
		{
			Node[] componentsInChildren = VehicleNodesContainer.GetComponentsInChildren<Node>();
			Node[] array = componentsInChildren;
			foreach (Node node in array)
			{
				if (node.NodeLinks.Count >= 3)
				{
					GameObject original = (node.NodeLinks.Count != 3) ? FourthTrafficLights : TripleTrafficLights;
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, node.transform.position, Quaternion.identity);
					gameObject.transform.parent = trafficLightsContainer.transform;
					gameObject.name = node.name + "_" + gameObject.name;
				}
			}
		}

		private void SpawnTrafficLightsWorkpieces()
		{
			GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
			bool flag = false;
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				if (gameObject.name == WorkpieceName)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(ConfigurateWorkpiecePrefab);
					gameObject2.GetComponent<TrafficLight>().RedFirst = flag;
					flag = !flag;
					Transform transform = gameObject2.transform.Find(WorkpieceName);
					gameObject2.transform.parent = trafficLightsContainer.transform;
					transform.parent = gameObject2.transform.parent;
					gameObject2.transform.parent = transform;
					transform.position = gameObject.transform.position;
					transform.rotation = gameObject.transform.rotation;
					gameObject2.transform.parent = transform.parent;
					transform.parent = gameObject2.transform;
					gameObject.SetActive(value: false);
				}
			}
		}
	}
}
