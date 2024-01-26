using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
public class NativeAdsManager : MonoBehaviour
{
    //ca-app-pub-3926209354441959/6378012994
    //test
    //ca-app-pub-3940256099942544/2247696110 
    public const string ID_NATIVE = "ca-app-pub-3926209354441959/6378012994";
    public bool nativeAdsLoaded;
    private Queue<UnityAction> actionRequestSuccess = new Queue<UnityAction>();

    private static NativeAdsManager m_Instance;
    public static NativeAdsManager Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = GameObject.FindObjectOfType<NativeAdsManager>();

            return m_Instance;
        }
    }
    public void OnInitAdmobDone()
    {
        nativeAdsLoaded = true;
        ShowNative();
    }
    private void ShowNative()
    {
        if (actionRequestSuccess.Count > 0)
        {
            actionRequestSuccess.Dequeue()?.Invoke();
            ShowNative();
        }
    }
    public void InitNativeAds(UnityAction action)
    {
        if (nativeAdsLoaded)
        {
            action?.Invoke();
        }
        else
        {
            actionRequestSuccess.Enqueue(action);
        }
    }
}