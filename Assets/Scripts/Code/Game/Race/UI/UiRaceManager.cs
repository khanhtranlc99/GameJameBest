using Code.Game.Race.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Game.Race.UI
{
	public class UiRaceManager : MonoBehaviour
	{
		public GameObject objectStrongBegginnig;
	    public int FirstOpenRace
		{
			get
			{
				return PlayerPrefs.GetInt("FirstOpenRace",0);
			}
			set
			{
				PlayerPrefs.SetInt("FirstOpenRace", value);
			}
		}
		private static UiRaceManager instance;

		private RaceState raceState;

		[SerializeField]
		private Text racePositionView;

		[SerializeField]
		private Text lapNumberView;

		[SerializeField]
		private ResultTableInfoPanel resultTableInfoPanelView;

		[SerializeField]
		private Animator countdown;

		[SerializeField]
		private AnimationClip animationCountdown;

		public static UiRaceManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("UiRaceManager is not initialized");
				}
				return instance;
			}
		}

		public AnimationClip GetAnimationCountdown()
		{
			return animationCountdown;
		}

		public void AddItemToResultTable(Racer racer, bool isPlayer = false)
		{
			resultTableInfoPanelView.AddRaceItem(racer, isPlayer);
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			RaceManager.OnRaceStateChanged += OnRaceStateChanged;
		}

		private void OnRaceStateChanged(RaceState raceState)
		{
			this.raceState = raceState;
			switch (this.raceState)
			{
			case RaceState.BeforeStart:
				ActivateRaceUiElements(value: false);
				resultTableInfoPanelView.gameObject.SetActive(value: false);
				resultTableInfoPanelView.ClearResultTable();
					Debug.LogError("BeforeStart");
					break;
			case RaceState.Start:
				StartCoroutine(CountDown());
				break;
			case RaceState.Race:
				ActivateRaceUiElements(value: true);
				resultTableInfoPanelView.ClearResultTable();
				break;
			case RaceState.Finish:
				ActivateRaceUiElements(value: false);
				resultTableInfoPanelView.gameObject.SetActive(value: true);
				Finish();
					if(FirstOpenRace == 0)
					{
						ObjectStrongBegining.Instance.ObBegining.SetActive(true);
						FirstOpenRace = 1;
					}


					break;
			default:
				throw new ArgumentOutOfRangeException("raceState", raceState, "Unsupported type of RaceState");
			}
		}

		private void ActivateRaceUiElements(bool value)
		{
			racePositionView.gameObject.SetActive(value);
			if (RaceManager.Instance.GetLaps() > 1)
			{
				lapNumberView.gameObject.SetActive(value);
			}
		}

		private IEnumerator CountDown()
		{
			float animTime = animationCountdown.length;
			countdown.SetBool("StartCountdown", value: true);
			yield return new WaitForSeconds(animTime);
			yield return new WaitForSeconds(1f);
			countdown.SetBool("StartCountdown", value: false);
		}

		private void Update()
		{
			RefreshPosition();
			RefreshLapNumber();
		}

		private void RefreshPosition()
		{
			if (raceState == RaceState.Race)
			{
				racePositionView.text = RacePositionController.Instance.GetIndexOfItem(RacePositionController.Instance.GetPlayerRacer()) + 1 + "/" + RacePositionController.Instance.GetRaceTable().Count;
			}
		}

		private void RefreshLapNumber()
		{
			if (raceState == RaceState.Race && RaceManager.Instance.GetLaps() > 1)
			{
				lapNumberView.text = "LAP " + (RaceManager.Instance.GetCurrentLap() + 1) + "/" + RaceManager.Instance.GetLaps();
			}
		}

		private void Finish()
		{
			int itemPosition = resultTableInfoPanelView.GetItemPosition();
			for (int i = itemPosition; i < RacePositionController.Instance.GetRaceTable().Count; i++)
			{
				AddItemToResultTable(RacePositionController.Instance.GetRaceTable()[i]);
			}
		}
	}
}
