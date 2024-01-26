using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.Items;
using Game.Shop;
using Game.UI;
using Root.Scripts.Helper;
using RopeHeroViceCity.UI_Features.UI_UniversalPopUp.Scripts;
using UI.TryWeapon;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public enum WeaponTryType
    {
        Melee = 0,
        Pistol = 1,
        Rifle = 2,
        Shotgun = 3,
        Heavy = 4,
    }
    public class UITryWeapon : MonoBehaviour
    {
        [SerializeField] private TryWeaponData[] allDataTry;
        [SerializeField] private Sprite[] allIconTry;
        [SerializeField] private Button[] btnTries;
        [SerializeField] private GameObject objTry;
        private WeaponTryType[] tryTypes = new WeaponTryType[2];
        private TryWeaponAction currentTryActions;

        // [SerializeField] private GameItemWeapon[] allMeleeWeapons;
        // [SerializeField] private GameItemWeapon[] allPistolWeapons;
        // [SerializeField] private GameItemWeapon[] allRifleWeapons;
        // [SerializeField] private GameItemWeapon[] allShotgunWeapons;
        // [SerializeField] private GameItemWeapon[] allHeavyWeapons;

        private int currentIndex;
        private void Awake()
        {
            for (int i = 0; i < btnTries.Length; i++)
            {
                int j = i;
                btnTries[i].onClick.AddListener(() =>
                {
                    OnClickTry(j);
                });
            }
        }

        private void OnClickTry(int index)
        {
            //if (AdManager.Instance.IsRewardVideoLoaded())
            //{
            //    currentIndex = index;
            //    AdManager.Instance.ShowRewardVideo(OnWatchAdsDone);
            //}
            //else
            //{
                UI_GeneralPopup.ShowPopup(StringHelper.ADS_NOT_AVAILABLE_HEADER, StringHelper.ADS_NOT_AVAILABLE);
            //}
        }

        private void OnWatchAdsDone(bool isDone)
        {
            objTry.SetActive(false);
            // Debug.LogError("Active try weapon "+tryTypes[currentIndex]);
            ActiveTryItem(currentIndex);
        }

        private void ActiveTryItem(int index)
        {
            var weaponData = allDataTry[index];
            TryWeaponAction newAction;
            switch (weaponData.typeTry)
            {
                case WeaponTryType.Melee:
                    newAction = new TryActionMelee(weaponData.ExpireAmount);
                    break;
                default:
                    newAction = new TryActionGun(weaponData.ExpireAmount);
                    break;
            }
            currentTryActions = newAction;
            var choosedWeapon = GetItemWeaponByType(weaponData.typeTry);
            newAction.ApplyItemToTry(this, choosedWeapon);
        }

        public void RegisterEvent(UnityAction onGunShoot)
        {
            var player = FindObjectOfType<Player>();
            if (player != null)
                player.OnPlayerShootOne.AddListener(onGunShoot);
        }
        public void UnregisterEvent(UnityAction onGunShoot)
        {
            var player = FindObjectOfType<Player>();
            if (player != null)
                player.OnPlayerShootOne.RemoveListener(onGunShoot);
        }

        private GameItemWeapon GetItemWeaponByType(WeaponTryType type)
        {
            GameItemWeapon[] allWeaponOfType = null;
            switch (type)
            {
                case WeaponTryType.Melee:
                    allWeaponOfType = ItemsManager.Instance.allMeleeWeapons;
                    break;
                case WeaponTryType.Pistol:
                    allWeaponOfType = ItemsManager.Instance.allPistolWeapons;
                    break;
                case WeaponTryType.Rifle:
                    allWeaponOfType = ItemsManager.Instance.allRifleWeapons;
                    break;
                case WeaponTryType.Shotgun:
                    allWeaponOfType = ItemsManager.Instance.allShotgunWeapons;
                    break;
                case WeaponTryType.Heavy:
                    allWeaponOfType = ItemsManager.Instance.allHeavyWeapons;
                    break;
            }

            if (allWeaponOfType == null) return null;
            return allWeaponOfType[Random.Range(0, allWeaponOfType.Length)];
        }

        private IEnumerator ICheckExpireAction()
        {
            var interval = new WaitForSeconds(1f);
            while (true)
            {
                yield return interval;
                if (currentTryActions == null)
                    continue;
                if (currentTryActions.CheckExpire())
                {
                    PlayerManager.Instance.WeaponController.RemoveTryWeapon();
                    currentTryActions = null;
                }
            }
        }

        public void ForceEndTry()
        {
            if (currentTryActions == null)
                return;
            currentTryActions = null;
        }
        private void Start()
        {
            StartCoroutine(I_RandomWeaponTry());
            StartCoroutine(ICheckExpireAction());
            var player = FindObjectOfType<Player>();
            if (player != null)
                player.OnPlayerChooseOtherWeapon.AddListener(ForceEndTry);
        }

        private void OnDestroy()
        {
            var player = FindObjectOfType<Player>();
            if (player != null)
                player.OnPlayerChooseOtherWeapon.RemoveListener(ForceEndTry);
        }

        private IEnumerator I_RandomWeaponTry()
        {
            var intervalTime = new WaitForSeconds(30f);
            while (true)
            {
                if (currentTryActions == null)
                {
                    var typeOne = Random.Range(0, 5);
                    int typeTwo = typeOne;
                    while (typeTwo == typeOne)
                    {
                        typeTwo = Random.Range(0, 5);
                    }
                    if (!objTry.activeSelf)
                        objTry.SetActive(true);
                    btnTries[0].image.sprite = allIconTry[typeOne];
                    btnTries[1].image.sprite = allIconTry[typeTwo];
                    tryTypes[0] = (WeaponTryType)typeOne;
                    tryTypes[1] = (WeaponTryType)typeTwo;
                }

                yield return intervalTime;
            }
        }
    }
}
