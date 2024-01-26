using DG.Tweening;
using Game.Character;
using Root.Scripts.Helper;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyStaminaBox : AbsRopePopup
{
    //public GameObject content;
    public GameObject claimBtn;

    public void ShowADS()
    {
        //content.gameObject.SetActive(true);
        claimBtn.transform.DOKill();
        claimBtn.transform.localScale = 1.2f * Vector3.one;
        claimBtn.transform.DOScale(1.35f, 0.3f).SetLoops(-1, LoopType.Yoyo);
    }

    public void OnClickClaim()
    {
        //if (AdManager.Instance.IsRewardVideoLoaded())
        //{
        //    AdManager.Instance.ShowRewardVideo((s) =>
        //    {
        //        PlayerInteractionsManager.Instance.Player.stats.stamina.Set(300);
                Close();
                //content.gameObject.SetActive(false);
        //    });
        //}
        //else
        //{
            //UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.ADS_NOT_AVAILABLE);
        //}
    }

    //public override void Close()
    //{
    //    if (canvasGroup)
    //        canvasGroup.interactable = false;
    //    if (layerAnimType == AnimType.None)
    //        graphicRaycaster.enabled = false;
    //    UICanvasController.Instance.HideLayer(this);
    //}
}
