using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class SecondBreathAds : HelpfulAds
	{
		[Range(0f, 1f)]
		public float RecoveryProcent = 0.5f;

		public override HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.Stamina;
		}

		public override void HelpAccepted()
		{
			Player player = PlayerInteractionsManager.Instance.Player;
			float amount = player.stats.stamina.Max * RecoveryProcent;
			player.stats.stamina.Change(amount);
		}
	}
}
