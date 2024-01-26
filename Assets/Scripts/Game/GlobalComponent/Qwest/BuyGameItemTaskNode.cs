using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class BuyGameItemTaskNode : TaskNode
	{
		[Separator("Specific")]
		[Space]
		public int AddMoneyBeforeStartTask;

		[Tooltip("Итемы ТОЛЬКО лежащие под GameItems НА СЦЕНЕ")]
		public int ItemID;

		public bool InGems;

		public override BaseTask ToPo()
		{
			BuyGameItemTask buyGameItemTask = new BuyGameItemTask();
			buyGameItemTask.ItemID = ItemID;
			buyGameItemTask.AddMoneyBeforeStartTask = AddMoneyBeforeStartTask;
			buyGameItemTask.InGems = InGems;
			BuyGameItemTask buyGameItemTask2 = buyGameItemTask;
			ToPoBase(buyGameItemTask2);
			return buyGameItemTask2;
		}
	}
}
