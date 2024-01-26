using Game.Character.CharacterController;

namespace Game.GlobalComponent.Qwest
{
	public class CollectItemTaskNode : TaskNode
	{
		[Separator("Specific")]
		public int ItemCount;

		public QwestPickupType QwestPickupType;

		public Faction TagetFaction;

		public int MarksCount = 2;

		[SelectiveString("MarkType")]
		public string MarksTypeNPC = "Kill";

		[SelectiveString("MarkType")]
		public string MarksTypePickUp = "Pickup";

		public override BaseTask ToPo()
		{
			CollectItemsTask collectItemsTask = new CollectItemsTask();
			collectItemsTask.PickupType = QwestPickupType;
			collectItemsTask.InitialCountToCollect = ItemCount;
			collectItemsTask.TargetFaction = TagetFaction;
			collectItemsTask.MarksTypeNPC = MarksTypeNPC;
			collectItemsTask.MarksTypePickUp = MarksTypePickUp;
			collectItemsTask.MarksCount = MarksCount;
			ToPoBase(collectItemsTask);
			return collectItemsTask;
		}
	}
}
