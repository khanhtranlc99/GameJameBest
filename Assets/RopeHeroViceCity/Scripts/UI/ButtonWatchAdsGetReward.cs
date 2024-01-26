using System;
using Game.Character;
using Game.GlobalComponent;
using Game.Shop;
using Root.Scripts.Helper;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace RopeHeroViceCity.Scripts.UI
{
    public class ButtonWatchAdsGetReward : MonoBehaviour
    {
        [SerializeField] private int amountReward;
        [SerializeField] private bool isGem, shouldAsk;
        private Button btnOnClick;
        public bool offAds;
        private void Awake()
        {
            btnOnClick = GetComponent<Button>();
            btnOnClick.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            //if (AdManager.Instance.IsRewardVideoLoaded())
            //{
            //    AdManager.Instance.ShowRewardVideo(OnWatchVideoDone);
            //}
            //else
            //{
                UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.ADS_NOT_AVAILABLE, offAds);
            //}
        }


        private void OnWatchVideoDone(bool isDone)
        {
            if (!isDone)
                return;
            if (isGem)
            {
                PlayerInfoManager.Gems += amountReward;
            }
            else
            {
                PlayerInfoManager.Money += amountReward;
                if (InGameLogManager.Instance != null)
                    InGameLogManager.Instance.RegisterNewMessage(MessageType.Money, amountReward.ToString());
            }
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.UpdateInfo();
            }
        }
    }
}
