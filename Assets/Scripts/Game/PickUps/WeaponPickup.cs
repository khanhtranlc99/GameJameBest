using Game.Character;
using Game.Character.CharacterController;
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
			int slotIndex;
			switch (WeaponItem.Weapon.Type)
			{
				case WeaponTypes.Melee:
					slotIndex = 1;
					break;
				case WeaponTypes.Pistol:
				case WeaponTypes.SMG:
                case WeaponTypes.Shotgun:
                    slotIndex = 2;
					break;
				case WeaponTypes.Rifle:
					slotIndex = 3;
					break;
				case WeaponTypes.Heavy:
					slotIndex = 4;
					break;
				default:
                    int emptySlotOfTypeInt = PlayerManager.Instance.DefaultWeaponController.WeaponSet.GetEmptySlotOfTypeInt(PlayerManager.Instance.DefaultWeaponController.GetTargetSlot(WeaponItem.Weapon));
					bool isReadySlot = StuffManager.AlredyEquiped(WeaponItem);
					if (emptySlotOfTypeInt != -1 && !isReadySlot)
					{
						slotIndex = emptySlotOfTypeInt;
					}
					else slotIndex = 5;
                    break;
			}
			StuffManager.Instance.EquipWeapon(WeaponItem, slotIndex);
            PlayerManager.Instance.Player.WeaponController.ChooseSlot(slotIndex);
            //PlayerManager.Instance.DefaultWeaponController.InitWeapon(slotIndex);
            //defaultWeaponController.CurrentWeapon = WeaponItem;

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
