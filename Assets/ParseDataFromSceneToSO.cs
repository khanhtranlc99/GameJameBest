using System.Collections;
using System.Collections.Generic;
using Game.Items;
using Game.Shop;
using UnityEngine;

public class ParseDataFromSceneToSO : MonoBehaviour
{
    public ShopItem[] itemMono;
    public ShopItem[] itemSO;

    [ContextMenu("Try Parse")]
    void TryParse()
    {
        for (int i = 0; i < itemMono.Length; i++)
        {
            var old = itemMono[i];
            var newData = itemSO[i];
            newData.idItem = old.GameItem.ID;
        }
    }

}
