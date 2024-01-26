using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Firebase.Analytics;
using Firebase;
using Facebook.Unity;
using Firebase.Analytics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using AppsFlyerSDK;
using Falcon;

public class AnalyticsController : MonoBehaviour
{
    #region Init
    static UnityEvent onFinishFirebaseInit = new UnityEvent();
    private static bool m_firebaseInitialized = false;
    public static bool firebaseInitialized
    {
        get
        {
            return m_firebaseInitialized;
        }
        set
        {
            m_firebaseInitialized = value;
            if (value == true)
            {
                if (onFinishFirebaseInit != null)
                {
                    onFinishFirebaseInit.Invoke();
                    onFinishFirebaseInit.RemoveAllListeners();
                }

                //SetUserProperties();
            }
        }
    }
    #endregion

    private static void LogBuyInappAdjust(string inappID, string trancstionID)
    {

    }

    public static void LogEventFirebase(string eventName, Parameter[] parameters)
    {

        if (firebaseInitialized)
        {

            FirebaseAnalytics.LogEvent(eventName, parameters);
        }
        else
        {
            onFinishFirebaseInit.AddListener(() =>
            {
                FirebaseAnalytics.LogEvent(eventName, parameters);
            });
        }
    }

    public static void LogEventFacebook(string eventName, Dictionary<string, object> parameters)
    {
        if (FB.IsInitialized)
        {
#if !ENV_PROD
            parameters["test"] = true;
#endif
            FB.LogAppEvent(eventName, null, parameters);
        }
    }

    public static void SetUserProperties()
    {
        if (!firebaseInitialized) return;

        FirebaseAnalytics.SetUserProperty(StringHelper.RETENTION_D, UseProfile.RetentionD.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.DAYS_PLAYED, UseProfile.DaysPlayed.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.PAYING_TYPE, UseProfile.PayingType.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.LEVEL, UseProfile.CurrentLevel.ToString());
    }

    #region Event
    public void LogWatchVideo(ActionWatchVideo action, bool isHasVideo, bool isHasInternet, string level)
    {
        if (!firebaseInitialized) return;
        //Parameter[] parameters = new Parameter[4]
        //{
        //    new Parameter("actionWatch", action.ToString()) ,
        //     new Parameter("has_ads", isHasVideo.ToString()) ,
        //      new Parameter("has_internet", isHasInternet.ToString()) ,
        //       new Parameter("level", level)
        //};

        //FirebaseAnalytics.LogEvent("watch_video_game", parameters);
    }

    public void LogWatchInter(string action, bool isHasVideo, bool isHasInternet, string level)
    {
        if (!firebaseInitialized) return;
        //Parameter[] parameters = new Parameter[4]
        //{
        //    new Parameter("actionWatch", action.ToString()) ,
        //     new Parameter("has_ads", isHasVideo.ToString()) ,
        //      new Parameter("has_internet", isHasInternet.ToString()) ,
        //      new Parameter("level", level)
        //};

        //FirebaseAnalytics.LogEvent("show_inter", parameters);
    }

    public static void LogBuyInapp(string inappID, string trancstionID)
    {
        try
        {
            LogBuyInappAdjust(inappID, trancstionID);
        }
        catch
        {

        }
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                new Parameter("id", inappID),
                };
                LogEventFirebase("inapp_event", parameters);
            }
        }
        catch
        {

        }
    }

    public void LogStartLevel(int level)
    {
        try
        {
            if (!firebaseInitialized) return;

            Parameter[] parameters = new Parameter[1]
            {
            new Parameter("level", level.ToString())
            };


            FirebaseAnalytics.LogEvent("level_start", parameters);
        }
        catch
        {

        }
    }

    public void LogLevelFail(int level)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[1]
       {
            new Parameter("level", level.ToString())
       };


        FirebaseAnalytics.LogEvent("level_fail", parameters);
    }

    public void LogRequestVideoReward(string placement)
    {
        try
        {
            //if (firebaseInitialized)
            //{
            //    Parameter[] parameters = new Parameter[1]
            //   {
            //new Parameter("placement", placement.ToString())
            //   };


            //    FirebaseAnalytics.LogEvent("ads_reward_offer", parameters);
            //}
        }
        catch
        {

        }
    }

    public void LogVideoRewardEligible()
    {
        try
        {
            Dictionary<string, string> paramas = new Dictionary<string, string>();
            AppsFlyer.sendEvent("af_rewarded_ad_eligible", paramas);
        }
        catch
        {

        }
    }

    public void LogClickToVideoReward(string placement)
    {
        // if (!firebaseInitialized) return;
        // Parameter[] parameters = new Parameter[1]
        //{
        //     new Parameter("placement", placement.ToString())
        //};


        // FirebaseAnalytics.LogEvent("ads_reward_click", parameters);
    }

    public void LogVideoRewardShow(string placement)
    {
        try
        {
            //if (firebaseInitialized)
            //{
            //    Parameter[] parameters = new Parameter[1]
            //   {
            //new Parameter("placement", placement.ToString())
            //   };


            //    FirebaseAnalytics.LogEvent("ads_reward_show", parameters);
            //}
        }
        catch
        {

        }

        try
        {
            Dictionary<string, string> paramas = new Dictionary<string, string>();
            AppsFlyer.sendEvent("af_rewarded_displayed", paramas);
        }
        catch
        {

        }
    }

    public void LogVideoRewardLoadFail(string placement, string errormsg)
    {
        // if (!firebaseInitialized) return;
        // Parameter[] parameters = new Parameter[2]
        //{
        //     new Parameter("placement", placement.ToString()),
        //     new Parameter("errormsg", errormsg.ToString())
        //};


        // FirebaseAnalytics.LogEvent("ads_reward_fail", parameters);
    }

    public void LogVideoRewardShowDone(string placement)
    {
        try
        {
            //if (firebaseInitialized)
            //{
            //    Parameter[] parameters = new Parameter[1]
            //   {
            //new Parameter("placement", placement.ToString()),
            //   };


            //    FirebaseAnalytics.LogEvent("ads_reward_complete", parameters);
            //}
        }
        catch
        {

        }

        try
        {
            Dictionary<string, string> paramas = new Dictionary<string, string>();
            AppsFlyer.sendEvent("af_rewarded_ad_completed", paramas);
        }
        catch
        {

        }

        try
        {
            //DWHLog.Log.AdsLog("", AdsType.VideoReward.ToString(), AdmobAds.RewardedAdUnitId.ToString(), "", "", placement, StateAds.Rewarded.ToString());
            // DWHLog.Log.AdsLog(UseProfile.CurrentLevel, AdType.reward, placement);
        }
        catch
        {

        }
    }

    public void LogInterLoadFail(string errormsg)
    {
        // if (!firebaseInitialized) return;
        // Parameter[] parameters = new Parameter[1]
        //{
        //     new Parameter("errormsg", errormsg.ToString())
        //};


        // FirebaseAnalytics.LogEvent("ad_inter_fail", parameters);
    }

    public void LogInterLoad()
    {
        try
        {
            //if (firebaseInitialized)
            //    FirebaseAnalytics.LogEvent("ad_inter_load");
        }
        catch
        {

        }


    }

    public void LoadInterEligible()
    {
        try
        {
            Dictionary<string, string> paramas = new Dictionary<string, string>();
            AppsFlyer.sendEvent("af_inters_ad_eligible", paramas);
        }
        catch
        {

        }
    }

    public void LogInterShow(string actionWatchLog = "other")
    {
        try
        {
            //if (firebaseInitialized)
            //    FirebaseAnalytics.LogEvent("ad_inter_show");

        }
        catch
        {

        }

        try
        {
            Dictionary<string, string> paramas = new Dictionary<string, string>();
            AppsFlyer.sendEvent("af_inters_displayed", paramas);
        }
        catch
        {

        }

        try
        {
            //DWHLog.Log.AdsLog("", AdsType.Interstitial.ToString(), AdmobAds.InterstitialAdUnitId.ToString(), "", "", actionWatchLog, StateAds.Show.ToString());
            //   DWHLog.Log.AdsLog(UseProfile.CurrentLevel, AdType.interstitial, actionWatchLog);
        }
        catch
        {

        }
    }

    public void LogInterClick()
    {
        //if (!firebaseInitialized) return;
        //FirebaseAnalytics.LogEvent("ad_inter_click");
    }

    public void LogInterReady()
    {
        try
        {
            Dictionary<string, string> paramas = new Dictionary<string, string>();
            AppsFlyer.sendEvent("af_inters_api_called", paramas);
        }
        catch
        {

        }
    }

    public void LogVideoRewardReady()
    {
        try
        {
            Dictionary<string, string> paramas = new Dictionary<string, string>();
            AppsFlyer.sendEvent("af_rewarded_api_called", paramas);
        }
        catch
        {

        }
    }

    public void LogTutLevelStart(int level)
    {
        try
        {
            if (firebaseInitialized)
                FirebaseAnalytics.LogEvent(string.Format("tutorial_start_{0}", level));

        }
        catch
        {

        }
    }

    public void LogTutLevelEnd(int level)
    {
        try
        {
            if (firebaseInitialized)
                FirebaseAnalytics.LogEvent(string.Format("tutorial_end_{0}", level));

        }
        catch
        {

        }
        try
        {
            Dictionary<string, string> paramas = new Dictionary<string, string>();
            paramas.Add("af_success", level.ToString());
            paramas.Add("af_tutorial_id", level.ToString());
            AppsFlyer.sendEvent("af_tutorial_completion", paramas);
        }
        catch
        {

        }
    }

    public void LogRevenueDay01(float revenue)
    {
        if (UseProfile.RetentionD <= 1)
        {
            UseProfile.RevenueD0_D1 += revenue;
            int cents = (int)(UseProfile.RevenueD0_D1 * 100);
            if (cents >= 1 && cents <= 60)
            {
                if (cents != UseProfile.PreviousRevenueD0_D1)
                {
                    UseProfile.PreviousRevenueD0_D1 = cents;
                    try
                    {
                        if (firebaseInitialized)
                        {
                            Debug.LogError("firebase log d0 revenue " + cents);
                            Firebase.Analytics.FirebaseAnalytics.LogEvent(string.Format("RevD0_{1}_Cents", cents));
                        }
                    }
                    catch
                    { }
                    try
                    {
                        Debug.LogError("appflyers log d0 revenue " + cents);
                        Dictionary<string, string> paramas = new Dictionary<string, string>();
                        AppsFlyer.sendEvent(string.Format("RevD0+D1_{1}_Cents", cents), paramas);
                    }
                    catch
                    {
                        Debug.LogError("facebook log d0 revenue " + cents);
                        FB.LogAppEvent(string.Format("RevD0+D1_{1}_Cents", cents));
                    }
                }
            }
        }
    }
    public void LogDisplayedInterstitialDay01()
    {
        if (UseProfile.RetentionD <= 1)
        {
            if (UseProfile.NumberOfDisplayedInterstitialD0_D1 >= 3 && UseProfile.NumberOfDisplayedInterstitialD0_D1 <= 30)
            {
                try
                {
                    if (firebaseInitialized)
                    {
                        Firebase.Analytics.FirebaseAnalytics.LogEvent(string.Format("D0+D1_{0}_Interstitals", UseProfile.NumberOfDisplayedInterstitialD0_D1));
                        Debug.LogError("firebase log D0+D1 interstitials " + UseProfile.NumberOfDisplayedInterstitialD0_D1);
                    }
                }
                catch
                { }
                try
                {
                    Debug.LogError("facebook log D0+D1 interstitials " + UseProfile.NumberOfDisplayedInterstitialD0_D1);
                    Dictionary<string, string> paramas = new Dictionary<string, string>();
                    AppsFlyer.sendEvent(string.Format("D0+D1_{0}_Interstitals", UseProfile.NumberOfDisplayedInterstitialD0_D1), paramas);
                }
                catch
                { }
                try
                {
                    Debug.LogError("facebook log D0+D1 interstitials " + UseProfile.NumberOfDisplayedInterstitialD0_D1);
                    FB.LogAppEvent(string.Format("D0+D1_{0}_Interstitals", UseProfile.NumberOfDisplayedInterstitialD0_D1));
                }
                catch { }
            }
        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        SetUserProperties();
    }
}

public enum ActionClick
{
    None = 0,
    Play = 1,
    Rate = 2,
    Share = 3,
    Policy = 4,
    Feedback = 5,
    Term = 6,
    NoAds = 10,
    Settings = 11,
    ReplayLevel = 12,
    SkipLevel = 13,
    Return = 14,
    BuyStand = 15
}

public enum ActionWatchVideo
{
    None = 0,
    Skip_level = 1,
    Return = 2,
    BuyStand = 3,
    BuyExtral = 4,
    ClaimSkin = 5,
    BuyTurnMiniGame = 6,
    ResetShop = 7
}

public enum ActionShowInter
{
    None = 0,
    Skip_level = 1,
    Return = 2,
    BuyStand = 3,

    EndGame = 4,
    Click_Setting = 5,
    Click_Replay = 6
}
