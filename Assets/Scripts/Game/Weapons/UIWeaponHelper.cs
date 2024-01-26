using Game.Character;
using Game.Character.CharacterController;
using Game.Shop;
using RopeHeroViceCity.Scripts.Gampelay;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Weapons
{
	public class UIWeaponHelper : MonoBehaviour
	{
		public int RelatedSlotIndex;

		public Button Button;

		public Image ButtonIcon;

		public Image WeaponIcon;

		public Sprite EmptySlotIcon;

		public GameObject WaterMark;
		public bool ignoreTransparent;

		public WeaponSlot RelatedSlot
		{
			get
			{
				WeaponController weaponController = PlayerManager.Instance.WeaponController;
				return weaponController.WeaponSet.Slots[RelatedSlotIndex];
			}
		}

		private bool SlotIsLocked => RelatedSlot.BuyToUnlock != null && !PlayerInfoManager.Instance.BoughtAlredy(RelatedSlot.BuyToUnlock);

		private void Awake()
		{
			if (Button == null)
			{
				Button = GetComponent<Button>();
			}
			if (ButtonIcon == null)
			{
				ButtonIcon = GetComponent<Image>();

			}
			if (ignoreTransparent)
				ButtonIcon.alphaHitTestMinimumThreshold = .1f;
		}

		public void OnClick()
		{
			if (SlotIsLocked)
			{
				WeaponDialogPanel.BuyWeaponSlot(RelatedSlot.BuyToUnlock, UpdateImage);
				return;
			}
			PlayerManager.Instance.WeaponController.ChooseSlot(RelatedSlotIndex);
			WeaponManager.Instance.CloseWeaponPanel();
		}

		private void OnEnable()
		{
			UpdateImage();
		}

		private void UpdateImage()
		{
			Weapon weaponPrefab = PlayerManager.Instance.WeaponController.WeaponSet.Slots[RelatedSlotIndex].WeaponPrefab;
			if (weaponPrefab == null)
			{
				Button.interactable = SlotIsLocked;
				WeaponIcon.sprite = ((!SlotIsLocked) ? EmptySlotIcon : ResourcesManager.Instance.ShopIcons.LockedSlotSprite);

				if ((bool)WaterMark)
				{
					WaterMark.SetActive(value: true);
				}
			}
			else
			{
				Button.interactable = true;
				WeaponIcon.sprite = weaponPrefab.image;
				if ((bool)WaterMark)
				{
					WaterMark.SetActive(value: false);
				}
			}
			WeaponIcon.SetNativeSize();
		}
	}
}
