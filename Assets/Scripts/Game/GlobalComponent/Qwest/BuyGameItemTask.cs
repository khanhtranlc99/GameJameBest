using Game.Character;
using Game.Items;
using Game.Shop;

namespace Game.GlobalComponent.Qwest
{
	public class BuyGameItemTask : BaseTask
	{
		public int ItemID;

		public int AddMoneyBeforeStartTask;

		public bool InGems;

		private GameItem targetItem;

		public override void TaskStart()
		{
			base.TaskStart();
			targetItem = ItemsManager.Instance.GetItem(ItemID);
			bool flag = ShopManager.Instance.ItemAvailableForBuy(targetItem);
			bool flag2 = StuffManager.AlredyEquiped(targetItem);
			bool flag3 = ShopManager.Instance.EnoughMoneyToBuyItem(targetItem);
			bool flag4 = PlayerInfoManager.Instance.BoughtAlredy(targetItem);
			if (targetItem.ShopVariables.MaxAmount == 1 && flag4)
			{
				CurrentQwest.MoveToTask(NextTask);
				return;
			}
			if (AddMoneyBeforeStartTask > 0)
			{
				string text = AddMoneyBeforeStartTask.ToString();
				if (PlayerInfoManager.VipLevel > 0)
				{
					text = text + " + " + (float)AddMoneyBeforeStartTask * PlayerInfoManager.Instance.GetVipMult(PlayerInfoType.Money) / 100f;
				}
				if (PlayerInteractionsManager.Instance.Player.IsTransformer && !InGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Experience, text);
				}
				else if (InGems)
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Gems, text);
				}
				else
				{
					InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, text);
				}
				if (!InGems)
				{
					PlayerInfoManager.Instance.ChangeInfoValue(PlayerInfoType.Money, AddMoneyBeforeStartTask, useVipMultipler: true);
				}
				else
				{
					PlayerInfoManager.Gems += AddMoneyBeforeStartTask;
				}
			}
			if (UIHighlightsManager.InstanceExist)
			{
				UIHighlightsManager.Instance.SetTargetItem(targetItem);
				UIHighlightsManager.Instance.ActivateShopButtonsHighlights(value: true);
			}
		}

		public override void Finished()
		{
			DeactivateHighLights();
			base.Finished();
		}

		public override void BuyItemEvent(GameItem item)
		{
			if (!(item == null) && ItemID == item.ID)
			{
				DeactivateHighLights();
				CurrentQwest.MoveToTask(NextTask);
			}
		}

		private void DeactivateHighLights()
		{
			if (UIHighlightsManager.InstanceExist)
			{
				UIHighlightsManager.Instance.ActivateShopButtonsHighlights(value: false);
				UIHighlightsManager.Instance.SetTargetItem(null);
				UIHighlightsManager.Instance.ActivateExitShopButtonsHighlights(value: true);
			}
		}
	}
}
