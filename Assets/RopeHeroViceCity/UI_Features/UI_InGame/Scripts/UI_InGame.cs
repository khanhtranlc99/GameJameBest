using System;
using System.Collections;
using System.Threading;
using Code.Game.Race;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.GlobalComponent;
using Game.Items;
using Game.Managers;
using Game.MiniMap;
using Game.Shop;
using Root.Scripts.Helper;
using RopeHeroViceCity.Scripts.Gampelay;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RopeHeroViceCity.UI_Features.UI_InGame.Scripts
{
    public class UI_InGame : AbsUICanvas
    {
        [SerializeField] private Button btnPause, btnShop, btnLevelUp, btnRace, btnGetInVehicle, btnGetOutVehicle, btnGetInMetro, btnDeInitRace;
        [Separator]
        [SerializeField] private Slider CopRelationSlider;
        [SerializeField] private Slider CopRelationSubSlider;
        [SerializeField] private Image PlayerIcon;
        [SerializeField] private Image VisibleAreaImage;
        [SerializeField] private Image imgWeapon, imgCrosshair;
        [SerializeField] private RectTransform crosshairStart;
        [SerializeField] private Text txtAmmo;
        [SerializeField] private GameObject weaponPanel;
        [SerializeField] private GameObject objCutscene;
        public GameObject btnShootRope, btnSprint, btnFlyOn, btnFlyOff, btnKick;
        public Transform achivementTransform;

        [SerializeField] private Transform comboPanel;
        [SerializeField] private Text comboText;
        [SerializeField] private Slider comboSlider;
        public static UI_InGame Instance;
        private Player player;

        public override void StartLayer()
        {
            base.StartLayer();
            Instance = this;
            btnPause.onClick.AddListener(OnClickPause);
            btnLevelUp.onClick.AddListener(OnClickLevelUp);
            btnShop.onClick.AddListener(OnClickShop);
            btnRace.onClick.AddListener(OnClickRace);
            PlayerStoreProfile.LoadLoadout();

            StartCoroutine(LoadPreviewRoom());
            ItemsManager.Instance.Init();
            CopRelationSubSlider.maxValue = Mathf.Abs(-10f);
            CopRelationSlider.maxValue = Mathf.Abs(-10f) / 2f;
            float value = 0f;
            CopRelationSlider.value = value;
            CopRelationSubSlider.value = value;
            EventManager.Instance.onFactionUpdate.AddListener(UpdateFactionValue);
            EventManager.Instance.onUpdateAmmo.AddListener(UpdateAmmo);
            MiniMap.Instance.ApplyPlayerImage(PlayerIcon, VisibleAreaImage);
            player = FindObjectOfType<Player>();
            player.WeaponController.WeaponImage = imgWeapon;
            var animController = player.GetAnimController;
            animController.SprintButton = btnSprint;
            animController.ShootRopeButtons[0] = btnShootRope;
            animController.FlyInputs[0] = btnFlyOn;
            animController.FlyInputs[1] = btnFlyOff;
            PlayerInteractionsManager.Instance.GetInVehicleButton = btnGetInVehicle;
            PlayerInteractionsManager.Instance.GetOutVehicleButton = btnGetOutVehicle;
            PlayerInteractionsManager.Instance.GetInMetroButton = btnGetInMetro;
            MetroManager.Instance.Init();
            var supaKick = player.GetComponentInChildren<SuperKick>();
            supaKick.SuperKickButton = btnKick;
            TargetManager.Instance.CrosshairImage = imgCrosshair;
            TargetManager.Instance.CrosshairStart = crosshairStart;
            TargetManager.Instance.LoadAutoAim();
            if (TargetManager.Instance.UseAutoAim)
                TargetManager.Instance.StartAutoAim();
            btnGetInMetro.onClick.AddListener(() =>
            {
                MetroManager.Instance.GetInMetro();
                MetroManager.Instance.isButtonMetro = true;
                Close();
            });
            btnGetInVehicle.onClick.AddListener(() =>
            {
                PlayerInteractionsManager.Instance.GetIntoVehicle();
            });
            btnDeInitRace.onClick.AddListener(RaceManager.Instance.DeInit);
            ComboManager.Instance.ComboPanel = comboPanel;
            ComboManager.Instance.ComboMeterText = comboText;
            ComboManager.Instance.ComboMeterSlider = comboSlider;
            ComboManager.Instance.Init();
            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.SHOW_MARK);
        }
        public override void ShowLayer()
        {
            base.ShowLayer();
            GameplayUtils.ResumeGame();
            MiniMap.Instance.ChangeMapSize(false);
        }

        private void OnClickRace()
        {

        }

        public void OpenMapPopup()
        {
            UICanvasController.Instance.ShowLayer(UICanvasKey.MAP);
        }

        public void OnGetOutFromVehicle()
        {
            PlayerInteractionsManager.Instance.GetOutFromVehicle();
        }

        public void SetStateCutScene(bool isActive)
        {
            objCutscene.SetActive(isActive);
        }

        public Image GetCutsceneImage => objCutscene.GetComponent<Image>();

        private void OnClickLevelUp()
        {
            UICanvasController.Instance.ShowLayer(UICanvasKey.SKILLS);
        }

        private void OnClickPause()
        {
            UICanvasController.Instance.ShowLayer(UICanvasKey.PAUSE_MENU);
            Close();
        }

        private void UpdateFactionValue(float newVal)
        {
            CopRelationSubSlider.value = Mathf.Abs(newVal);
            CopRelationSlider.value = (int)Math.Truncate(Mathf.Abs(newVal) / 2f);
        }

        private void UpdateAmmo(string ammoShow)
        {
            txtAmmo.text = ammoShow;
        }

        private IEnumerator LoadPreviewRoom()
        {
            yield return null;
            SceneManager.LoadSceneAsync("ShopRoom", LoadSceneMode.Additive);
            StuffManager.Instance.Init();
        }
        public void OpenShop()
        {
            Invoke("OnClickShop", 0f);
        }
        public void OnClickShop()
        {
            Close();
            GameplayUtils.PauseGame();
            MiniMap.Instance.ChangeMapSize(true);
            //if (AdManager.Instance.IsInterstitialLoaded())
            //    AdManager.Instance.ShowInterstitial();
            UI_Shop.Scripts.UI_Shop.OpenShop(this);
            // UICanvasController.Instance.ShowLayer(UICanvasKey.SHOP);
            ShopManager.Instance.Init();
            ShopManager.Instance.Enable();

        }

        public void SetStateWeaponPanel(bool isActive)
        {
            weaponPanel.SetActive(isActive);
        }
        public void SetStateClose()
        {
            var control = weaponPanel.GetComponent<ChooseWeaponPanel>();
            control.ClosePanel();
        }

        public override void ProcessBackButton()
        {
            base.ProcessBackButton();
            GameplayUtils.PauseGame();
            UI_GeneralPopup.ShowPopup("Exit game", "Are you sure?", false, delegate
             {
                 EventManager.Instance.OnUserExitToMainMenu();
                 Thread.Sleep(500);
                 SceneManager.LoadScene(Constants.Scenes.Menu.ToString());
             });
        }
    }
}
