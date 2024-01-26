using Game.Items;
using System.Collections.Generic;
using UnityEngine;

public class TestItemSystem : MonoBehaviour
{
	[InspectorButton("ShowAllItems")]
	public bool ShowItems;

	[InspectorButton("ClearAllItems")]
	public bool ClearItems;

	public void ShowAllItems()
	{
		Dictionary<int, GameItem> items = ItemsManager.Instance.Items;
		MonoBehaviour.print(items.Count);
		foreach (KeyValuePair<int, GameItem> item in items)
		{
			UnityEngine.Debug.LogFormat(item.Value, "name {0} ID {1}", item.Value.name, item.Value.ID);
		}
	}

	public void ClearAllItems()
	{
		ItemsManager.Instance.Items.Clear();
		MonoBehaviour.print(ItemsManager.Instance.Items.Count);
	}
}
