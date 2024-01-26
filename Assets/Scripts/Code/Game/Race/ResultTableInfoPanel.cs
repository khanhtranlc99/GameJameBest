using Code.Game.Race.Utils;
using UnityEngine;

namespace Code.Game.Race
{
	public class ResultTableInfoPanel : MonoBehaviour
	{
		[SerializeField]
		private RaceItemView raceItemViewPrefab;

		[SerializeField]
		private RectTransform raceItemViewHolder;

		private int itemPosition;

		private int playerPosition;

		public static event RacerFinished OnRacerFinished;

		public int GetItemPosition()
		{
			return itemPosition;
		}

		public int GetPlayerPosition()
		{
			return playerPosition;
		}

		public void ClearResultTable()
		{
			itemPosition = 0;
			playerPosition = 0;
			raceItemViewHolder.DestroyChildrens();
		}

		public void AddRaceItem(Racer racer, bool isPlayer = false)
		{
			itemPosition++;
			if (isPlayer)
			{
				playerPosition = itemPosition;
			}
			if (ResultTableInfoPanel.OnRacerFinished != null)
			{
				ResultTableInfoPanel.OnRacerFinished(itemPosition, playerPosition);
			}
			AddView(racer);
		}

		private void AddView(Racer racer)
		{
			RaceItemView raceItemView = UnityEngine.Object.Instantiate(raceItemViewPrefab, raceItemViewHolder, worldPositionStays: false) as RaceItemView;
			if (!(raceItemView == null))
			{
				raceItemView.SetPosition(itemPosition);
				raceItemView.SetRacerName(racer.GetRacerName());
			}
		}

		private void OnDestroy()
		{
			ResultTableInfoPanel.OnRacerFinished = null;
		}
	}
}
