using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AdsController : MonoBehaviour
{
    private UnityAction onInterstitialClosed;
    private UnityAction onInterstitialShowFailed;

    private UnityAction onRewardedClosed;
    private UnityAction onRewaredShowFailed;

    private bool gotRewarded;
    private bool isShownBanner;
    private bool loadBannerCalled;

    private const string IronsouceAppID = Config.IRONSOURCE_DEV_KEY;

    private IEnumerator reloadInterCoru;

    public void Init()
    {
        IronSource.Agent.init(IronsouceAppID, IronSourceAdUnits.BANNER, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.REWARDED_VIDEO);
        IronSourceEvents.onInterstitialAdClosedEvent += IronSourceEvents_onInterstitialAdClosedEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += IronSourceEvents_onInterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += IronSourceEvents_onInterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdReadyEvent += IronSourceEvents_onInterstitialReady;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += IronSourceEvents_onInterstitialAdShowSucceededEvent;

        IronSourceEvents.onRewardedVideoAdClosedEvent += IronSourceEvents_onRewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += IronSourceEvents_onRewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += IronSourceEvents_onRewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdReadyEvent += IronSourceEvents_onRewardedReady;

        IronSourceEvents.onBannerAdLoadFailedEvent += OnBannerLoadFail;
        IronSourceEvents.onBannerAdLoadedEvent += OnBannerLoaded;
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        // Load banner & interstitial
        loadInterstitial();
        //#if !ENV_PROD
        IronSource.Agent.validateIntegration();
        //#endif

        IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;
#if ENV_LOG
Debug.Log("AdsController initialized");
#endif
    }


    private void IronSourceEvents_onInterstitialAdShowSucceededEvent()
    {
        if (reloadInterCoru != null)
        {
            StopCoroutine(reloadInterCoru);
            reloadInterCoru = null;
        }
    }
    bool reward;
    private void IronSourceEvents_onRewardedVideoAdRewardedEvent(IronSourcePlacement obj)
    {
        reward = true;

        // GameController.Instance.AnalyticsController.LogVideoRewardShowDone("");
        // GameController.Instance.dataContains.questDatabase.UpdateQuest(QuestEnum.Watch_X_Video, 1);
    }

    private void IronSourceEvents_onRewardedReady()
    {
        // GameController.Instance.AnalyticsController.LogVideoRewardReady();
        // GameController.Instance.dataContains.questDatabase.UpdateQuest(QuestEnum.Watch_X_Video, 1);
    }

    private void IronSourceEvents_onRewardedVideoAdShowFailedEvent(IronSourceError obj)
    {
        onRewaredShowFailed?.Invoke();
    }

    private void IronSourceEvents_onRewardedVideoAdClosedEvent()
    {
        if (reward)
        {
            if (onRewardedClosed != null)
                onRewardedClosed();
            reward = false;
        }
        //InvokeRewardedClosedLater();
    }

    private void IronSourceEvents_onInterstitialAdShowFailedEvent(IronSourceError obj)
    {

        onInterstitialShowFailed?.Invoke();
    }

    private void IronSourceEvents_onInterstitialAdLoadFailedEvent(IronSourceError obj)
    {
        if (reloadInterCoru != null)
        {
            StopCoroutine(reloadInterCoru);
            reloadInterCoru = null;
        }

        reloadInterCoru = Helper.StartAction(() => { loadInterstitial(); }, 0.3f);
        StartCoroutine(reloadInterCoru);
    }

    private void IronSourceEvents_onInterstitialReady()
    {
        if (reloadInterCoru != null)
        {
            StopCoroutine(reloadInterCoru);
            reloadInterCoru = null;
        }
        //GameController.Instance.AnalyticsController.LogInterReady();
    }

    private void IronSourceEvents_onInterstitialAdClosedEvent()
    {
        loadInterstitial();
        onInterstitialClosed?.Invoke();
    }

    public void loadInterstitial()
    {
        if (reloadInterCoru != null)
        {
            StopCoroutine(reloadInterCoru);
            reloadInterCoru = null;
        }

        if (!IronSource.Agent.isInterstitialReady())
            IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitial(UnityAction interstitialShowFailedAction, UnityAction interstitialClosedAction)
    {
        this.onInterstitialClosed = interstitialClosedAction;
        this.onInterstitialShowFailed = interstitialShowFailedAction;
        this.onRewaredShowFailed = null;
        this.onRewardedClosed = null;
        if (!IronSource.Agent.isInterstitialReady())
        {
            this.onInterstitialShowFailed?.Invoke();
        }
        else
        {
            IronSource.Agent.showInterstitial();
        }
    }

    public bool IsRewardedVideoAvailable
    {
        get
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }
    }
    public bool IsInterstitialLoaded
    {
        get
        {
            return IronSource.Agent.isInterstitialReady();
        }
    }

    public bool ShowRewardedVideo(UnityAction rewardedShowFailedAction, UnityAction rewardedClosedAction)
    {
        this.onRewaredShowFailed = rewardedShowFailedAction;
        this.onRewardedClosed = rewardedClosedAction;
        this.onInterstitialClosed = rewardedClosedAction;
        this.onInterstitialShowFailed = rewardedShowFailedAction;
        this.gotRewarded = false;
        if (!IronSource.Agent.isRewardedVideoAvailable())
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                //GameController.Instance.dataContains.questDatabase.UpdateQuest(QuestEnum.Watch_X_Video, 1);
                IronSource.Agent.showInterstitial();
                //GameController.Instance.AnalyticsController.LogVideoRewardEligible();
                return true;
            }
            else
            {
                this.onRewaredShowFailed?.Invoke();
                return false;
            }

        }
        else
        {
            IronSource.Agent.showRewardedVideo();
            //GameController.Instance.AnalyticsController.LogVideoRewardEligible();
            return true;
        }
    }

    public void ShowBanner()
    {
        IronSource.Agent.displayBanner();
    }

    public void HideBanner()
    {
        isShownBanner = false;
        //if (IronSource.Agent.IsBa)
        IronSource.Agent.hideBanner();
    }

    private IEnumerator reloadBannerCoru;
    public void OnBannerLoadFail(IronSourceError err)
    {
        if (reloadBannerCoru != null)
        {
            StopCoroutine(reloadBannerCoru);
            reloadBannerCoru = null;
        }

        reloadBannerCoru = Helper.StartAction(() => { IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM); }, 0.3f);
        StartCoroutine(reloadBannerCoru);
    }

    public void OnBannerLoaded()
    {
        if (reloadBannerCoru != null)
        {
            StopCoroutine(reloadBannerCoru);
            reloadBannerCoru = null;
        }

        ShowBanner();
    }

    //private async void InvokeRewardedClosedLater()
    //{
    //    await Task.Delay(10);
    //    //onRewardedClosed?.Invoke(gotRewarded);  
    //}

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
    {
        double revenue = impressionData.revenue.Value;
        var impressionParameters = new[] {
    new Firebase.Analytics.Parameter("ad_platform", "Level Play"),
    new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
    new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
    new Firebase.Analytics.Parameter("ad_format", impressionData.placement),
    new Firebase.Analytics.Parameter("value", impressionData.revenue.Value),
    new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
};
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        //GameController.Instance.AnalyticsController.LogRevenueDay01((float)revenue);

        Dictionary<string, object> paramas = new Dictionary<string, object>();
        FB.LogPurchase((decimal)revenue, "USD", paramas);
    }
}

