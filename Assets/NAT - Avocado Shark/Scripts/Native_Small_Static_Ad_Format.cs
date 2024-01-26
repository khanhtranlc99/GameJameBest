using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AvocadoShark
{
    public class Native_Small_Static_Ad_Format : MonoBehaviour
    {
        public NativeAd nativeAd;
        public GameObject mainPanel;
        private bool nativeAdLoaded;
        private bool isInited;
        private bool disconnectInternet;
        public static float reloadTime = 30f;
        public GameObject Ad_Title;
        public GameObject Ad_Description;
        public GameObject Ad_Calltoactiontext;
        public GameObject Ad_Icon;
        public GameObject Ad_Choices;
        public CanvasGroup mainCanvas;

        AdUnitSettings Units;

        IEnumerator reloadNative;
        System.DateTime lastTimeShow;
        private void Awake()
        {
            //EventDispatcher.EventDispatcher.Instance.RegisterListener(EventID.ON_ADS, OnAds);
            //EventDispatcher.EventDispatcher.Instance.RegisterListener(EventID.OFF_ADS, OffAds);
            //EventDispatcher.EventDispatcher.Instance.RegisterListener(EventID.ON_ADS_CANVAS, OnAdsCanvas);
            //EventDispatcher.EventDispatcher.Instance.RegisterListener(EventID.OFF_ADS_CANVAS, OffAdsCanvas);

            //mainPanel.SetActive(false);
            //lastTimeShow = System.DateTime.Now;
            //StartCoroutine(AutoInit());
        }
        private void OnDestroy()
        {
            //EventDispatcher.EventDispatcher.Instance.RemoveListener(EventID.ON_ADS, OnAds);
            //EventDispatcher.EventDispatcher.Instance.RemoveListener(EventID.OFF_ADS, OffAds);
            //EventDispatcher.EventDispatcher.Instance.RemoveListener(EventID.ON_ADS_CANVAS, OnAdsCanvas);
            //EventDispatcher.EventDispatcher.Instance.RemoveListener(EventID.OFF_ADS_CANVAS, OffAdsCanvas);
        }
        private void OnAdsCanvas(object obj = null)
        {
            this.gameObject.SetActive(true);
        }
        private void OffAdsCanvas(object obj = null)
        {
            this.gameObject.SetActive(false);
        }
        private void OnAds(object obj = null)
        {
            if (!this.gameObject.activeSelf) return;

            if (mainCanvas != null)
            {
                mainCanvas.blocksRaycasts = true;
                mainCanvas.interactable = true;
            }
            SetUpCollider(true);
        }
        private void OffAds(object obj = null)
        {
            if (!this.gameObject.activeSelf) return;

            if (mainCanvas != null)
            {
                mainCanvas.blocksRaycasts = false;
                mainCanvas.interactable = false;
            }
            SetUpCollider(false);
        }
        private void SetUpCollider(bool isActive)
        {
            Ad_Title.GetComponent<BoxCollider>().enabled = isActive;
            Ad_Description.GetComponent<BoxCollider>().enabled = isActive;
            Ad_Calltoactiontext.GetComponent<BoxCollider>().enabled = isActive;
            Ad_Icon.GetComponent<BoxCollider>().enabled = isActive;
            Ad_Choices.GetComponent<BoxCollider>().enabled = isActive;
        }
        private void Init()
        {
            if (NativeAdsManager.Instance != null)
                NativeAdsManager.Instance.InitNativeAds(RequestNativeAd);
            isInited = true;
        }
        WaitForSeconds wait = new WaitForSeconds(5);
        private IEnumerator AutoInit()
        {
            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    yield return wait;
                }
                else
                {
                    if (isInited)
                    {
                        yield break;
                    }
                    else
                    {
                        if (NativeAdsManager.Instance.nativeAdsLoaded)
                        {
                            Init();
                            yield break;
                        }
                    }
                    yield return wait;
                }
            }
        }
        private void RequestNativeAd()
        {
            string adUnit = null;
            Debug.Log("requestedthenate");
            adUnit = NativeAdsManager.ID_NATIVE;
            AdLoader adLoader = new AdLoader.Builder(adUnit)
          .ForNativeAd()
          .Build();
            adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
            adLoader.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            adLoader.LoadAd(new AdRequest.Builder().Build());
        }
        private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            Debug.Log("nateadfailed");
            isInited = false;
            disconnectInternet = true;
            StartCoroutine(AutoInit());
        }
        private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
        {
            Debug.Log("Native ad loaded.");
            this.nativeAd = args.nativeAd;
            nativeAdLoaded = true;
            disconnectInternet = false;
            Register();
        }

        void Register()
        {
            Ad_Icon.GetComponent<RawImage>().texture = this.nativeAd.GetIconTexture();
            Ad_Choices.GetComponent<RawImage>().texture = this.nativeAd.GetAdChoicesLogoTexture();
            if (Ad_Choices.GetComponent<RawImage>().texture == null)
            {
                Ad_Choices.gameObject.SetActive(false);
            }
            else
            {
                Ad_Choices.gameObject.SetActive(true);
            }
            Ad_Title.GetComponent<Text>().text = this.nativeAd.GetHeadlineText();
            Ad_Description.GetComponent<Text>().text = this.nativeAd.GetBodyText();
            Ad_Calltoactiontext.GetComponent<Text>().text = this.nativeAd.GetCallToActionText();

            this.nativeAd.RegisterIconImageGameObject(Ad_Icon);
            this.nativeAd.RegisterHeadlineTextGameObject(Ad_Title);
            this.nativeAd.RegisterBodyTextGameObject(Ad_Description);
            this.nativeAd.RegisterAdChoicesLogoGameObject(Ad_Choices);
            this.nativeAd.RegisterCallToActionGameObject(Ad_Calltoactiontext);

            mainPanel.SetActive(true);

            if (reloadNative != null)
                StopCoroutine(reloadNative);

            reloadNative = Helper.StartAction(() => Refreshad(), reloadTime);
            StartCoroutine(reloadNative);

            lastTimeShow = System.DateTime.Now;
        }
        void Refreshad()
        {
            RequestNativeAd();
        }
        void OnEnable()
        {
            if (Gplay.TimeManager.TimeManager.CaculateTime(lastTimeShow, System.DateTime.Now) >= reloadTime || disconnectInternet)
            {
                Refreshad();
            }
            else
            {
                if (reloadNative != null)
                    StopCoroutine(reloadNative);

                reloadNative = Helper.StartAction(() => Refreshad(), reloadTime - Gplay.TimeManager.TimeManager.CaculateTime(lastTimeShow, System.DateTime.Now));
                StartCoroutine(reloadNative);
            }
        }
        private void OnDisable()
        {
            if (reloadNative != null)
                StopCoroutine(reloadNative);
            StopAllCoroutines();
        }
    }
}

