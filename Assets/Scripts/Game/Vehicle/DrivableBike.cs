using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Vehicle
{
	public class DrivableBike : DrivableVehicle
	{
		public Transform metarig;

		public Transform DriverStatPoint;

		public BikeTrigger BikeTrigger;

		[HideInInspector]
		public Animator animator => GetComponent<Animator>();

		public override VehicleType GetVehicleType()
		{
			return VehicleType.Bicycle;
		}

		public override void Init()
		{
		}

		public override bool IsControlsPlayerAnimations()
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

		public override void Drive(Player driver)
		{
			base.Drive(driver);
			if ((bool)BikeTrigger)
			{
				BikeTrigger.enabled = true;
				BikeTrigger.Init();
			}
		}

		public override void GetOut()
		{
			base.GetOut();
			if ((bool)BikeTrigger)
			{
				BikeTrigger.enabled = false;
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			if (PlayerInteractionsManager.Instance.inVehicle && (bool)controller)
			{
				((BikeController)controller).DropFromSpeed(Vector3.Dot(col.relativeVelocity, col.contacts[0].normal));
			}
		}
	}
}
