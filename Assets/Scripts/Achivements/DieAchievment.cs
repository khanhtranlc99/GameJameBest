public class DieAchievment : Achievement
{
	private const int ACHIEVTARGERT = 20;

	public override void Init()
	{
		achiveParams = new SaveLoadAchievmentStruct(false, 0, 20);
	}

	public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
	{
		if (achiveParams.achiveCounter < achiveParams.achiveTarget)
		{
			achiveParams.achiveCounter++;
		}
		else
		{
			AchievComplite();
		}
	}
}
