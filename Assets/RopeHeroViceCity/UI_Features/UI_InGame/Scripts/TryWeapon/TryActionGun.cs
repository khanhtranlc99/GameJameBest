using Game.Character;
using Game.Items;
using Game.Shop;
using UnityEngine;

namespace UI.TryWeapon
{
    public class TryActionGun : TryWeaponAction
    {
        protected UITryWeapon controller;
        public TryActionGun(int valueAmount) : base(valueAmount)
        {
        }

        public override bool CheckExpire()
        {
            var isExpire = valueAmount <= 0;
            if (isExpire)
            {
                controller.UnregisterEvent(OnPlayerShoot);
                return true;
            }
            return false;
        }

        public override void ApplyItemToTry(UITryWeapon controller,GameItemWeapon choosedWeapon)
        {

            this.controller = controller;
            ShopCategory ammoCategory;
            ShopItem ammoShopItem;
            GameItemAmmo shopItemByType = ShopManager.Instance.GetShopItemByType<GameItemAmmo>(ItemsTypes.PatronContainer, new object[1]
            {
                choosedWeapon.Weapon.AmmoType
            }, out ammoCategory, out ammoShopItem);
            PlayerInfoManager.Instance.Give(shopItemByType,false,valueAmount);
            controller.RegisterEvent(OnPlayerShoot);
            base.ApplyItemToTry(controller,choosedWeapon);
        }

        private void OnPlayerShoot()
        {
            valueAmount--;
        }
    }
}
