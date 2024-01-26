using Game.Character.CharacterController.Enums;
using Game.Character.Extras;
using Game.Character.Modes;
using Game.Enemy;
using Game.GlobalComponent;
using Game.PickUps;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;

namespace Game.Character.CharacterController
{
    public class Human : HitEntity, IInitable
    {
        private const float OnBackCollisionRelativeForceCouter = 2f;

        private const float OnBigDynamicCollisionForceCounter = 3f;

        private const float OnCollisionDamageCounter = 1f;

        private const float VelocityReduction = 5f;

        private const float VelocityTreshold = 35f;

        private const float AlarmShoutRange = 20f;

        private const float MinVelocityToFall = 22f;

        private const float CheckSpeedTime = 2f;

        private const float lowInputMultiplier = 4f;

        private const int MinPlayerHealthToHealthPack = 50;

        private static int chatacterLayerNumber = -1;

        private static int bigDynamicLayerNumber = -1;

        [Separator("Audio settings")]
        public AudioSource AudioSource;

        public AudioClip[] footsteps;

        [Space(5f)]
        public bool IsSwiming;

        public bool IsDrowning;

        public AudioClip[] SwimSplashes;

        public float SwimOffset = -1.6f;

        public float SwimLerpMult = 5f;

        public float DamagePerDrow = 10f;

        public ParticleSystem WaterEffect;

        [Separator("Ragdoll Options")]
        public GameObject Ragdoll;

        public GameObject RagdollWakeUper;

        [Tooltip("Если равно 0, то не уничтожать")]
        public float DestroyTime;

        [Tooltip("По умолчанию 12 = Small Dynamic")]
        public int LayerNumber = 12;

        [Separator]
        public GameObject PickupPrefab;

        public GameObject rootModel;

        [Separator("Transformer")]
        public bool IsTransformer;

        public Transformer Transformer;

        public float OnCollisionKnockingTreshold = 10f;

        protected GameObject currentRagdoll;

        protected RagdollDrowning ragdollDrowning;

        public float DestroyRagdollTime;

        protected AnimationController animController;

        protected WeaponController weaponController;

        protected bool rdCollInvul;

        protected int rdCollInvulCount;

        protected float timeToClipEnd;

        private int footstepIndex;

        private int SplashIndex;

        private SlowUpdateProc slowUpdateProc;

        private Vector3 lastPosition;

        private Vector3 velocityDamage;

        private float checkSpeedTimer;

        private Rigidbody mainRigidbody;

        private Transform metarig;

        private Transform hips;

        private readonly AttackState attackState = new AttackState();

        protected bool rdExpInvul;

        protected int rdExpInvulCount;

        public GameObject CurrentRagdoll => currentRagdoll;

        public GameObject specificRagdoll
        {
            get;
            protected set;
        }

        public bool RDCollInvul
        {
            get
            {
                return rdCollInvul;
            }
            set
            {
                if (value)
                {
                    rdCollInvulCount++;
                }
                else if (rdCollInvulCount > 0)
                {
                    rdCollInvulCount--;
                }
                else
                {
                    rdCollInvulCount = 0;
                }
                if (rdCollInvulCount > 0)
                {
                    rdCollInvul = true;
                }
                else
                {
                    rdCollInvul = false;
                }
            }
        }

        public bool RDExpInvul
        {
            get
            {
                return rdExpInvul;
            }
            set
            {
                if (value)
                {
                    rdExpInvulCount++;
                }
                else if (rdExpInvulCount > 0)
                {
                    rdExpInvulCount--;
                }
                else
                {
                    rdExpInvulCount = 0;
                }
                if (rdExpInvulCount > 0)
                {
                    rdExpInvul = true;
                }
                else
                {
                    rdExpInvul = false;
                }
            }
        }

        public bool RDFullInvul
        {
            get
            {
                return rdExpInvul && rdCollInvul;
            }
            set
            {
                if (value)
                {
                    rdExpInvulCount++;
                    rdCollInvulCount++;
                }
                else
                {
                    if (rdExpInvulCount > 0)
                    {
                        rdExpInvulCount--;
                    }
                    else
                    {
                        rdExpInvulCount = 0;
                    }
                    if (rdCollInvulCount > 0)
                    {
                        rdCollInvulCount--;
                    }
                    else
                    {
                        rdCollInvulCount = 0;
                    }
                }
                if (rdCollInvulCount > 0)
                {
                    rdCollInvul = true;
                }
                else
                {
                    rdCollInvul = false;
                }
                if (rdExpInvulCount > 0)
                {
                    rdExpInvul = true;
                }
                else
                {
                    rdExpInvul = false;
                }
            }
        }

        public bool Remote
        {
            get;
            set;
        }

        public virtual Rigidbody MainRigidbody()
        {
            if (!mainRigidbody)
            {
                mainRigidbody = GetComponent<Rigidbody>();
            }
            return mainRigidbody;
        }

        protected new virtual void Awake()
        {
            slowUpdateProc = new SlowUpdateProc(SlowUpdate, 1f);
            lastPosition = base.transform.position;
            if (chatacterLayerNumber == -1)
            {
                chatacterLayerNumber = LayerMask.NameToLayer("Character");
            }
            if (bigDynamicLayerNumber == -1)
            {
                bigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
            }
            if ((bool)RagdollWakeUper)
            {
                PoolManager.Instance.InitPoolingPrefab(RagdollWakeUper);
            }
        }

        public override void Initialization(bool setUpHealth = true)
        {
            base.Initialization(setUpHealth);
            if (rootModel == null)
            {
                Animator[] componentsInChildren = GetComponentsInChildren<Animator>();
                foreach (Animator animator in componentsInChildren)
                {
                    if (animator != GetComponent<Animator>())
                    {
                        rootModel = animator.gameObject;
                    }
                }
            }
            animController = GetComponent<AnimationController>();
            ScriptsInitialization();
            AudioInitialize();
        }

        public void AudioInitialize()
        {
            if ((bool)GetComponent<AudioSource>())
            {
                AudioSource = GetComponent<AudioSource>();
            }
            else
            {
                AudioSource = base.transform.gameObject.AddComponent<AudioSource>();
            }
        }

        public void Init()
        {
            Health.Setup();
            Dead = false;
            DiedEvent = null;
            EntityManager.Instance.Register(this);
        }

        public void DeInit()
        {
            ClearCurrentRagdoll();
        }

        public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
        {
            base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
            if ((bool)owner)
            {
                EntityManager.Instance.OverallAlarm(owner, this, base.transform.position, 20f);
            }
        }

        public override void Drowning(float waterHight, float drowningDamageMult = 1f)
        {
            if (!base.IsDead)
            {
                OnHit(DamageType.Water, null, DamagePerDrow * drowningDamageMult * Time.deltaTime, Vector3.zero, Vector3.zero, 0f);
            }
        }

        public void InitializationWithoutHPAndAnimator()
        {
            ScriptsInitialization();
        }

        private void ScriptsInitialization()
        {
            animController = GetComponent<AnimationController>();
            weaponController = GetComponent<WeaponController>();
        }

        public void SetRootModel(GameObject newRootModel)
        {
            rootModel = newRootModel;
        }

        public WeaponArchetype GetWeaponType()
        {
            if (!weaponController)
            {
                return WeaponArchetype.Melee;
            }
            return weaponController.CurrentWeapon.Archetype;
        }

        public RangedWeaponType GetRangedWeaponType()
        {
            return weaponController.CurrentWeapon.GetComponent<RangedWeapon>().RangedWeaponType;
        }

        public bool ChangeTypeOfWeapon(WeaponArchetype weapArchetype)
        {
            return weaponController.ActivateWeaponByType(weapArchetype);
        }

        public bool CheckWeaponAmmoExist()
        {
            if (weaponController.CurrentWeapon.Archetype == WeaponArchetype.Ranged)
            {
                return weaponController.CurrentWeapon.GetComponent<RangedWeapon>().AmmoCount > 0;
            }
            return true;
        }

        public virtual void Footsteps()
        {
            if (!animController.SurfaceSensor.AboveGround && !animController.SurfaceSensor.InWater)
            {
                return;
            }
            float a = Mathf.Abs(animController.GetForwardAmount());
            float b = Mathf.Abs(animController.GetStrafeAmount());
            float num = 2f * Mathf.Max(a, b);
            if ((double)num <= 0.6)
            {
                num /= 4f;
            }
            if ((double)num <= 0.1)
            {
                return;
            }
            if (!AudioSource.isPlaying)
            {
                AudioSource.clip = footsteps[footstepIndex];
                timeToClipEnd = AudioSource.clip.length;
                AudioSource.Play();
                return;
            }
            timeToClipEnd -= Time.deltaTime * num;
            if (timeToClipEnd <= 0f)
            {
                footstepIndex++;
                SplashIndex++;
                if (footstepIndex > footsteps.Length - 1)
                {
                    footstepIndex = 0;
                }
                if (SplashIndex > SwimSplashes.Length - 1)
                {
                    SplashIndex = 0;
                }
                timeToClipEnd = AudioSource.clip.length;
                AudioClip clip = (!IsInWater) ? footsteps[footstepIndex] : SwimSplashes[SplashIndex];
                AudioSource.PlayOneShot(clip);
            }
        }

        protected virtual void Swim()
        {
            Transform transform = base.transform;
            Vector3 position = base.transform.position;
            Vector3 position2 = base.transform.position;
            float x = position2.x;
            float y = animController.SurfaceSensor.CurrWaterSurfaceHeight + SwimOffset;
            Vector3 position3 = base.transform.position;
            transform.position = Vector3.Lerp(position, new Vector3(x, y, position3.z), SwimLerpMult * Time.deltaTime);
            if (!weaponController.CurrentWeapon.name.Equals("Fists"))
            {
                weaponController.ActivateFists();
            }
        }

        public void Aim(bool status)
        {
            if (status)
            {
                weaponController.Aim();
            }
            else
            {
                weaponController.Hold();
            }
        }

        public void Attack()
        {
            weaponController.Attack();
        }

        public void Attack(HitEntity entity)
        {
            weaponController.Attack(entity);
        }

        public void Attack(Vector3 direction)
        {
            weaponController.Attack(direction);
        }

        protected override void OnDie()
        {
            base.OnDie();
            if (!GetComponentInParent<PoolManager>())
            {
                DropPickup();
                OnDieSpecific();
            }
        }

        protected virtual void OnDieSpecific()
        {
            if (!currentRagdoll)
            {
                ReplaceOnRagdoll(false, IsInWater);
            }
            PoolManager.Instance.ReturnToPool(base.gameObject);
        }

        public virtual void ReplaceOnRagdoll(bool canWakeUp, bool isDrowning = false)
        {
            if (!IsTransformer || (!canWakeUp && Transformer.currentForm != TransformerForm.Car))
            {
                GameObject _;
                ReplaceOnRagdoll(canWakeUp, out _, isDrowning);
            }
        }

        public virtual void ReplaceOnRagdoll(bool canWakeUp, Vector3 forceVector, bool isDrowning = false)
        {
            if (!IsTransformer || !canWakeUp)
            {
                GameObject initRagdoll;
                ReplaceOnRagdoll(canWakeUp, out initRagdoll, isDrowning);
                Transform transform = initRagdoll.transform.Find("hips");
                if (transform == null)
                {
                    transform = initRagdoll.transform.Find("metarig").Find("hips");
                }
                Rigidbody component = transform.GetComponent<Rigidbody>();
                component.AddForce(forceVector, ForceMode.Impulse);
            }
        }

        public virtual void ReplaceOnRagdoll(bool canWakeUp, out GameObject initRagdoll, bool isDrowning = false)
        {
            if (!canWakeUp && !IsTransformer)
            {
                specificRagdoll = PlayerDieManager.Instance.GetSpecificRagdoll(LastDamageType);
            }
            if (!currentRagdoll)
            {
                if (specificRagdoll != null)
                {
                    currentRagdoll = PoolManager.Instance.GetFromPool(specificRagdoll);
                }
                else
                {
                    currentRagdoll = PlayerManager.Instance.PlayerRagdoll;
                }
            }
            currentRagdoll.transform.position = transform.position;
            currentRagdoll.transform.rotation = transform.rotation;
            velocityDamage = LastHitVector;
            if (LastDamage > 35f)
            {
                LastDamage = 35f;
            }
            velocityDamage *= LastDamage / 5f;
            currentRagdoll.SetActive(value: false);
            CopyTransformRecurse(rootModel.transform, currentRagdoll);
            currentRagdoll.SetActive(value: true);
            currentRagdoll.transform.parent = base.transform.parent;
            initRagdoll = currentRagdoll;
            base.gameObject.SetActive(value: false);
            if ((bool)mainRigidbody)
            {
                mainRigidbody = GetComponent<Rigidbody>();
                mainRigidbody.velocity = Vector3.zero;
            }
            animController.Reset();
            if (isDrowning)
            {
                ragdollDrowning = currentRagdoll.AddComponent<RagdollDrowning>();
                ragdollDrowning.Init(currentRagdoll.transform.Find("metarig").Find("hips"), animController.SurfaceSensor.CurrWaterSurfaceHeight);
            }
            if ((bool)(this as Player))
            {
                if (canWakeUp)
                {
                    CameraManager.Instance.SetCameraTarget(GetRagdollHips());
                    CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Ragdoll");
                }
                else
                {
                    CameraManager.Instance.SetCameraTarget(GetRagdollHips());
                    CameraManager.Instance.SetMode(Type.Dead);
                }
            }
            if (!canWakeUp)
            {
                if (DestroyTime > 0f)
                {
                    DestroyRagdollTime = Time.time + DestroyTime;
                }
            }
            else if ((bool)RagdollWakeUper)
            {
                RagdollWakeUper ragdollWakeUper = currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
                if (ragdollWakeUper == null)
                {
                    ragdollWakeUper = PoolManager.Instance.GetFromPool(RagdollWakeUper.gameObject).GetComponent<RagdollWakeUper>();
                    ragdollWakeUper.transform.parent = GetRagdollHips();
                    ragdollWakeUper.transform.localPosition = Vector3.zero;
                }
                ragdollWakeUper.Init(base.gameObject, Health.Max, Health.Current, Defence, Faction);
            }
        }

        public void ClearCurrentRagdoll()
        {
            //Debug.LogError("Clear current ragdoll");
            if (!(currentRagdoll == null))
            {
                if (specificRagdoll != null)
                {
                    PoolManager.Instance.ReturnToPool(currentRagdoll);
                    specificRagdoll = null;
                }
                else
                {
                    currentRagdoll.SetActive(value: false);
                }
                if ((bool)ragdollDrowning)
                {
                    Destroy(ragdollDrowning);
                }
                currentRagdoll = null;
            }
        }

        public GameObject GetCurrentRagdoll()
        {
            return currentRagdoll;
        }

        protected void CopyTransformRecurse(Transform mainModelTransform, GameObject ragdoll)
        {
            ragdoll.transform.position = mainModelTransform.position;
            ragdoll.transform.rotation = mainModelTransform.rotation;
            ragdoll.transform.localScale = mainModelTransform.localScale;
            ragdoll.layer = LayerNumber;
            if ((bool)ragdoll.GetComponent<Rigidbody>())
            {
                ragdoll.GetComponent<Rigidbody>().velocity = velocityDamage;
            }
            foreach (Transform item in ragdoll.transform)
            {
                Transform transform2 = mainModelTransform.Find(item.name);
                if ((bool)transform2)
                {
                    CopyTransformRecurse(transform2, item.gameObject);
                }
            }
        }

        public AttackState UpdateAttackState(bool attack)
        {
            attackState.MeleeAttackState = MeleeAttackState.None;
            attackState.RangedAttackState = RangedAttackState.None;
            attackState.RangedWeaponType = RangedWeaponType.None;
            attackState.MeleeWeaponType = MeleeWeapon.MeleeWeaponType.Hand;
            attackState.Aim = false;
            attackState.CanAttack = false;
            RangedWeapon rangedWeapon = weaponController.CurrentWeapon as RangedWeapon;
            if (rangedWeapon != null)
            {
                attackState.RangedWeaponType = rangedWeapon.RangedWeaponType;
            }
            if (attack)
            {
                MeleeWeapon meleeWeapon = weaponController.CurrentWeapon as MeleeWeapon;
                if (rangedWeapon != null)
                {
                    attackState.Aim = true;
                    attackState.RangedWeaponType = rangedWeapon.RangedWeaponType;
                    attackState.RangedAttackState = rangedWeapon.GetRangedAttackState();
                    if (attackState.RangedAttackState == RangedAttackState.Shoot)
                    {
                        attackState.CanAttack = true;
                    }
                }
                else if (meleeWeapon != null)
                {
                    attackState.MeleeWeaponType = meleeWeapon.MeleeType;
                    if (meleeWeapon.IsOnCooldown)
                    {
                        attackState.MeleeAttackState = MeleeAttackState.Idle;
                        attackState.Aim = true;
                    }
                    else
                    {
                        attackState.MeleeAttackState = meleeWeapon.GetMeleeAttackState();
                        if (attackState.MeleeAttackState != MeleeAttackState.None && attackState.MeleeAttackState != MeleeAttackState.Idle)
                        {
                            attackState.CanAttack = true;
                            attackState.Aim = true;
                        }
                    }
                }
            }
            return attackState;
        }

        protected virtual void DropPickup()
        {
            PickUpManager.Instance.GenerateMoneyOnPoint(base.transform.position - base.transform.forward);
            if (PlayerInteractionsManager.Instance.Player.Health.Current > 50f)
            {
                if (GetWeaponType() == WeaponArchetype.Ranged)
                {
                    PickUpManager.Instance.GenerateBulletsOnPoint(base.transform.position + base.transform.forward, weaponController.CurrentWeapon.AmmoType);
                }
            }
            else
            {
                PickUpManager.Instance.GenerateHealthPackOnPoint(base.transform.position + base.transform.forward, PickUpManager.HealthPackType.Random);
            }
        }

        protected virtual void OnCollisionEnter(Collision col)
        {
            if ((bool)RagdollWakeUper && !IsInWater)
            {
                float num = Vector3.Dot(col.relativeVelocity, col.contacts[0].normal);
                if (col.collider.gameObject.layer == bigDynamicLayerNumber && !(this as Player))
                {
                    num *= 3f;
                }
                if (Vector3.Dot(base.transform.forward, col.relativeVelocity.normalized) > 0.2f)
                {
                    num *= 2f;
                }
                if (!(Mathf.Abs(num) < OnCollisionKnockingTreshold))
                {
                    HitEntity owner = FindCollisionDriver(col);
                    OnHit(DamageType.Collision, owner, num * 1f, col.contacts[0].point, col.contacts[0].normal.normalized, 0f);
                    OnCollisionSpecific(col);
                }
            }
        }

        protected virtual void OnCollisionSpecific(Collision col)
        {
            if (!base.IsDead && !RDCollInvul)
            {
                ReplaceOnRagdoll(canWakeUp: true);
            }
        }

        protected HitEntity FindCollisionDriver(Collision col)
        {
            VehicleStatus component = col.collider.gameObject.GetComponent<VehicleStatus>();
            if (component != null)
            {
                return component.GetVehicleDriver();
            }
            return null;
        }

        private void SlowUpdate()
        {
            VelocityCheck();
        }

        protected virtual void VelocityCheck()
        {
            lastPosition.y = 0f;
            Vector3 position = base.transform.position;
            position.y = 0f;
            float num = (lastPosition - position).magnitude / slowUpdateProc.DeltaTime;
            lastPosition = base.transform.position;
            if (checkSpeedTimer > 0f)
            {
                checkSpeedTimer -= slowUpdateProc.DeltaTime;
            }
            else if (num > 22f && !RDCollInvul)
            {
                ReplaceOnRagdoll(canWakeUp: true);
            }
        }

        protected override void FixedUpdate()
        {
            slowUpdateProc.ProceedOnFixedUpdate();
            IsInWater = animController.SurfaceSensor.InWater;
            if (DestroyRagdollTime != 0f && Time.time >= DestroyRagdollTime)
            {
                ClearCurrentRagdoll();
                DestroyRagdollTime = 0f;
            }
        }

        protected virtual void OnEnable()
        {
            lastPosition = base.transform.position;
            checkSpeedTimer = 2f;
            if (animController == null)
            {
                animController = GetComponent<AnimationController>();
            }
        }

        public void CheckReloadOnWakeUp()
        {
            if ((bool)weaponController)
            {
                weaponController.CheckReloadOnWakeUp();
            }
        }

        public Transform GetHips()
        {
            if (hips == null)
            {
                hips = GetHipsTransform(rootModel);
            }
            return hips;
        }

        public Transform GetMetarig()
        {
            if (metarig == null)
            {
                metarig = rootModel.transform.Find("metarig");
            }
            return metarig;
        }

        public Transform GetRagdollHips()
        {
            if ((bool)currentRagdoll)
            {
                return GetHipsTransform(currentRagdoll);
            }
            return null;
        }

        private Transform GetHipsTransform(GameObject model)
        {
            return model.transform.Find("metarig/hips");
        }
    }
}
