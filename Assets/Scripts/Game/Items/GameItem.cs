using UnityEngine;

namespace Game.Items
{
	public class GameItem : MonoBehaviour
	{
		[ShowOnly]
		public int ID;

		public ItemsTypes Type;
		public int indexSkin;

		public ShopVariables ShopVariables;

		public PreviewVariables PreviewVariables;

		public virtual bool CanBeBought => true;

		public virtual bool CanBeEquiped => true;

		public virtual void Init()
		{
		}

		public virtual void UpdateItem()
		{
		}

		public virtual void OnBuy()
		{
		}

		public virtual void OnEquip()
		{
		}

		public virtual void OnUnequip()
		{
		}

		public virtual bool SameParametrWithOther(object[] parametrs)
		{
			return false;
		}
	}
}
