using Game.Items;
using UnityEngine;

namespace Game.Shop
{
	public class ShopCategory : ShopElement
	{
		public GameObject Container;

		public override void OnClick()
		{
			ShopManager.Instance.ChangeCategory(this);
		}

		public void Init()
		{
			if (GameItem == null)
				GameItem = ItemsManager.Instance.GetItem(idItem);
		}

		public override void SetUP()
		{
			base.SetUP();
			Container.SetActive(value: false);

		}
	}
}
