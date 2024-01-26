using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Stats
{
    public class UpgradeElement : MonoBehaviour
    {
        public PanelsList panel;

        public StatsList stat;

        public Image ButtonImage;

        public Slider progressBar;

        private void Start()
        {
            progressBar.value = StatsManager.GetStat(stat);
        }

        public void Upgrade()
        {
            UpgradeManager.Instance.UpgradeStat(this);
            // UpdateButtonImage();
        }

        public void SwitchButton()
        {
            UpgradeManager.Instance.SwitchBackground(panel);
        }

        public void UpdateButtonImage()
        {
            if (PlayerInfoManager.UpgradePoints >= 1 && UpgradeManager.Instance.EnoughLevelForStatUping(this))
            {
                ButtonImage.sprite = UpgradeManager.Instance.activeUpgrade;
                ButtonImage.color = Color.white;
            }
            else
            {
                ButtonImage.sprite = UpgradeManager.Instance.deActiveUpgrade;
                ButtonImage.color = new Color(1f, 1f, 1f, 0.3f);
            }
            //ButtonImage.color = ((!UpgradeManager.Instance.EnoughLevelForStatUping(this)) ? new Color(1f, 1f, 1f, 0.3f) : Color.white);
        }

        private void OnEnable()
        {
            UpdateButtonImage();
        }
    }
}
