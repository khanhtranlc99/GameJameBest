using Game.Character;
using Game.Character.CharacterController;
using Game.Items;
using System;
using System.Collections.Generic;
using Root.Scripts.Helper;
using RopeHeroViceCity.Scripts.Gampelay;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    public class PreviewManager : MonoBehaviour
    {
        public PrevewAnimationController.PreviewAnimType currentPreviewAnim;

        public GameObject ShopObject;

        public GameObject PreviewDummy;

        public Camera PreviewCamera;

        public float CameraLerpMult;

        public int FadeOutSpeedMultipler;

        public bool UseFadeOut;

        [Space(10f)]
        public CamPositionToTransform[] CamPositionsToTransforms;

        public DummyPositionToTransform[] DummyPositionsToTransforms;

        public ItemPositionToTransform[] ItemPositionsToTransforms;

        private PrevewAnimationController animController;

        private GameObject currentPreviewObject;

        private PreviewStuffHelper previewHelper;

        private Transform cameraTargetTransform;

        private bool needToMove;

        private Loadout PreviewLoadout;

        private PreviewRotator rotator;

        private bool faded;

        private Image background;

        private float currentAddCameraDistance;

        private static PreviewManager instance;

        public static PreviewManager Instance => instance ? instance : (instance = FindObjectOfType<PreviewManager>());

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            MoveCamera();
            ControllFadeOut();
        }

        public void Init()
        {
            instance = this;
            animController = PreviewDummy.GetComponent<PrevewAnimationController>();
            rotator = GetComponent<PreviewRotator>();
            previewHelper = PreviewDummy.GetComponent<PreviewStuffHelper>();
            previewHelper.DefaultClotheses = PlayerManager.Instance.DefaultStuffHelper.DefaultClotheses;
            CopyTransform(PlayerManager.Instance.DefaultStuffHelper.LeftBraceletPlaceholder, previewHelper.LeftBraceletPlaceholder);
            CopyTransform(PlayerManager.Instance.DefaultStuffHelper.RightBraceletPlaceholder, previewHelper.RightBraceletPlaceholder);
            CopyTransform(PlayerManager.Instance.DefaultStuffHelper.LeftHucklePlaceholder, previewHelper.LeftHucklePlaceholder);
            CopyTransform(PlayerManager.Instance.DefaultStuffHelper.RightHucklePlaceholder, previewHelper.RightHucklePlaceholder);
            CopyTransform(PlayerManager.Instance.DefaultStuffHelper.GlassPlaceholder, previewHelper.GlassPlaceholder);
            CopyTransform(PlayerManager.Instance.DefaultStuffHelper.HatPlaceholder, previewHelper.HatPlaceholder);
            CopyTransform(PlayerManager.Instance.DefaultStuffHelper.MaskPlaceholder, previewHelper.MaskPlaceholder);
            ResetPreviewLoadout();
            // ShopManager shopManager = ShopManager.Instance;
            // shopManager.ShopOpeningEvent = (ShopManager.ShopOpened)Delegate.Combine(shopManager.ShopOpeningEvent, new ShopManager.ShopOpened(OnShopOpening));
            // ShopManager shopManager2 = ShopManager.Instance;
            // shopManager2.ShopCloseningEvent = (ShopManager.ShopClosed)Delegate.Combine(shopManager2.ShopCloseningEvent, new ShopManager.ShopClosed(OnShopClosed));

            EventManager.Instance.ShopOpeningEvent.AddListener(OnShopOpening);
            EventManager.Instance.ShopClosingEvent.AddListener(OnShopClosed);
            EventManager.Instance.OnPausePanelOpen.AddListener(OnShopOpening);
            EventManager.Instance.OnPausePanelClose.AddListener(OnShopClosed);
            ShopObject.SetActive(value: false);
        }

        public void ShowItem(ShopItem item, bool showOrigin = false)
        {
            MoveDummyTo(item.GameItem.PreviewVariables.DummyPosition, item.GameItem.PreviewVariables.PositionOffset);
            MoveCameraTo(item.GameItem.PreviewVariables.CameraPosition, item.GameItem.PreviewVariables.AdditionalCameraDistance, immediatly: false);
            if (currentPreviewObject != null)
            {
                UnityEngine.Object.Destroy(currentPreviewObject);
            }
            ResetPreviewLoadout();
            if (!showOrigin)
            {
                SetNewItem(item.GameItem);
            }
            StuffManager.Instance.UpdateAllAccesories(previewHelper, PreviewLoadout);
            StuffManager.Instance.UpdateClothes(previewHelper, PreviewLoadout);
            AnimateDummy(item.GameItem);
        }

        private void OnShopOpening()
        {
            if (ShopManager.Instance != null)
                background = ShopManager.Instance.Links.Background;
            ShopObject.SetActive(value: true);
            //AdManager.Instance.ToggleBannerVisibility(true);
            PreviewCamera.depth = 2f;
            CameraManager.Instance.SetCameraStatus(status: false);
            ResetRotators();
        }

        private void OnShopClosed()
        {
            ShopObject.SetActive(value: false);
            //AdManager.Instance.ToggleBannerVisibility(false);
            PreviewCamera.depth = -2f;
            CameraManager.Instance.SetCameraStatus(status: true);
        }

        private void ResetPreviewLoadout()
        {
            if (PreviewLoadout == null)
            {
                PreviewLoadout = new Loadout();
                PreviewLoadout.Weapons = new Dictionary<string, int>
                {
                    {
                        "MeleeWeaponID",
                        0
                    },
                    {
                        "AdditionalWeaponID",
                        0
                    },
                    {
                        "MainWeaponID",
                        0
                    },
                    {
                        "HeavyWeaponID",
                        0
                    },
                    {
                        "UniversalWeaponID",
                        0
                    },
                    {
                        "SpecialWeaponID",
                        0
                    }
                };
                PreviewLoadout.Skin = new Dictionary<string, int>
                {
                    {
                        "HeadID",
                        0
                    },
                    {
                        "FaceID",
                        0
                    },
                    {
                        "BodyID",
                        0
                    },
                    {
                        "ArmsID",
                        0
                    },
                    {
                        "ForearmsID",
                        0
                    },
                    {
                        "HandsID",
                        0
                    },
                    {
                        "UpperLegsID",
                        0
                    },
                    {
                        "LowerLegsID",
                        0
                    },
                    {
                        "FootsID",
                        0
                    },
                    {
                        "HatID",
                        0
                    },
                    {
                        "GlassID",
                        0
                    },
                    {
                        "MaskID",
                        0
                    },
                    {
                        "LeftBraceletID",
                        0
                    },
                    {
                        "RightBraceletID",
                        0
                    },
                    {
                        "LeftHuckleID",
                        0
                    },
                    {
                        "RightHuckleID",
                        0
                    },
                    {
                        "LeftPalmID",
                        0
                    },
                    {
                        "RightPalmID",
                        0
                    },
                    {
                        "LeftToeID",
                        0
                    },
                    {
                        "RightToeID",
                        0
                    },
                    {
                        "ExternalBodyID",
                        0
                    },
                    {
                        "ExternalForearmsID",
                        0
                    },
                    {
                        "ExternalFootsID",
                        0
                    }
                };
            }
            foreach (string key in PlayerStoreProfile.CurrentLoadout.Skin.Keys)
            {
                PreviewLoadout.Skin[key] = PlayerStoreProfile.CurrentLoadout.Skin[key];
            }
        }

        private void SetNewItem(GameItem item)
        {
            if (item == null)
            {
                return;
            }
            switch (item.Type)
            {
                case ItemsTypes.Accessory:
                    SetNewAccessory(item);
                    return;
                case ItemsTypes.Clothes:
                    SetNewClothes(item);
                    return;
                case ItemsTypes.Weapon:
                    SetNewWeapon(item);
                    return;
                case ItemsTypes.Ability:
                    SetNewAbility(item);
                    return;
            }
            if (item.PreviewVariables.PreviewModel == null)
            {
                UnityEngine.Debug.Log("Невозможно отобразить предмет, т.к не задана превью модель!");
            }
            else
            {
                ShowDefaultPreview(item);
            }
        }

        private void SetNewAccessory(GameItem item)
        {
            GameItemAccessory gameItemAccessory = item as GameItemAccessory;
            if (gameItemAccessory == null)
            {
                UnityEngine.Debug.LogWarning("Trying to preview unknown item as accessory.");
                return;
            }
            SkinSlot[] occupiedSlots = gameItemAccessory.OccupiedSlots;
            foreach (SkinSlot slot in occupiedSlots)
            {
                GameItemSkin gameItemSkin = StuffManager.ItemInSkinSlot(slot) as GameItemSkin;
                if (gameItemSkin != null)
                {
                    if (gameItemSkin.Type == ItemsTypes.Accessory)
                        previewHelper.UpdateNewAcessories(gameItemSkin.indexSkin);
                    SkinSlot[] occupiedSlots2 = gameItemSkin.OccupiedSlots;
                    foreach (SkinSlot key in occupiedSlots2)
                    {
                        PreviewLoadout.Skin[Loadout.KeyFromSkinSlot[key]] = 0;
                    }
                }
            }
            SkinSlot[] occupiedSlots3 = gameItemAccessory.OccupiedSlots;
            foreach (SkinSlot key2 in occupiedSlots3)
            {
                PreviewLoadout.Skin[Loadout.KeyFromSkinSlot[key2]] = gameItemAccessory.ID;
            }
        }

        private void ShowDefaultPreview(GameItem item)
        {
            currentPreviewObject = (UnityEngine.Object.Instantiate(item.PreviewVariables.PreviewModel, GetTransform(item.PreviewVariables.ItemPosition)) as GameObject);
            currentPreviewObject.transform.localPosition = item.PreviewVariables.PositionOffset;
            currentPreviewObject.transform.localRotation = Quaternion.identity;
        }

        private void SetNewClothes(GameItem item)
        {
            GameItemClothes gameItemClothes = item as GameItemClothes;
            if (gameItemClothes == null)
            {
                UnityEngine.Debug.LogWarning("Trying to preview unknown item as skin.");
                return;
            }
            SkinSlot[] occupiedSlots = gameItemClothes.OccupiedSlots;
            foreach (SkinSlot slot in occupiedSlots)
            {
                GameItemSkin gameItemSkin = StuffManager.ItemInSkinSlot(slot) as GameItemSkin;
                if (gameItemSkin != null)
                {
                    previewHelper.UpdateNewSkin(gameItemSkin.indexSkin);
                    SkinSlot[] occupiedSlots2 = gameItemSkin.OccupiedSlots;
                    foreach (SkinSlot key in occupiedSlots2)
                    {
                        PreviewLoadout.Skin[Loadout.KeyFromSkinSlot[key]] = 0;
                    }
                }
            }
            SkinSlot[] occupiedSlots3 = gameItemClothes.OccupiedSlots;
            foreach (SkinSlot key2 in occupiedSlots3)
            {
                PreviewLoadout.Skin[Loadout.KeyFromSkinSlot[key2]] = gameItemClothes.ID;
            }
        }

        private void SetNewAbility(GameItem item)
        {
            GameItemAbility gameItemAbility = item as GameItemAbility;
            if (gameItemAbility == null)
            {
                UnityEngine.Debug.LogWarning("Trying to preview unknown item as ability.");
                return;
            }
            if (gameItemAbility.RelatedSkins != null && gameItemAbility.RelatedSkins.Length > 0)
            {
                GameItemSkin[] relatedSkins = gameItemAbility.RelatedSkins;
                foreach (GameItemSkin gameItemSkin in relatedSkins)
                {
                    if (gameItemSkin is GameItemAccessory)
                    {
                        SetNewAccessory(gameItemSkin);
                    }
                    else
                    {
                        SetNewClothes(gameItemSkin);
                    }
                }
            }
            if (gameItemAbility.PreviewVariables.PreviewModel != null)
            {
                ShowDefaultPreview(gameItemAbility);
            }
        }

        private void SetNewWeapon(GameItem item)
        {
            GameItemWeapon gameItemWeapon = item as GameItemWeapon;
            if (gameItemWeapon == null)
            {
                UnityEngine.Debug.LogWarning("Trying to preview unknown item as weapon.");
                return;
            }
            Transform previewWeaponPlaceholder = previewHelper.PreviewWeaponPlaceholder;
            currentPreviewObject = (UnityEngine.Object.Instantiate(gameItemWeapon.PreviewVariables.PreviewModel, previewWeaponPlaceholder) as GameObject);
            currentPreviewObject.transform.localPosition = Vector3.zero;
            currentPreviewObject.transform.localRotation = Quaternion.identity;
        }

        private void AnimateDummy(GameItem item)
        {
            animController.SetPreviewAnimation(item.PreviewVariables.PreviewAnimation);
            currentPreviewAnim = item.PreviewVariables.PreviewAnimation;
        }

        private void MoveDummyTo(PreviewDummyPositions targetPosition, Vector3 offset)
        {
            Transform transform = GetTransform(targetPosition);
            PreviewDummy.transform.parent = transform;
            PreviewDummy.transform.localPosition = offset;
            PreviewDummy.transform.localRotation = Quaternion.identity;
        }

        private void MoveCameraTo(PreviewCameraPositions targetPosition, float addDistance, bool immediatly)
        {
            Transform transform = GetTransform(targetPosition);
            if (transform != null && (transform != cameraTargetTransform || Math.Abs(addDistance - currentAddCameraDistance) > 0f))
            {
                if (immediatly)
                {
                    CopyTransform(PreviewCamera.transform, transform);
                    ControllBackgroundAlpha(0f, immediatly: true);
                }
                cameraTargetTransform = transform;
                currentAddCameraDistance = addDistance;
                needToMove = !immediatly;
                faded = false;
            }
        }

        private void MoveCamera()
        {
            if (needToMove)
            {
                Vector3 b = cameraTargetTransform.position - PreviewCamera.transform.forward * currentAddCameraDistance;
                bool flag = (double)Vector3.Distance(PreviewCamera.transform.position, b) > 0.01;
                bool flag2 = Quaternion.Angle(PreviewCamera.transform.rotation, cameraTargetTransform.rotation) > 1f;
                if (flag)
                {
                    PreviewCamera.transform.position = Vector3.Lerp(PreviewCamera.transform.position, b, Time.fixedDeltaTime * CameraLerpMult);
                }
                if (flag2)
                {
                    PreviewCamera.transform.rotation = Quaternion.Slerp(PreviewCamera.transform.rotation, cameraTargetTransform.rotation, Time.fixedDeltaTime * CameraLerpMult);
                }
                needToMove = (flag || flag2);
            }
        }

        private void ControllFadeOut()
        {
            if (UseFadeOut)
            {
                if (needToMove && !faded)
                {
                    ControllBackgroundAlpha(1f, immediatly: false);
                    Color color = background.color;
                    faded = (color.a >= 0.95f);
                }
                else
                {
                    ControllBackgroundAlpha(0f, immediatly: false);
                    Color color2 = background.color;
                    faded = (color2.a >= 0.05f || needToMove);
                }
            }
        }

        private void ControllBackgroundAlpha(float toAlpha, bool immediatly)
        {
            Color color = background.color;
            color.a = ((!immediatly) ? Mathf.Lerp(color.a, toAlpha, Time.fixedDeltaTime * (float)FadeOutSpeedMultipler) : toAlpha);
            background.color = color;
        }

        private void CopyTransform(Transform from, Transform to)
        {
            if (from == null) return;
            to.localPosition = from.localPosition;
            to.localRotation = from.localRotation;
            to.localScale = from.localScale;
        }

        private void ResetRotators()
        {
            rotator.ResetRotators();
        }

        private Transform GetTransform(PreviewCameraPositions position)
        {
            CamPositionToTransform[] camPositionsToTransforms = CamPositionsToTransforms;
            foreach (CamPositionToTransform camPositionToTransform in camPositionsToTransforms)
            {
                if (camPositionToTransform.CamPosition == position)
                {
                    return camPositionToTransform.CamTransform;
                }
            }
            UnityEngine.Debug.LogError("Для выбранной позиции не задан соответствующий трансформ!");
            return null;
        }

        private Transform GetTransform(PreviewDummyPositions position)
        {
            DummyPositionToTransform[] dummyPositionsToTransforms = DummyPositionsToTransforms;
            foreach (DummyPositionToTransform dummyPositionToTransform in dummyPositionsToTransforms)
            {
                if (dummyPositionToTransform.DummyPosition == position)
                {
                    return dummyPositionToTransform.DummyTransform;
                }
            }
            UnityEngine.Debug.LogError("Для выбранной позиции не задан соответствующий трансформ!");
            return null;
        }

        private Transform GetTransform(PreviewItemPositions position)
        {
            ItemPositionToTransform[] itemPositionsToTransforms = ItemPositionsToTransforms;
            foreach (ItemPositionToTransform itemPositionToTransform in itemPositionsToTransforms)
            {
                if (itemPositionToTransform.ItemPosition == position)
                {
                    return itemPositionToTransform.ItemTransform;
                }
            }
            UnityEngine.Debug.LogError("Для выбранной позиции не задан соответствующий трансформ!");
            return null;
        }
    }
}
