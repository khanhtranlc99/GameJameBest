using Game.Items;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
	public class ShopInfoPanelWeapons : ShopInfoPanel
	{
		[Space(5f)]
		public Slider DamageSlider;

		public Slider AttackSpeedSlider;

		[Space(5f)]
		public Text DefenceIgnorenceText;

		public Text DamageTypeText;

		[Space(5f)]
		public GameObject AmmoTypeContainer;

		public Text AmmoTypeText;

		public Image AmmoTypeImage,ItemIcon;

		private ShopCategory ammoCategory;

		private ShopItem ammoShopItem;

		public override void ShowInfo(GameItem incItem)
		{
			base.ShowInfo(incItem);
			GameItemWeapon gameItemWeapon = incItem as GameItemWeapon;
			if (gameItemWeapon == null)
			{
				UnityEngine.Debug.LogWarning("Пытался показать инфо оружия для неизвестного типа предмета.");
				return;
			}
			DamageSlider.minValue = 0f;
			DamageSlider.maxValue = GameItemWeapon.maxDamage;
			DamageSlider.value = ((!(gameItemWeapon.Weapon.Damage < GameItemWeapon.maxDamage)) ? GameItemWeapon.maxDamage : gameItemWeapon.Weapon.Damage);
			AttackSpeedSlider.minValue = 0f;
			AttackSpeedSlider.maxValue = 1f / GameItemWeapon.minAttackDelay;
			AttackSpeedSlider.value = 1f / gameItemWeapon.Weapon.AttackDelay;
			DefenceIgnorenceText.text = "Defence Ignorence: " + gameItemWeapon.Weapon.DefenceIgnorence * 100f + "%";
			DamageTypeText.text = "DamageType: " + gameItemWeapon.Weapon.WeaponDamageType;
			AmmoTypeContainer.SetActive(gameItemWeapon.Weapon.AmmoType != AmmoTypes.None);
			ItemIcon.sprite = gameItemWeapon.ShopVariables.ItemIcon;
			if (gameItemWeapon.Weapon.AmmoType != AmmoTypes.None)
			{
				AmmoTypeText.text = gameItemWeapon.Weapon.AmmoType.ToString();
				GameItemAmmo shopItemByType = ShopManager.Instance.GetShopItemByType<GameItemAmmo>(ItemsTypes.PatronContainer, new object[1]
				{
					gameItemWeapon.Weapon.AmmoType
				}, out ammoCategory, out ammoShopItem);
				if ((bool)shopItemByType)
				{
					AmmoTypeImage.sprite = shopItemByType.ShopVariables.ItemIcon;
				}
			}
		}

		public void JumpToAmmoBuying()
		{
			ShopManager.Instance.ChangeCategory(ammoCategory);
			ShopManager.Instance.SelectItem(ammoShopItem);
		}
	}
}
