using Game.Vehicle;
using System;
using UnityEngine;

public class TaxiDriverAchievement : Achievement
{
	private const int StartTime = 600;

	private float timeLeft;

	private bool isTimerOn;

	public override void Init()
	{
		timeLeft = 600f;
		achiveParams = new SaveLoadAchievmentStruct(false, 600 - (int)timeLeft, 600);
	}

	public override void SaveAchiev()
	{
		achiveParams.achiveCounter = 600 - (int)timeLeft;
		BaseProfile.StoreValue(achiveParams, achievementName);
	}

	public override void LoadAchiev()
	{
		try
		{
			achiveParams = BaseProfile.ResolveValue(achievementName, achiveParams);
			timeLeft = 600 - achiveParams.achiveCounter;
		}
		catch (Exception)
		{
			UnityEngine.Debug.Log("Oops achiveParams");
			Init();
			SaveAchiev();
		}
	}

	public override void GetIntoVehicleEvent(DrivableVehicle vehicle)
	{
		if (vehicle.VehicleSpecificPrefab.Name == VehicleList.Taxi)
		{
			isTimerOn = true;
		}
	}

	public override void GetOutVehicleEvent(DrivableVehicle vehicle)
	{
		if (isTimerOn)
		{
			isTimerOn = false;
		}
	}

	public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
	{
		if (isTimerOn)
		{
			isTimerOn = false;
		}
	}

	private void LateUpdate()
	{
		if (isTimerOn)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0f)
			{
				isTimerOn = false;
				AchievComplite();
			}
		}
	}

	private void OnDestroy()
	{
		if (isTimerOn)
		{
			isTimerOn = false;
		}
	}
}
