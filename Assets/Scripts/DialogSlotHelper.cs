using UnityEngine;
using UnityEngine.UI;

public class DialogSlotHelper : MonoBehaviour
{
    public ShopDialogPanel DialogPanel;

    public bool BuyFirst;

    public int SlotIndex;

    [Space(10f)]
    public Button Button;

    public Image ButtonIcon;

    public Image ItemIcon;

    public Sprite EmptySlotIcon;

    [Space(10f)]
    public Color AvailavleColor;

    public Color NotAvailavleColor;

    public Color HighlightedColor;

    public bool ignoreTransparent;

    private void Start()
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
        if (!BuyFirst)
        {
            DialogPanel.ProceedSlot(this);
        }
        else
        {
            DialogPanel.BuySlot(this);
        }
    }

    public virtual void UpdateSlot(bool IsAvailable, Sprite sprite, bool highlighted)
    {
        if (IsAvailable)
        {
            // Button.interactable = true;
            ButtonIcon.color = AvailavleColor;
        }
        else
        {
            // Button.interactable = false;
            ButtonIcon.color = NotAvailavleColor;
        }
        if (highlighted)
        {
            ButtonIcon.color = HighlightedColor;
        }
        if ((bool)sprite)
        {
            ItemIcon.sprite = sprite;
            //ItemIcon.SetNativeSize();
            ItemIcon.gameObject.SetActive(value: true);
        }
        else
        {
            ItemIcon.gameObject.SetActive(value: false);
        }
    }
}
