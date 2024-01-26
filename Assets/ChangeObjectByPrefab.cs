using System.Collections;
using System.Collections.Generic;
using Game.Shop;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChangeObjectByPrefab : MonoBehaviour
{
    public GameObject prefab;

#if UNITY_EDITOR
    [ContextMenu("TryConvert")]
    void TryConvert()
    {
        var allShopItem = GetComponentsInChildren<ShopItem>(true);
        foreach (var shopItem in allShopItem)
        {
            var newPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            var allComponent = shopItem.gameObject.GetComponents<Component>();
            foreach (var component in allComponent)
            {
                newPrefab.name = shopItem.name;
                newPrefab.transform.SetParent(shopItem.transform.parent);
                newPrefab.transform.localPosition = shopItem.transform.localPosition;
                var newGameItem = newPrefab.GetComponent<ShopItem>();
                newGameItem.idItem = shopItem.idItem;
                newGameItem.Icon.sprite = shopItem.Icon.sprite;
                newGameItem.SaleImage.sprite = shopItem.SaleImage.sprite;

            }
        }
    }
#endif
}
