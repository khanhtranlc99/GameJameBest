using Game.Shop;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.CharacterController
{
    public class TargetManager : MonoBehaviour
    {
        private const float VehiclePierceRange = 4f;

        private const string AutoAimKey = "AutoAim";

        private static TargetManager instance;

        public LayerMask AimingLayerMask;

        public LayerMask ShootingLayerMask;

        [Space(5f)]
        public WeaponNameList weaponsWithoutAim;

        [Space(5f)]
        public bool HideCrosshair;

        private CastResult castResult;

        public Image CrosshairImage;

        public Color CrosshairColor = Color.white;

        public RectTransform CrosshairStart;

        [SerializeField]
        private float autoAimPositioningSpeed = 1f;

        [SerializeField]
        private float aimingUpdateRate = 0.5f;

        private Vector3 crosshairImagePosition;

        private static int chatacterLayerNumber;

        public bool UseAutoAim;

        public bool MoveCrosshair = true;

        public bool ColorCrosshair = true;

        public Transform AutoAimTarget;

        public Camera Camera;

        [Tooltip("Высота, на которой человек среднего роста держит оружие")]
        public Vector3 HumanoidWeaponHoldPosition = new Vector3(0f, 1f, 0f);

        private Vector3 ropeAimPosition;

        private Player player;

        private Vector3 autoAimOffset = Vector3.zero;

        public static TargetManager Instance => instance;

        public Vector3 TargetLocalOffset
        {
            get
            {
                if (AutoAimTarget == null)
                {
                    return Vector3.zero;
                }
                return AutoAimTarget.right * autoAimOffset.x + AutoAimTarget.up * autoAimOffset.y + AutoAimTarget.forward * autoAimOffset.z;
            }
        }

        public Vector3 RopeAimPosition => ropeAimPosition;

        public Vector3 CurrCrosshairPosition => CrosshairImage.rectTransform.position;

        private void Awake()
        {
            instance = this;
            chatacterLayerNumber = LayerMask.NameToLayer("Character");
        }

        private void Start()
        {
            player = PlayerInteractionsManager.Instance.Player;
            castResult = new CastResult();
            if (!Camera)
            {
                Camera = CameraManager.Instance.UnityCamera;
            }
            if (!CrosshairImage)
            {
                UnityEngine.Debug.LogWarning("Crosshair Image is not set!");
            }
            if (!CrosshairImage || !CrosshairStart)
            {
                UseAutoAim = false;
                return;
            }
            StartAutoAim();
        }
        public void StartAutoAim()
        {
            crosshairImagePosition = CrosshairStart.position;
            InvokeRepeating("SlowUpdate", 0f, aimingUpdateRate);
            LoadAutoAim();
        }

        private void SlowUpdate()
        {
            AutoAimTarget = null;
            if (!base.enabled)
            {
                return;
            }
            Ray ray = Camera.ScreenPointToRay(CrosshairStart.position);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Camera.farClipPlane, AimingLayerMask))
            {
                HitEntity component = hitInfo.transform.GetComponent<HitEntity>();
                if (component != null && !component.IsDead)
                {
                    AutoAimTarget = component.transform;
                    autoAimOffset = component.NPCShootVectorOffset;
                }
            }
        }

        private void Update()
        {
            ropeAimPosition = Vector3.Lerp(ropeAimPosition, crosshairImagePosition, autoAimPositioningSpeed * Time.deltaTime);
            if (UseAutoAim)
            {
                CrosshairImage.rectTransform.position = ropeAimPosition;
            }
            UpdateCrosshair();
        }

        public void UpdateCrosshair()
        {
            if (CrosshairImage == null) return;
            if (HideCrosshair)
            {
                if (CrosshairImage.enabled)
                {
                    CrosshairImage.enabled = false;
                }
                return;
            }
            if (!CrosshairImage.enabled)
            {
                CrosshairImage.enabled = true;
            }
            if (AutoAimTarget != null)
            {
                if (MoveCrosshair)
                {
                    crosshairImagePosition = Camera.WorldToScreenPoint(AutoAimTarget.position + TargetLocalOffset);
                }
                else
                {
                    crosshairImagePosition = CrosshairStart.position;
                }
                if (ColorCrosshair && UseAutoAim)
                {
                    CrosshairImage.color = Color.red;
                }
                else
                {
                    CrosshairImage.color = CrosshairColor;
                }
            }
            else
            {
                crosshairImagePosition = CrosshairStart.position;
                CrosshairImage.color = CrosshairColor;
            }
        }

        public CastResult ShootFromCamera(HitEntity owner, Vector3 scatterVector, RangedWeapon customWeapon)
        {
            Vector3 shotDirVector = default(Vector3);
            return ShootFromCamera(owner, scatterVector, out shotDirVector, (!(customWeapon == null)) ? customWeapon.AttackDistance : 0f, customWeapon);
        }

        public CastResult ShootFromCamera(HitEntity owner, Vector3 scatterVector, out Vector3 shotDirVector, float maxShotDistance = 0f, RangedWeapon customWeapon = null, bool humanoidShoot = false)
        {
            if (maxShotDistance <= 0f)
            {
                maxShotDistance = Camera.farClipPlane;
            }
            RangedWeapon rangedWeapon = (!(customWeapon != null)) ? (player.WeaponController.CurrentWeapon as RangedWeapon) : customWeapon;
            Ray ray = Camera.ScreenPointToRay(CurrCrosshairPosition);
            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo, Camera.farClipPlane, ShootingLayerMask))
            {
                hitInfo.point = ray.origin + ray.direction * Camera.farClipPlane;
            }
            Vector3 vector;
            if (humanoidShoot)
            {
                vector = owner.transform.InverseTransformPoint(rangedWeapon.Muzzle.position);
                vector.z = 0f;
                vector.y = HumanoidWeaponHoldPosition.y;
                vector = owner.transform.TransformPoint(vector);
            }
            else
            {
                vector = rangedWeapon.Muzzle.position;
            }
            Ray ray2 = new Ray(vector, (hitInfo.point - vector).normalized);
            if (!rangedWeapon.IgnoreMuzzleDirection && Vector3.Dot(ray2.direction, rangedWeapon.Muzzle.forward) < 0f)
            {
                ray2.direction = Camera.transform.forward;
            }
            ray2.direction += scatterVector;
            shotDirVector = ray2.direction;
            return CastWithRay(owner, ray2, maxShotDistance);
        }

        private CastResult CastWithRay(HitEntity owner, Ray ray, float maxDistance = 0f)
        {
            if (maxDistance <= 0f)
            {
                maxDistance = Camera.farClipPlane;
            }
            castResult.TargetObject = null;
            castResult.HitEntity = null;
            castResult.TargetType = TargetType.None;
            castResult.HitVector = ray.direction;
            castResult.RayLength = maxDistance;
            castResult.HitPosition = ray.origin + ray.direction * maxDistance;
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, ShootingLayerMask))
            {
                castResult.TargetObject = hitInfo.collider.gameObject;
                castResult.TargetType = TargetType.Default;
                castResult.HitPosition = hitInfo.point;
                castResult.RayLength = (hitInfo.point - ray.origin).magnitude;
                HitEntity component = castResult.TargetObject.GetComponent<HitEntity>();
                if (component == owner)
                {
                    return castResult;
                }
                castResult.HitEntity = component;
                if ((bool)castResult.HitEntity && !castResult.HitEntity.IsDead && FactionsManager.Instance.GetRelations(owner.Faction, castResult.HitEntity.Faction) != 0)
                {
                    castResult.TargetType = TargetType.Enemy;
                    VehicleStatus x = castResult.HitEntity as VehicleStatus;
                    if (x != null)
                    {
                        LayerMask mask = 1 << chatacterLayerNumber;
                        RaycastHit hitInfo2;
                        if (Physics.Raycast(hitInfo.point, ray.direction, out hitInfo2, 4f, mask))
                        {
                            HitEntity component2 = hitInfo2.collider.gameObject.GetComponent<HitEntity>();
                            if (component2 != null)
                            {
                                castResult.HitPosition = hitInfo2.point;
                                castResult.HitEntity = component2;
                            }
                        }
                    }
                }
            }
            return castResult;
        }

        public Ray GetCameraScreenPointToRay()
        {
            return Camera.ScreenPointToRay(CrosshairStart.position);
        }

        public CastResult ShootAt(HitEntity owner, Vector3 direction, float maxShootDistance = 0f)
        {
            Ray ray = new Ray(owner.transform.position + HumanoidWeaponHoldPosition, direction);
            return CastWithRay(owner, ray, maxShootDistance);
        }

        public CastResult ShootFromAt(HitEntity owner, Vector3 fromPos, Vector3 direction, float maxShootDistance = 0f)
        {
            Ray ray = new Ray(fromPos, direction);
            return CastWithRay(owner, ray, maxShootDistance);
        }

        private void SaveAutoAim()
        {
            BaseProfile.StoreValue(UseAutoAim, "AutoAim");
        }

        public void LoadAutoAim()
        {
            UseAutoAim = BaseProfile.ResolveValue("AutoAim", defaultValue: true);
        }

        public void ChangeAutoAim()
        {
            UseAutoAim = !UseAutoAim;
            CrosshairImage.rectTransform.position = CrosshairStart.position;
            SaveAutoAim();
        }
    }
}
