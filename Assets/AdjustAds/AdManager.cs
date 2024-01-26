using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Globalization;
using System.Linq;
using AppsFlyerSDK;
using GoogleMobileAds.Api;
using MEC;

namespace Root.Scripts.Helper
{
    public class AdManager : SingletonMonoBehavior<AdManager>
    {
        public static AdManager Instance;
        [SerializeField] private AdsController ironsourceController;

#if UNITY_ANDROID
        private const string InterstitialAdUnitId = "b54ac498d13ef37f";
        private const string RewardedAdUnitId = "6b227ce71cafca73";
        private const string BanerAdUnitId = "a7c39fcfb47b2e38";
#elif UNITY_IOS
    private const string InterstitialAdUnitId = "c8d31e48f08ed31e";
    private const string RewardedAdUnitId = "02932bb866cbb369";
    private const string BanerAdUnitId = "ff665c0a75cadcc4";
#endif

        public float countdownAds;
        public float layerAdTimer;
        public float MININUM_TIME_WATCH_INTER_ADS = 40;
        private void Awake()
        {
            Instance = this;
            Init();
        }
        public void Init()
        {
            ironsourceController.Init();
            countdownAds = 1000;
            layerAdTimer = 0;
        }

        #region Interstitial
        public bool ShowInterstitial(bool isShowImmediatly = false, string actionWatchLog = "other", UnityAction actionIniterClose = null, string level = null)
        {
#if UNITY_EDITOR
            actionIniterClose?.Invoke();
            countdownAds = 0;
            return true;
#endif
            //GameController.Instance.AnalyticsController.LoadInterEligible();
            if (countdownAds > MININUM_TIME_WATCH_INTER_ADS)
            {
                ironsourceController.ShowInterstitial(actionIniterClose,
            () =>
            {
                UseProfile.NumberOfAdsInDay = UseProfile.NumberOfAdsInDay + 1;
                UseProfile.NumberOfAdsInPlay = UseProfile.NumberOfAdsInPlay + 1;
                actionIniterClose?.Invoke();
                //GameController.Instance.AnalyticsController.LogInterShow(actionWatchLog);
                countdownAds = 0;
            });
            }
            else
            {
                if (actionIniterClose != null)
                    actionIniterClose();
            }
            return true;
        }

        public bool ShowInterstitialBetweenLayers(bool isShowImmediatly = false, string actionWatchLog = "other", UnityAction actionIniterClose = null, string level = null)
        {
#if UNITY_EDITOR
            actionIniterClose?.Invoke();
            layerAdTimer = 0;
            return true;
#endif
            if (layerAdTimer > MININUM_TIME_WATCH_INTER_ADS)
            {
                Debug.Log("Show quang cao giua layer");
                ironsourceController.ShowInterstitial(actionIniterClose,
                    () =>
                    {
                        actionIniterClose?.Invoke();
                        layerAdTimer = 0;
                    });
            }
            else
            {
                if (actionIniterClose != null)
                    actionIniterClose();
            }
            return true;
        }

        #endregion

        #region Video Reward

        /// <summary>
        /// Xử lý Show Video
        /// </summary>
        /// <param name="actionReward">Hành động khi xem xong Video và nhận thưởng </param>
        /// <param name="actionNotLoadedVideo"> Hành động báo lỗi không có video để xem </param>
        /// <param name="actionClose"> Hành động khi đóng video (Đóng lúc đang xem dở hoặc đã xem hết) </param>
        public void ShowRewardVideo(UnityAction<bool> actionReward)
        {
#if UNITY_EDITOR
            actionReward?.Invoke(true);
            return;
#endif

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                actionReward?.Invoke(false);
                //GameController.Instance.AnalyticsController.LogWatchVideo(ActionWatchVideo.None, true, false, "");
                return;
            }

            if (!ironsourceController.ShowRewardedVideo(() => { actionReward?.Invoke(false); }, () => { actionReward?.Invoke(true); countdownAds = 0; }))
            {
                actionReward?.Invoke(false);
            }
        }

        #endregion

        #region Banner
        public void ShowBanner()
        {
            ironsourceController.ShowBanner();

        }

        public void DestroyBanner()
        {
            ironsourceController.HideBanner();
        }
        #endregion

        private void Update()
        {
            countdownAds += Time.unscaledDeltaTime;
            layerAdTimer += Time.unscaledDeltaTime;
        }
        public bool IsRewardVideoLoaded()
        {
#if UNITY_EDITOR
            return true;
#endif
            return ironsourceController.IsRewardedVideoAvailable;
        }
        public bool IsInterstitialLoaded()
        {
            if (countdownAds < MININUM_TIME_WATCH_INTER_ADS)
                return false;
#if UNITY_EDITOR
            return true;
#endif
            var isLoaded = ironsourceController.IsInterstitialLoaded;
            return isLoaded;
        }
    }
}
