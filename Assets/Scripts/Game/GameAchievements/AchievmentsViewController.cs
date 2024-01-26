using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Game.GameAchievements
{
	public class AchievmentsViewController : MonoBehaviour
	{
		public RectTransform achievHolder;

		public AchievementPanelItem AchievPanelItem;

		public GameEventManager GameEventManagerPrefab;

		private void OnEnable()
		{
			achievHolder.DestroyChildrens();
			if (GameEventManagerPrefab != null)
			{
				Achievement[] componentsInChildren = GameEventManagerPrefab.GetComponentsInChildren<Achievement>();
				Achievement[] array = componentsInChildren;
				foreach (Achievement achievment in array)
				{
					AchievementPanelItem achievementPanelItem = (AchievementPanelItem)UnityEngine.Object.Instantiate(AchievPanelItem, achievHolder, worldPositionStays: false);
					achievementPanelItem.InitInfo(GenerateInfo(achievment));
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

		private void OnDisable()
		{
		}
	}
}
