using Game.UI;
using System;
using Root.Scripts.Helper;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
    public class HelpfullAdsManager : MonoBehaviour
    {
        private static HelpfullAdsManager instance;

        public float HelpTimerLength;

        private HelpfulAds[] helps;

        private float lastHelpTime = -60;

        private Action<bool> lastCallback;

        private HelpfulAds lastHelpfulAds;

        public static HelpfullAdsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    UnityEngine.Debug.Log("HelpfullAdsManager is not initialized");
                }
                return instance;
            }
        }

        public bool IsReady => Time.time > lastHelpTime + HelpTimerLength;

        public HelpfulAds GetAdsByType(HelpfullAdsType helpType)
        {
            //if (helps == null || helps.Length == 0)
            //{
            //    return null;
            //}
            //HelpfulAds[] array = helps;
            //foreach (HelpfulAds helpfulAds in array)
            //{
            //    if (helpfulAds.HelpType() == helpType)
            //    {
            //        return helpfulAds;
            //    }
            //}
            return null;
        }

        public void OfferAssistance(HelpfullAdsType helpType, Action<bool> helpCallback)
        {
            //if (!IsReady)
            //{
            //    helpCallback?.Invoke(obj: false);
            //    if (helpType == HelpfullAdsType.FreeGems || helpType == HelpfullAdsType.FreeMoney)
            //    {
            //        UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.ADS_NOT_AVAILABLE);
            //    }
            //    return;
            //}
            //lastHelpfulAds = null;
            //HelpfulAds[] array = helps;
            //foreach (HelpfulAds helpfulAds in array)
            //{
            //    if (helpfulAds.HelpType() == helpType)
            //    {
            //        lastHelpfulAds = helpfulAds;
            //        break;
            //    }
            //}
            //if (lastHelpfulAds == null)
            //{
            //    throw new Exception("HelpfullAds for " + helpType + " type not found!");
            //}
            //lastCallback = helpCallback;
            //UI_GeneralPopup.ShowPopup(null, lastHelpfulAds.Message, false, delegate
            // {
            //     SelectionChoosed(accepted: true);
            // }, delegate
            // {
            //     SelectionChoosed(accepted: false);
            // });
        }

        public void SelectionChoosed(bool accepted)
        {
            //lastHelpTime = Time.time;
            //if (!accepted)
            //{
            //    if (lastCallback != null)
            //    {
            //        lastCallback(obj: false);
            //    }
            //}
            //else
            //{
            //    AdManager.Instance.ShowRewardVideo(Callback);
            //}
        }

        public void ShowRewardVideo()
        {
            //AdManager.Instance.ShowRewardVideo(Callback);
        }

        private void Callback(bool isReward)
        {
            MainThreadExecuter.Instance.Run(delegate
            {
                CallbackResolver(isReward);
            });
        }

        private void CallbackResolver(bool isReward)
        {
            if (!isReward)
            {
                if (lastCallback != null)
                {
                    lastCallback(obj: false);
                }
                return;
            }
            if (lastHelpfulAds != null)
                lastHelpfulAds.HelpAccepted();
            if (lastCallback != null)
            {
                lastCallback(obj: true);
            }
        }

        private void Awake()
        {
            instance = this;
            helps = GetComponentsInChildren<HelpfulAds>();
            lastHelpTime = Time.time - HelpTimerLength;
        }
    }
}
