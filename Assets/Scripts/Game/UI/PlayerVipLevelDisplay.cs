using Game.Character;
using Game.Items;
using Game.Shop;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayerVipLevelDisplay : PlayerInfoDisplayBase
    {
        public Image ImageLink;

        protected override PlayerInfoType GetInfoType()
        {
            return PlayerInfoType.VipLvL;
        }

        protected override void Display()
        {
            StartCoroutine(DisplayVip());
        }
        IEnumerator DisplayVip()
        {
            yield return new WaitUntil(() => ShopManager.Instance.loadStuffDone);
            if (PlayerInfoManager.VipLevel > 0)
            {
                GameItem shopItemByType = ShopManager.Instance.GetShopItemByType<GameItemBonus>(ItemsTypes.Bonus, new object[2]
                {
                    BonusTypes.VIP,
                    PlayerInfoManager.VipLevel
                });
                ImageLink.sprite = shopItemByType.ShopVariables.ItemIcon;
            }
        }
    }
}
