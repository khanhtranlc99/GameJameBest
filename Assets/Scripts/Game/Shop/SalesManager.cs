using Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character;
using UnityEngine;

namespace Game.Shop
{
	public class SalesManager : MonoBehaviour
	{
		private const string lastSalesUpdateKey = "SalesUpdateTime";

		private const string currentSalesKey = "CurrentSales";

		public bool DebugLog;

		public Sprite[] SalesSprites;

		public int SalesCount = 2;

		private readonly int[] SalesValues = new int[3]
		{
			10,
			20,
			30
		};

		private long lastSalesUpdateTime;

		private static Dictionary<int, int> currentSales = new Dictionary<int, int>();

		private static SalesManager instance;

		private bool inited;

		public static SalesManager Instance => instance ?? (instance = UnityEngine.Object.FindObjectOfType<SalesManager>());

		public void Init()
		{
			if (!inited)
			{
				lastSalesUpdateTime = BaseProfile.ResolveValue("SalesUpdateTime", 0L);
				if (lastSalesUpdateTime == 0L)
				{
					BaseProfile.StoreValue(DateTime.Today.ToFileTime(), "SalesUpdateTime");
				}
				UpdateSales();
				inited = true;
			}
		}

		private void UpdateSales()
		{
			if (TimeManager.AnotherDay(lastSalesUpdateTime))
			{
				currentSales = new Dictionary<int, int>();
				int count = ItemsManager.Instance.Items.Count;
				int num = SalesCount;
				int num2 = 10000;
				while (num > 0 && num2 > 0)
				{
					int index = UnityEngine.Random.Range(1, count);
					int key = ItemsManager.Instance.Items.ElementAt(index).Key;
					GameItem item = ItemsManager.Instance.GetItem(key);
					if (!PlayerInfoManager.Instance.BoughtAlredy(item) && !item.ShopVariables.isDivision && !item.ShopVariables.HideInShop && item.ShopVariables.price != 0 && !currentSales.ContainsKey(key))
					{
						int value = SalesValues[UnityEngine.Random.Range(0, SalesValues.Length)];
						currentSales.Add(key, value);
						num--;
						num2--;
					}
				}
				BaseProfile.StoreValue(currentSales, "CurrentSales");
				lastSalesUpdateTime = DateTime.Today.ToFileTime();
				BaseProfile.StoreValue(lastSalesUpdateTime, "SalesUpdateTime");
			}
			else
			{
				currentSales = BaseProfile.ResolveValue("CurrentSales", new Dictionary<int, int>());
			}
			if (DebugLog)
			{
				foreach (KeyValuePair<int, int> currentSale in currentSales)
				{
					UnityEngine.Debug.Log(currentSale.Value + " sale for item " + ItemsManager.Instance.GetItem(currentSale.Key).ShopVariables.Name);
				}
			}
		}

		public static float GetSale(int itemID)
		{
			float result = 0f;
			if (currentSales.ContainsKey(itemID))
			{
				result = currentSales[itemID];
			}
			return result;
		}

		public int GetSale(int itemID, out Sprite sprite)
		{
			sprite = null;
			int num = 0;
			if (currentSales.ContainsKey(itemID))
			{
				num = currentSales[itemID];
				switch (num)
				{
				case 10:
					sprite = SalesSprites[0];
					break;
				case 20:
					sprite = SalesSprites[1];
					break;
				case 30:
					sprite = SalesSprites[2];
					break;
				}
			}
			return num;
		}
	}
}
