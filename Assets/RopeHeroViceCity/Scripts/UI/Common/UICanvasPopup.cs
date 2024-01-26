using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UICanvasPopup : AbsUICanvas
{

    #region Properties

    [Space(20)]
    public TextMeshProUGUI txtInfo;
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtBtOk;
    public TextMeshProUGUI txtBtCancel;

    public Button btnOk;
    public Button btnNotOk;
    public Button btnClose;

    public GameObject gButtonGroup;

    private Action<bool> ResultAction;

    #endregion

    #region Implement

    public override void StartLayer()
    {
        base.StartLayer();
        btnNotOk.onClick.AddListener(OnClickButtonNotOk);
        btnOk.onClick.AddListener(OnClickButtonOk);
        btnClose.onClick.AddListener(OnClickButtonCancel);
    }
    public override void HideLayer()
    {
        base.HideLayer();

        gButtonGroup.SetActive(false);

        btnOk.gameObject.SetActive(false);
        btnNotOk.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
    }

    #endregion

    #region Listener

    public void OnClickButtonOk()
    {
        Close();
        if (ResultAction != null)
            ResultAction(true);
    }

    public void OnClickButtonNotOk()
    {
        Close();
        if (ResultAction != null)
            ResultAction(false);
    }

    public void OnClickButtonCancel()
    {
        Close();
        if (ResultAction != null)
            ResultAction(false);
    }

    #endregion

    #region Method
    
    
    
    public void ShowPopup(string title = StringHelper.TITTLE_OOPS, string strInfo = "", string strBtOK = "", string strBtnNotOk = "", System.Action<bool> action = null, bool isClose = false)
    {
        this.ResultAction = action;

        txtInfo.text = strInfo;
        txtTitle.text = title;

        if (!string.IsNullOrEmpty(strBtOK) || !string.IsNullOrEmpty(strBtnNotOk) || isClose)
        {
            gButtonGroup.SetActive(true);

            if (!string.IsNullOrEmpty(strBtOK) || isClose)
            {
                btnOk.gameObject.SetActive(true);
                txtBtOk.text = !string.IsNullOrEmpty(strBtOK) ?  strBtOK : StringHelper.CONTENT_OK;
            }
            else
            {
                btnOk.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(strBtnNotOk))
            {
                btnNotOk.gameObject.SetActive(true);
                txtBtCancel.text = strBtnNotOk;
            }
            else
            {
                btnNotOk.gameObject.SetActive(false);
            }
        }
        else
        {
            gButtonGroup.SetActive(false);
        }

        btnClose.gameObject.SetActive(isClose);
    }

    #endregion


    #region Open Popup
    public static void OpenPopup(string title, string content, bool isClose = true)
    {
        UICanvasPopup Cpopup = UICanvasController.Instance.GetLayer<UICanvasPopup>();
        if (Cpopup != null && Cpopup.txtInfo.text.Equals(content))
            return;
        ((UICanvasPopup)UICanvasController.Instance.ShowLayer(UICanvasKey.POPUP)).ShowPopup(title: title, strInfo: content, isClose: isClose);
    }

    public static void OpenPopup(string title, string content, Action<bool> action, bool isClose)
    {
        UICanvasPopup lPopup = UICanvasController.Instance.GetLayer<UICanvasPopup>();
        if (lPopup != null && lPopup.txtInfo.text.Equals(content))
            return;
        ((UICanvasPopup)UICanvasController.Instance.ShowLayer(UICanvasKey.POPUP)).ShowPopup(title: title, strInfo: content, strBtOK: "OK", action: action, isClose: isClose);
    }

    public static void OpenPopup(string title, string content, string strBtOk, string strBtCancel, Action<bool> action, bool isClose)
    {
        UICanvasPopup lPopup = UICanvasController.Instance.GetLayer<UICanvasPopup>();
        if (lPopup != null && lPopup.txtInfo.text.Equals(content))
            return;

        ((UICanvasPopup)UICanvasController.Instance.ShowLayer(UICanvasKey.POPUP)).ShowPopup(title: title, strInfo: content, strBtOK: strBtOk, strBtnNotOk: strBtCancel, action: action, isClose: isClose);
    }

    //public static void OpenPopupTop(string title, string content)
    //{
    //    ((UICanvasPopup)UICanvasController.Instance.ShowLayer(UICanvasKey.POPUP)).ShowPopup(title: title, strInfo: content, isClose: true);
    //}

    //public static void OpenPopupTop(string title, string content, Action<bool> action, bool isClose)
    //{
    //    UICanvasPopup CPopup = UICanvasController.Instance.GetLayer<UICanvasPopup>();
    //    if (CPopup != null && CPopup.txtInfo.text.Equals(content))
    //        return;
    //    ((UICanvasPopup)UICanvasController.Instance.ShowLayer(UICanvasKey.POPUP)).ShowPopup(title: title, strInfo: content, strBtOK: "OK", action: action, isClose: isClose);
    //}

    //public static void OpenPopupTop(string title, string content, string strBtOk, string strBtCancel, Action<bool> action, bool isClose)
    //{
    //    UICanvasPopup CPopup = UICanvasController.Instance.GetLayer<UICanvasPopup>();
    //    if (CPopup != null && CPopup.txtInfo.text.Equals(content))
    //        return;

    //    ((UICanvasPopup)UICanvasController.Instance.ShowLayer(UICanvasKey.POPUP)).ShowPopup(title: title, strInfo: content, strBtOK: strBtOk, strBtClose: strBtCancel, action: action, isClose: isClose);
    //}

    #endregion

}
