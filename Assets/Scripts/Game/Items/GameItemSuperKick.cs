using Game.Character;
using Game.Character.CharacterController;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;

namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemKickData", menuName = "RopeData/ItemData/Kick", order = 100)]
	public class GameItemSuperKick : GameItemAbility
	{
		public GameObject KickButton;

		public override void Enable()
		{
			PlayerManager.Instance.Player.GetComponentInChildren<SuperKick>().enabled = true;
			if (KickButton == null)
				KickButton = UI_InGame.Instance.btnKick;
			KickButton.SetActive(value: true);
		}

		public override void Disable()
		{
			PlayerManager.Instance.Player.GetComponentInChildren<SuperKick>().enabled = false;
						if (KickButton == null)
            				KickButton = UI_InGame.Instance.btnKick;
			KickButton.SetActive(value: false);
		}
	}
}
