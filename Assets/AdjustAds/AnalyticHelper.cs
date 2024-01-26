using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;
using UnityEngine;
using Firebase.Analytics;
using Root.Scripts;

public class AnalyticHelper
{
    public static void LogEvent(string eventName)
    {
        //if(!FirebaseManager.Instance.isFirebaseInitialized)
        return;
#if !UNITY_EDITOR
#if FIRE_BASE
        try
        {
            FirebaseAnalytics.LogEvent(eventName);
        if (eventName.Contains("click_"))
        {
            SetUserProperty(TrackingConstants.USER_PROPERTY_LAST_FEATURE, eventName);
        }
        }
        catch (System.Exception e)
        {
            //FunctionHelper.ShowDebug(e);
        }
#endif
#endif
        //FunctionHelper.ShowDebug("log firebase :"+ eventName);
    }
    public static void LogEvent(string eventName, string paramName, string paramValue)
    {
        //if(!FirebaseManager.Instance.isFirebaseInitialized)
        return;
#if !UNITY_EDITOR
// #if FIRE_BASE
        try
        {
           FirebaseAnalytics.LogEvent(eventName, paramName, paramValue);
        }
        catch (System.Exception e)
        {
            //FunctionHelper.ShowDebug(e);
        }
//#endif
#endif
        Debug.Log("LogEvent: " + eventName + "\nParam: " + paramName + " | " + paramValue);
    }
    public static void LogEvent(string eventName, string paramName1, string paramValue1, string paramName2, int paramValue2)
    {
        // if(!FirebaseManager.Instance.isFirebaseInitialized)
        return;
#if !UNITY_EDITOR
#if FIRE_BASE
        try
        {
        Parameter[] param =
        {
            new Parameter(paramName1, paramValue1),
            new Parameter(paramName2, paramValue2)
        };
           FirebaseAnalytics.LogEvent(eventName, param);
        }
        catch (System.Exception e)
        {
           // FunctionHelper.ShowDebug(e);
        }
#endif
#endif

        UnityEngine.Debug.Log("LogEvent: " + eventName
                                           + "\nParam: " + paramName1 + " | " + paramValue1
                                           + "\nParam: " + paramName2 + " | " + paramValue2);
    }
    public static void LogEvent(string eventName, string paramName1, string paramValue1, string paramName2, int paramValue2, string paramName3, int paramValue3)
    {
        //if(!FirebaseManager.Instance.isFirebaseInitialized)
        return;
#if !UNITY_EDITOR
#if FIRE_BASE
        try
        {
        Parameter[] param =
        {
            new Parameter(paramName1, paramValue1),
            new Parameter(paramName2, paramValue2),
            new Parameter(paramName3, paramValue3)
        };
           FirebaseAnalytics.LogEvent(eventName, param);
        }
        catch (System.Exception e)
        {
            //FunctionHelper.ShowDebug(e);
        }
#endif
#endif

        UnityEngine.Debug.Log("LogEvent: " + eventName
                                           + "\nParam: " + paramName1 + " | " + paramValue1
                                           + "\nParam: " + paramName2 + " | " + paramValue2);
    }

    public static void LogTimePlay(int totalSecondPlay)
    {
        var totalMinute = 0;
        string keyLog = string.Empty;
        if (totalSecondPlay < 300)
        {
            totalMinute = 5;
            keyLog = TrackingConstants.PLAY_5_MINUTES;
        }
        else if (totalSecondPlay < 600)
        {
            totalMinute = 10;
            keyLog = TrackingConstants.PLAY_10_MINUTES;
        }
        else if (totalSecondPlay < 900)
        {
            totalMinute = 15;
            keyLog = TrackingConstants.PLAY_15_MINUTES;
        }
        else if (totalSecondPlay < 1200)
        {
            totalMinute = 20;
            keyLog = TrackingConstants.PLAY_20_MINUTES;
        }
        else
        {
            totalMinute = 30;
            keyLog = TrackingConstants.PLAY_30_MINUTES;
        }

        var key = "KEY_SAVE_LOG_TIME_PLAY_BY_MINUTES_CHECKPOINT" + totalMinute;
        if (PlayerPrefs.HasKey(key))
            return;
        PlayerPrefs.SetInt(key, 1);
        LogEvent(keyLog);
    }
    public static void LogEventShowReward(bool isRewardAds, bool isHadAds, string placement)
    {
        // #if DEBUG_BUILD
        //         LogAdCount.Instance.SetSourceText(placement);
        // #endif
        //if(!FirebaseManager.Instance.isFirebaseInitialized)
        return;
#if !UNITY_EDITOR
#if FIRE_BASE
var isHaveInternetConnection = Application.internetReachability != NetworkReachability.NotReachable;
        try
        {
        Parameter[] param =
        {
            new Parameter(TrackingConstants.PARAM_HAS_ADS, isHadAds.ToString()),
            new Parameter(TrackingConstants.PARAM_INTERNET_AVAILABLE, isHaveInternetConnection.ToString()),
            new Parameter(TrackingConstants.PARAM_PLACEMENT, placement)
        };
           FirebaseAnalytics.LogEvent(isRewardAds ? TrackingConstants.SHOW_REWARD : TrackingConstants.SHOW_INTER, param);
        }
        catch (System.Exception e)
        {
            //FunctionHelper.ShowDebug(e);
        }
#endif
#endif

        // UnityEngine.Debug.Log("LogEvent: " + eventName
        //                                    + "\nParam: " + paramName1 + " | " + paramValue1
        //                                    + "\nParam: " + paramName2 + " | " + paramValue2);
    }

    public static void LogImpressAds(bool isReward)
    {
        if (isReward)
        {
            var counterTimeWatchReward = PlayerPrefs.GetInt("KEY_COUNTER_TIME_WATCH_REWARD_ADS_IN_LIFE", 0);
            counterTimeWatchReward++;
            PlayerPrefs.SetInt("KEY_COUNTER_TIME_WATCH_REWARD_ADS_IN_LIFE", counterTimeWatchReward);
            LogEvent(TrackingConstants.IMPRESS_REWARD, TrackingConstants.PARAM_IMP_PASS, counterTimeWatchReward);
            //AppsFlyer.sendEvent(AFInAppEvents.PARAM_2,null);
            //AdjustSdkManager.Instance.TrackSimpleEvent(AdjustEventConstants.impress_reward,TrackingConstants.PARAM_IMP_PASS,counterTimeWatchReward);
        }
        else
        {
            var counterTimeWatchInter = PlayerPrefs.GetInt("KEY_COUNTER_TIME_WATCH_INTER_ADS_IN_LIFE", 0);
            counterTimeWatchInter++;
            PlayerPrefs.SetInt("KEY_COUNTER_TIME_WATCH_INTER_ADS_IN_LIFE", counterTimeWatchInter);
            LogEvent(TrackingConstants.IMPRESS_INTER, TrackingConstants.PARAM_IMP_PASS, counterTimeWatchInter);
            //AdjustSdkManager.Instance.TrackSimpleEvent(AdjustEventConstants.impress_inter,TrackingConstants.PARAM_IMP_PASS,counterTimeWatchInter);
        }
    }
    public static void LogEvent(string eventName, string paramName, int paramValue)
    {
        // if(!FirebaseManager.Instance.isFirebaseInitialized)
        return;
#if !UNITY_EDITOR
#if FIRE_BASE
        try
        {
            FirebaseAnalytics.LogEvent(eventName, paramName, paramValue);
        }
        catch (System.Exception e)
        {
            //FunctionHelper.ShowDebug(e);
        }
#endif
#endif
        UnityEngine.Debug.Log("LogEvent: " + eventName + "\nParam: " + paramName + " | " + paramValue);
    }

    public static void SetUserProperty(string name, string property)
    {
        //if(!FirebaseManager.Instance.isFirebaseInitialized)
        return;
#if !UNITY_EDITOR

        FirebaseAnalytics.SetUserProperty(name, property);
#endif
        Debug.Log(string.Format("SetUserProperty_{0}_{1}", name, property));
    }

}
