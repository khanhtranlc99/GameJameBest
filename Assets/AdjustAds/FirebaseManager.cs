using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Truongtv.Utilities;
using System.Threading.Tasks;

using Firebase.Extensions;
using Firebase.RemoteConfig;

using UnityEngine;

public class FirebaseManager : SingletonMonoBehavior<FirebaseManager>
{

    public bool isFirebaseInitialized;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    void Start()
    {
        StartCoroutine(I_InitFirebase());
    }
    private IEnumerator I_InitFirebase()
    {
        while (!isFirebaseInitialized)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    //InitializeFirebase();
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    //   app = Firebase.FirebaseApp.DefaultInstance;
                    InitRemoteConfig();
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    // Dictionary<string, object> defaults =
                    //     new Dictionary<string, object>();
                    // FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
                    //     .ContinueWithOnMainThread(task => {
                    //         // [END set_defaults]
                    //         isFirebaseInitialized = true;
                    //     });
                    isFirebaseInitialized = true;
                }
                else
                {
                    UnityEngine.Debug.LogError(string.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                }
            });
            yield return new WaitForSeconds(10f);
        }
    }
        private void InitRemoteConfig()
    {
        Dictionary<string, object> defaults =
new Dictionary<string, object>();
        
        // These are the values that are used if we haven't fetched data from the
        // service yet, or if we ask for values that the service doesn't have:
        //defaults.Add(StringHelper.REMOTE_PAUSE_AD, DataConfig.INTERVAL_SHOW_AD_PAUSE);
        // defaults.Add(StringHelper.REMOTE_WIN_AD, DataConfig.INTERVAL_SHOW_AD_WIN);
        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
        FetchDataAsync();
    }
        public Task FetchDataAsync()
    {
        // FetchAsync only fetches new data if the current data is older than the provided
        // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
        // By default the timespan is 12 hours, and for production apps, this is a good
        // number.  For this example though, it's set to a timespan of zero, so that
        // changes in the console will always show up immediately.
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    void FetchComplete(Task fetchTask)
    {
        isFirebaseInitialized = true;
        if (fetchTask.IsCanceled)
        {
            //  DebugLog("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
              Debug.Log("Fetch encountered an error.");
            //FunctionHelper.ShowDebug("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
             Debug.Log("Fetch completed successfully!");
            //FunctionHelper.ShowDebug("Fetch completed successfully!");

        }
        var info = FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case LastFetchStatus.Success:
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                
                Debug.Log(string.Format("Remote data loaded and ready (last fetch time {0}).",
                                       info.FetchTime));
                break;
            case LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
        //intervalShowAdPause = (int)FirebaseRemoteConfig.GetValue(StringHelper.REMOTE_PAUSE_AD).LongValue;
        //intervalShowAdWin = (int)FirebaseRemoteConfig.GetValue(StringHelper.REMOTE_WIN_AD).LongValue;
    }
//     private void OnApplicationPause(bool pause)
//     {
//         // string failedEvents = JsonHelper.ListToJson(mFailedEvents);
//         // PlayerPrefs.SetString(FAILED_EVENTS, failedEvents);
// #if  UNITY_EDITOR
//         return;
// #endif
//         if (pause)
//         {
//             var idLvl = DataManager.instance.userProfile.GetRegionLevel();
//             AnalyticHelper.SetUserProperty(TrackingConstants.USER_PROPERTY_MAX_LEVEL,  idLvl.ToString());
//             AnalyticHelper.SetUserProperty(TrackingConstants.USER_PROPERTY_TIME_PLAY,  DataManager.instance.userProfile.CounterPlayTime().ToString());
//         }
//     }

//     private void OnApplicationQuit()
//     {
//         // string failedEvents = JsonHelper.ListToJson(mFailedEvents);
//         // PlayerPrefs.SetString(FAILED_EVENTS, failedEvents);
//         //     
// #if  UNITY_EDITOR
//         return;
// #endif
//         var idLvl = DataManager.instance.userProfile.GetRegionLevel();
//         AnalyticHelper.SetUserProperty(TrackingConstants.USER_PROPERTY_MAX_LEVEL,  idLvl.ToString());
//         AnalyticHelper.SetUserProperty(TrackingConstants.USER_PROPERTY_TIME_PLAY,  DataManager.instance.userProfile.CounterPlayTime().ToString());
//     }


}
