using Game.Managers;
using Game.Vehicle;
using System;
using UnityEngine;

namespace Game.Traffic
{
	public class VehicleDriversWeight : MonoBehaviour
	{
		[Serializable]
		public class VehicleDistribution
		{
			public DrivableVehicle Vehicle;

			public PrefabDistribution Distribution;
		}

		public DummyDriver DefaultDriver;

		public VehicleDistribution[] VehicleDistributions;

		public DummyDriver GetVehicleDriver(DrivableVehicle vehicle)
		{
			DummyDriver dummyDriver = null;
			VehicleDistribution[] vehicleDistributions = VehicleDistributions;
			foreach (VehicleDistribution vehicleDistribution in vehicleDistributions)
			{
				if (vehicleDistribution.Vehicle == vehicle)
				{
					dummyDriver = vehicleDistribution.Distribution.GetRandomPrefab().GetComponent<DummyDriver>();
					break;
				}
			}
			if (dummyDriver == null)
			{
				if (GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log("Driver for '" + vehicle.name + "' not find");
				}
				dummyDriver = DefaultDriver;
			}
			return dummyDriver;
		}
	}
}
