using Game.Character;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class RepairAds : HelpfulAds
	{
		[Range(0f, 1f)]
		public float RepairProcent = 0.5f;

		public override HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.VehicleRepair;
		}

		public override void HelpAccepted()
		{
			VehicleStatus vehicleStatus = PlayerInteractionsManager.Instance.LastDrivableVehicle.GetVehicleStatus();
			float amount = vehicleStatus.Health.Max * RepairProcent;
			vehicleStatus.Repair(amount);
		}
	}
}
