using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using System.Collections.Generic;

namespace Game.PickUps
{
	public class HealthPickup : PickUp
	{
		private static IDictionary<PickUpManager.HealthPackType, float> healthPacksValue = new Dictionary<PickUpManager.HealthPackType, float>
		{
			{
				PickUpManager.HealthPackType.Small,
				0.2f
			},
			{
				PickUpManager.HealthPackType.Medium,
				0.4f
			},
			{
				PickUpManager.HealthPackType.Large,
				0.8f
			}
		};

		public PickUpManager.HealthPackType HealthPackType;

		protected override void TakePickUp()
		{
			Player player = PlayerInteractionsManager.Instance.Player;
			if (!(player.Health.Current >= player.Health.Max))
			{
				float num = healthPacksValue[HealthPackType];
				int num2 = (int)(player.Health.Max * num);
				InGameLogManager.Instance.RegisterNewMessage(MessageType.HealthPack, ((int)(num * 100f)).ToString());
				PlayerInteractionsManager.Instance.Player.AddHealth(num2);
				base.TakePickUp();
			}
		}
	}
}
