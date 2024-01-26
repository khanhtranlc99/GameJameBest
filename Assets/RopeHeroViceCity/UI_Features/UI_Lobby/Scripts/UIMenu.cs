using RopeHeroViceCity.UI_Features.DailyReward.Scripts;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.UI_Lobby.Scripts
{
    public class UIMenu : AbsUICanvas
    {
        public MenuPanelManager PanelManager;

        public LoadSceneController SceneController;

        public Animator LoadingPanel;


        //public int WatchAdMoneyReward;
        [SerializeField] private GameObject prefabMoney;
        [SerializeField] private GameObject prefabGems;
        [SerializeField] private GameObject nativeAds;
        private GameObject gemsPopup;
        private GameObject moneyPopup;
        [SerializeField] private Button btnDailyReward, btnSetting, btnBuyGems, btnBuyMoney;

        public override void StartLayer()
        {
            base.StartLayer();
            btnDailyReward.onClick.AddListener(() =>
            {
                EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.OFF_ADS);
                UICanvasController.Instance.ShowLayer(UICanvasKey.DAILY_REWARD);
            });
            // btnAchievement.onClick.AddListener(() =>
            // {
            // 	UI_Achivement.ShowPopup(false);
            // });
            btnSetting.onClick.AddListener(() =>
            {
                EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.OFF_ADS);
                UI_Setting.Scripts.UI_Setting.OpenSetting(false);
            });
            btnBuyGems.onClick.AddListener(BuyGems);
            btnBuyMoney.onClick.AddListener(BuyMoney);
            // btnWatchAdsGetMoney.onClick.AddListener(() =>
            // {
            //              AdManager.Instance.ShowRewardVideo(OnWatchVideoGetMoneyDone);
            //          });
        }
        private void BuyGems()
        {
            //open gem popup
            if (gemsPopup == null)
            {
                gemsPopup = Instantiate(prefabGems, this.transform);
                gemsPopup.GetComponent<BuyGemsDialogPanel>().close.onClick.AddListener(OffRayCast);
            }
            else
            {
                gemsPopup.gameObject.SetActive(true);
            }
            //nativeAds.SetActive(false);
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.OFF_ADS_CANVAS);
            gemsPopup.GetComponent<ShopDialogPanel>().UpdatePanel();
        }
        private void BuyMoney()
        {
            //open money popup
            if (moneyPopup == null)
            {
                moneyPopup = Instantiate(prefabMoney, this.transform);
                moneyPopup.GetComponent<BuyGemsDialogPanel>().close.onClick.AddListener(OffRayCast);
            }
            else
            {
                moneyPopup.gameObject.SetActive(true);
            }
            nativeAds.SetActive(false);
            moneyPopup.GetComponent<ShopDialogPanel>().UpdatePanel();
        }
        private void OffRayCast()
        {
            //nativeAds.SetActive(true);
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.ON_ADS_CANVAS);

        }
        public override void ShowLayer()
        {
            //base.ShowLayer();
            //if (UI_DailyReward.CanReceiveRewardToday())
            //{
            //    EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.OFF_ADS);
            //    UICanvasController.Instance.ShowLayerToQueueCache(UICanvasKey.DAILY_REWARD);
            //}
        }

        public override void ProcessBackButton()
        {
            base.ProcessBackButton();
            UI_GeneralPopup.ShowPopup(null, "Are you sure you want to quit?", false, delegate
              {
                  PanelManager.ExitApplication();
              });
        }

    }
}
