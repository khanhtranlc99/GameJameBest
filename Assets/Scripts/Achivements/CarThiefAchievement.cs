using Game.Managers;
using Game.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarThiefAchievement : Achievement
{
	public List<VehicleList> vehicleList = new List<VehicleList>();

	public List<VehicleList> carsYouNeed = new List<VehicleList>();

	public override void Init()
	{
		vehicleList = carsYouNeed;
		achiveParams = new SaveLoadAchievmentStruct(false, 0, vehicleList.Count);
	}

	public override void SaveAchiev()
	{
		BaseProfile.StoreValue(achiveParams, achievementName);
		BaseProfile.StoreArray(carsYouNeed.ToArray(), achievementName + "CarsList");
	}

	public override void LoadAchiev()
	{
		try
		{
			achiveParams = BaseProfile.ResolveValue(achievementName, achiveParams);
		}
		catch (Exception)
		{
			if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Oops achiveParams");
			}
			Init();
			SaveAchiev();
		}
		if (!achiveParams.isDone)
		{
			VehicleList[] array = BaseProfile.ResolveArray<VehicleList>(achievementName + "CarsList");
			if (array != null)
			{
				carsYouNeed = array.ToList();
			}
		}
	}

	public override void GetIntoVehicleEvent(DrivableVehicle vehicle)
	{
		if (carsYouNeed.Contains(vehicle.VehicleSpecificPrefab.Name))
		{
			if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("Dude, You stole my " + vehicle.VehicleSpecificPrefab.Name.ToString());
			}
			carsYouNeed.Remove(vehicle.VehicleSpecificPrefab.Name);
			achiveParams.achiveCounter++;
			if (carsYouNeed.Count == 0)
			{
				AchievComplite();
			}
		}
	}
}
