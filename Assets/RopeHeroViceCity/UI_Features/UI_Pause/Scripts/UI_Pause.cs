using Game.Managers;
using RopeHeroViceCity.Scripts.Gampelay;
using UnityEngine;
using UnityEngine.UI;
using RopeHeroViceCity.UI_Features.UI_Setting.Scripts;
namespace RopeHeroViceCity.UI_Features.UI_Pause.Scripts
{
    public class UI_Pause : AbsUICanvas
    {
        [SerializeField] private Button btnSetting, btnSkill, btnShop, btnAchievements;
        [SerializeField] private Button btnMoney, btnDiamond;

        public override void StartLayer()
        {
            base.StartLayer();
            btnSetting.onClick.AddListener(OnClickSetting);
            btnSkill.onClick.AddListener(OnClickSkill);
            btnShop.onClick.AddListener(OnClickShop);
            btnAchievements.onClick.AddListener(OnClickAchievement);
            btnMoney.onClick.AddListener(OnClickBuyMoney);
            btnDiamond.onClick.AddListener(OnClickBuyDiamond);
        }
        private void OnClickBuyMoney()
        {
            OnClickShop();
            Game.Shop.ShopManager.Instance.JumpToMoneyCategory();
        }
        private void OnClickBuyDiamond()
        {
            OnClickShop();
            Game.Shop.ShopManager.Instance.OpenGemsPanel();
        }

        private void OnClickSetting()
        {
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.OFF_ADS);
            UI_Setting.Scripts.UI_Setting.OpenSetting(true);
            //UICanvasController.Instance.ShowLayer(UICanvasKey.SETTING);
        }
        private void OnClickShop()
        {
            // UICanvasController.Instance.ShowLayer(UICanvasKey.SHOP);
            UI_Shop.Scripts.UI_Shop.OpenShop(this);
            Game.Shop.ShopManager.Instance.Init();
            Game.Shop.ShopManager.Instance.Enable();
            //UI_InGame.Scripts.UI_InGame.Instance.OnClickShop();
            //load scene shop
        }
        private void OnClickAchievement()
        {
            // UICanvasController.Instance.ShowLayer(UICanvasKey.SHOP);
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.OFF_ADS);
            UICanvasController.Instance.ShowLayer(UICanvasKey.Achievement);
            //load scene shop
        }
        private void OnClickSkill()
        {
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.OFF_ADS);
            UICanvasController.Instance.ShowLayer(UICanvasKey.SKILLS);
        }

        public override void ShowLayer()
        {
            base.ShowLayer();
            GameplayUtils.PauseGame();
            EventManager.Instance.OnOpenPausePanel();
        }

        public override void Close()
        {
            base.Close();
            GameplayUtils.ResumeGame();
            EventManager.Instance.OnClosePausePanel();
            UICanvasController.Instance.ShowLayer(UICanvasKey.INGAME);
        }
    }
}
