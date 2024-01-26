namespace Game.GlobalComponent.Qwest
{
	public class MassacreTaskNode : TaskNode
	{
		[Separator("Specific")]
		public int WeaponItemID;

		public int RequiredVictimsCount = 50;

		public int MarksCount = 5;

		[SelectiveString("MarkType")]
		public string MarksTypeNPC = "Kill";

		public override BaseTask ToPo()
		{
			MassacreTask massacreTask = new MassacreTask();
			massacreTask.RequiredVictimsCount = RequiredVictimsCount;
			massacreTask.WeaponItemID = WeaponItemID;
			massacreTask.MarksTypeNPC = MarksTypeNPC;
			massacreTask.MarksCount = MarksCount;
			ToPoBase(massacreTask);
			return massacreTask;
		}
	}
}
