using Game.Character;
using Game.Items;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.GlobalComponent.Qwest
{
	public class QwestCompletePanel : MonoBehaviour
	{
	
		
		private static QwestCompletePanel instance;

		public float DeactivateDelay;

		public CanvasGroup CanvasGroup;

		public RectTransform MissionCompletePanel;

		public RectTransform ContentContainer;

		public RectTransform LineWithTextSample;

		public RectTransform LineWithImageSample;

		public Text HeaderText;

		private bool timeWasFreezed;

		private bool rewardShow;

		public static QwestCompletePanel Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("Qwest Complete Panel Not Find");
				}
				return instance;
			}
		}

		public void ShowCompletedQwestInfo(string headerMessage, UniversalReward reward)
		{
			if (!reward.IsHaveReward())
			{
				return;
			}
			

			StartCoroutine(DelayComplete(headerMessage, reward));
		}
	   public IEnumerator DelayComplete(string headerMessage, UniversalReward reward)
		{
			yield return new WaitForSeconds(0.5f);
			MissionCompletePanel.gameObject.SetActive(value: true);


			BackButton.ChangeBackButtonsStatus(active: false);
			HeaderText.text = headerMessage;
			int childCount = ContentContainer.childCount;
			for (int i = 0; i < childCount; i++)
			{
				PoolManager.Instance.ReturnToPool(ContentContainer.GetChild(0));
			}
			if (reward.MoneyReward > 0)
			{
				string text = reward.MoneyReward.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text = text + " + " + (float)reward.MoneyReward * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Money) / 100f;
				}
				if (PlayerInteractionsManager.Instance.Player.IsTransformer)
				{
					if (PlayerInfoManager.VipLevel > 0)
					{
						AddTextInfo("+" + text + " EXP!", Color.white);
					}
				}
				else if (reward.RewardInGems)
				{
					AddTextInfo("+" + reward.MoneyReward + " gems!", Color.white);
				}
				else
				{
					AddTextInfo("+" + text + " $!", Color.white);
				}
			}
			if (reward.ExperienceReward > 0)
			{
				string text2 = reward.ExperienceReward.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text2 = text2 + " + " + (float)reward.ExperienceReward * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Experience) / 100f;
				}
				AddTextInfo("+" + text2 + " EXP!", Color.white);
			}
			GameItem item = ItemsManager.Instance.GetItem(reward.ItemRewardID);
			if (item != null)
			{
				AddImageInfo(item.ShopVariables.ItemIcon);
			}
			if (reward.RelationRewards != null && reward.RelationRewards.Length > 0)
			{
				FactionRelationReward[] relationRewards = reward.RelationRewards;
				foreach (FactionRelationReward factionRelationReward in relationRewards)
				{
					if (factionRelationReward.ChangeRelationValue > 0f)
					{
						AddTextInfo("+" + factionRelationReward.Faction + " respect", Color.green);
					}
					if (factionRelationReward.ChangeRelationValue < 0f)
					{
						AddTextInfo("-" + factionRelationReward.Faction + " respect", Color.red);
					}
				}
			}
			if (!GameplayUtils.OnPause)
			{
				GameplayUtils.PauseGame();
				timeWasFreezed = true;
			}
			rewardShow = true;

		    


		}

	



		public void HideQwestInfo()
		{
			if (timeWasFreezed)
			{
				GameplayUtils.ResumeGame();
				timeWasFreezed = false;
			}
			BackButton.ChangeBackButtonsStatus(active: true);
			MissionCompletePanel.gameObject.SetActive(value: false);
			rewardShow = false;
			//StartCoroutine(HideEnumerator());
		}

		// private IEnumerator HideEnumerator()
		// {
		// 	WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
		// 	var a = CanvasGroup.alpha;
		// 	while (a >0)
		// 	{
		// 		yield return null;
		// 		a -= Time.deltaTime;
		// 		CanvasGroup.alpha = a;
		// 	}
		//
		// }

		private void Awake()
		{
			instance = this;
		}

		private void Update()
		{
			if (rewardShow && !timeWasFreezed && !GameplayUtils.OnPause)
			{
				GameplayUtils.PauseGame();
				timeWasFreezed = true;
			}
		}

		private void AddTextInfo(string text, Color color)
		{
			QwestCompleteLineContent qwestCompleteLineContent = CreateNewLine(LineWithTextSample);
			qwestCompleteLineContent.TextSample.text = text;
			qwestCompleteLineContent.TextSample.color = color;
		}

		private void AddImageInfo(Sprite image)
		{
			QwestCompleteLineContent qwestCompleteLineContent = CreateNewLine(LineWithImageSample);
			qwestCompleteLineContent.ImageSample.sprite = image;
		}

		private QwestCompleteLineContent CreateNewLine(RectTransform prefab)
		{
			RectTransform fromPool = PoolManager.Instance.GetFromPool(prefab);
			fromPool.parent = ContentContainer;
			fromPool.localScale = Vector3.one;
			return fromPool.GetComponent<QwestCompleteLineContent>();
		}
	}
}
