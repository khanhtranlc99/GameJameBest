using DG.Tweening;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Superpowers;
using Game.Shop;
using Game.Weapons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Items
{
	public class StuffManager : SingletonMonoBehavior<StuffManager>
	{
		public static readonly List<GameItemPowerUp> ActivePowerUps = new List<GameItemPowerUp>();

		public StuffHelper CurrentHelper => PlayerManager.Instance.DefaultStuffHelper;

		public StuffHelper CurrentHelperRagdoll => PlayerManager.Instance.DefaultRagdollStuffHelper;

		public void Init()
		{

			EquipAll();
		}

		public void EquipItem(GameItem item, bool equipOnly = false)
		{
			if (!(item == null))
			{
				switch (item.Type)
				{
				case ItemsTypes.Accessory:
					ProceedSkin(item, equipOnly);
					return;
				case ItemsTypes.Clothes:
					ProceedSkin(item, equipOnly);
					return;
				case ItemsTypes.Weapon:
					ProceedWeapon(item, equipOnly);
					return;
				case ItemsTypes.PatronContainer:
					ProceedPatronContainer(item);
					break;
				case ItemsTypes.Ability:
					ProceedAbility(item);
					return;
				case ItemsTypes.Bonus:
					ProceedBonus(item);
					break;
				case ItemsTypes.PowerUp:
					ProceedPowerUp(item);
					break;
				case ItemsTypes.Pack:
					ProceedPack(item);
					break;
				case ItemsTypes.Vehicle:
					ProceedVehicle(item);
					break;
				default:
					UnityEngine.Debug.LogError("Unknown item type");
					break;
				}
				PlayerStoreProfile.SaveLoadout();
			}
		}

		public bool CanEquipInstantly(GameItem item, bool onBuy = false)
		{
			switch (item.Type)
			{
			case ItemsTypes.Accessory:
				return true;
			case ItemsTypes.Clothes:
				return true;
			case ItemsTypes.Weapon:
			{
				GameItemWeapon gameItemWeapon = item as GameItemWeapon;
				int num = -1;
				if (gameItemWeapon != null)
				{
					num = PlayerManager.Instance.WeaponController.WeaponSet.GetEmptySlotOfTypeInt(PlayerManager.Instance.WeaponController.GetTargetSlot(gameItemWeapon.Weapon));
				}
				return onBuy || num != -1;
			}
			case ItemsTypes.Vehicle:
				return true;
			default:
				return false;
			}
		}

		private void ProceedSkin(GameItem item, bool equipOnly = false)
		{
			GameItemSkin gameItemSkin = item as GameItemSkin;
			if (gameItemSkin == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as skin.");
			}
			else if (AlredyEquiped(gameItemSkin) && !equipOnly)
			{
				UnequipSkin(gameItemSkin);
			}
			else
			{
				EquipSkin(gameItemSkin);
			}
		}

		private void EquipSkin(GameItemSkin skin, bool withoutUpdate = false)
		{
			if (skin == null)
			{
				return;
			}
			SkinSlot[] occupiedSlots = skin.OccupiedSlots;
			foreach (SkinSlot slot in occupiedSlots)
			{
				GameItemSkin gameItemSkin = ItemInSkinSlot(slot) as GameItemSkin;
				if (gameItemSkin != skin)
				{
					UnequipSkin(gameItemSkin, withAbility: true, withoutUpdate: true);
				}
			}
			SkinSlot[] occupiedSlots2 = skin.OccupiedSlots;
			foreach (SkinSlot key in occupiedSlots2)
			{
				PlayerStoreProfile.CurrentLoadout.Skin[Loadout.KeyFromSkinSlot[key]] = skin.ID;
			}
			GameItemClothes gameItemClothes = skin as GameItemClothes;
			if (gameItemClothes != null)
			{
				GameItemAccessory[] relatedAccesories = gameItemClothes.RelatedAccesories;
				foreach (GameItemAccessory skin2 in relatedAccesories)
				{
					EquipSkin(skin2, withoutUpdate: true);
				}
			}
			if (!withoutUpdate)
			{
				UpdateAllAccesories();
				UpdateAllAccesories(CurrentHelperRagdoll);
				UpdateClothes();
				UpdateClothes(CurrentHelperRagdoll);
			}
			PlayerStoreProfile.SaveLoadout();
		}

		private void UnequipSkin(GameItemSkin skin, bool withAbility = true, bool withoutUpdate = false)
		{
			if (skin == null)
			{
				return;
			}
			SkinSlot[] occupiedSlots = skin.OccupiedSlots;
			foreach (SkinSlot key in occupiedSlots)
			{
				PlayerStoreProfile.CurrentLoadout.Skin[Loadout.KeyFromSkinSlot[key]] = 0;
			}
			GameItemClothes gameItemClothes = skin as GameItemClothes;
			if (gameItemClothes != null)
			{
				GameItemAccessory[] relatedAccesories = gameItemClothes.RelatedAccesories;
				foreach (GameItemAccessory skin2 in relatedAccesories)
				{
					UnequipSkin(skin2, withAbility: true, withoutUpdate: true);
				}
			}
			if (!withoutUpdate)
			{
				UpdateAllAccesories();
				UpdateAllAccesories(CurrentHelperRagdoll);
				UpdateClothes();
				UpdateClothes(CurrentHelperRagdoll);
			}
			if (withAbility)
			{
				GameItemAbility[] relatedAbilitys = skin.RelatedAbilitys;
				foreach (GameItemAbility abilityItem in relatedAbilitys)
				{
					UnequipAbility(abilityItem, withSkin: false);
				}
			}
			PlayerStoreProfile.SaveLoadout();
		}

		public void UpdateAllAccesories(StuffHelper customHelper = null, Loadout customLoadout = null)
		{
			StuffHelper targetHelper = customHelper ? customHelper : CurrentHelper;
			Loadout targetLoadout = customLoadout ?? PlayerStoreProfile.CurrentLoadout;
			UpdateOneAccesory(targetHelper, targetLoadout, "HatID");
			UpdateOneAccesory(targetHelper, targetLoadout, "MaskID");
			UpdateOneAccesory(targetHelper, targetLoadout, "GlassID");
			UpdateOneAccesory(targetHelper, targetLoadout, "LeftBraceletID");
			UpdateOneAccesory(targetHelper, targetLoadout, "RightBraceletID");
			UpdateOneAccesory(targetHelper, targetLoadout, "LeftHuckleID");
			UpdateOneAccesory(targetHelper, targetLoadout, "RightHuckleID");
			UpdateOneAccesory(targetHelper, targetLoadout, "LeftPalmID");
			UpdateOneAccesory(targetHelper, targetLoadout, "RightPalmID");
			UpdateOneAccesory(targetHelper, targetLoadout, "LeftToeID");
			UpdateOneAccesory(targetHelper, targetLoadout, "RightToeID");
		}

		private void UpdateOneAccesory(StuffHelper targetHelper, Loadout targetLoadout, string key)
		{
			Transform transform = null;
			int num = 0;
			GameItemAccessory gameItemAccessory = null;
			transform = targetHelper.GetPlaceholder(Loadout.SkinSlotFromKey[key]);
			num = targetLoadout.Skin[key];
			gameItemAccessory = (ItemsManager.Instance.GetItem(num) as GameItemAccessory);
			ClearPlaceholder(transform);
			if (gameItemAccessory is { })
			{
				if(key.Equals("HatID"))
					targetHelper.UpdateNewAcessories(gameItemAccessory.indexSkin);
				if (gameItemAccessory != null && Loadout.KeyFromSkinSlot[gameItemAccessory.OccupiedSlots[0]] == key)
				{
					ClotheAnAccessory(gameItemAccessory, transform);
				}
			}
		}

		public void UpdateClothes(StuffHelper customHelper = null, Loadout customLoadout = null)
		{
			StuffHelper stuffHelper = customHelper ? customHelper : CurrentHelper;
			Loadout loadout = customLoadout ?? PlayerStoreProfile.CurrentLoadout;
			foreach (string key2 in loadout.Skin.Keys)
			{
				int num = loadout.Skin[key2];
				if (num != 0)
				{
					GameItemClothes gameItemClothes = ItemsManager.Instance.GetItem(num) as GameItemClothes;
					if (gameItemClothes != null)
					{
						stuffHelper.UpdateNewSkin(gameItemClothes.indexSkin);
						for (int i = 0; i < gameItemClothes.OccupiedSlots.Length; i++)
						{
							SkinnedMeshRenderer renderer = stuffHelper.SlotRenderers.GetRenderer(gameItemClothes.OccupiedSlots[i]);
							if (i == 0)
							{
								if (!(renderer == null))
								{
									renderer.gameObject.SetActive(value: true);
									renderer.sharedMesh = gameItemClothes.SkinMesh;
									renderer.sharedMaterials = gameItemClothes.SkinMaterials;
								}
							}
							else
							{
								ClearSkinSlot(gameItemClothes.OccupiedSlots[i], stuffHelper);
							}
						}
					}
				}
			}
			GameItemClothes[] defaultClotheses = stuffHelper.DefaultClotheses;
			foreach (GameItemClothes gameItemClothes2 in defaultClotheses)
			{
				bool flag = true;
				SkinSlot[] occupiedSlots = gameItemClothes2.OccupiedSlots;
				foreach (SkinSlot key in occupiedSlots)
				{
					if (loadout.Skin[Loadout.KeyFromSkinSlot[key]] != 0)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				for (int l = 0; l < gameItemClothes2.OccupiedSlots.Length; l++)
				{
					SkinnedMeshRenderer renderer2 = stuffHelper.SlotRenderers.GetRenderer(gameItemClothes2.OccupiedSlots[l]);
					if (!(renderer2 == null))
					{
						if (l == 0 && !gameItemClothes2.HideByDefault)
						{
							renderer2.gameObject.SetActive(value: true);
							renderer2.sharedMesh = gameItemClothes2.SkinMesh;
							renderer2.sharedMaterials = gameItemClothes2.SkinMaterials;
						}
						else
						{
							renderer2.gameObject.SetActive(value: false);
						}
					}
				}
			}
		}

		private void ProceedPack(GameItem item)
		{
			GameItemPack gameItemPack = item as GameItemPack;
			if (gameItemPack == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as ItemPack.");
				return;
			}
			GameItem[] packedItems = gameItemPack.PackedItems;
			foreach (GameItem item2 in packedItems)
			{
				PlayerInfoManager.Instance.Give(item2);
			}
		}

		private void ClotheAnAccessory(GameItemAccessory accessory, Transform placeholder)
		{
			if (!(accessory == null) && !(placeholder == null))
			{
				if ((bool)accessory.ModelPrefab)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(accessory.ModelPrefab, placeholder) as GameObject;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
				}
				else
				{
					UnityEngine.Debug.LogError("Accesorry model doesn't exist.");
				}
			}
		}

		public void TryWeapon(GameItemWeapon item)
		{
			if (item == null)
			{
				return;
			}
			int emptySlotOfTypeInt = PlayerManager.Instance.DefaultWeaponController.WeaponSet.GetEmptySlotOfTypeInt(PlayerManager.Instance.DefaultWeaponController.GetTargetSlot(item.Weapon));
			if (emptySlotOfTypeInt != -1 && !AlredyEquiped(item))
			{
				PlayerManager.Instance.DefaultWeaponController.EquipWeapon(item, emptySlotOfTypeInt);
			}
		}

		private void ProceedWeapon(GameItem item, bool equipOnly = false)
		{
			if (item == null)
			{
				return;
			}

			GameItemWeapon gameItemWeapon = item as GameItemWeapon;
			string key;
			if (gameItemWeapon == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as weapon.");
			}
			else if (AlredyEquiped(gameItemWeapon, out key))
			{
				if (!equipOnly)
				{
					UnequipWeapon(key);
				}
			}
			else if (CanEquipInstantly(item) && !ShopManager.IsOpen)
			{
				int emptySlotOfTypeInt = PlayerManager.Instance.DefaultWeaponController.WeaponSet.GetEmptySlotOfTypeInt(PlayerManager.Instance.DefaultWeaponController.GetTargetSlot(gameItemWeapon.Weapon));
				if (emptySlotOfTypeInt != -1 && !AlredyEquiped(gameItemWeapon))
				{
					PlayerManager.Instance.DefaultWeaponController.EquipWeapon(gameItemWeapon, emptySlotOfTypeInt);
				}
			}
			else if (ShopManager.IsOpen)
			{
				ShopManager.Instance.OpenDialogPanel(item);
			}
		}

		public void EquipWeapon(GameItemWeapon weaponItem, int slotIndex, bool equipOnly = false)
		{
			if (weaponItem == null)
			{
				return;
			}
			WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
			string key;
			if (AlredyEquiped(weaponItem, out key) && !equipOnly)
			{
				int num = Loadout.WeaponSlotFromKey(key);
				defaultWeaponController.UnEquipWeapon(num);
				if (num == slotIndex)
				{
					ShopManager.Instance.UpdateInfo();
					return;
				}
			}
			defaultWeaponController.EquipWeapon(weaponItem, slotIndex);
			if (!equipOnly)
			{
				PlayerStoreProfile.SaveLoadout();
				if (ShopManager.Instance != null && ShopManager.IsOpen)
					ShopManager.Instance.UpdateInfo();
			}
		}

		public void UnequipWeapon(string slotKey)
		{
			WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
			int slotIndex = Loadout.WeaponSlotFromKey(slotKey);
			defaultWeaponController.UnEquipWeapon(slotIndex);
			ShopManager.Instance.UpdateInfo();
			PlayerStoreProfile.SaveLoadout();
		}

		private void ProceedPatronContainer(GameItem item)
		{
			GameItemAmmo gameItemAmmo = item as GameItemAmmo;
			if (gameItemAmmo == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as ammo.");
			}
			else
			{
				AmmoManager.Instance.UpdateAmmo(gameItemAmmo.AmmoType);
			}
		}

		private void ProceedBonus(GameItem item)
		{
			GameItemBonus gameItemBonus = item as GameItemBonus;
			if (gameItemBonus == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as bonus.");
				return;
			}
			switch (gameItemBonus.BonusType)
			{
			case BonusTypes.VIP:
				if (gameItemBonus.BonusValue > PlayerInfoManager.VipLevel)
				{
					PlayerInfoManager.VipLevel = gameItemBonus.BonusValue;
				}
				break;
			case BonusTypes.Money:
				PlayerInfoManager.Money += gameItemBonus.BonusValue;
				break;
			}
		}

		private void ProceedPowerUp(GameItem item, bool equipOnly = false)
		{
			GameItemPowerUp gameItemPowerUp = item as GameItemPowerUp;
			if (gameItemPowerUp == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as PowerUp.");
			}
			else if (AlredyEquiped(gameItemPowerUp) && !equipOnly)
			{
				gameItemPowerUp.Deactivate();
			}
			else
			{
				gameItemPowerUp.Activate();
			}
		}

		private void ProceedAbility(GameItem item)
		{
			GameItemAbility gameItemAbility = item as GameItemAbility;
			if (gameItemAbility == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as ability.");
			}
			else if (AlredyEquiped(gameItemAbility))
			{
				UnequipAbility(gameItemAbility);
			}
			else
			{
				ShopManager.Instance.OpenDialogPanel(gameItemAbility);
			}
		}

		public void EquipAbility(GameItemAbility abilityItem, int slotIndex)
		{
			if (!(abilityItem == null))
			{
				PlayerAbilityManager.AddAbility(abilityItem, slotIndex);
				GameItemSkin[] relatedSkins = abilityItem.RelatedSkins;
				foreach (GameItemSkin skin in relatedSkins)
				{
					EquipSkin(skin);
				}
			}
		}

		private void UnequipAbility(GameItemAbility abilityItem, bool withSkin = true)
		{
			if (abilityItem == null)
			{
				return;
			}
			PlayerAbilityManager.RemoveAbility(abilityItem);
			if (!withSkin)
			{
				return;
			}
			GameItemSkin[] relatedSkins = abilityItem.RelatedSkins;
			foreach (GameItemSkin skin in relatedSkins)
			{
				if (PlayerAbilityManager.SkinCanBeRemoved(skin, abilityItem))
				{
					UnequipSkin(skin, withAbility: false);
				}
			}
		}

		private void ClearSkinSlot(SkinSlot slot, StuffHelper helper)
		{
			SkinnedMeshRenderer renderer = helper.SlotRenderers.GetRenderer(slot);
			if (renderer != null)
			{
				renderer.gameObject.SetActive(value: false);
			}
		}

		private void ClearPlaceholder(Transform placeholder)
		{
			if (!(placeholder == null))
			{
				for (int i = 0; i < placeholder.childCount; i++)
				{
					UnityEngine.Object.Destroy(placeholder.GetChild(i).gameObject);
				}
			}
		}

		private void ProceedVehicle(GameItem item)
		{
			GameItemVehicle gameItemVehicle = item as GameItemVehicle;
			if (gameItemVehicle == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as vehicle.");
			}
			else
			{
				GarageManager.Instance.SetVehicle(gameItemVehicle);
			}
		}

		private void EquipAll()
		{
			if (!(CurrentHelper == null))
			{
				List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
				list.AddRange(PlayerStoreProfile.CurrentLoadout.Weapons.ToList());
				List<int> list2 = new List<int>();
				list2.AddRange(PlayerStoreProfile.CurrentLoadout.Skin.Values.ToList());
				foreach (KeyValuePair<string, int> item in list)
				{
					GameItemWeapon weaponItem = ItemsManager.Instance.GetItem(item.Value) as GameItemWeapon;
					EquipWeapon(weaponItem, Loadout.WeaponSlotFromKey(item.Key), equipOnly: true);
				}
				foreach (int item2 in list2)
				{
					EquipItem(ItemsManager.Instance.GetItem(item2), equipOnly: true);
				}
			}
		}

		public static bool AlredyEquiped(GameItem item)
		{
			string key;
			return AlredyEquiped(item, out key);
		}

		public static bool AlredyEquiped(GameItem item, out string key)
		{
			key = string.Empty;
			if (item is GameItemPowerUp)
			{
				return (item as GameItemPowerUp).isActive;
			}
			if (item is GameItemVehicle)
			{
				return GarageManager.Instance.MainRespawner.ObjectPrefab == (item as GameItemVehicle).VehiclePrefab;
			}
			if (item is GameItemAbility)
			{
				return PlayerAbilityManager.IsAbilityAdded((GameItemAbility)item);
			}
			foreach (KeyValuePair<string, int> weapon in PlayerStoreProfile.CurrentLoadout.Weapons)
			{
				if (weapon.Value == item.ID)
				{
					key = weapon.Key;
					return true;
				}
			}
			foreach (KeyValuePair<string, int> item2 in PlayerStoreProfile.CurrentLoadout.Skin)
			{
				if (item2.Value == item.ID)
				{
					key = item2.Key;
					return true;
				}
			}
			return false;
		}

		public static bool SkinSlotIsEmpty(SkinSlot slot)
		{
			if (PlayerStoreProfile.CurrentLoadout.Skin[Loadout.KeyFromSkinSlot[slot]] == 0)
			{
				return true;
			}
			return false;
		}

		public static bool WeaponSlotIsEmpty(int slotIndex)
		{
			return PlayerManager.Instance.DefaultWeaponController.WeaponSet.SlotIsEmpty(slotIndex);
		}

		public static int IDInSkinSlot(SkinSlot slot)
		{
			return PlayerStoreProfile.CurrentLoadout.Skin[Loadout.KeyFromSkinSlot[slot]];
		}

		public static GameItem ItemInSkinSlot(SkinSlot slot)
		{
			int id = IDInSkinSlot(slot);
			return ItemsManager.Instance.GetItem(id);
		}

		public List<GameItem> GetSkinConflicts(GameItem item)
		{
			GameItemSkin gameItemSkin = item as GameItemSkin;
			List<GameItem> list = new List<GameItem>();
			if (gameItemSkin == null)
			{
				UnityEngine.Debug.LogWarning("Trying to proceed unknown item as skin or accessory.");
				return list;
			}
			for (int i = 1; i < gameItemSkin.OccupiedSlots.Length; i++)
			{
				GameItemSkin gameItemSkin2 = ItemInSkinSlot(gameItemSkin.OccupiedSlots[i]) as GameItemSkin;
				if (gameItemSkin2 != null && gameItemSkin2.OccupiedSlots[0] != gameItemSkin.OccupiedSlots[0])
				{
					list.Add(gameItemSkin2);
				}
			}
			return list;
		}

		private void OnDestroy()
		{
			ActivePowerUps.Clear();
		}
	}
}
