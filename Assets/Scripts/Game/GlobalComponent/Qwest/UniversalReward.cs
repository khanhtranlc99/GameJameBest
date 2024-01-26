using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.Items;
using Game.Shop;
using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	[Serializable]
	public class UniversalReward
	{
		public int ExperienceReward;

		public int MoneyReward;

		public bool RewardInGems;

		[Tooltip("Итемы ТОЛЬКО лежащие под GameItems НА СЦЕНЕ")]
		public int ItemRewardID;

		public FactionRelationReward[] RelationRewards;

		public bool IsHaveReward()
		{
			return MoneyReward > 0 || ExperienceReward > 0 || ItemsManager.Instance.GetItem(ItemRewardID) != null || (RelationRewards != null && RelationRewards.Length != 0);
		}

		public void GiveReward()
		{
			if (MoneyReward > 0)
			{
				string text = MoneyReward.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text = text + " + " + (float)MoneyReward * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Money) / 100f;
				}
				if (PlayerInteractionsManager.Instance.Player.IsTransformer && !RewardInGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Experience, text);
				}
				else if (RewardInGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Gems, text);
				}
				else
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, text);
				}
				if (!RewardInGems)
				{
					PlayerInfoManager.Instance.ChangeInfoValue(PlayerInfoType.Money, MoneyReward, useVipMultipler: true);
				}
				else
				{
					PlayerInfoManager.Gems += MoneyReward;
				}
			}
			if (ExperienceReward > 0)
			{
				string text2 = ExperienceReward.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text2 = text2 + " + " + (float)ExperienceReward * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Experience) / 100f;
				}
				if (PlayerInteractionsManager.Instance.Player.IsTransformer && !RewardInGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Experience, text2);
				}
				LevelManager.Instance.AddExperience(ExperienceReward, useVIPmult: true);
				InGameLogManager.Instance.RegisterNewMessage(MessageType.Experience, text2);
			}
			//Debug.LogError(ItemRewardID);
			GameItem item = ItemsManager.Instance.GetItem(ItemRewardID);
			if (item != null)
			{
				InGameLogManager.Instance.RegisterNewMessage(MessageType.Item, item.ShopVariables.Name);
				if (item is GameItemWeapon && PlayerInfoManager.Instance.BoughtAlredy(item))
				{
					AmmoManager.Instance.AddAmmo((item as GameItemWeapon).Weapon.AmmoType);
				}
				else
				{
					PlayerInfoManager.Instance.Give(item);
				}
			}
			if (RelationRewards != null && RelationRewards.Length != 0)
			{
				FactionRelationReward[] relationRewards = RelationRewards;
				foreach (FactionRelationReward factionRelationReward in relationRewards)
				{
					FactionsManager.Instance.ChangePlayerRelations(factionRelationReward.Faction, factionRelationReward.ChangeRelationValue);
					FactionsManager.Instance.SavePlayerRelations();
				}
			}
		}
	}
}
