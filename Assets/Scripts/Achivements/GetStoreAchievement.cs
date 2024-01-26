public class GetStoreAchievement : Achievement
{
	private const int ACHIEVTARGERT = 10;

	public override void Init()
	{
		achiveParams = new SaveLoadAchievmentStruct(false, 0, 10);
	}

	public override void GetShopEvent()
	{
		achiveParams.achiveCounter++;
		if (achiveParams.achiveCounter >= achiveParams.achiveTarget)
		{
			AchievComplite();
		}
	}
}
