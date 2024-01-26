using Code.Game.Race.UI;
using Code.Game.Race.Utils;
using Game.Character;
using Game.GlobalComponent;
using Game.Traffic;
using Game.Vehicle;
using System;
using System.Collections;
using UnityEngine;

namespace Code.Game.Race
{
	public class RaceManager : MonoBehaviour
	{
		private const float CheckDistance = 30f;

		private static RaceManager instance;

		[SerializeField]
		private GameObject nextPointHighlighterPrefab;

		[SerializeField]
		private ControlableObjectRespawner garage;

		[SerializeField]
		private Race[] races;

		[SerializeField]
		private CutsceneManager enterCutsceneManager;

		[SerializeField]
		private CutsceneManager exitCutsceneManager;

		private RaceState currentRaceState;

		private GameObject nextPointHighlighter;

		private DrivableVehicle playerDrivableVehicle;

		private Race currentRace;

		private Racer[] racers;

		private bool isAutopiloted;

		private Vector3[] vehicleSpawnPositions;

		private Transform[] checkPoints;

		private int checkpointIndex;

		private float distanceToPoint;

		private int laps;

		private int lapIndex;

		private Racer playerRacer;

		public static RaceManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("RaceManager is not initialized");
				}
				return instance;
			}
		}

		public static event RaceStateChanged OnRaceStateChanged;

		public int GetLaps()
		{
			return laps;
		}

		public int GetCurrentLap()
		{
			return lapIndex;
		}

		public Race GetCurrentRace()
		{
			return currentRace;
		}

		public void InitRace(int raceNumber)
		{
			raceNumber = Mathf.Clamp(raceNumber, 0, races.Length - 1);
			currentRace = races[raceNumber];
			laps = currentRace.GetLapsCount();
			lapIndex = 0;
			checkPoints = currentRace.GetCheckPoints();
			checkpointIndex = 0;
			RacePositionController.Instance.GetRaceTable().Clear();
			laps = Mathf.Clamp(laps, 1, int.MaxValue);
			racers = new Racer[currentRace.GetRacers().Length];
			isAutopiloted = false;
			vehicleSpawnPositions = currentRace.GetGrid().CalculateGrid(racers.Length + 1);
			vehicleSpawnPositions = RaceUtils.CalculateGridAdvanced(vehicleSpawnPositions, currentRace.GetStartPoint().rotation);
			currentRace.SetPlayerPlaceOnStartGrid(Mathf.Clamp(currentRace.GetPlayerPlaceOnStartGrid(), 1, vehicleSpawnPositions.Length));
			RaceUtils.SwapElements(vehicleSpawnPositions, currentRace.GetPlayerPlaceOnStartGrid() - 1, vehicleSpawnPositions.Length - 1);
			SpawnRacers();
		}

		private void SpawnRacers()
		{
			TrafficManager.Instance.AllowSpawnTraffic(value: false);
			for (int i = 0; i < racers.Length; i++)
			{
				Vector3 bestPoint = currentRace.GetStartPoint().position + vehicleSpawnPositions[i];
				DrivableVehicle drivableVehicle = currentRace.GetRacers()[i].GetDrivableVehicle();
				DrivableVehicle drivableVehicle2 = TrafficManager.Instance.SpawnConcreteVehicleForRace(drivableVehicle, bestPoint, "Racer");
				racers[i] = new Racer(drivableVehicle2, currentRace.GetRacers()[i].GetRacerName());
				racers[i].GetDrivableVehicle().transform.rotation = currentRace.GetStartPoint().rotation;
			}
			StartCoroutine(InitPlayerRacer());
		}

		public void DeInit()
		{
			TrafficManager.Instance.AllowSpawnTraffic(value: true);
			if (currentRaceState != RaceState.Finish)
			{
				QuitRace();
			}
			else
			{
				GameplayUtils.ResumeGame();
			}
			SetRaceState(RaceState.BeforeStart);
			RaceUtils.BlockVehicleControl(playerDrivableVehicle.controller, block: false);
			StopAllCoroutines();
			AddAutopilotsForRacers();
		}

		private void Finish()
		{
			GameplayUtils.PauseGame();
			QuitRace();
			SetRaceState(RaceState.Finish);
			RaceUtils.BlockVehicleControl(playerDrivableVehicle.controller, block: true);
		}

		private void QuitRace()
		{
			RaceUtils.RefreshCheckPointArrow(null, activate: false);
			UnityEngine.Object.Destroy(nextPointHighlighter);
			Racer[] array = racers;
			foreach (Racer racer in array)
			{
				racer.GetDrivableVehicle().tag = "Untagged";
			}
			playerDrivableVehicle.tag = "Untagged";
		}

		private void Awake()
		{
			instance = this;
		}

		private void FixedUpdate()
		{
			if (playerDrivableVehicle != null && playerDrivableVehicle.CompareTag("PlayerRacer") && currentRaceState == RaceState.Race)
			{
				RaceFixedUpdate();
			}
		}

		private void EnterToCutscene()
		{
			enterCutsceneManager.Init(ExitFromCutscene, ExitFromCutscene);
		}

		private void ExitFromCutscene()
		{
			exitCutsceneManager.Init(Callback, Callback);
		}

		private void Callback()
		{
			UnityEngine.Debug.Log("Callback");
		}

		private IEnumerator InitPlayerRacer()
		{
			yield return new WaitForSeconds(0.5f);
			EnterToCutscene();
			yield return new WaitForSeconds(1f);
			Vector3 driverVehicleSpawnPosition = GetDriverVehicleSpawnPosition();
			DrivableVehicle playerDrivableVehiclePrefab = garage.ObjectPrefab.GetComponent<DrivableVehicle>();
			playerDrivableVehicle = TrafficManager.Instance.SpawnConcreteVehicleForRace(playerDrivableVehiclePrefab, driverVehicleSpawnPosition, "PlayerRacer", withDriver: false);
			playerDrivableVehicle.transform.rotation = currentRace.GetStartPoint().rotation;
			PlayerInteractionsManager.Instance.Player.transform.position = playerDrivableVehicle.GetExitPosition(toLeft: false);
			PlayerInteractionsManager.Instance.LastDrivableVehicle = playerDrivableVehicle;
			PlayerInteractionsManager.Instance.GetIntoVehicle();
			yield return new WaitUntil(() => playerDrivableVehicle.controller != null);
			RaceUtils.BlockVehicleControl(playerDrivableVehicle.controller, block: true);
			yield return new WaitForSeconds(6f);
			playerRacer = new Racer(playerDrivableVehicle, "Player");
			RacePositionController.Instance.AddItem(playerRacer, isPlayer: true);
			StartCoroutine(StartRace());
		}

		private IEnumerator StartRace()
		{
			SetRaceState(RaceState.Start);
			if (PlayerInteractionsManager.Instance.sitInVehicle && playerDrivableVehicle.controller.VehicleSpecific.HasRadio)
			{
				RadioManager.Instance.DisableRadio();
			}
			float animTime = UiRaceManager.Instance.GetAnimationCountdown().length;
			yield return new WaitForSeconds(animTime);
			if (PlayerInteractionsManager.Instance.sitInVehicle && playerDrivableVehicle.controller != null && playerDrivableVehicle.controller.VehicleSpecific.HasRadio)
			{
				RadioManager.Instance.EnableRadio();
			}
			SetRaceState(RaceState.Race);
			TrafficManager.Instance.AllowSpawnTraffic(value: true);
			RaceUtils.BlockVehicleControl(playerDrivableVehicle.controller, block: false);
			AddAutopilotsForRacers();
			nextPointHighlighter = UnityEngine.Object.Instantiate(nextPointHighlighterPrefab);
			nextPointHighlighter.transform.position = checkPoints[0].transform.position;
			RaceUtils.RefreshCheckPointArrow(checkPoints[checkpointIndex].transform, currentRaceState == RaceState.Race);
		}

		private void AddAutopilotsForRacers()
		{
			if (!isAutopiloted)
			{
				Racer[] array = racers;
				foreach (Racer racer in array)
				{
					TrafficManager.Instance.AddAutopilotForRacer(racer, checkPoints, laps);
				}
				isAutopiloted = true;
			}
		}

		private Vector3 GetDriverVehicleSpawnPosition()
		{
			return currentRace.GetStartPoint().position + vehicleSpawnPositions[vehicleSpawnPositions.Length - 1];
		}

		private void RaceFixedUpdate()
		{
			Transform transform = checkPoints[checkpointIndex];
			distanceToPoint = Vector3.Distance(playerDrivableVehicle.transform.position, transform.position);
			playerRacer.SetLap(lapIndex);
			playerRacer.SetWaypointIndex(checkpointIndex);
			playerRacer.SetDistanceToPoint(distanceToPoint);
			if (!(distanceToPoint < 30f))
			{
				return;
			}
			checkpointIndex = (checkpointIndex + 1) % checkPoints.Length;
			nextPointHighlighter.transform.position = checkPoints[checkpointIndex].transform.position;
			if (checkpointIndex == 0)
			{
				lapIndex++;
				if (laps == lapIndex)
				{
					UiRaceManager.Instance.AddItemToResultTable(playerRacer, isPlayer: true);
					Finish();
				}
			}
			RaceUtils.RefreshCheckPointArrow(checkPoints[checkpointIndex].transform, currentRaceState == RaceState.Race);
		}

		private void SetRaceState(RaceState raceState)
		{
			currentRaceState = raceState;
			if (RaceManager.OnRaceStateChanged != null)
			{
				RaceManager.OnRaceStateChanged(currentRaceState);
			}
		}

		private void OnDestroy()
		{
			RaceManager.OnRaceStateChanged = null;
		}
	}
}
