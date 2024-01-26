using Game.Character.CharacterController;
using Game.Items;
using Game.MiniMap;
using Game.PickUps;
using Game.UI;
using Game.Vehicle;
using System;
using System.Collections.Generic;
using Game.Enemy;
using RopeHeroViceCity.Scripts.Gampelay;
using UI.DailyMission;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class GameEventManager : MonoBehaviour
	{
		private class GameEvent : IEvent
		{
			private delegate void QwestAction(Qwest qwest);

			private delegate void AchievementAction(Achievement achievement);

			public void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
			{
				ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().PlayerDeadEvent();
				});
				ByByPass(delegate(Achievement achievement)
				{
					achievement.PlayerDeadEvent(i);
				});
			}

			public void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
			{
				ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().NpcKilledEvent(position, npcFaction, victim, killer);
				});
				ByByPass(delegate(Achievement achievement)
				{
					achievement.NpcKilledEvent(position, npcFaction, victim, killer);
				});
				if (killer is Player)
				{
					if (victim is HumanoidStatusNPC)
					{
						DailyMissionManager.Instance.ApplyATask(DailyMissionType.KILL_HUMANS);
					}
					else if(victim is VehicleStatus)
					{
						var vehicleStaus = victim as VehicleStatus;
						if (vehicleStaus.rootDrivableVehicle is DrivableMotorcycle)
						{
							DailyMissionManager.Instance.ApplyATask(DailyMissionType.DESTROY_MOTORS);
						}
						else if (vehicleStaus.rootDrivableVehicle is DrivableCar)
						{
							DailyMissionManager.Instance.ApplyATask(DailyMissionType.DESTROY_CARS);
						}
					}
	
				}
			}

			public void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask)
			{
				ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().PickedQwestItemEvent(position, pickupType, relatedTask);
				});
				ByByPass(delegate(Achievement achievement)
				{
					achievement.PickedQwestItemEvent(position, pickupType, relatedTask);
				});
			}

			public void PointReachedEvent(Vector3 position, BaseTask task)
			{
				ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().PointReachedEvent(position, task);
				});
				ByByPass(delegate(Achievement achievement)
				{
					achievement.PointReachedEvent(position, task);
				});
			}

			public void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle)
			{
				ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().PointReachedByVehicleEvent(position, task, vehicle);
				});
				ByByPass(delegate(Achievement achievement)
				{
					achievement.PointReachedByVehicleEvent(position, task, vehicle);
				});
			}

			public void GetIntoVehicleEvent(DrivableVehicle vehicle)
			{
				ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().GetIntoVehicleEvent(vehicle);
				});
				ByByPass(delegate(Achievement achievement)
				{
					achievement.GetIntoVehicleEvent(vehicle);
				});
			}

			public void GetOutVehicleEvent(DrivableVehicle vehicle)
			{
				ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().GetOutVehicleEvent(vehicle);
				});
				ByByPass(delegate(Achievement achievement)
				{
					achievement.GetOutVehicleEvent(vehicle);
				});
			}

			public void PickUpCollectionEvent(string CollectionName)
			{
				ByByPass(delegate(Achievement achievement)
				{
					achievement.PickUpCollectionEvent(CollectionName);
				});
			}

			public void GetLevelEvent(int level)
			{
				ByByPass(delegate(Achievement achievement)
				{
					achievement.GetLevelEvent(level);
				});
			}

			public void GetShopEvent()
			{
				ByByPass(delegate(Achievement achievement)
				{
					achievement.GetShopEvent();
				});
			}

			public void VehicleDrawingEvent(DrivableVehicle vehicle)
			{
				ByByPass(delegate(Achievement achievement)
				{
					achievement.VehicleDrawingEvent(vehicle);
				});
			}

			public void BuyItemEvent(GameItem item)
			{
				ByPass(delegate(Qwest qwest)
				{
					qwest.GetCurrentTask().BuyItemEvent(item);
				});
			}

			private void ByPass(QwestAction action)
			{
				Qwest[] array = Instance.ActiveQwests.ToArray();
				foreach (Qwest qwest in array)
				{
					if (!Instance.TimeQwestActive || Instance.CurrentTimeQwest.Equals(qwest))
					{
						action(qwest);
					}
				}
			}

			private void ByByPass(AchievementAction action)
			{
				Achievement[] array = Instance.activeAchievements.ToArray();
				foreach (Achievement achievement in array)
				{
					action(achievement);
				}
			}
		}

		private static GameEventManager instance;

		public readonly IEvent Event = new GameEvent();

		[Separator("Qwest Data")]
		public TextAsset SerializedQwests;

		[Separator("Mini Map")]
		public MarkForMiniMap MMMark;

		public Transform MapMarksListTransform;

		[Separator("Prefabs")]
		public QuestPickUp[] questPickUps;

		public QwestStart QwestStartPrefab;

		public QwestPoint QwestPointPrefab;

		public QwestVehiclePoint QwestVehiclePointPrefab;

		public Qwest MarkedQwest;

		[HideInInspector]
		public Qwest CurrentTimeQwest;

		public bool MassacreTaskActive;

		[Separator("Achievments")]
		public bool AchievmentsReset;

		public List<Achievement> activeAchievements = new List<Achievement>();

		public List<Achievement> allAchievements = new List<Achievement>();

		public readonly List<Qwest> ActiveQwests = new List<Qwest>();

		private readonly List<Qwest> availableQwests = new List<Qwest>();

		private readonly List<Qwest> allQwests = new List<Qwest>();

		private readonly List<Qwest> nextQuests = new List<Qwest>();

		public static GameEventManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("GameEventManager is not initialized");
				}
				return instance;
			}
		}

		public bool TaskSelectionAvailable => !MassacreTaskActive;

		public bool TimeQwestActive => CurrentTimeQwest != null;

		public void StartQwest(Qwest qwest)
		{
			if (!TimeQwestActive && availableQwests.Contains(qwest))
			{
				ActiveQwests.Add(qwest);
				qwest.Init();
				MarkedQwest = qwest;
				RefreshQwestArrow();
				availableQwests.Remove(qwest);
				ToggleQwestMark(qwest, toggle: false);
				if (qwest.IsTimeQwest)
				{
					CurrentTimeQwest = qwest;
					MarkedQwest = qwest;
					BackgroundMusic.instance.PlayTimeQuestClip();
				}
				//AdsManager.ShowFullscreenAd();
			}
		}

		public void QwestFailed(Qwest qwest)
		{
			if (ActiveQwests.Contains(qwest))
			{
				qwest.GetCurrentTask()?.Finished();
				ActiveQwests.Remove(qwest);
				if (qwest.Equals(MarkedQwest))
				{
					MarkedQwest = ((ActiveQwests.Count <= 0) ? null : ActiveQwests[0]);
					RefreshQwestArrow();
				}
				PlaceQuestOnMap(qwest);
				InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestFailed, qwest.QwestTitle);
				PointSoundManager.Instance.PlaySoundAtPoint(Vector3.zero, "QwestFailed");
				if (qwest.Equals(CurrentTimeQwest))
				{
					CurrentTimeQwest = null;
					BackgroundMusic.instance.StopTimeQuestClip();
				}
			}
		}

		public void QwestCancel(Qwest qwest)
		{
			if (ActiveQwests.Contains(qwest))
			{
				qwest.GetCurrentTask()?.Cancel();
				ActiveQwests.Remove(qwest);
				if (qwest.Equals(MarkedQwest))
				{
					MarkedQwest = ((ActiveQwests.Count <= 0) ? null : ActiveQwests[0]);
					RefreshQwestArrow();
				}
				InGameLogManager.Instance.RegisterNewMessage(MessageType.QuestItem, qwest.QwestTitle + " canceled");
				if (qwest.Equals(CurrentTimeQwest))
				{
					CurrentTimeQwest = null;
					BackgroundMusic.instance.StopTimeQuestClip();
				}
			}
		}

		public void ResetQwestCancel(Qwest qwest)
		{
			PlaceQuestOnMap(qwest);
		}

		public void QwestResolved(Qwest qwest)
		{
			if (TimeQwestActive && !qwest.Equals(CurrentTimeQwest))
			{
				return;
			}
			if (TimeQwestActive && qwest.Equals(CurrentTimeQwest))
			{
				CurrentTimeQwest = null;
				BackgroundMusic.instance.StopTimeQuestClip();
			}
			nextQuests.Clear();
			ActiveQwests.Remove(qwest);
			if (qwest.RepeatableQuest)
			{
				nextQuests.Add(qwest);
			}
			else
			{
				Dictionary<string, bool> qwestStatus = QwestProfile.QwestStatus;
				if (!qwestStatus.ContainsKey(qwest.Name))
				{
					qwestStatus.Add(qwest.Name, value: true);
					QwestProfile.QwestStatus = qwestStatus;
				}
				for (int i = 0; i < qwest.QwestTree.Count; i++)
				{
					Qwest item = qwest.QwestTree[i];
					nextQuests.Add(item);
				}
			}
			for (int j = 0; j < nextQuests.Count; j++)
			{
				Qwest quest = nextQuests[j];
				PlaceQuestOnMap(quest);
			}
			if (qwest.Equals(MarkedQwest))
			{
				MarkedQwest = ((ActiveQwests.Count <= 0) ? null : ActiveQwests[0]);
				RefreshQwestArrow();
			}
			//AdsManager.ShowFullscreenAd();
		}

		public void RefreshQwestArrow()
		{
			if (UIMarkManager.InstanceExist)
			{
				if (MarkedQwest != null && MarkedQwest.GetQwestTarget() != null && QwestProfile.QwestArrow)
				{
					UIMarkManager.Instance.TargetStaticMark = MarkedQwest.GetQwestTarget();
					UIMarkManager.Instance.ActivateStaticMark(value: true);
				}
				else
				{
					UIMarkManager.Instance.ActivateStaticMark(value: false);
				}
			}
		}

		private void GenerateQwestStartPoint(Qwest qwest)
		{
			QwestStart qwestStart = PoolManager.Instance.GetFromPool(QwestStartPrefab);
			PoolManager.Instance.AddBeforeReturnEvent(qwestStart, delegate
			{
				qwestStart.Qwest = null;
			});
			qwestStart.Qwest = qwest;
			qwestStart.transform.parent = base.transform;
			qwestStart.transform.position = qwest.StartPosition;
			qwestStart.transform.localScale = new Vector3(1f + qwest.AdditionalStartPointRadius, 1f + qwest.AdditionalStartPointRadius, 1f + qwest.AdditionalStartPointRadius);
			ToggleQwestMark(qwest, toggle: true);
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				// UIGame uIGame = UIGame.Instance;
				// uIGame.OnExitInMenu = (Action)Delegate.Combine(uIGame.OnExitInMenu, new Action(OnExitInMenu));
				// UIGame uIGame2 = UIGame.Instance;
				// uIGame2.OnPausePanelOpen = (Action)Delegate.Combine(uIGame2.OnPausePanelOpen, new Action(OnPausePanelOpen));
				
				EventManager.Instance.OnExitInMenu.AddListener(OnExitInMenu);
				EventManager.Instance.OnPausePanelOpen.AddListener(OnPausePanelOpen);
			}
			if (MapMarksListTransform != null)
			{
				MarkForMiniMap[] componentsInChildren = MapMarksListTransform.GetComponentsInChildren<MarkForMiniMap>();
				if (componentsInChildren != null)
				{
					MarkForMiniMap[] array = componentsInChildren;
					foreach (MarkForMiniMap markForMiniMap in array)
					{
						markForMiniMap.HideIcon();
					}
				}
			}
			var listquest = MiamiSerializier.JSONDeserialize<List<Qwest>>(SerializedQwests.text);
			allQwests.AddRange(listquest);
			Dictionary<string, bool> qwestStatus = QwestProfile.QwestStatus;
			foreach (Qwest allQwest in allQwests)
			{
				if (!qwestStatus.ContainsKey(allQwest.Name) && (allQwest.ParentQwest == null || (allQwest.ParentQwest != null && qwestStatus.ContainsKey(allQwest.ParentQwest.Name))))
				{
					PlaceQuestOnMap(allQwest);
				}
			}
			allAchievements.RemoveAll((Achievement elem) => elem != null);
			activeAchievements.RemoveAll((Achievement elem) => elem != null);
			Achievement[] componentsInChildren2 = GetComponentsInChildren<Achievement>();
			foreach (Achievement achievement in componentsInChildren2)
			{
				achievement.Init();
				allAchievements.Add(achievement);
			}
			if (AchievmentsReset)
			{
				SaveAchievements();
			}
			LoadAchievements();
			foreach (Achievement allAchievement in allAchievements)
			{
				if (!allAchievement.achiveParams.isDone)
				{
					activeAchievements.Add(allAchievement);
				}
			}
		}

		private void PlaceQuestOnMap(Qwest quest)
		{
			availableQwests.Add(quest);
			GenerateQwestStartPoint(quest);
		}

		private void SaveAchievements()
		{
			foreach (Achievement allAchievement in allAchievements)
			{
				allAchievement.SaveAchiev();
			}
			CollectionPickUpsManager.Instance.SaveCollections();
		}

		private void LoadAchievements()
		{
			foreach (Achievement allAchievement in allAchievements)
			{
				allAchievement.LoadAchiev();
			}
		}

		private void Update()
		{
			if (MMMark != null)
			{
				if (!MMMark.isActiveAndEnabled)
				{
					MMMark.gameObject.SetActive(value: true);
				}
				if (MarkedQwest != null && MarkedQwest.GetQwestTarget() != null)
				{
					MMMark.transform.position = MarkedQwest.GetQwestTarget().position;
					MMMark.ShowIcon();
				}
				else
				{
					MMMark.HideIcon();
				}
			}
		}

		private void ToggleQwestMark(Qwest qwest, bool toggle)
		{
			if (qwest.MMMarkId != -1)
			{
				MarkForMiniMap markForMiniMap = qwest.MarkForMiniMap;
				if (markForMiniMap == null)
				{
					markForMiniMap = (qwest.MarkForMiniMap = MapMarksListTransform.GetChild(qwest.MMMarkId).GetComponent<MarkForMiniMap>());
				}
				if (toggle)
				{
					markForMiniMap.transform.position = qwest.StartPosition;
					markForMiniMap.gameObject.SetActive(value: true);
					markForMiniMap.ShowIcon();
				}
				else
				{
					markForMiniMap.HideIcon();
					markForMiniMap.gameObject.SetActive(value: false);
				}
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				SaveAchievements();
			}
		}

		private void OnApplicationQuit()
		{
			SaveAchievements();
		}

		private void OnExitInMenu()
		{
			SaveAchievements();
		}

		private void OnPausePanelOpen()
		{
			SaveAchievements();
		}
	}
}
