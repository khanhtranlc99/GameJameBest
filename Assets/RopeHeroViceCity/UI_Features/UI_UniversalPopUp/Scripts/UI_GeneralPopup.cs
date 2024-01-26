using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts
{
    public class UI_GeneralPopup : AbsRopePopup
    {
        [SerializeField] private RectTransform contentHeader;

        [SerializeField] private TextMeshProUGUI txtHeader;

        [SerializeField] private TextMeshProUGUI txtMessenger;

        [SerializeField] private Button btnYes, btnNo;

        private Action currentYesAction;

        private Action currentNoAction;
        public bool offAds;


        public override void StartLayer()
        {
            base.StartLayer();
            btnYes.onClick.AddListener(OnClickYes);
            btnNo.onClick.AddListener(OnClickNo);
        }

        public void OpenPopup(string header, string message, Action onYesAction, Action onNoAction,
            bool disableYesButton)
        {
            var isNullHeader = string.IsNullOrEmpty(header);
            if (!isNullHeader)
            {
                contentHeader.gameObject.SetActive(true);
                txtHeader.text = header;
            }
            else
            {
                contentHeader.gameObject.SetActive(false);
            }
            txtMessenger.text = message;
            currentYesAction = onYesAction;
            currentNoAction = onNoAction;
            btnYes.interactable = !disableYesButton;
            if (!GameplayUtils.OnPause)
            {
                GameplayUtils.PauseGame();
            }
        }

        private void OnClickYes()
        {
            if (offAds)
            {
                EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.ON_ADS);
            }
            if (currentYesAction != null)
            {
                currentYesAction();
            }
            Close();
        }

        private void OnClickNo()
        {
            if (offAds)
            {
                EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.ON_ADS);
            }
            if (currentNoAction != null)
            {
                currentNoAction();
            }
            Close();
        }

        public override void Close()
        {
            base.Close();
            if (GameplayUtils.OnPause)
            {
                GameplayUtils.ResumeGame();
            }

            currentYesAction = null;
            currentNoAction = null;
        }

        public static void ShowPopup(string header, string message, bool offAds = false, Action onYesAction = null, Action onNoAction = null,
            bool disableYesButton = false)
        {
            var popup = (UI_GeneralPopup)UICanvasController.Instance.ShowLayer(UICanvasKey.POPUP);
            popup.offAds = offAds;
            if (popup is { }) popup.OpenPopup(header, message, onYesAction, onNoAction, disableYesButton);
        }
    }
}