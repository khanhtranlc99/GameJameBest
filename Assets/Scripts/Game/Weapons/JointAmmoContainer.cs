using Game.Character;
using Game.Character.CharacterController;
using Game.Items;
using Game.Shop;
using UnityEngine;

namespace Game.Weapons
{
	public class JointAmmoContainer : MonoBehaviour
	{
		public GameItemAmmo GameItemAmmo;

		[SerializeField]
		private int ammoCount;

		public int AmmoCount
		{
			get
			{
				return GetAmmoCount();
			}
			set
			{
				ammoCount = value;
			}
		}

		public virtual int GetAmmoCount()
		{
			if (ammoCount <= 0)
			{
				ammoCount += 1000;
			}
			return ammoCount;
		}

		public void SaveAmmo()
		{
			PlayerInfoManager.Instance.SetBIValue(GameItemAmmo.ID, AmmoCount);
		}

		public void UpdateAmmo()
		{
			AmmoCount = PlayerInfoManager.Instance.GetBIValue(GameItemAmmo.ID);
			RangedWeapon rangedWeapon = PlayerManager.Instance.WeaponController.CurrentWeapon as RangedWeapon;
			if (rangedWeapon != null && rangedWeapon.AmmoContainer == this)
			{
				PlayerManager.Instance.WeaponController.UpdateAmmoText(rangedWeapon.AmmoCountText);
			}
		}
	}
}
