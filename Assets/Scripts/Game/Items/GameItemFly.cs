using Game.Character.CharacterController;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;

namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemFlyData", menuName = "RopeData/ItemData/Fly", order = 100)]
	public class GameItemFly : GameItemAbility
	{
		public GameObject FlyButton;

		public bool ExplosionInvulnerInFly;

		public bool CollisionInvulnerInFly;

		public override void Enable()
		{
			if (ExplosionInvulnerInFly)
			{
				PlayerManager.Instance.AnimationController.SetExplosionInvulStatus(status: true);
				return;
			}
			if (CollisionInvulnerInFly)
			{
				PlayerManager.Instance.AnimationController.SetCollisionInvulStatus(status: true);
				return;
			}
			PlayerManager.Instance.AnimationController.UseSuperFly = true;
			if (FlyButton == null)
				FlyButton = UI_InGame.Instance.btnFlyOn;
			FlyButton.SetActive(value: true);
		}

		public override void Disable()
		{
			if (ExplosionInvulnerInFly)
			{
				PlayerManager.Instance.AnimationController.SetExplosionInvulStatus(status: false);
			}
			else if (CollisionInvulnerInFly)
			{
				PlayerManager.Instance.AnimationController.SetCollisionInvulStatus(status: false);
			}
			else
			{
				PlayerManager.Instance.AnimationController.UseSuperFly = false;
				if (FlyButton == null)
					FlyButton = UI_InGame.Instance.btnFlyOn;
				FlyButton.SetActive(value: false);
			}
			PlayerManager.Instance.AnimationController.Reset();
			PlayerManager.Instance.Player.ResetMoveState();
		}
	}
}
