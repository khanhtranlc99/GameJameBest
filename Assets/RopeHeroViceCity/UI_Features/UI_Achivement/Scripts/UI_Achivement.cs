using System.Collections.Generic;
using Game.GameAchievements;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace RopeHeroViceCity.UI_Features.UI_Achivement.Scripts
{
    public class UI_Achivement : AbsRopePopup
    {
        public RectTransform achievHolder;

        public AchievementPanelItem AchievPanelItem;

        public GameEventManager GameEventManagerPrefab;
        private readonly List<AchievementPanelItem> achievItems = new List<AchievementPanelItem>();

        public static bool shouldFullInit = false;

        public override void StartLayer()
        {
            base.StartLayer();
            if (GameEventManagerPrefab != null)
            {
                Achievement[] array = GameEventManagerPrefab.GetComponentsInChildren<Achievement>();
                foreach (Achievement achievment in array)
                {
                    AchievementPanelItem achievementPanelItem = Instantiate(AchievPanelItem, achievHolder, worldPositionStays: false);
                    achievementPanelItem.InitInfo(GenerateInfo(achievment));
                    achievItems.Add(achievementPanelItem);
                    if (shouldFullInit)
                    {
                        achievementPanelItem.Init(achievment);
                    }
                }
            }
        }
        private AchievmentInfo GenerateInfo(Achievement achievment)
        {
            Achievement.SaveLoadAchievmentStruct defaultValue = new Achievement.SaveLoadAchievmentStruct(false, 0, 0);
            AchievmentInfo achievmentInfo = new AchievmentInfo();
            achievmentInfo.id = achievment.achievementName;
            achievmentInfo.description = achievment.achievementDiscription;
            achievmentInfo.icon = achievment.achievementPicture;
            achievmentInfo.showFullInfo = false;
            AchievmentInfo achievmentInfo2 = achievmentInfo;
            if (BaseProfile.HasKey(achievment.achievementName))
            {
                defaultValue = BaseProfile.ResolveValue(achievment.achievementName, defaultValue);
                achievmentInfo2.showFullInfo = true;
                achievmentInfo2.isDone = defaultValue.isDone;
                achievmentInfo2.currentProgress = defaultValue.achiveCounter;
                achievmentInfo2.targetValue = defaultValue.achiveTarget;
            }
            return achievmentInfo2;
        }

        public static void ShowPopup(bool showInit)
        {
            shouldFullInit = showInit;
            UICanvasController.Instance.ShowLayer(UICanvasKey.Achievement);
        }
        public void OnAds()
        {
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.ON_ADS);
        }
    }
}
