using Game.Character;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    public class ShopInfoPanelManager : MonoBehaviour
    {
        public ShopInfoPanel DefaultInfoPanelPrefab;

        public ShopInfoPanel[] InfoPanelsPrefabs;

        [Space(10f)]
        public Color ReqSatisfiedColor = Color.white;

        public Color ReqNotSatisfiedColor = Color.red;

        public GameObject BlockedItemPanel;
        public GameObject VIPRequired;

        public Image VIPImage;

        public Text LvlText;

        private static ShopInfoPanelManager instance;

        private GameObject currPanelGO;

        private ShopInfoPanel currInfoPanel;

        private ShopCategory currentItemCategory;

        private ShopItem currentShopItem;

        private bool inited;

        public static ShopInfoPanelManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UnityEngine.Object.FindObjectOfType<ShopInfoPanelManager>();
                }
                return instance;
            }
        }

        public void Init()
        {
            if (!inited)
            {
                instance = this;
                currInfoPanel = DefaultInfoPanelPrefab;
                currPanelGO = currInfoPanel.gameObject;
                currPanelGO.SetActive(value: true);
                inited = true;
            }
        }

        public void ShowItemInfo(GameItem item, bool isAvailable)
        {
            ItemsTypes type = item.Type;
            if (type != currInfoPanel.Type)
            {
                currPanelGO.SetActive(value: false);
                currInfoPanel = GetPanelByType(type);
                currPanelGO = currInfoPanel.gameObject;
                currPanelGO.SetActive(value: true);
            }
            currInfoPanel.ShowInfo(item);
            ShowBlockedItemPanel(item, isAvailable);
        }

        public void JumpToVIP()
        {
            ShopManager.Instance.ChangeCategory(currentItemCategory);
            ShopManager.Instance.SelectItem(currentShopItem);
        }

        private void ShowBlockedItemPanel(GameItem item, bool isAvailable)
        {
            if (isAvailable)
            {
                BlockedItemPanel.SetActive(value: false);
                return;
            }
            bool active = false;
            if (item.ShopVariables.VipLvl > PlayerInfoManager.VipLevel)
            {
                ShopManager.Instance.GetShopItemByType<GameItemBonus>(ItemsTypes.Bonus, new object[2]
                {
                    BonusTypes.VIP,
                    item.ShopVariables.VipLvl
                }, out currentItemCategory, out currentShopItem);
                VIPImage.sprite = currentShopItem.Icon.sprite;
                VIPRequired.SetActive(true);
                VIPImage.gameObject.SetActive(value: true);
                active = true;
            }
            else
            {
                VIPRequired.SetActive(false);
                VIPImage.gameObject.SetActive(value: false);
            }
            if (item.ShopVariables.playerLvl > PlayerInfoManager.Level)
            {
                active = true;
            }
            LvlText.text = item.ShopVariables.playerLvl.ToString();
            BlockedItemPanel.SetActive(active);
        }

        private ShopInfoPanel GetPanelByType(ItemsTypes neededType)
        {
            ShopInfoPanel[] infoPanelsPrefabs = InfoPanelsPrefabs;
            foreach (ShopInfoPanel shopInfoPanel in infoPanelsPrefabs)
            {
                if (shopInfoPanel.Type == neededType)
                {
                    return shopInfoPanel;
                }
            }
            return DefaultInfoPanelPrefab;
        }

        private void FixPanelRect()
        {
            RectTransform component = currPanelGO.GetComponent<RectTransform>();
            component.localScale = Vector3.one;
            component.pivot = new Vector2(0.5f, 0.5f);
            Vector2 vector2 = component.offsetMin = (component.offsetMax = Vector2.zero);
        }
    }
}
