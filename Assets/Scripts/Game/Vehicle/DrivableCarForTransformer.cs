using Game.Character.CharacterController;
using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableCarForTransformer : DrivableCar
	{
		private const float DamagePerDrow = 10f;

		private const float waterDepth = -14f;

		public override bool IsAbleToEnter()
		{
			return true;
		}

		public override bool HasExitAnimation()
		{
			return false;
		}

		public override bool HasEnterAnimation()
		{
			return false;
		}

		public override bool IsControlsPlayerAnimations()
		{
			return true;
		}

		public override void Init()
		{
			if (VehicleControllerPrefab == null)
			{
				throw new Exception($"{base.gameObject.name} DrivableVehicle is missing VehicleControllerPrefab");
			}
			if (vehStatus == null)
			{
				vehStatus = GetComponentInChildren<VehicleStatus>();
			}
			vehStatus.Initialization(setUpHealth: false);
			ApplyCenterOfMass(VehiclePoints.CenterOfMass);
			ChangeBodyColor(BodyRenderers);
			if ((bool)WaterSensor)
			{
				WaterSensor.Init();
			}
		}

		protected override void FixedUpdate()
		{
			if ((bool)CurrentDriver && CurrentDriver.IsPlayer)
			{
				Vector3 position = base.transform.position;
				if (-14f > position.y + VehicleSpecificPrefab.MaxHeight / 2f)
				{
					CurrentDriver.OnHit(DamageType.Water, null, 10f * Time.deltaTime, Vector3.zero, Vector3.zero, 0f);
				}
			}
			base.FixedUpdate();
		}
	}
}
