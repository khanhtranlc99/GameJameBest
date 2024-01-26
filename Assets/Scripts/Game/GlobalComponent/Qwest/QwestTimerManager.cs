using Game.GlobalComponent.HelpfulAds;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Qwest
{
    public class QwestTimerManager : MonoBehaviour
    {
        private static QwestTimerManager instance;

        public Text TimerText;

        private bool countdown;

        private float currentTimerValue;

        private float maxTimerValue;

        private Qwest currentQwest;

        private float displayedMin;

        private float displayedSec;

        private bool onPopup;

        public static QwestTimerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("Qwest Timer Manager not found!");
                }
                return instance;
            }
        }

        public void StartCountdown(float timerValue, Qwest qwest)
        {
            countdown = true;
            maxTimerValue = timerValue;
            currentTimerValue = timerValue;
            currentQwest = qwest;
            UpdateDisplayedTime();
            TimerText.gameObject.SetActive(value: true);
        }

        public void AddAdditionalTime(float value)
        {
            currentTimerValue += value;
            onPopup = false;
        }

        public void AddAdditionalTimeOfProcentMaxTime(float procent)
        {
            currentTimerValue += maxTimerValue * procent;
            onPopup = false;
            InGameLogManager.Instance.RegisterNewMessage(MessageType.AddQuestTime, ((int)(maxTimerValue * procent)).ToString());
        }

        public void EndCountdown()
        {
            countdown = false;
            TimerText.text = string.Empty;
            displayedMin = 0f;
            displayedSec = 0f;
            currentTimerValue = 0f;
            currentQwest = null;
            TimerText.gameObject.SetActive(value: false);
        }

        public void QwestCanceled(Qwest qwest)
        {
            if (qwest == currentQwest)
            {
                EndCountdown();
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void FixedUpdate()
        {
            if (!countdown)
            {
                return;
            }
            if (currentTimerValue > 0f)
            {
                currentTimerValue -= Time.deltaTime;
                if (currentTimerValue < 5f && HelpfullAdsManager.Instance != null)
                {
                    if (!onPopup)
                    {
                        onPopup = true;
                        HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Time, null);
                    }
                }
                UpdateDisplayedTime();
            }
            else
            {
                onPopup = false;
                GameEventManager.Instance.QwestFailed(currentQwest);
                EndCountdown();
            }
        }

        private void UpdateDisplayedTime()
        {
            float num = Mathf.Floor(currentTimerValue / 60f);
            float num2 = Mathf.Floor(currentTimerValue % 60f);
            if (Math.Abs(displayedMin - num) > 0f || Math.Abs(displayedSec - num2) > 0f)
            {
                TimerText.text = num.ToString("00") + ":" + num2.ToString("00");
                displayedMin = num;
                displayedSec = num2;
            }
        }
    }
}
