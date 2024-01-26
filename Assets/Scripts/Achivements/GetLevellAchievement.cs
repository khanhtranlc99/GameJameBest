public class GetLevellAchievement : Achievement
{
	private const int ACHIEVTARGERT = 20;

	public override void Init()
	{
		achiveParams = new SaveLoadAchievmentStruct(false, 0, 20);
	}

	public override void GetLevelEvent(int level)
	{
		if (achiveParams.achiveCounter < achiveParams.achiveTarget)
		{
			achiveParams.achiveCounter = level;
		}
		else
		{
			AchievComplite();
		}
	}
}
