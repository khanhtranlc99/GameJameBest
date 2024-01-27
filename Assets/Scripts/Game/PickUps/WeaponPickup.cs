using Game.Character;
using Game.GlobalComponent;
using Game.Items;
using Game.Shop;
using Game.Weapons;
using UnityEngine;

namespace Game.PickUps
{
	public class WeaponPickup : PickUp
	{
		[Tooltip("Итемы ТОЛЬКО лежащие под GameItems НА СЦЕНЕ")]
		public int GameItemWeaponId;

		private GameItemWeapon WeaponItem;

		protected override void TakePickUp()
		{
			WeaponItem = (GameItemWeapon)ItemsManager.Instance.GetItem(GameItemWeaponId);
			if (PlayerInfoManager.Instance.BoughtAlredy(WeaponItem))
			{
				AmmoManager.Instance.AddAmmo(WeaponItem.Weapon.AmmoType);
			}
			else
			{
				PlayerInfoManager.Instance.Give(WeaponItem, true);
            }
			PlayerInfoManager.Instance.Equip(WeaponItem, equipOnly: true); 
			StuffManager.Instance.EquipWeapon(WeaponItem, 2);
            //PlayerInfoManager.Instance.onEqipItem += Equip; 
            //PlayerInfoManager.Instance.Equip(null, equipOnly: false);
            InGameLogManager.Instance.RegisterNewMessage(MessageType.Item, WeaponItem.ShopVariables.Name);
			PointSoundManager.Instance.PlayCustomClipAtPoint(base.transform.position, PickUpManager.Instance.WeaponItemPickupSound);
			CollectionPickUpsManager.Instance.ElementWasTaken(base.gameObject);
			base.TakePickUp();
		}

		public void Equip(GameItem item, bool equipOnly = false)
		{
            if (item == null)
            {
                item = WeaponItem;
            }
            StuffManager.Instance.EquipItem(item, equipOnly);
        }
	}
}
