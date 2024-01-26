
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif


public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public UseProfile useProfile;
    //public AnalyticsController AnalyticsController;
    [HideInInspector] public SceneType currentScene;

    protected void Awake()
    {
        Instance = this;
        Init();
        DontDestroyOnLoad(this);

        //GameController.Instance.useProfile.IsRemoveAds = true;


#if UNITY_IOS

    if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == 
    ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
    {

        ATTrackingStatusBinding.RequestAuthorizationTracking();

    }

#endif

    }

    public void Init()
    {

        UseProfile.NumberOfAdsInPlay = 0;
        //Application.targetFrameRate = 60;

        //useProfile.IsRemoveAds = true;
        useProfile.CurrentLevelPlay = UseProfile.CurrentLevel;
        // GameController.Instance.admobAds.ShowBanner();
    }
    public static void SetUserProperties()
    {
        if (UseProfile.IsFirstTimeInstall)
        {
            UseProfile.FirstTimeOpenGame = UnbiasedTime.Instance.Now();
            UseProfile.LastTimeOpenGame = UseProfile.FirstTimeOpenGame;
            UseProfile.IsFirstTimeInstall = false;
        }

        var lastTimeOpen = UseProfile.LastTimeOpenGame;
        UseProfile.RetentionD = (UnbiasedTime.Instance.Now() - UseProfile.FirstTimeOpenGame).Days;

        var dayPlayerd = (Gplay.TimeManager.TimeManager.ParseTimeStartDay(UnbiasedTime.Instance.Now()) - Gplay.TimeManager.TimeManager.ParseTimeStartDay(UseProfile.LastTimeOpenGame)).Days;
        if (dayPlayerd >= 1)
        {
            UseProfile.LastTimeOpenGame = UnbiasedTime.Instance.Now();
            UseProfile.DaysPlayed++;
        }

        //AnalyticsController.SetUserProperties();
    }
}
public enum SceneType
{
    StartLoading = 0,
    MainHome = 1,
    GamePlay = 2
}