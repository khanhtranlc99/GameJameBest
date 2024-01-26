using System.Collections.Generic;
using UnityEngine;

namespace Code.Game.Race
{
	public class RacePositionController : MonoBehaviour
	{
		private static RacePositionController instance;

		[SerializeField]
		private List<Racer> raceTable;

		private Racer playerRacer;

		public static RacePositionController Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UnityEngine.Object.FindObjectOfType<RacePositionController>();
				}
				return instance;
			}
		}

		private void Awake()
		{
			raceTable = new List<Racer>();
		}

		public Racer AddItem(Racer racer, bool isPlayer = false)
		{
			if (isPlayer)
			{
				playerRacer = racer;
			}
			raceTable.Add(racer);
			return racer;
		}

		public int GetIndexOfItem(Racer racer)
		{
			return raceTable.IndexOf(racer, 0);
		}

		public Racer GetPlayerRacer()
		{
			return playerRacer;
		}

		public List<Racer> GetRaceTable()
		{
			return raceTable;
		}

		private void Update()
		{
			raceTable.Sort();
		}
	}
}
