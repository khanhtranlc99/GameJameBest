using System.Collections.Generic;
using Root.Scripts.Helper;
using UnityEngine;

public class IAPController : MonoBehaviour
{
    public enum Item
    {
        NoAds,
        Money100,
        Money200,
        Money300,
        Money400,
        Gems1,
        Gems2,
        Gems3,
        Gems4
    }

    public delegate void BuyCallback(string receipt, bool succeeded);

    private static readonly Dictionary<Item, IAPItem> _items;

    public static Dictionary<Item, IAPItem> Items => _items;

    public IAPController()
    {
        _items[Item.NoAds].Callback = delegate (string receipt, bool succeeded)
        {
            if (succeeded)
            {
                // AdsProfiler.AdsEnabled = false;
                // AdsManager.UpdateBanner();
                //AdManager.Instance.ToggleBannerVisibility(true);
            }
        };
    }

    static IAPController()
    {
        _items = new Dictionary<Item, IAPItem>();
        _items.Add(Item.NoAds, new IAPItem("com.iap.adsfree", IAPItem.IAPItemType.Durable));
        _items.Add(Item.Gems1, new IAPItem("com.iap.gems1"));
        _items.Add(Item.Gems2, new IAPItem("com.iap.gems2"));
        _items.Add(Item.Gems3, new IAPItem("com.iap.gems3"));
        _items.Add(Item.Gems4, new IAPItem("com.iap.gems4"));
    }

    public static void Buy(Item item)
    {
        IAPManager.Instance.Buy(Items[item]);
    }
}
