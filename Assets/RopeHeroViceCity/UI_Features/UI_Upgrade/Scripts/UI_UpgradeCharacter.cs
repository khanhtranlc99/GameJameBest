using Game.Character;
using Game.MiniMap;
using Root.Scripts.Helper;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;

namespace RopeHeroViceCity.UI_Features.UI_Upgrade.Scripts
{
    public class UI_UpgradeCharacter : AbsRopePopup
    {

        public override void ShowLayer()
        {
            base.ShowLayer();
            //if (AdManager.Instance.IsInterstitialLoaded())
            //    AdManager.Instance.ShowInterstitial();
        }

        public void OnClickWatchADButton()
        {
            //if (AdManager.Instance.IsRewardVideoLoaded())
            //{
            //    AdManager.Instance.ShowRewardVideo((s) =>
            //    {
            //        PlayerInfoManager.UpgradePoints += 1;
            //        OnAds();
            //        Close();
            //        //content.gameObject.SetActive(false);
            //    });
            //}
            //else
            //{
                UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.ADS_NOT_AVAILABLE);
            //}
        }
        public void OpenMoney()
        {
            base.Close();
            OnClickShop();
            Game.Shop.ShopManager.Instance.JumpToMoneyCategory();
        }
        public void OpenDiamond()
        {
            base.Close();
            OnClickShop();
            Game.Shop.ShopManager.Instance.OpenGemsPanel();
        }
        private void OnClickShop()
        {
            UI_Shop.Scripts.UI_Shop.OpenShopFromSkill();
            Game.Shop.ShopManager.Instance.Init();
            Game.Shop.ShopManager.Instance.Enable();
        }
        public void OnAds()
        {
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.ON_ADS);
        }
    }
}
