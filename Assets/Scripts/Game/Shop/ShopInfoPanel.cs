using Game.Character;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanel : MonoBehaviour
	{
		public ItemsTypes Type = ItemsTypes.None;

		public Text ItemName;

		[Space(5f)]
		public Text LvlRequirement;

		public Text VipRequirement;

		[Space(5f)]
		public Text Description;

		public virtual void ShowInfo(GameItem incItem)
		{
			ItemName.text = incItem.ShopVariables.Name;
			Description.text = incItem.ShopVariables.Description;
			LvlRequirement.text = incItem.ShopVariables.playerLvl.ToString();
			if (incItem.ShopVariables.playerLvl > PlayerInfoManager.Level)
			{
				LvlRequirement.color = ShopInfoPanelManager.Instance.ReqNotSatisfiedColor;
			}
			else
			{
				LvlRequirement.color = ShopInfoPanelManager.Instance.ReqSatisfiedColor;
			}
			VipRequirement.text = incItem.ShopVariables.VipLvl.ToString();
			if (incItem.ShopVariables.VipLvl > PlayerInfoManager.VipLevel)
			{
				VipRequirement.color = ShopInfoPanelManager.Instance.ReqNotSatisfiedColor;
			}
			else
			{
				VipRequirement.color = ShopInfoPanelManager.Instance.ReqSatisfiedColor;
			}
		}
	}
}
