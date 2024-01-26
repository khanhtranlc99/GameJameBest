using System;
using Root.Scripts.Helper;
using TMPro;
using UI.DailyMission;
using UnityEngine;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.DailyMissionManager.Scripts
{
    public class UIDailyMission : AbsUICanvas
    {
        [SerializeField] private Button btnClaim;
        [SerializeField] private GameObject objVideo, btnClose;
        [SerializeField] private TextMeshProUGUI txtAmountReward;
        [SerializeField] private Image imgUnitReward;
        [SerializeField] private Sprite sprMoney, sprDiamond;
        [SerializeField] private DailyMissionElement[] allElements;
        private float counterOffVideo;

        private DailyMissionData currentMission;
        public override void StartLayer()
        {
            base.StartLayer();
            btnClaim.onClick.AddListener(OnClickClaim);
        }

        public void LoadDataMission()
        {
            DailyMissionElement tempElement;
            for (int i = 0; i < allElements.Length; i++)
            {
                tempElement = allElements[i];
                tempElement.SetActive(i < currentMission.AllMissions.Length);
            }

            var isMoney = currentMission.money != 0;
            if (isMoney)
            {
                txtAmountReward.text = currentMission.money.ToString();
                imgUnitReward.sprite = sprMoney;
            }
            else
            {
                txtAmountReward.text = currentMission.gems.ToString();
                imgUnitReward.sprite = sprDiamond;
            }
        }

        public override void ShowLayer()
        {
            base.ShowLayer();
            currentMission = UI.DailyMission.DailyMissionManager.Instance.GetCurrentMission;
            counterOffVideo = 3;
            LoadDataMission();
            GameplayUtils.PauseGame();
            //AdManager.Instance.ToggleBannerVisibility(false);
            Refresh();
        }
        private void OnDisable()
        {
            //AdManager.Instance.ToggleBannerVisibility(true);
        }

        public override void Close()
        {
            base.Close();
            GameplayUtils.ResumeGame();
        }

        private void OnClickClaim()
        {
            //var isVideoReward = objVideo.activeSelf;
            //if (isVideoReward)
            //{
            //    AdManager.Instance.ShowRewardVideo(OnWatchVideoDone);
            //}
            //else
            //{
                UI.DailyMission.DailyMissionManager.Instance.ClaimReward();
                Close();
            //}

        }

        private void OnWatchVideoDone(bool isDone)
        {
            if (!isDone) return;
            UI.DailyMission.DailyMissionManager.Instance.ClaimReward(true);
            Close();
        }

        private void LateUpdate()
        {
            if (!objVideo.activeSelf) return;
            counterOffVideo -= Time.unscaledDeltaTime;
            if (counterOffVideo <= 0)
                objVideo.SetActive(false);
        }
        private void Refresh()
        {
            for (int i = 0; i < currentMission.AllMissions.Length; i++)
            {
                var currentAmountDone =
                    UI.DailyMission.DailyMissionManager.Instance.GetTaskProgress(currentMission.AllMissions[i].dailyMissionType);
                var isDone = currentAmountDone >= currentMission.AllMissions[i].goalAmount;
                allElements[i].SetProcess(string.Format("{0}/{1}", currentAmountDone, currentMission.AllMissions[i].goalAmount));
                allElements[i].SetDone(isDone);
                allElements[i].SetDecription(currentMission.AllMissions[i].description);
            }
            //if (UI.DailyMission.DailyMissionManager.Instance.isDoneTodayMission)
            //{
            //    btnClaim.gameObject.SetActive(true);
            //    btnClose.gameObject.SetActive(false);
            //    var isMoney = currentMission.money != 0;

            //    txtAmountReward.text =
            //        isMoney ? (currentMission.money * 3).ToString() : (currentMission.gems * 3).ToString();
            //    if (AdManager.Instance.IsRewardVideoLoaded())
            //    {
            //        objVideo.SetActive(true);
            //        Invoke(nameof(DisableVideoReward), 3f);
            //    }
            //}
            //else
            //{
                btnClaim.gameObject.SetActive(false);
                btnClose.gameObject.SetActive(true);
            //}
        }

        private void DisableVideoReward()
        {
            objVideo.SetActive(false);
            var isMoney = currentMission.money != 0;
            txtAmountReward.text =
                isMoney ? currentMission.money.ToString() : currentMission.gems.ToString();
        }
    }
}
