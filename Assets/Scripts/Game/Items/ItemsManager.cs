using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.Items
{
	public class ItemsManager : SingletonMonoBehavior<ItemsManager>
	{

		public bool ShowDebug;

		// public Sprite[] ssss;
		//
		// public GameItem[] items;

		private bool inited;
		
		public GameItemWeapon[] allMeleeWeapons;
		public GameItemWeapon[] allPistolWeapons;
		public GameItemWeapon[] allRifleWeapons;
		public GameItemWeapon[] allShotgunWeapons;
		public GameItemWeapon[] allHeavyWeapons;

		public Dictionary<int, GameItem> Items
		{
			get;
			private set;
		}
		//
		// [ContextMenu("FIX")]
		// public void FixLinks()
		// {
		// 	string oldValue = "PreviewImg";
		// 	GameItem[] array = items;
		// 	foreach (GameItem gameItem in array)
		// 	{
		// 		Sprite[] array2 = ssss;
		// 		foreach (Sprite sprite in array2)
		// 		{
		// 			string a = new StringBuilder(sprite.name).Replace(oldValue, string.Empty).ToString();
		// 			if (a == gameItem.ShopVariables.Name)
		// 			{
		// 				gameItem.ShopVariables.ItemIcon = sprite;
		// 			}
		// 		}
		// 	}
		// }
		public void Init()
		{
			if (!inited)
			{
				AssembleGameitems();
				inited = true;
			}
		}

		public bool AddItem(GameItem item)
		{
			if (Items.ContainsKey(item.ID))
			{
				if (ShowDebug)
				{
					UnityEngine.Debug.LogWarning(item.ShopVariables.Name + " - предмет с таким ID уже сеществует. Пытаюсь переполучить ID.");
				}
				item.ID = GenerateID(item);
			}
			if (!Items.ContainsKey(item.ID))
			{
				Items.Add(item.ID, item);
				if (ShowDebug)
				{
					//UnityEngine.Debug.LogFormat(item.gameObject, "{0} Added item name : {1}", item.ID, item.gameObject.name);
				}
				return true;
			}
			if (ShowDebug)
			{
				UnityEngine.Debug.LogError(item.ShopVariables.Name + " не добавлен. Предмет с таким ID уже сеществует.");
			}
			return false;
		}

		public GameItem GetItem(int id)
		{
			GameItem value = null;
			bool flag = Items.TryGetValue(id, out value);
			if (ShowDebug)
			{
				foreach (KeyValuePair<int, GameItem> item in Items)
				{
					UnityEngine.Debug.Log(item.Key + " " + item.Value.ShopVariables.Name + " " + (item.Value == null));
				}
				return value;
			}
			return value;
		}

		public int GenerateID(GameItem item)
		{
			return item.ID = item.GetInstanceID();
		}

		public Dictionary<int, GameItem> AssembleGameitems()
		{
			Items = new Dictionary<int, GameItem>();
			GameItem[] componentsInChildren = GetComponentsInChildren<GameItem>();
			GameItem[] array = componentsInChildren;
			foreach (GameItem gameItem in array)
			{
				Instance.AddItem(gameItem);
				gameItem.Init();
			}
			return Items;
		}
	}
}
