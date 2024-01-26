using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameAchievements
{
	public class AchievmentPanel : MonoBehaviour
	{
		public RectTransform achievHolder;

		public AchievementPanelItem AchievPanelItem;

		private readonly List<AchievementPanelItem> achievItems = new List<AchievementPanelItem>();

		private void OnEnable()
		{
			foreach (Achievement allAchievement in GameEventManager.Instance.allAchievements)
			{
				AchievementPanelItem fromPool = PoolManager.Instance.GetFromPool(AchievPanelItem);
				fromPool.transform.parent = achievHolder;
				fromPool.transform.localScale = Vector3.one;
				fromPool.Init(allAchievement);
				achievItems.Add(fromPool);
			}
		}

		private void OnDisable()
		{
			foreach (AchievementPanelItem achievItem in achievItems)
			{
				PoolManager.Instance.ReturnToPool(achievItem);
			}
			achievItems.Clear();
		}
	}
}
