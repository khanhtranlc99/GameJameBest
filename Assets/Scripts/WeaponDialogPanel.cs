using Game.Character.CharacterController;
using Game.Items;
using Game.Shop;
using Game.UI;
using Game.Weapons;
using System;
using Game.Character;
using RopeHeroViceCity.Scripts.Gampelay;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;

public class WeaponDialogPanel : ShopDialogPanel
{
    public static void BuyWeaponSlot(GameItem weaponSlot, Action additionalAction = null)
    {
        bool disableYesBitton = !ShopManager.Instance.ItemAvailableForBuy(weaponSlot) || !ShopManager.Instance.EnoughMoneyToBuyItem(weaponSlot);
        string message = "Do you realy want to buy " + weaponSlot.ShopVariables.Name + " for " + weaponSlot.ShopVariables.price + " gems?";
        EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.OFF_ADS);
        UI_GeneralPopup.ShowPopup("Buy slot?", message, true, delegate
         {
             ShopManager.Instance.Buy(weaponSlot);
             ShopManager.Instance.UpdateDialogPanel();
             if (additionalAction != null)
             {
                 additionalAction();
             }
         }, null, disableYesBitton);
    }

    public override void ProceedSlot(DialogSlotHelper helper)
    {
        DialogWeaponSlotHelper dialogWeaponSlotHelper = helper as DialogWeaponSlotHelper;
        if (!(dialogWeaponSlotHelper == null))
        {
            StuffManager.Instance.EquipWeapon(currItem as GameItemWeapon, dialogWeaponSlotHelper.SlotIndex);
            Deinit();
        }
    }

    public override void BuySlot(DialogSlotHelper helper)
    {
        DialogWeaponSlotHelper dialogWeaponSlotHelper = helper as DialogWeaponSlotHelper;
        if (!(dialogWeaponSlotHelper == null))
        {
            BuyWeaponSlot(dialogWeaponSlotHelper.RelatedSlot.BuyToUnlock);
        }
    }

    public override bool CheckAvailable(DialogSlotHelper helper, GameItem item)
    {
        DialogWeaponSlotHelper dialogWeaponSlotHelper = helper as DialogWeaponSlotHelper;
        if (dialogWeaponSlotHelper == null)
        {
            return false;
        }
        GameItemWeapon gameItemWeapon = item as GameItemWeapon;
        if (gameItemWeapon == null)
        {
            return false;
        }
        WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
        dialogWeaponSlotHelper.BuyFirst = (dialogWeaponSlotHelper.RelatedSlot.BuyToUnlock != null && !PlayerInfoManager.Instance.BoughtAlredy(dialogWeaponSlotHelper.RelatedSlot.BuyToUnlock));
        return dialogWeaponSlotHelper.RelatedSlot.WeaponSlotType == defaultWeaponController.GetTargetSlot(gameItemWeapon.Weapon) || dialogWeaponSlotHelper.RelatedSlot.WeaponSlotType == WeaponSlotTypes.Universal;
    }

    public override Sprite GetImage(DialogSlotHelper helper)
    {
        DialogWeaponSlotHelper dialogWeaponSlotHelper = helper as DialogWeaponSlotHelper;
        if (dialogWeaponSlotHelper == null)
        {
            return null;
        }
        WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
        Sprite result = null;
        if (dialogWeaponSlotHelper.BuyFirst)
        {
            result = ResourcesManager.Instance.ShopIcons.LockedSlotSprite;
        }
        else
        {
            Weapon weapon = defaultWeaponController.WeaponSet.WeaponInSlot(dialogWeaponSlotHelper.SlotIndex);
            if (weapon != null)
            {
                result = weapon.image;
            }
        }
        return result;
    }
}
