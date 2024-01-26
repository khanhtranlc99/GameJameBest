using UnityEngine;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.UI_Setting.Scripts
{
    public class UI_Setting : AbsUICanvas
    {
        public static bool isApplyNow;
        [SerializeField] private Toggle tglSound, tglQuality;
        [SerializeField] private Text txtSound, txtQuality;
        public override void StartLayer()
        {
            base.StartLayer();
            tglSound.onValueChanged.AddListener(OnSoundChange);
            tglQuality.onValueChanged.AddListener(OnQualityChange);
        }

        private void OnSoundChange(bool isEnable)
        {
            txtSound.color = !isEnable ? Color.white : Color.black;
        }
        private void OnQualityChange(bool isEnable)
        {

            txtQuality.color = !isEnable ? Color.white : Color.black;
        }

        public static void OpenSetting(bool directApply)
        {
            isApplyNow = directApply;
            UICanvasController.Instance.ShowLayer(UICanvasKey.SETTING);
        }
        public void OnAds()
        {
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.ON_ADS);
        }
    }
}
