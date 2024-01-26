using Game.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.UI_Shop.Scripts
{
    public class UI_Shop : AbsUICanvas
    {
        [SerializeField] private Button btnBuy, btnEquip, btnCloseShop, btnOpenExchancePanel;
        public static UICanvasKey lastPopupKey;

        public override void StartLayer()
        {
            base.StartLayer();
            btnBuy.onClick.AddListener(OnClickBuyItem);
            btnEquip.onClick.AddListener(OnClickEquipItem);
            btnCloseShop.onClick.AddListener(OnClickCloseShop);
            btnOpenExchancePanel.onClick.AddListener(OnClickExchange);
        }

        public override void ShowLayer()
        {
            base.ShowLayer();
            UICanvasController.Instance.HideLayer(lastPopupKey);
        }

        private void OnClickBuyItem()
        {
            ShopManager.Instance.Buy();
        }
        private void OnClickEquipItem()
        {
            ShopManager.Instance.EquipCurrentItem();
        }
        private void OnClickExchange()
        {
            ShopManager.Instance.OpenExchangePanel();
        }
        private void OnClickCloseShop()
        {
            ShopManager.Instance.Disable();
            Close();
            UICanvasController.Instance.ShowLayer(lastPopupKey);
            GameplayUtils.ResumeGame();
        }

        public static void OpenShop(AbsUICanvas lastPopup)
        {
            lastPopupKey = lastPopup.layerKey;
            UICanvasController.Instance.ShowLayer(UICanvasKey.SHOP);
        }
        public static void OpenShopFromSkill()
        {
            lastPopupKey = UICanvasKey.PAUSE_MENU;
            UICanvasController.Instance.ShowLayer(UICanvasKey.SHOP);
        }
    }
}
