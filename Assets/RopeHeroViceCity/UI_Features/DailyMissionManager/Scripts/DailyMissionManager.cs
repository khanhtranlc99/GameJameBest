using System;
using Game.Character;
using RopeHeroViceCity.UI_Features.DailyMissionManager.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.DailyMission
{
    public class DailyMissionManager : SingletonMonoBehavior<DailyMissionManager>
    {
        [SerializeField] private DailyMissionData[] allDailyMiss;

        private const string KEY_NEW_DAY_COME = "DailyMissionManager_KEY_NEW_DAY_COME";
        private const string KEY_MISSION_TODAY = "DailyMissionManager_KEY_MISSION_TODAY";
        private const string KEY_MISSION_TODAY_DONE = "DailyMissionManager_KEY_MISSION_TODAY_DONE";
        private const string KEY_FORMAT_MISSION_TYPE_REACH = "DailyMissionManager_KEY_FORMAT_MISSION_TYPE_REACH_{0}";

        public int todayMissionIndex { get; private set; }
        public bool isDoneTodayMission { get; private set; }
        private void Start()
        {
            if (!PlayerPrefs.HasKey(KEY_NEW_DAY_COME))
            {
                var index = Random.Range(0, allDailyMiss.Length);
                PlayerPrefs.SetInt(KEY_MISSION_TODAY,index);
                PlayerPrefs.SetInt(KEY_NEW_DAY_COME,1);
                PlayerPrefs.DeleteKey(KEY_MISSION_TODAY_DONE);
            }

            todayMissionIndex = PlayerPrefs.GetInt(KEY_MISSION_TODAY);
            //uiDailyMission.LoadDataMission(allDailyMiss[todayMissionIndex]);
            CheckIfDoneMission();
        }

        public void ApplyATask(DailyMissionType type,int amount =1)
        {
            if(isDoneTodayMission)
                return;
            var key = string.Format(KEY_FORMAT_MISSION_TYPE_REACH, type);
            var currentAmount = PlayerPrefs.GetInt(key, 0);
            currentAmount += amount;
            PlayerPrefs.SetInt(key,currentAmount);
            CheckIfDoneMission();
        }

        public int GetTaskProgress(DailyMissionType taskType)
        {
            var key = string.Format(KEY_FORMAT_MISSION_TYPE_REACH, taskType);
            var currentAmount = PlayerPrefs.GetInt(key, 0);
            return currentAmount;
        }
        
        public void ClaimReward(bool isWatchVideo = false)
        {
            var data = allDailyMiss[todayMissionIndex];
            var multiple = isWatchVideo ? 3 : 1;
            PlayerInfoManager.Money += data.money * multiple;
            PlayerInfoManager.Gems += data.gems * multiple;
            PlayerPrefsX.SetBool(KEY_MISSION_TODAY_DONE,true);
            foreach (var mission in data.AllMissions)
            {
                var key = string.Format(KEY_FORMAT_MISSION_TYPE_REACH, mission.dailyMissionType); 
                PlayerPrefs.DeleteKey(key);
            }
        }

        private void CheckIfDoneMission(bool showEvenNotDone = false)
        {
            if (PlayerPrefs.HasKey(KEY_MISSION_TODAY_DONE)) return;
            var todayMissionData = allDailyMiss[todayMissionIndex];
            foreach (var mission in todayMissionData.AllMissions)
            {
                var key = string.Format(KEY_FORMAT_MISSION_TYPE_REACH, mission.dailyMissionType);
                var reachValue = PlayerPrefs.GetInt(key, 0);
                if(reachValue>= mission.goalAmount) continue;
                return;
            }
            isDoneTodayMission = true;
            UICanvasController.Instance.ShowLayerToQueueCache(UICanvasKey.DAILY_MISSION);
        }

        public DailyMissionData GetCurrentMission => allDailyMiss[todayMissionIndex];

        public static void OnDayChange(int day)
        {
            if (day >= 0)
            {
                PlayerPrefs.DeleteKey(KEY_NEW_DAY_COME);
            }
        }
    }
}
