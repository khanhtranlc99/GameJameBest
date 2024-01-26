using System;
using System.Collections.Generic;
using Game.Character;
using Game.Shop;
using Root.Scripts.Helper;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.DailyReward.Scripts
{
    public class UI_DailyReward : AbsUICanvas
    {
        public DailyBonusData dataBonus;

        [Space(10f)]
        public ScrollRect BonusListRect;

        public GameObject EmptyBonus;

        public GameObject GetBonusButton;
        public GameObject GetBonusX2Button;

        //public GameObject objVideoReward;
        private int currentBonusIndex;

        private int todayBonusIndex;

        private long bonusReceivingTime;

        private bool inited;
        private int multipleValue = 1;

        public bool SomeBonusAvailable
        {
            get
            {
                return TimeManager.AnotherDay(bonusReceivingTime);
            }
        }
        public override void StartLayer()
        {
            base.StartLayer();
            Init();
        }

        public void Init()
        {
            multipleValue = 1;
            if (!inited)
            {
                currentBonusIndex = BaseProfile.ResolveValue("BonusIndex", 0);
                todayBonusIndex = currentBonusIndex;
                bonusReceivingTime = BaseProfile.ResolveValue("BonusReceiving", 0L);
                GenerateBonusList();
                inited = true;
            }
        }

        public static bool CanReceiveRewardToday()
        {
            return TimeManager.AnotherDay(BaseProfile.ResolveValue("BonusReceiving", 0L));
        }
        public DailyBonus GetTodayBonus()
        {
            return dataBonus.Bonuses[todayBonusIndex];
        }

        public void ProceedCurrentBonus()
        {
            ClaimReward();
        }

        public void ProceedCurrentBonusX2()
        {
            //if (objVideoReward.activeSelf)
            //{
            //if (AdManager.Instance.IsRewardVideoLoaded())
            //{
            //    AdManager.Instance.ShowRewardVideo(OnWatchVideoDone);

            //}
            //else
            //{
                UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.ADS_NOT_AVAILABLE);
            //}
            //}
        }

        private void ClaimReward()
        {
            ProceedBonus(GetTodayBonus());
            currentBonusIndex = todayBonusIndex + 1;
            if (currentBonusIndex >= dataBonus.Bonuses.Length)
            {
                currentBonusIndex = 0;
            }
            bonusReceivingTime = DateTime.Today.ToFileTime();
            BaseProfile.StoreValue(currentBonusIndex, "BonusIndex");
            BaseProfile.StoreValue(bonusReceivingTime, "BonusReceiving");
            Close();
            multipleValue = 1;
        }

        private void OnWatchVideoDone(bool isDone)
        {
            multipleValue = 2;
            OnAds();
            ClaimReward();
        }

        private void ProceedBonus(DailyBonus bonus)
        {
            PlayerInfoManager.Money += bonus.Money * multipleValue;
            PlayerInfoManager.Gems += bonus.Gems * multipleValue;
            if (bonus.Item != null)
            {
                PlayerInfoManager.Instance.Give(bonus.Item);
            }
        }

        public override void ShowLayer()
        {
            base.ShowLayer();
            BonusListRect.transform.parent.parent.gameObject.SetActive(value: true);
            //bool allowClaim = inited && CanReceiveRewardToday();
            //if (allowClaim)
            //{
            //    GetBonusButton.SetActive(true);
            //    if (AdManager.Instance.IsRewardVideoLoaded())
            //    {
            //        objVideoReward.SetActive(true);
            //        Invoke(nameof(DisableVideoReward), 3f);
            //    }
            //}
            //else
            //{
                GetBonusButton.SetActive(false);
                GetBonusX2Button.SetActive(false);
            //}
        }
        private void DisableVideoReward()
        {
            //objVideoReward.SetActive(false);
            GetBonusX2Button.SetActive(false);
        }

        private void GenerateBonusList()
        {
            var Bonuses = dataBonus.Bonuses;
            DailyBonus temp = null;
            for (int i = 0; i < Bonuses.Length; i++)
            {
                DailyBonusUIButton component = Instantiate(EmptyBonus, BonusListRect.content.transform).GetComponent<DailyBonusUIButton>();
                component.transform.localScale = Vector3.one;
                temp = Bonuses[i];
                component.SetData(temp.Icon, temp.Gems > 0 ? temp.Gems : temp.Money, "Day " + (i + 1),
                    isClaimed: !(i >= currentBonusIndex),
                    isCurrentIndex: (i == currentBonusIndex && currentBonusIndex != Bonuses.Length - 1));
                if (i == currentBonusIndex && SomeBonusAvailable)
                {
                    component.gameObject.AddComponent<AutomaticScaleButton>();
                }
            }

        }
        public void OnAds()
        {
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.ON_ADS);
        }
    }
}
