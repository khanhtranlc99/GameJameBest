using Game.Weapons;
using UnityEngine;

namespace Game.Enemy
{
	public class WeaponForSmartHumanoidNPC : MonoBehaviour
	{
		public GameObject RightHandWeaponPlaceholder;

		public GameObject LeftHandWeaponPlaceholder;

		private Weapon[] weaponsInCase;

		private int[] weaponsStartAmmo;

		private SmartHumanoidWeaponController currentWeaponController;

		public void Init(BaseNPC controlledNPC, SmartHumanoidWeaponController parentWeaponController)
		{
			WeaponsInitialize();
			MovePlaceholder(RightHandWeaponPlaceholder, controlledNPC.SpecificNpcLinks.RightHand.transform);
			MovePlaceholder(LeftHandWeaponPlaceholder, controlledNPC.SpecificNpcLinks.LeftHand.transform);
			currentWeaponController = parentWeaponController;
			currentWeaponController.Weapons.AddRange(weaponsInCase);
		}

		public void DeInit()
		{
			currentWeaponController.Weapons.Clear();
			currentWeaponController = null;
			MovePlaceholder(RightHandWeaponPlaceholder, base.transform);
			MovePlaceholder(LeftHandWeaponPlaceholder, base.transform);
		}

		private void WeaponsInitialize()
		{
			if (weaponsInCase == null)
			{
				weaponsInCase = GetComponentsInChildren<Weapon>();
				weaponsStartAmmo = new int[weaponsInCase.Length];
				for (int i = 0; i < weaponsInCase.Length; i++)
				{
					RangedWeapon rangedWeapon = weaponsInCase[i] as RangedWeapon;
					weaponsStartAmmo[i] = ((rangedWeapon != null) ? rangedWeapon.AmmoOutOfCartridgeCount : 0);
					weaponsInCase[i].gameObject.SetActive(value: false);
				}
				return;
			}
			for (int j = 0; j < weaponsInCase.Length; j++)
			{
				RangedWeapon rangedWeapon2 = weaponsInCase[j] as RangedWeapon;
				if (rangedWeapon2 != null)
				{
					rangedWeapon2.AmmoOutOfCartridgeCount = weaponsStartAmmo[j];
				}
			}
		}

		private void MovePlaceholder(GameObject placeHolder, Transform toTransform)
		{
			placeHolder.transform.SetParent(toTransform, worldPositionStays: false);
		}
	}
}
