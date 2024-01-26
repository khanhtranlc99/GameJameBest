using Game.GlobalComponent;
using System;
using System.Collections.Generic;
using System.Globalization;
using AppsFlyerSDK;
using Root.Scripts.Helper;
using UITemplate;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public delegate void BuyCallbackMethod(string inAppId, string receipt, bool succeeded);

    private static readonly IDictionary<string, BuyCallbackMethod> OneTimeBuyCallback = new Dictionary<string, BuyCallbackMethod>();

    private static readonly HashSet<string> AlreadyBoughtNonConsumableProducts = new HashSet<string>();

    private static IStoreController storeController;

    private static IExtensionProvider storeExtensionProvider;

    private static IAPManager instance;

    private bool canDestroy;

    public static IAPManager Instance => instance;

    public static void BuyIAP(string itemId, BuyCallbackMethod callback)
    {
        if (!OneTimeBuyCallback.ContainsKey(itemId))
        {
            OneTimeBuyCallback.Add(itemId, delegate
            {
            });
        }
        IDictionary<string, BuyCallbackMethod> oneTimeBuyCallback;
        IDictionary<string, BuyCallbackMethod> dictionary = oneTimeBuyCallback = OneTimeBuyCallback;
        string key;
        string key2 = key = itemId;
        BuyCallbackMethod a = oneTimeBuyCallback[key];
        dictionary[key2] = (BuyCallbackMethod)Delegate.Combine(a, callback);
        instance.Buy(itemId);
    }

    public static bool IsBought(IAPItem iapItem)
    {
        if (iapItem.IapItemType == IAPItem.IAPItemType.Consumable)
        {
            return false;
        }
        return AlreadyBoughtNonConsumableProducts.Contains(iapItem.Id);
    }

    public void Buy(IAPItem item)
    {
        Buy(item.Id);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        UITemplate.Debug.LogFormat("IAP: OnInitialized: FAILED, {0}", error.ToString());
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        UITemplate.Debug.LogFormat("IAP: Success purchase. id = {0}", e.purchasedProduct.definition.id);
        PurchaseProcessing(e.purchasedProduct, status: true);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product purchasedProduct, PurchaseFailureReason p)
    {
        UITemplate.Debug.LogFormat("IAP: OnPurchaseFailed id = {0}, recipt = {1}, reason = {2}", purchasedProduct.definition.id, purchasedProduct.receipt, p);
        PurchaseProcessing(purchasedProduct, status: false);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        UITemplate.Debug.LogFormat("IAP: OnInitialized: Succeed. Items count = {0}", controller.products.all.Length);
        Product[] all = controller.products.all;
        foreach (Product product in all)
        {
            string storeSpecificId = product.definition.storeSpecificId;
            UITemplate.Debug.LogFormat("IAP item: type = {3}, id = {0}, name = {1}, price = {2}, hasRecipt = {4}", storeSpecificId, product.metadata.localizedTitle, product.metadata.localizedPriceString, product.definition.type.ToString(), product.hasReceipt);
            if (product.hasReceipt && !AlreadyBoughtNonConsumableProducts.Contains(storeSpecificId))
            {
                AlreadyBoughtNonConsumableProducts.Add(storeSpecificId);
            }
            if (product.hasReceipt && storeSpecificId.Equals(IAPController.Items[IAPController.Item.NoAds].Id))
            {
                MainThreadExecuter.Instance.Run(delegate
                {
                    // AdsProfiler.AdsEnabled = false;
                    // AdsManager.CurrentInstance.ToggleBanner(showBanner: false);
                    //AdManager.Instance.ToggleBannerVisibility(false);
                });
            }
            foreach (IAPItem value in IAPController.Items.Values)
            {
                if (storeSpecificId.Equals(value.Id))
                {
                    value.FormattedPrice = product.metadata.localizedPriceString + " " + product.metadata.isoCurrencyCode;
                    break;
                }
            }
        }
        storeController = controller;
        storeExtensionProvider = extensions;
    }

    public void RestorePurchases()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            IAppleExtensions extension = storeExtensionProvider.GetExtension<IAppleExtensions>();
            extension.RestoreTransactions(delegate (bool result)
            {
                UITemplate.Debug.Log("IAP: RestorePurchases for apple result: " + result);
            });
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            canDestroy = false;
            UnityEngine.Object.Destroy(base.gameObject);
            return;
        }
        canDestroy = true;
        instance = this;
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        if (!IsInitialized())
        {
            ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            int num = 0;
            foreach (IAPItem value in IAPController.Items.Values)
            {
                configurationBuilder.AddProduct(value.Id, (value.IapItemType != 0) ? ProductType.NonConsumable : ProductType.Consumable, new IDs
                {
                    {
                        value.Id,
                        "AppleAppStore"
                    },
                    {
                        value.Id,
                        "GooglePlay"
                    }
                });
                num++;
            }
            UnityPurchasing.Initialize(this, configurationBuilder);
        }
    }

    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    private void OnDestroy()
    {
        if (canDestroy)
        {
            instance = null;
        }
    }

    private void Buy(string itemId)
    {
        try
        {
            if (IsInitialized())
            {
                Product product = storeController.products.WithID(itemId);
                if (product != null)
                {
                    string id = product.definition.id;
                    if (product.availableToPurchase && !AlreadyBoughtNonConsumableProducts.Contains(id))
                    {
                        UITemplate.Debug.LogFormat("IAP: Purchasing product asychronously: '{0}'", id);
                        storeController.InitiatePurchase(product);
                    }
                    else if (AlreadyBoughtNonConsumableProducts.Contains(id))
                    {
                        PurchaseProcessing(product, status: true);
                    }
                    else
                    {
                        UITemplate.Debug.Log("IAP: Buy FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
            }
        }
        catch (Exception arg)
        {
            UITemplate.Debug.Log("IAP: Exception during purchase. " + arg);
        }
    }

    private void PurchaseProcessing(Product product, bool status)
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add(AFInAppEvents.CURRENCY, "USD");
        eventValues.Add(AFInAppEvents.REVENUE, product.metadata.localizedPrice.ToString(CultureInfo.InvariantCulture));
        eventValues.Add("af_quantity", "1");
        AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventValues);
        string id = product.definition.id;
        if (status && product.definition.type == ProductType.NonConsumable && !AlreadyBoughtNonConsumableProducts.Contains(id))
        {
            AlreadyBoughtNonConsumableProducts.Add(id);
        }
        if (OneTimeBuyCallback.ContainsKey(id))
        {
            OneTimeBuyCallback[id](id, product.receipt, status);
            OneTimeBuyCallback.Remove(id);
        }
        else
        {
            foreach (IAPItem value in IAPController.Items.Values)
            {
                if (id.Equals(value.Id))
                {
                    value.Callback(product.receipt, status);
                }
            }
        }
    }
}
