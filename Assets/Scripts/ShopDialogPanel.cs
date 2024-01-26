using Game.Items;
using Game.Shop;
using UnityEngine;

public class ShopDialogPanel : MonoBehaviour
{
	public ItemsTypes[] DialogPanelTypes;

	public DialogSlotHelper[] DialogHelpers;

	protected GameItem currItem;

	public void UpdatePanel()
	{
		UpdatePanel(null);
	}

	public void UpdatePanel(GameItem item)
	{
		if (item != null)
		{
			currItem = item;
		}
		DialogSlotHelper[] dialogHelpers = DialogHelpers;
		foreach (DialogSlotHelper dialogSlotHelper in dialogHelpers)
		{
			dialogSlotHelper.UpdateSlot(CheckAvailable(dialogSlotHelper, currItem), GetImage(dialogSlotHelper), CheckHighlighted(dialogSlotHelper, currItem));
		}
	}

	public void Deinit()
	{
		ShopManager.Instance.CloseDialogPanel();
	}

	public virtual void ProceedSlot(DialogSlotHelper helper)
	{
	}

	public virtual void BuySlot(DialogSlotHelper helper)
	{
	}

	public virtual bool CheckAvailable(DialogSlotHelper helper, GameItem item)
	{
		return true;
	}

	public virtual bool CheckHighlighted(DialogSlotHelper helper, GameItem item)
	{
		return false;
	}

	public virtual Sprite GetImage(DialogSlotHelper helper)
	{
		return null;
	}
}
