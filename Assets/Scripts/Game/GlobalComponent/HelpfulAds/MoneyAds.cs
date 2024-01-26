using Game.Character;
using Game.Shop;

namespace Game.GlobalComponent.HelpfulAds
{
	public class MoneyAds : HelpfulAds
	{
		public bool ByGems;

		public int AddedMoney = 300;

		public HelpfullAdsType AdsType = HelpfullAdsType.Money;

		public override HelpfullAdsType HelpType()
		{
			return AdsType;
		}

		public override void HelpAccepted()
		{
			if (ByGems)
			{
				PlayerInfoManager.Gems += AddedMoney;
			}
			else
			{
				PlayerInfoManager.Money += AddedMoney;
				InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, AddedMoney.ToString());
			}
			if (ShopManager.Instance != null)
			{
				ShopManager.Instance.UpdateInfo();
			}
		}
	}
}
