public class LevelsProfile
{
	private const string LevelInfo_isAvaiable = "LevelInfo.isAvaiable";

	private const string LevelInfo_StarsCount = "LevelInfo.StarsCount";

	public static LevelInfo GetLevelInfo(int level)
	{
		LevelInfo levelInfo = new LevelInfo();
		levelInfo.IsAvailable = BaseProfile.ResolveValue(GetLevelId(level, "LevelInfo.isAvaiable"), defaultValue: false);
		levelInfo.StarsCount = BaseProfile.ResolveValue(GetLevelId(level, "LevelInfo.StarsCount"), 0);
		return levelInfo;
	}

	public static void SetLevelInfo(int level, LevelInfo info)
	{
		BaseProfile.StoreValue(info.IsAvailable, GetLevelId(level, "LevelInfo.isAvaiable"));
		BaseProfile.StoreValue(info.StarsCount, GetLevelId(level, "LevelInfo.StarsCount"));
	}

	private static string GetLevelId(int level, string param)
	{
		return "LevelInfo_" + level + "_" + param;
	}
}
