using UnityEngine;
using UnityEngine.UI;

namespace Game.GameAchievements
{
	public class AchievementPanelItem : MonoBehaviour
	{
		public Text AchievName;

		public Text AchievDescription;

		public Image AchievIcon;

		public Slider StatusSlider;

		public Text StatusText;

		public RectTransform DoneContrainer;

		public RectTransform NotDoneContainer;

		public void Init(Achievement achievement)
		{
			int achiveTarget = achievement.achiveParams.achiveTarget;
			int achiveCounter = achievement.achiveParams.achiveCounter;
			StatusSlider.gameObject.SetActive(achiveTarget != 0);
			if (achiveTarget != 0)
			{
				StatusSlider.maxValue = achiveTarget;
				StatusSlider.value = achiveCounter;
				StatusSlider.interactable = false;
				StatusText.text = achiveCounter + " / " + achiveTarget;
			}
			AchievName.text = achievement.achievementName;
			AchievDescription.text = achievement.achievementDiscription;
			AchievIcon.sprite = achievement.achievementPicture;
			DoneContrainer.gameObject.SetActive(achievement.achiveParams.isDone);
			NotDoneContainer.gameObject.SetActive(!achievement.achiveParams.isDone);
		}

		public void InitInfo(AchievmentInfo achievmentInfo)
		{
			if (achievmentInfo.showFullInfo)
			{
				int targetValue = achievmentInfo.targetValue;
				int currentProgress = achievmentInfo.currentProgress;
				StatusSlider.gameObject.SetActive(targetValue != 0);
				if (targetValue != 0)
				{
					StatusSlider.maxValue = targetValue;
					StatusSlider.value = currentProgress;
					StatusSlider.interactable = false;
					StatusText.text = currentProgress + " / " + targetValue;
				}
				AchievName.text = achievmentInfo.id;
				AchievDescription.text = achievmentInfo.description;
				AchievIcon.sprite = achievmentInfo.icon;
				DoneContrainer.gameObject.SetActive(achievmentInfo.isDone);
				NotDoneContainer.gameObject.SetActive(!achievmentInfo.isDone);
			}
			else
			{
				AchievName.text = achievmentInfo.id;
				AchievDescription.text = achievmentInfo.description;
				AchievIcon.sprite = achievmentInfo.icon;
				DoneContrainer.gameObject.SetActive(value: false);
				NotDoneContainer.gameObject.SetActive(value: false);
				StatusSlider.gameObject.SetActive(value: false);
			}
		}
	}
}
