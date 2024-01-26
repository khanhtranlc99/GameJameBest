using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindLostFont : MonoBehaviour
{
    public Font font;
    public GameObject[] allPrefab;

    [ContextMenu("FindAll")]
    public void FindAll()
    {
        foreach (var prefab in allPrefab)
        {
            var allTextUI = prefab.GetComponentsInChildren<Text>(true);
            foreach (var text in allTextUI)
            {
                if (text.font == null)
                {
                    text.font = font;
                }
            }
        }

    }
}
