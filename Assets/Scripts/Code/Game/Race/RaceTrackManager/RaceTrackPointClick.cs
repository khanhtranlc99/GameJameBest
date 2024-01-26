using Game.Traffic;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Game.Race.RaceTrackManager
{
	[ExecuteInEditMode]
	public class RaceTrackPointClick : MonoBehaviour
	{
		private static RaceTrackPointClick instance;

		private static bool selectRaceTraffic;

		[SerializeField]
		private List<GameObject> raceNodesList = new List<GameObject>();

		[SerializeField]
		private GameObject raceNode;

		[SerializeField]
		private bool raceTrafficSelected;

		private bool selectCarTrafficCurrState;

		public static RaceTrackPointClick Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UnityEngine.Object.FindObjectOfType<RaceTrackPointClick>();
					if (instance == null)
					{
						UnityEngine.Debug.LogError("RaceTrackPointClick отсутствует на сцене");
					}
				}
				return instance;
			}
		}

		public static bool IsSelectedRaceTraffic()
		{
			return selectRaceTraffic;
		}

		public List<GameObject> GetRaceNodesList()
		{
			return raceNodesList;
		}

		public GameObject GetRaceNode()
		{
			return raceNode;
		}

		public void SetRaceNode(GameObject raceNode)
		{
			this.raceNode = raceNode;
		}

		public void CtrlZ()
		{
			if (raceNodesList.Count <= 0)
			{
				return;
			}
			UnityEngine.Object.DestroyImmediate(raceNodesList[raceNodesList.Count - 1]);
			raceNodesList.Remove(raceNodesList[raceNodesList.Count - 1]);
			if (raceNodesList.Count > 0)
			{
				GameObject gameObject = raceNodesList[raceNodesList.Count - 1];
				if (gameObject.transform.parent.name == "RaceNodes")
				{
					raceNode = gameObject;
				}
			}
		}

		public void ClearLastLinks()
		{
			if (raceNodesList.Count > 0)
			{
				raceNodesList[raceNodesList.Count - 1].GetComponent<Node>().NodeLinks.Clear();
			}
		}
	}
}
