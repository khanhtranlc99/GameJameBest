using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopItem : ShopElement
	{
		public Image SaleImage;

		public void Init()
		{
			if (GameItem == null)
				GameItem = ItemsManager.Instance.GetItem(idItem);
			CheckSale();
		}

		public override void OnClick()
		{
			ShopManager.Instance.SelectItem(this);
		}

		private void CheckSale()
		{
			Sprite sprite;
			float num = SalesManager.Instance.GetSale(GameItem.ID, out sprite);
			if (num > 0f)
			{
				SaleImage.sprite = sprite;
				SaleImage.gameObject.SetActive(value: true);
			}
			else
			{
				SaleImage.gameObject.SetActive(value: false);
			}
		}
	}
}
