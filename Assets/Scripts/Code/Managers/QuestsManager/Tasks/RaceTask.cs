using Code.Game.Race;
using Code.Game.Race.Utils;
using Game.GlobalComponent.Qwest;
using Game.Vehicle;
using UnityEngine;

namespace Code.Managers.QuestsManager.Tasks
{
	public class RaceTask : BaseTask
	{
		public int RaceNumber;

		private RaceState raceState;

		private GameObject searchGo;

		private int playerPlace;

		public override void TaskStart()
		{
			base.TaskStart();
			RaceManager.OnRaceStateChanged += CheckForComplete;
			ResultTableInfoPanel.OnRacerFinished += CheckForLose;
			QuestToRaceAdapter.Instance.InitQuest(RaceNumber, CurrentQwest);
			PlayerPrefs.SetInt("IsDoMission", 1);
		}

		private void CheckForLose(int opponentPlace, int playerPlace)
		{
			this.playerPlace = playerPlace;
			if (playerPlace <= 0)
			{
				int minPlaceForWin = RaceManager.Instance.GetCurrentRace().GetMinPlaceForWin();
				if (opponentPlace == minPlaceForWin)
				{
					GameEventManager.Instance.QwestFailed(CurrentQwest);
				}
			}
		}

		private void CheckForComplete(RaceState raceState)
		{
			this.raceState = raceState;
			if (raceState == RaceState.Finish)
			{
				CurrentQwest.Rewards = RaceManager.Instance.GetCurrentRace().GetRewards()[playerPlace - 1];
				CurrentQwest.MoveToTask(NextTask);
			}
		}

		public override void Cancel()
		{
			base.Cancel();
			RaceManager.OnRaceStateChanged -= CheckForComplete;
			ResultTableInfoPanel.OnRacerFinished -= CheckForLose;
		}

		public override void Finished()
		{
			base.Finished();
			if (raceState != RaceState.Finish)
			{
				RaceManager.Instance.DeInit();
			}
			RaceManager.OnRaceStateChanged -= CheckForComplete;
			ResultTableInfoPanel.OnRacerFinished -= CheckForLose;
		}

		public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
			QwestTimerManager.Instance.EndCountdown();
			GameEventManager.Instance.QwestFailed(CurrentQwest);
		}

		public override void GetOutVehicleEvent(DrivableVehicle vehicle)
		{
			if (vehicle.CompareTag("PlayerRacer"))
			{
				QwestTimerManager.Instance.StartCountdown(30f, CurrentQwest);
				searchGo = new GameObject
				{
					name = "SearchProcessing_" + GetType().Name
				};
				SearchProcess<DrivableVehicle> searchProcess = new SearchProcess<DrivableVehicle>(CheckCondition);
				searchProcess.countMarks = 1;
				searchProcess.markType = RaceManager.Instance.GetCurrentRace().GetMarksType();
				SearchProcess<DrivableVehicle> process = searchProcess;
				SearchProcessing searchProcessing = searchGo.AddComponent<SearchProcessing>();
				searchProcessing.process = process;
				searchProcessing.Init();
			}
		}

		public override void GetIntoVehicleEvent(DrivableVehicle vehicle)
		{
			if (vehicle.CompareTag("PlayerRacer"))
			{
				if ((bool)searchGo)
				{
					UnityEngine.Object.Destroy(searchGo);
				}
				QwestTimerManager.Instance.EndCountdown();
			}
		}

		private bool CheckCondition(DrivableVehicle vehicle)
		{
			return vehicle.CompareTag("PlayerRacer");
		}
	}
}
