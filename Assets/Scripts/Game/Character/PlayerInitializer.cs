using Game.Character.CharacterController;
using Game.Weapons;
using UnityEngine;

namespace Game.Character
{
	public class PlayerInitializer : MonoBehaviour
	{
		public AnimationController PlayerAm;

		public WeaponController PlayerWc;

		public Player PlayerStatus;

		public SurfaceSensor WaterSensor;

		public void Initialaize()
		{
			if (!PlayerAm)
			{
				PlayerAm = GetComponent<AnimationController>();
			}
			if (!PlayerWc)
			{
				PlayerWc = GetComponent<WeaponController>();
			}
			if (!PlayerStatus)
			{
				PlayerStatus = GetComponent<Player>();
			}
			if (!WaterSensor)
			{
				WaterSensor = GetComponentInChildren<SurfaceSensor>();
			}
			PlayerAm.Initialization();
			PlayerWc.Initialization();
			PlayerStatus.Initialization();
			WaterSensor.Init();
		}
	}
}
