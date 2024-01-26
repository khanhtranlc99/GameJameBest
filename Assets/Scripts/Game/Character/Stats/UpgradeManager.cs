using Game.Managers;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;

namespace Game.Character.Stats
{
    public class UpgradeManager : MonoBehaviour
    {
        public UpgradePanel[] Panels;
        public PanelsList currentPanel;
        public int[] LevelPerStatLevel;
        public Sprite activeUpgrade;
        public Sprite deActiveUpgrade;
        public GameObject fx;
        public AudioClip audio;

        private static UpgradeManager instance;

        public static UpgradeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UnityEngine.Object.FindObjectOfType<UpgradeManager>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                currentPanel = PanelsList.Personage;
            }
        }

        public void SwitchBackground(PanelsList panel)
        {
            UpgradePanel[] panels = Panels;
            foreach (UpgradePanel upgradePanel in panels)
            {
                if (upgradePanel.type == panel)
                {
                    upgradePanel.OnState.SetActive(value: true);
                    upgradePanel.OffState.SetActive(value: false);
                }
                else
                {
                    upgradePanel.OnState.SetActive(value: false);
                    upgradePanel.OffState.SetActive(value: true);
                }
            }
            currentPanel = panel;
        }
        public void UpdateButtonUpgrade()
        {
            UpgradePanel[] panels = Panels;
            foreach (UpgradePanel upgradePanel in panels)
            {
                if (upgradePanel.type == currentPanel)
                {
                    for (int i = 0; i < upgradePanel.upgradeElements.Length; i++)
                    {
                        upgradePanel.upgradeElements[i].UpdateButtonImage();
                    }
                }
            }
        }
        public void UpgradeStat(UpgradeElement element)
        {
            Upgrade(element);
        }

        private void Upgrade(UpgradeElement element)
        {
            if (PlayerInfoManager.UpgradePoints >= 1 && EnoughLevelForStatUping(element))
            {
                int stat = StatsManager.GetStat(element.stat);
                if (stat < 5)
                {
                    stat++;
                    PlayerInfoManager.UpgradePoints--;
                    StatsManager.SaveStat(element.stat, stat);
                    element.progressBar.value = stat;
                    var tempFx = Instantiate(fx);
                    tempFx.transform.SetParent(element.transform);
                    tempFx.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                    SoundSource.Instance.PlayClip(audio);
                    PlayerInteractionsManager.Instance.Player.UpdateStats();
                    UpdateButtonUpgrade();
                }
                else if (GameManager.ShowDebugs)
                {
                    UnityEngine.Debug.Log("max lvl for this stat");
                }
            }
            else
            {
                if (PlayerInfoManager.UpgradePoints < 1)
                {
                    UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.NOT_ENOUGHT_SKILL_POINT);
                }
                else
                {
                    UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.NOT_ENOUGHT_LEVEL);
                }
                UnityEngine.Debug.Log("Not enught upgrade points or not Enough Level For Stat Uping");
            }
        }


        public bool EnoughLevelForStatUping(UpgradeElement element)
        {
            if (LevelPerStatLevel.Length == 0)
            {
                return true;
            }
            if (PlayerInfoManager.Level >= LevelPerStatLevel[LevelPerStatLevel.Length - 1])
            {
                return true;
            }
            if (LevelPerStatLevel[StatsManager.GetStat(element.stat)] <= PlayerInfoManager.Level)
            {
                return true;
            }
            return false;
        }
    }
}
