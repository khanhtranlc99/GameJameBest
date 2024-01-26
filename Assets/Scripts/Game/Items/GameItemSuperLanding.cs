using Game.Character.CharacterController;
using UnityEngine;
namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemSuperLandingData", menuName = "RopeData/ItemData/SuperLanding", order = 100)]
	public class GameItemSuperLanding : GameItemAbility
	{
		public bool SuperLandingExplosion;

		public override void Enable()
		{
			if (SuperLandingExplosion)
			{
				PlayerManager.Instance.Player.UseSuperLandingExplosion = true;
				return;
			}
			PlayerManager.Instance.AnimationController.useSuperheroLandings = true;
			PlayerManager.Instance.Player.UpdateOnFallImpact();
		}

		public override void Disable()
		{
			if (SuperLandingExplosion)
			{
				PlayerManager.Instance.Player.UseSuperLandingExplosion = false;
				return;
			}
			PlayerManager.Instance.AnimationController.useSuperheroLandings = false;
			PlayerManager.Instance.Player.UpdateOnFallImpact();
		}
	}
}
