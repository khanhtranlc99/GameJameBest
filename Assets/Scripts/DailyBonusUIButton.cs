using RopeHeroViceCity.UI_Features.DailyReward.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusUIButton : MonoBehaviour
{
    public Image Image;
    public Text txtAmount;
    public GameObject blackLayer;
    public Image Frame;
    public Text dayText;
    [SerializeField] private Sprite selectedFrameSprite;

    public void SetData(Sprite sprite, int amount, string dayText, bool isClaimed = false, bool isCurrentIndex = false)
    {
        Image.sprite = sprite;
        this.dayText.text = dayText;
        txtAmount.text = amount.ToString();
        blackLayer.SetActive(isClaimed);
        if (isCurrentIndex)
        {
            Frame.sprite = selectedFrameSprite;
        }
    }
}
