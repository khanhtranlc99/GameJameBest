using System;
using UnityEngine;

public static class TimeManager
{
	public static bool OnCoolDown(long cdStartTime, long cooldown)
	{
		long num = DateTime.Now.ToFileTime();
		return num < cdStartTime + cooldown * 10000000;
	}

	public static int RemainingCooldawn(long cdStartTime, long cooldown)
	{
		long num = DateTime.Now.ToFileTime();
		return Mathf.RoundToInt((cdStartTime + cooldown * 10000000 - num) / 10000000);
	}

	public static bool AnotherDay(long cdStartTime)
	{
		DateTime date = DateTime.FromFileTime(cdStartTime).Date;
		return DateTime.Today > date;
	}
}
