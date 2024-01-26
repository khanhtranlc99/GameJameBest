using System;

namespace Game.Weapons
{
	[Serializable]
	public class WeaponSet
	{
		public WeaponSlot[] Slots;

		public bool Locked;

		public bool SlotIsEmpty(int slotIndex)
		{
			return WeaponInSlot(slotIndex) == null;
		}

		public Weapon WeaponInSlot(int slotIndex)
		{
			if (slotIndex < 0 || slotIndex >= Slots.Length)
			{
				return null;
			}
			return Slots[slotIndex].WeaponPrefab;
		}

		public int FirstSlotOfType(WeaponSlotTypes wantedType)
		{
			for (int i = 0; i < Slots.Length; i++)
			{
				WeaponSlot weaponSlot = Slots[i];
				if (weaponSlot.WeaponSlotType == wantedType)
				{
					return i;
				}
			}
			return -1;
		}

		public WeaponSlot GetFirstSlotOfType(WeaponSlotTypes wantedType)
		{
			for (int i = 0; i < Slots.Length; i++)
			{
				WeaponSlot weaponSlot = Slots[i];
				if (weaponSlot.WeaponSlotType == wantedType)
				{
					return weaponSlot;
				}
			}
			return null;
		}

		public WeaponSlot GetEmptySlotOfType(WeaponSlotTypes wantedType)
		{
			WeaponSlot[] slots = Slots;
			foreach (WeaponSlot weaponSlot in slots)
			{
				if (weaponSlot.WeaponSlotType == wantedType && weaponSlot.WeaponPrefab == null)
				{
					return weaponSlot;
				}
			}
			return null;
		}

		public int GetEmptySlotOfTypeInt(WeaponSlotTypes wantedType,bool isAvailableExcept = false)
		{
			for (int i = 0; i < Slots.Length; i++)
			{
				WeaponSlot weaponSlot = Slots[i];
				if ((weaponSlot.WeaponSlotType == wantedType || weaponSlot.WeaponSlotType == WeaponSlotTypes.Universal) && (weaponSlot.Available && weaponSlot.WeaponPrefab == null || isAvailableExcept))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
