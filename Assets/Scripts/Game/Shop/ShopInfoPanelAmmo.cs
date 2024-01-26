using Game.Character;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelAmmo : ShopInfoPanel
	{
		[Space(5f)]
		public Text AmmoTypeText;

		public Text CurrAmmoAmountText;

		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			GameItemAmmo gameItemAmmo = incItem as GameItemAmmo;
			if (gameItemAmmo == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо о патронах для неизвестного типа предмета.");
				return;
			}
			AmmoTypeText.text = "AmmoType: " + gameItemAmmo.AmmoType;
			CurrAmmoAmountText.text = "Current amount: " + PlayerInfoManager.Instance.GetBIValue(gameItemAmmo.ID, gameItemAmmo.ShopVariables.gemPrice);
		}
	}
}
