using Game.Character;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.PickUps
{
	public class MoneyPickup : PickUp
	{
		protected override void TakePickUp()
		{
			int amount = Random.Range(PickUpManager.MinMoney, PickUpManager.MaxMoney + 1);
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, amount.ToString());
			PlayerInfoManager.Instance.ChangeInfoValue(PlayerInfoType.Money, amount, useVipMultipler: true);
			base.TakePickUp();
		}
	}
}
