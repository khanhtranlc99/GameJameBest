using Game.Character.CharacterController;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;

namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemRopeData", menuName = "RopeData/ItemData/Rope", order = 100)]
	public class GameItemRope : GameItemAbility
	{
		public GameObject ShotRopeButton;

		public bool UseInFly;

		public override void Enable()
		{
			if (UseInFly)
			{
				PlayerManager.Instance.AnimationController.UseRopeWhileFlying = true;
				return;
			}
			PlayerManager.Instance.AnimationController.useRope = true;
			if (ShotRopeButton == null)
				ShotRopeButton = UI_InGame.Instance.btnShootRope;
			ShotRopeButton.SetActive(value: true);
		}

		public override void Disable()
		{
			if (UseInFly)
			{
				PlayerManager.Instance.AnimationController.UseRopeWhileFlying = false;
				return;
			}
			PlayerManager.Instance.AnimationController.useRope = false;
			if (ShotRopeButton == null)
				ShotRopeButton = UI_InGame.Instance.btnShootRope;
			ShotRopeButton.SetActive(value: false);
		}
	}
}
