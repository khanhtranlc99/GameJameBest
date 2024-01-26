using System;
using Game.Character;
using Game.Character.Superpowers;
using Game.GlobalComponent.Qwest;
using Game.Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RopeHeroViceCity.Scripts.Gampelay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using UnityEngine.Events;

namespace Game.Shop
{
    public class ShopManager : SingletonMonoBehavior<ShopManager>
    {
        public delegate void ShopOpened();

        public delegate void ShopClosed();

        private const string BIKeysArrayName = "BoughtItemsKeys";

        private const string BIValuesArrayName = "BoughtItemsValues";

        private const string GemsPrefix = "Gems";

        public bool loadStuffDone;
        public bool ShowDebug;

        [Space(10f)]
        public ShopLinks Links;

        public Sprite isChoosing;
        public Sprite isUnChoose;

        private ShopIcons ShopIcons;


        [Space(10f)]
        public GameObject BlankElement;

        public GameObject BlankCategory;

        public GameObject BlankContainer;

        [Space(10f)]
        public GameObject BuyGemsPanel;

        [Space(10f)]
        public ShopCategory activeCategory;

        public ShopItem currentItem;

        // public ShopOpened ShopOpeningEvent;
        //
        // public ShopClosed ShopCloseningEvent;

        [Space(10f)]
        public Text NeedMoneyText;

        public Text MoneyText;

        public Text GemText;

        public GameItemBonus ExchangeItem;
        public int idExchangeItem;

        private bool inited;

        public bool Inited => inited;

        private bool selected;

        private ShopDialogPanel currDialogPanel;

        public Dictionary<ItemsTypes, Dictionary<ShopCategory, List<ShopItem>>> ShopStuff = new Dictionary<ItemsTypes, Dictionary<ShopCategory, List<ShopItem>>>();

        private int gems;

        private int money;


        public static bool IsOpen => Instance.Links.Categories.activeInHierarchy;


        public void Init()
        {
            if (!inited)
            {

                PlayerAbilityManager.LoadAbilities();
                PlayerAbilityManager.EnableEbilities();
                Links.InfoPanelManager.Init();
                SalesManager.Instance.Init();
                ShopIcons = ResourcesManager.Instance.ShopIcons;
                FillShopStuffDictionary();
                if (ExchangeItem == null)
                    ExchangeItem = (GameItemBonus)ItemsManager.Instance.GetItem(idExchangeItem);
                PlayerInfoManager.Instance.onBoughtItemDone.AddListener(UpdateInfo);
                PlayerInfoManager.Instance.onEqipItem += Equip;
                inited = true;
            }
        }

        private void OnDestroy()
        {
            PlayerInfoManager.Instance.onBoughtItemDone.RemoveListener(UpdateInfo);
            PlayerInfoManager.Instance.onEqipItem -= Equip;
        }

        public T GetShopItemByType<T>(ItemsTypes itemType, object[] parametrs) where T : GameItem
        {
            ShopCategory category;
            ShopItem item;
            return GetShopItemByType<T>(itemType, parametrs, out category, out item);
        }

        public T GetShopItemByType<T>(ItemsTypes itemType, object[] parametrs, out ShopCategory category, out ShopItem item) where T : GameItem
        {
            category = null;
            item = null;
            foreach (KeyValuePair<ShopCategory, List<ShopItem>> item2 in ShopStuff[itemType])
            {
                foreach (ShopItem item3 in item2.Value)
                {
                    T val = item3.GameItem as T;
                    if ((bool)(Object)val && item3.GameItem.SameParametrWithOther(parametrs))
                    {
                        category = item2.Key;
                        item = item3;
                        return val;
                    }
                }
            }
            return (T)null;
        }

        public List<ShopCategory> GetShopCategores()
        {
            List<ShopCategory> list = new List<ShopCategory>();
            foreach (KeyValuePair<ItemsTypes, Dictionary<ShopCategory, List<ShopItem>>> item in ShopStuff)
            {
                foreach (KeyValuePair<ShopCategory, List<ShopItem>> item2 in item.Value)
                {
                    list.Add(item2.Key);
                }
            }
            return list;
        }

        public bool GetShopItem(int id, out ShopItem item, out ShopCategory category)
        {
            category = null;
            item = null;
            foreach (KeyValuePair<ItemsTypes, Dictionary<ShopCategory, List<ShopItem>>> item2 in ShopStuff)
            {
                foreach (KeyValuePair<ShopCategory, List<ShopItem>> item3 in item2.Value)
                {
                    foreach (ShopItem item4 in item3.Value)
                    {
                        if (item4.GameItem.ID == id)
                        {
                            item = item4;
                            category = item3.Key;
                            return true;
                        }
                    }
                }
            }
            return false;
        }



        public void Enable()
        {
            // if (ShopOpeningEvent != null)
            // {
            // 	ShopOpeningEvent();
            // }
            EventManager.Instance.OnShopOpenning();
            ChangeCategory(GetFirstCategory());
        }


        public void Disable()
        {
            // if (ShopCloseningEvent != null)
            // {
            // 	ShopCloseningEvent();
            // }
            EventManager.Instance.OnShopClosing();
        }

        private bool BoughtAlredy(GameItem gameItem)
        {
            return PlayerInfoManager.Instance.BoughtAlredy(gameItem);
        }

        public bool BoughtAlredy(int gameItemID)
        {
            return BoughtAlredy(ItemsManager.Instance.GetItem(gameItemID));
        }

        public void ChangeCategory(ShopCategory category)
        {
            if (activeCategory != null)
            {
                activeCategory.Container.SetActive(value: false);
                activeCategory.GetComponent<Image>().sprite = isUnChoose;
                //activeCategory.Back.sprite = ShopIcons.ShopCategoryOff;
            }
            category.Container.SetActive(value: true);
            category.GetComponent<Image>().sprite = isChoosing;
            activeCategory = category;
            if (activeCategory != null)
            {
                //activeCategory.Back.sprite = ShopIcons.ShopCategoryOn;
            }
            SelectItem(GetFirstItemInCategory(activeCategory));
        }

        public void SelectItem(ShopItem item)
        {
            bool flag = currentItem == item;
            if (currentItem != null)
            {
                //currentItem.Back.sprite = ShopIcons.ShopButtonOff;
            }
            currentItem = item;
            if (currentItem != null)
            {
                //PHUONG sua code
                //currentItem.Back.sprite = ShopIcons.ShopItemOn;
                //currentItem.GameItem.UpdateItem();
                ShopInfoPanelManager.Instance.ShowItemInfo(currentItem.GameItem, true);

            }
            if (selected && flag && item.GameItem is GameItemSkin)
            {
                PreviewManager.Instance.ShowItem(item, showOrigin: true);
                selected = false;
            }
            else
            {
                PreviewManager.Instance.ShowItem(item);
                selected = true;
            }
            if (ShowDebug)
            {
                UnityEngine.Debug.LogFormat(currentItem, "item selected {0}", currentItem.name);
            }
            if (currentItem.GameItem.ShopVariables.price == 0 && ItemAvailableForBuy(currentItem.GameItem))
            {
                PlayerInfoManager.Instance.Give(currentItem.GameItem);
            }
            else
            {
                UpdateInfo();
            }
        }

        public void Buy()
        {
            Buy(null);
        }

        public void OpenExchangePanel()
        {
            if (currentItem.GameItem.ShopVariables.gemPrice)
            {
                OpenGemsPanel();
                return;
            }
            float num = SalesManager.GetSale(currentItem.GameItem.ID) / 100f;
            int num2 = Mathf.RoundToInt((float)currentItem.GameItem.ShopVariables.price - (float)currentItem.GameItem.ShopVariables.price * num);
            int num3 = num2 - PlayerInfoManager.Money;
            gems = Mathf.CeilToInt((float)num3 / 100f);
            money = gems * 100;
            if (PlayerInfoManager.Gems < gems)
            {
                OpenGemsPanel();
                return;
            }
            NeedMoneyText.text = "To buy this product you need " + num3 + " money more";
            MoneyText.text = money.ToString();
            GemText.text = gems.ToString();
            Links.ExchangePanel.SetActive(value: true);
        }

        public void CloseExchangePanel()
        {
            Links.ExchangePanel.SetActive(value: false);
        }

        public void Exchange()
        {
            if (PlayerInfoManager.Gems < gems)
            {
                OpenGemsPanel();
                return;
            }
            ExchangeItem.ShopVariables.price = gems;
            ExchangeItem.BonusValue = money;
            Buy(ExchangeItem);
        }

        public void Buy(GameItem item)
        {
            if (item == null)
            {
                item = currentItem.GameItem;
            }
            if (ShowDebug)
            {
                UnityEngine.Debug.LogFormat(item, "try buy item {0}", item.name);
            }
            float num = SalesManager.GetSale(item.ID) / 100f;
            int num2 = Mathf.RoundToInt((float)item.ShopVariables.price - (float)item.ShopVariables.price * num);
            if (item.ShopVariables.gemPrice)
            {
                PlayerInfoManager.Gems -= num2;
            }
            else
            {
                PlayerInfoManager.Money -= num2;
                PlayerInfoManager.Instance.AddSpendMoney(-num2);
            }
            GameEventManager.Instance.Event.GetShopEvent();
            PlayerInfoManager.Instance.Give(item, onBuy: true);
        }



        public void EquipCurrentItem()
        {
            PlayerInfoManager.Instance.Equip(null);
        }
        public void Equip(GameItem item, bool equipOnly = false)
        {
            if (item == null)
            {
                item = currentItem.GameItem;
            }
            StuffManager.Instance.EquipItem(item, equipOnly);
            UpdateInfo();
        }



        public void OpenDialogPanel(GameItem item)
        {
            GameObject gameObject = null;
            ShopDialogPanel[] dialogPanelPrefabs = Links.DialogPanelPrefabs;
            foreach (ShopDialogPanel shopDialogPanel in dialogPanelPrefabs)
            {
                bool flag = false;
                ItemsTypes[] dialogPanelTypes = shopDialogPanel.DialogPanelTypes;
                foreach (ItemsTypes itemsTypes in dialogPanelTypes)
                {
                    if (itemsTypes == item.Type)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    gameObject = shopDialogPanel.gameObject;
                    break;
                }
            }
            if (gameObject != null)
            {
                Links.DialogPanelPlaceholder.gameObject.SetActive(value: true);
                currDialogPanel = gameObject.GetComponent<ShopDialogPanel>();
                gameObject.SetActive(value: true);
                UpdateDialogPanel(item);
            }
        }

        public void CloseDialogPanel()
        {
            currDialogPanel.gameObject.SetActive(value: false);
            Links.DialogPanelPlaceholder.gameObject.SetActive(value: false);
        }

        public void UpdateDialogPanel(GameItem item = null)
        {
            if (currDialogPanel != null)
            {
                currDialogPanel.UpdatePanel(item);
            }
        }

        public void OpenGemsPanel()
        {
            Links.DialogPanelPlaceholder.gameObject.SetActive(value: true);
            currDialogPanel = BuyGemsPanel.GetComponent<ShopDialogPanel>();
            BuyGemsPanel.SetActive(value: true);
            UpdateDialogPanel(currentItem.GameItem);
        }

        public void CloseGemsPanel()
        {
            CloseDialogPanel();
        }

        // private void FixPanelRect(Transform targetTransform)
        // {
        // 	RectTransform rectTransform = targetTransform as RectTransform;
        // 	if (!(rectTransform == null))
        // 	{
        // 		rectTransform.localScale = Vector3.one;
        // 		rectTransform.pivot = new Vector2(0.5f, 0.5f);
        // 		rectTransform.anchorMin = Vector2.zero;
        // 		rectTransform.anchorMax = Vector2.one;
        // 		Vector2 vector2 = rectTransform.offsetMin = (rectTransform.offsetMax = Vector2.zero);
        // 	}
        // }


        public void GenerateUI()
        {
            ClearCategories();
            ClearElements();
            ItemsManager.Instance.AssembleGameitems();
            foreach (int key in ItemsManager.Instance.Items.Keys)
            {
                if (!ItemsManager.Instance.Items[key].ShopVariables.HideInShop && ItemsManager.Instance.Items[key].ShopVariables.isDivision)
                {
                    ShopCategory shopCategory = CreateShopCategory(ItemsManager.Instance.Items[key]);
                    GameItem[] componentsInChildren = ItemsManager.Instance.Items[key].GetComponentsInChildren<GameItem>();
                    foreach (GameItem gameItem in componentsInChildren)
                    {
                        if (!gameItem.ShopVariables.isDivision && !gameItem.ShopVariables.HideInShop)
                        {
                            CreateShopItem(gameItem, shopCategory.Container);
                        }
                    }
                }
            }
            UnityEngine.Debug.Log("Shop UI is generated");
        }

        public void UpdateInfo()
        {
            if (!(currentItem == null))
            {
                bool flag = ItemAvailableForBuy(currentItem.GameItem);
                bool alreadyEquiped = StuffManager.AlredyEquiped(currentItem.GameItem);
                ManageBuyPanel(flag, EnoughMoneyToBuyItem(currentItem.GameItem));
                ManageEquipButton(!flag & ItemAvailableForEquip(currentItem.GameItem), alreadyEquiped);
                Links.InfoPanelManager.ShowItemInfo(currentItem.GameItem, BoughtAlredy(currentItem.GameItem));
            }
        }

        public bool ItemAvailableForBuy(GameItem item)
        {
            if (!BoughtAlredy(item))
            {
                if (item.ShopVariables.playerLvl > PlayerInfoManager.Level || item.ShopVariables.VipLvl > PlayerInfoManager.VipLevel)
                {
                    return false;
                }
                return item.CanBeBought;
            }
            if (item.ShopVariables.MaxAmount == 0 || ((item.ShopVariables.MaxAmount > 1) & (PlayerInfoManager.Instance.GetBIValue(item.ID) + currentItem.GameItem.ShopVariables.PerStackAmount < item.ShopVariables.MaxAmount)))
            {
                return item.CanBeBought;
            }
            return false;
        }

        public bool EnoughMoneyToBuyItem(GameItem item)
        {
            float num = SalesManager.GetSale(item.ID) / 100f;
            int num2 = (int)((float)item.ShopVariables.price - (float)item.ShopVariables.price * num);
            if (item.ShopVariables.gemPrice)
            {
                if (PlayerInfoManager.Gems < num2)
                {
                    return false;
                }
            }
            else if (PlayerInfoManager.Money < num2)
            {
                return false;
            }
            return true;
        }

        public bool ItemAvailableForEquip(GameItem item)
        {
            return (!item.ShopVariables.InstantEquip & BoughtAlredy(item)) && item.CanBeEquiped;
        }

        public void JumpToMoneyCategory()
        {
            ShopCategory category;
            ShopItem item;
            GameItemBonus shopItemByType = GetShopItemByType<GameItemBonus>(ItemsTypes.Bonus, new object[1]
            {
                BonusTypes.Money
            }, out category, out item);
            ChangeCategory(category);
            SelectItem(item);
        }

        // private void SavePlayerInfo()
        // {
        // 	if (ShowDebug)
        // 	{
        // 		UnityEngine.Debug.Log("Saving player info");
        // 	}
        // 	PlayerInfoManager.Instance.SaveBI();
        // }




        private ShopCategory GetFirstCategory()
        {
            return Links.Categories.GetComponentInChildren<ShopCategory>();
        }

        private void ClearPlaceholder(Transform placeholder)
        {
            if (!(placeholder == null))
            {
                for (int i = 0; i < placeholder.childCount; i++)
                {
                    UnityEngine.Object.Destroy(placeholder.GetChild(i).gameObject);
                }
            }
        }

        private void ClearCategories()
        {
            int childCount = Links.Categories.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                UnityEngine.Object.Destroy(Links.Categories.transform.GetChild(i).gameObject);
            }
        }

        private void ClearElements()
        {
            int childCount = Links.Elements.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                UnityEngine.Object.Destroy(Links.Elements.transform.GetChild(i).gameObject);
            }
        }

        private ShopItem GetFirstItemInCategory(ShopCategory category)
        {
            return category.Container.GetComponentInChildren<ShopItem>();
        }

        private ShopCategory CreateShopCategory(GameItem item)
        {
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BlankCategory, Links.Categories.transform);
            gameObject.name = item.name;
            RectTransform component = gameObject.GetComponent<RectTransform>();
            component.localScale = Vector3.one;
            GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(BlankContainer, Links.Elements.transform);
            RectTransform component2 = gameObject2.GetComponent<RectTransform>();
            RectTransform component3 = gameObject2.transform.parent.GetComponent<RectTransform>();
            gameObject2.name = item.name;
            component2.anchoredPosition = component3.anchoredPosition;
            component2.sizeDelta = component3.sizeDelta;
            component2.eulerAngles = component3.eulerAngles;
            component2.localScale = Vector3.one;
            ShopCategory component4 = gameObject.GetComponent<ShopCategory>();
            component4.GameItem = item;
            component4.Container = gameObject2;
            component4.SetUP();
            return component4;
        }

        private ShopItem CreateShopItem(GameItem item, GameObject container)
        {
            Transform parent = container.transform.Find("Viewport/Content");
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(BlankElement, parent);
            gameObject.name = item.name;
            RectTransform component = gameObject.GetComponent<RectTransform>();
            component.localScale = Vector3.one;
            ShopItem component2 = gameObject.GetComponent<ShopItem>();
            component2.GameItem = item;
            component2.SetUP();
            return component2;
        }

        private void ManageEquipButton(bool interactable, bool alreadyEquiped)
        {
            Links.EquipButton.GetComponentInChildren<Text>(true).text = ((!alreadyEquiped) ? "Equip" : "UnEquip");
            Links.EquipButton.SetActive(interactable);
        }

        private void ManageBuyPanel(bool itemIsAvailableForBuy, bool enoughtMoney)
        {
            Links.BuyPanel.SetActive(itemIsAvailableForBuy);
            Links.BuyButton.SetActive(enoughtMoney);
            Links.OpenExchangePanelButton.SetActive(!enoughtMoney);
            Links.Price.color = ((!enoughtMoney) ? ShopIcons.NotAvailablePriceColor : ShopIcons.AvailablePriceColor);
            float num = (float)currentItem.GameItem.ShopVariables.price * SalesManager.GetSale(currentItem.GameItem.ID) / 100f;
            Links.Price.text = ((int)((float)currentItem.GameItem.ShopVariables.price - num)).ToString();
            Links.PriceIcon.sprite = ((!currentItem.GameItem.ShopVariables.gemPrice) ? ShopIcons.Money : ShopIcons.Gems);
        }

        private void ApplyPrefab(GameObject go)
        {
        }

        private Transform getProtoParent(Transform incTransform)
        {
            while (incTransform.parent != null)
            {
                incTransform = incTransform.parent;
            }
            return incTransform;
        }

        private void FillShopStuffDictionary()
        {
            for (int i = 0; i < Links.Categories.transform.childCount; i++)
            {
                Transform child = Links.Categories.transform.GetChild(i);
                ShopCategory component = child.GetComponent<ShopCategory>();
                component.Init();
                if (!ShopStuff.ContainsKey(component.GameItem.Type))
                {
                    ShopStuff.Add(component.GameItem.Type, new Dictionary<ShopCategory, List<ShopItem>>());
                }
                if (!ShopStuff[component.GameItem.Type].ContainsKey(component))
                {
                    ShopStuff[component.GameItem.Type].Add(component, new List<ShopItem>());
                }
                Transform child2 = component.Container.transform.GetChild(0).GetChild(0);
                for (int j = 0; j < child2.childCount; j++)
                {
                    child = child2.GetChild(j);
                    ShopItem component2 = child.GetComponent<ShopItem>();
                    component2.Init();
                    ShopStuff[component.GameItem.Type][component].Add(component2);
                }
            }
            loadStuffDone = true;
        }
    }
}
