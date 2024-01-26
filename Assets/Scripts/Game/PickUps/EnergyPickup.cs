using Game.Character;
using Game.GlobalComponent;

namespace Game.PickUps
{
	public class EnergyPickup : PickUp
	{
		public float enegryCountInPickUp = 35f;

		protected override void TakePickUp()
		{
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Enegry, enegryCountInPickUp.ToString());
			PlayerInteractionsManager.Instance.Player.stats.stamina.SetAmount(enegryCountInPickUp);
			base.TakePickUp();
		}
	}
}
