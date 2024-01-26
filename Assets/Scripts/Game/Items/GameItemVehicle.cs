using Game.Character.Stats;
using Game.Vehicle;
using UnityEngine;

namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemVehicleData", menuName = "RopeData/ItemData/Vehicles", order = 100)]
	public class GameItemVehicle : GameItem
	{
		public GameObject VehiclePrefab;

		public StatAttribute[] StatAttributes;

		public AdditionalFeature[] AdditionalFeatures;

		public static float maxSpeed
		{
			get;
			private set;
		}

		public static float maxAcceleration
		{
			get;
			private set;
		}

		public static float maxHealth
		{
			get;
			private set;
		}

		public override void Init()
		{
			base.Init();
			DrivableVehicle component = VehiclePrefab.GetComponent<DrivableVehicle>();
			VehicleStatus componentInChildren = VehiclePrefab.GetComponentInChildren<VehicleStatus>();
			if (component.MaxSpeed > maxSpeed)
			{
				maxSpeed = component.MaxSpeed;
			}
			if (component.Acceleration > maxAcceleration)
			{
				maxAcceleration = component.Acceleration;
			}
			if (componentInChildren.Health.Max > maxHealth)
			{
				maxHealth = componentInChildren.Health.Max;
			}
			for (int i = 0; i < StatAttributes.Length; i++)
			{
				switch (StatAttributes[i].StatType)
				{
				case StatsList.DrivingMaxSpeed:
					StatAttributes[i].SetStatValue(component.MaxSpeed);
					break;
				case StatsList.CarAcceleration:
					StatAttributes[i].SetStatValue(component.Acceleration);
					break;
				case StatsList.CarHealth:
					StatAttributes[i].SetStatValue(componentInChildren.Health.Max);
					break;
				}
			}
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return VehiclePrefab == (GameObject)parametrs[0];
		}
	}
}
