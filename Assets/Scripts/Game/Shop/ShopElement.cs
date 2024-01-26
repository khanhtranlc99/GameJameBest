using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopElement : MonoBehaviour
	{
		public GameItem GameItem;
		public int idItem;
		public Image Back;

		public Image Icon;

		public virtual void OnClick()
		{
		}

		public virtual void SetUP()
		{
			Icon.sprite = GameItem.ShopVariables.ItemIcon;
		}
	}
}
