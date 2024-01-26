using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Shop;
using Game.UI;
using Root.Scripts.Helper;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class ChooseWeaponPanel : MonoBehaviour
{
    [SerializeField] private Button btnChangeWeapon;
    [SerializeField] private Toggle tglStateAim;
    [SerializeField] private GameObject control;
    [SerializeField] private GameObject topLeft;
    [SerializeField] private GameObject topCenter;
    [SerializeField] private GameObject topRight;

    private void Awake()
    {
        btnChangeWeapon.onClick.AddListener(() =>
        {
            TargetManager.Instance.ChangeAutoAim();
            UpdateAnimState();
        });
    }

    private void OnEnable()
    {
        UpdateAnimState();
    }

    private void UpdateAnimState()
    {
        tglStateAim.isOn = TargetManager.Instance.UseAutoAim;
    }

    public void OpenPanel()
    {
        if (!PlayerInteractionsManager.Instance.inVehicle)
        {
            gameObject.SetActive(value: true);
            control.SetActive(false);
            topLeft.SetActive(false);
            topCenter.SetActive(false);
            topRight.SetActive(false);
            GameplayUtils.PauseGame();
            //UIGame.Instance.Pause();
            ShopManager shop = FindObjectOfType<ShopManager>(true);
            if (!shop)
            {
                this.gameObject.AddComponent<ShopManager>();
            }
            //init here
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(value: false);
        GameplayUtils.ResumeGame();
        control.SetActive(true);
        topLeft.SetActive(true);
        topCenter.SetActive(true);
        topRight.SetActive(true);
        //UIGame.Instance.Resume();
        ShopManager shop;
        if (TryGetComponent<ShopManager>(out shop))
        {
            Destroy(shop);
        }
    }
    public void CheckShop()
    {
        ShopManager shop;
        if (TryGetComponent<ShopManager>(out shop))
        {
            Destroy(shop);
        }
        UI_InGame.Instance.OpenShop();
    }
}
