using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AppOpenAdLauncher : SingletonMonoBehavior<AppOpenAdLauncher>
{

    protected override void Awake()
    {
        base.Awake();
#if !UNITY_EDITOR
        MobileAds.Initialize(status =>
        {
            AppOpenAdManager.Instance.LoadAd();
            NativeAdsManager.Instance.OnInitAdmobDone();
        });
#endif
    }
    private void OnApplicationPause(bool pause)
    {
#if !UNITY_EDITOR
        if (!pause && AppOpenAdManager.ConfigResumeApp && !AppOpenAdManager.ResumeFromAds)
        {
            AppOpenAdManager.Instance.ShowAdIfAvailable();
            return;
        }

        if (!pause && AppOpenAdManager.ResumeFromAds)
        {
            AppOpenAdManager.ResumeFromAds = false;
        }
#endif
    }
}