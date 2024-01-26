using Game.Character;
using Game.Shop;
using System;
using UnityEngine;

public class IAPMiamiManager : MonoBehaviour
{
    [Serializable]
    public class GemPack
    {
        public IAPController.Item Item;

        public Sprite Icon;

        public int GemValue;
    }

    public int SpToAdd;

    public int MoneyToAdd;

    [Space(10f)]
    public Sprite FreeGemsIcon;

    public GemPack[] GemPacks;

    private void Start()
    {
        //IAPController.Items[IAPController.Item.Money100].Callback = delegate (string receipt, bool succeeded)
        //{
        //    if (succeeded)
        //    {
        //        PlayerInfoManager.Money += GetPack(IAPController.Item.Money100).GemValue;
        //    }
        //};
        //IAPController.Items[IAPController.Item.Money200].Callback = delegate (string receipt, bool succeeded)
        //{
        //    if (succeeded)
        //    {
        //        PlayerInfoManager.Money += GetPack(IAPController.Item.Money200).GemValue;
        //    }
        //};
        //IAPController.Items[IAPController.Item.Money300].Callback = delegate (string receipt, bool succeeded)
        //{
        //    if (succeeded)
        //    {
        //        PlayerInfoManager.Money += GetPack(IAPController.Item.Money300).GemValue;
        //    }
        //};
        //IAPController.Items[IAPController.Item.Money400].Callback = delegate (string receipt, bool succeeded)
        //{
        //    if (succeeded)
        //    {
        //        PlayerInfoManager.Money += GetPack(IAPController.Item.Money400).GemValue;
        //    }
        //};
        ///
        IAPController.Items[IAPController.Item.Gems1].Callback = delegate (string receipt, bool succeeded)
        {
            if (succeeded)
            {
                PlayerInfoManager.Gems += GetPack(IAPController.Item.Gems1).GemValue;
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.UpdateInfo();
                }
            }
        };
        IAPController.Items[IAPController.Item.Gems2].Callback = delegate (string receipt, bool succeeded)
        {
            if (succeeded)
            {
                PlayerInfoManager.Gems += GetPack(IAPController.Item.Gems2).GemValue;
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.UpdateInfo();
                }
            }
        };
        IAPController.Items[IAPController.Item.Gems3].Callback = delegate (string receipt, bool succeeded)
        {
            if (succeeded)
            {
                PlayerInfoManager.Gems += GetPack(IAPController.Item.Gems3).GemValue;
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.UpdateInfo();
                }
            }
        };
        IAPController.Items[IAPController.Item.Gems4].Callback = delegate (string receipt, bool succeeded)
        {
            if (succeeded)
            {
                PlayerInfoManager.Gems += GetPack(IAPController.Item.Gems4).GemValue;
                if (ShopManager.Instance != null)
                {
                    ShopManager.Instance.UpdateInfo();
                }
            }
        };
    }

    public GemPack GetPack(IAPController.Item item)
    {
        for (int i = 0; i < GemPacks.Length; i++)
        {
            if (GemPacks[i].Item == item)
            {
                return GemPacks[i];
            }
        }
        return null;
    }
}
