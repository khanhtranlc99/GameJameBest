using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SuicideAchievment : Achievement
{
	public enum DethType
	{
		None,
		Drowing,
		Falling,
		Explosion,
		CarAccident,
		Shooting
	}

	private const int ACHIEVTARGERT = 20;

	public List<DethType> DeathList = new List<DethType>();

	public List<DethType> deathYouNeed = new List<DethType>();

	public override void Init()
	{
		DeathList = deathYouNeed;
		achiveParams = new SaveLoadAchievmentStruct(false, 0, DeathList.Count);
	}

	public override void SaveAchiev()
	{
		BaseProfile.StoreValue(achiveParams, achievementName);
		BaseProfile.StoreArray(deathYouNeed.ToArray(), achievementName + "deathYouNeed");
	}

	public override void LoadAchiev()
	{
		try
		{
			achiveParams = BaseProfile.ResolveValue(achievementName, achiveParams);
		}
		catch (Exception)
		{
			UnityEngine.Debug.Log("Oops achiveParams");
			Init();
			SaveAchiev();
		}
		try
		{
			deathYouNeed = BaseProfile.ResolveArray<DethType>(achievementName + "deathYouNeed").ToList();
		}
		catch (Exception)
		{
			UnityEngine.Debug.Log("Oops, No dethYouNeed");
		}
	}

	public override void PlayerDeadEvent(DethType i = DethType.None)
	{
		UnityEngine.Debug.Log("deth = " + i);
		if (deathYouNeed.Contains(i))
		{
			deathYouNeed.Remove(i);
			achiveParams.achiveCounter++;
			if (deathYouNeed.Count == 0)
			{
				AchievComplite();
			}
		}
	}
}
