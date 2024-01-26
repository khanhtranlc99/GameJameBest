using Code.Game.Race.RaceTrackManager;
using Code.Game.Race.StartGrid;
using Game.GlobalComponent.Qwest;
using System.Linq;
using UnityEngine;

namespace Code.Game.Race
{
	public class Race : MonoBehaviour
	{
		[SerializeField]
		private Racer[] racers;

		[SerializeField]
		private Transform startPoint;

		[SerializeField]
		private RaceTrackPointClick raceCheckPoints;

		[SerializeField]
		private int lapsCount;

		[SerializeField]
		private int playerPlaceOnStartGrid;

		[SerializeField]
		private Code.Game.Race.StartGrid.StartGrid grid;

		[SerializeField]
		private int minPlaceForWin;

		[SerializeField]
		private UniversalReward[] rewards;

		[SelectiveString("MarkType")]
		[SerializeField]
		private string marksType;

		public Racer[] GetRacers()
		{
			return racers;
		}

		public Transform GetStartPoint()
		{
			return startPoint;
		}

		public Transform[] GetCheckPoints()
		{
			return (from obj in raceCheckPoints.GetRaceNodesList()
				select obj.transform).ToArray();
		}

		public int GetLapsCount()
		{
			return lapsCount;
		}

		public int GetPlayerPlaceOnStartGrid()
		{
			return playerPlaceOnStartGrid;
		}

		public void SetPlayerPlaceOnStartGrid(int place)
		{
			playerPlaceOnStartGrid = place;
		}

		public Code.Game.Race.StartGrid.StartGrid GetGrid()
		{
			return grid;
		}

		public int GetMinPlaceForWin()
		{
			return minPlaceForWin;
		}

		public UniversalReward[] GetRewards()
		{
			return rewards;
		}

		public string GetMarksType()
		{
			return marksType;
		}
	}
}
