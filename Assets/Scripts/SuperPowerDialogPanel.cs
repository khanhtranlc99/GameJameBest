using Game.Character.Superpowers;
using Game.Items;
using Game.Shop;
using Game.Weapons;
using UnityEngine;

public class SuperPowerDialogPanel : ShopDialogPanel
{
	public override void ProceedSlot(DialogSlotHelper helper)
	{
		DialogSuperpowerSlotHelper dialogSuperpowerSlotHelper = helper as DialogSuperpowerSlotHelper;
		GameItemAbility gameItemAbility = currItem as GameItemAbility;
		if (!(dialogSuperpowerSlotHelper == null) && !(gameItemAbility == null))
		{
			StuffManager.Instance.EquipAbility(gameItemAbility, dialogSuperpowerSlotHelper.SlotIndex);
			ShopManager.Instance.UpdateInfo();
			Deinit();
		}
	}

	public override void BuySlot(DialogSlotHelper helper)
	{
	}

	public override bool CheckAvailable(DialogSlotHelper helper, GameItem item)
	{
		DialogSuperpowerSlotHelper dialogSuperpowerSlotHelper = helper as DialogSuperpowerSlotHelper;
		GameItemAbility gameItemAbility = item as GameItemAbility;
		if (dialogSuperpowerSlotHelper == null || gameItemAbility == null)
		{
			return false;
		}
		return !(dialogSuperpowerSlotHelper.Active ^ gameItemAbility.IsActive);
	}

	public override Sprite GetImage(DialogSlotHelper helper)
	{
		DialogSuperpowerSlotHelper dialogSuperpowerSlotHelper = helper as DialogSuperpowerSlotHelper;
		if (dialogSuperpowerSlotHelper == null)
		{
			return null;
		}
		Sprite result = null;
		GameItemAbility gameItemAbility = null;
		gameItemAbility = ((!dialogSuperpowerSlotHelper.Active) ? PlayerAbilityManager.PasiveAbilities[dialogSuperpowerSlotHelper.SlotIndex] : PlayerAbilityManager.ActiveAbilities[dialogSuperpowerSlotHelper.SlotIndex]);
		if (gameItemAbility != null)
		{
			result = gameItemAbility.ShopVariables.ItemIcon;
		}
		return result;
	}
}
