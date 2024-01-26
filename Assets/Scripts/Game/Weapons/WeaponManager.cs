using Game.Character.CharacterController;
using Game.UI;
using RopeHeroViceCity.UI_Features.UI_InGame.Scripts;
using UnityEngine;

namespace Game.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        private const float TracerDestroyTime = 1f;

        public Light FlashLight;

        public GunSFX GunShotSfx;

        public GunSFX LaserShotSfx;

        public GunSFX AlternativeLaserShotSfx;

        public bool DoFlash = true;

        public float FlashDuration = 0.1f;


        private static WeaponManager instance;

        private float flashTimeout;

        private bool isFlashing;

        public static WeaponManager Instance => instance;

        private void Awake()
        {
            instance = this;
            FlashLight.intensity = 0f;
        }

        private void Update()
        {
            if (DoFlash && isFlashing)
            {
                flashTimeout -= Time.deltaTime;
                if (flashTimeout < 0f)
                {
                    StopShootSFX();
                }
            }
        }

        public GunSFX GetShotSFX(ShotSFXType type)
        {
            switch (type)
            {
                case ShotSFXType.Gun:
                    return GunShotSfx;
                case ShotSFXType.Laser:
                    return LaserShotSfx;
                case ShotSFXType.AlternativeLaser:
                    return AlternativeLaserShotSfx;
                default:
                    return GunShotSfx;
            }
        }

        public void StartShootSFX(Transform parent, ShotSFXType type)
        {
            FlashLight.transform.parent = parent;
            FlashLight.transform.localPosition = Vector3.zero;
            FlashLight.intensity = 1f;
            flashTimeout = FlashDuration;
            isFlashing = true;
            GunSFX shotSFX = GetShotSFX(type);
            shotSFX.Emit(parent.position, parent.forward);
        }

        public void StartTraceSfx(Transform parent, GameObject traceSfx, Vector3 shootDirection, float traceLength)
        {
            TraceSFX.Instance.Emit(parent.position, shootDirection, traceSfx);
        }

        public void StopShootSFX()
        {
            if (isFlashing)
            {
                FlashLight.intensity = 0f;
                isFlashing = false;
            }
        }

        public void ResetShootSFX()
        {
            StopShootSFX();
            FlashLight.transform.parent = base.transform;
            GunShotSfx.transform.parent = base.transform;
            LaserShotSfx.transform.parent = base.transform;
            AlternativeLaserShotSfx.transform.parent = base.transform;
        }

        public void CloseWeaponPanel()
        {
            var ui_Ingame = FindObjectOfType<UI_InGame>();
            if (ui_Ingame != null)
                ui_Ingame.SetStateClose();
            GameplayUtils.ResumeGame();
        }
    }
}
