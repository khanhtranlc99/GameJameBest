using Game.Character.CameraEffects;
using Game.Character.CharacterController.Enums;
using Game.Character.Extras;
using Game.Character.Input;
using Game.Character.Modes;
using Game.Character.Stats;
using Game.Effects;
using Game.Enemy;
using Game.GlobalComponent;
using Game.GlobalComponent.HelpfulAds;
using Game.Managers;
using Game.MiniMap;
using Game.Shop;
using Game.Traffic;
using Game.UI;
using Game.Vehicle;
using Game.Weapons;
using System;
using System.Collections;
using Root.Scripts.Helper;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Character.CharacterController
{
    public class Player : Human
    {
        private enum MoveState
        {
            Default,
            Aim,
            Crouch,
            Sprint,
            Fly,
            FlySprint,
            FlyAim
        }

        [Serializable]
        private class Inputs
        {
            public bool crouch;

            public Vector2 moveInput;

            public bool sprint;

            public bool reset;

            public bool jump;

            public bool shootRope;

            public bool fly;
            public bool eyeLaser;
        }

        public delegate void PlayerEnableDisableDelegate();

        public delegate void PlayerGetInOutVehicle(bool isIn = true);

        private const float FallVelocityTreshhold = 10f;

        private const float MoveToCarTimeout = 10f;

        private const float BlocketMoveToCarSpeed = 7f;

        private const float StaminaRefillDelayTimeout = 1.5f;

        private const float RangeToStop = 15f;

        private const float DistanceForDoor = 0.5f;

        private static int smallDynamicLayerNumber = -1;

        [Separator("Player parameters")]
        public bool DebugLog;

        [Space(5f)]
        public StatsManager stats;

        public AudioSource StaminaAduioSource;

        public bool WalkByDefault;

        [HideInInspector]
        public bool MoveToCar;
        public bool CanUseSkill => animController.CanShootInCurrentState;

        [HideInInspector]
        public Transform CarToMove;

        [HideInInspector]
        public UnityEngine.AI.NavMeshAgent agent;

        [HideInInspector]
        public Collider collider;

        [HideInInspector]
        public Rigidbody rigidbody;

        [HideInInspector]
        public CharacterSensor CharacterSensor;

        public LayerMask BlockedLayerMask;

        [Space(10f)]
        public float StaminaPerSprint = 5.4f;

        public float StaminaPerFlySprint = 1.6f;

        public float StaminaPerSwim = 1.6f;

        public float StaminaPerJump = 20f;

        public float StaminaRefillDelay = 3f;

        [Space(10f)]
        public bool UseSuperLandingExplosion;

        public GameObject LandingExplosionPrefab;

        public Vector3 LandingExplosionOffset = new Vector3(0f, 0.03f, 0f);

        public float TriggerSpeed = 5f;

        [Tooltip("In local space")]
        public Vector3 TriggerSpeedMults = Vector3.up;

        public Vector3 ExplosionOffset = new Vector3(0f, -0.1f, 0f);

        [Space(10f)]
        public GameObject DriverStatusPrefab;

        public DriverStatus InitiatedDriverStatus;

        public GameObject initiatedDriverStatusGO;

        public AudioClip[] FlySounds;

        public PlayerEnableDisableDelegate PlayerDisableEvent;

        public PlayerEnableDisableDelegate PlayerEnableEvent;
        public PlayerEnableDisableDelegate PlayerStartUsingEyeLaser;
        public PlayerEnableDisableDelegate PlayerStoptUsingEyeLaser;
        public UnityEvent OnPlayerShootOne;
        public UnityEvent OnPlayerChooseOtherWeapon;
        public Action<float> OnStaminaChange;

        private Vector3 lookPos;

        private Transform cam;

        private Vector3 camForward;

        private Vector3 move;

        private InputManager inputManager;

        private InputFilter velocityFilter;

        private MoveState state;

        private float MoveToCarTimer;

        private Inputs inputs = new Inputs();

        private CameraMode mode;

        private PlayerInteractionsManager playerInteractionsManager;

        private DontGoThroughThings dontGoThroughThings;

        private float prevSwitchWeapon;

        private PlayerStoreProfile playerProfile;

        public PlayerGetInOutVehicle PlayerGetInOutVehicleEvent;

        private float moveStateSwitchTime;

        private MoveState defMoveState;

        private bool needToSwitchDefMoveState;

        private bool nearWalls;

        private string DangerMessage;

        private bool FlyingSound;
        private bool isChoosingDieOrAlive;

        public bool IsFlying => (defMoveState == MoveState.Fly) ? true : false;

        public bool ToHighToFly
        {
            get
            {
                int result;
                if (IsFlying)
                {
                    Vector3 position = base.transform.position;
                    result = ((position.y > animController.MaxHigh) ? 1 : 0);
                }
                else
                {
                    result = 0;
                }
                return (byte)result != 0;
            }
        }

        public bool IsSprinting => (state == MoveState.Sprint || state == MoveState.FlySprint) ? true : false;

        public Vector3 LookVector
        {
            get
            {
                if (cam == null)
                {
                    return base.transform.forward;
                }
                if (Vector3.Dot(cam.forward, base.transform.forward) >= 0f)
                {
                    return cam.forward;
                }
                return Vector3.Reflect(cam.forward, base.transform.forward);
            }
        }

        public WeaponController WeaponController => weaponController ? weaponController : (weaponController = GetComponent<WeaponController>());

        public AnimationController GetAnimController => animController ? animController : (animController = GetComponent<AnimationController>());

        protected override void Start()
        {
            if (smallDynamicLayerNumber == -1)
            {
                smallDynamicLayerNumber = LayerMask.NameToLayer("SmallDynamic");
            }
            playerInteractionsManager = PlayerInteractionsManager.Instance;
        }

        protected void OnDisable()
        {
            if (PlayerDisableEvent != null)
            {
                PlayerDisableEvent();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (PlayerEnableEvent != null)
            {
                PlayerEnableEvent();
            }
            animController.SetCapsuleToVertical();
        }

        private void OnDestroy()
        {
            stats.stamina.OnValueChange -= OnStaminaChange;
        }

        public override void Initialization(bool setUpHealth = true)
        {
            base.Initialization(setUpHealth);
            stats.Init();
            stats.stamina.OnValueChange += OnStaminaChange;
            UpdateStats();
            Health.Setup(stats.GetPlayerStat(StatsList.Health), stats.GetPlayerStat(StatsList.Health));
            Health.RegenPerSecond = stats.GetPlayerStat(StatsList.HealthRegeneration);
            EntityManager.Instance.RegisterPlayer(this);
            velocityFilter = new InputFilter(10, 1f);
            playerProfile = GetComponent<PlayerStoreProfile>();
            dontGoThroughThings = GetComponent<DontGoThroughThings>();
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody>();
            }
            if (collider == null)
            {
                collider = GetComponent<Collider>();
            }
            if (CharacterSensor == null)
            {
                CharacterSensor = GetComponentInChildren<CharacterSensor>();
            }
            if (agent == null)
            {
                agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent == null)
                {
                    UnityEngine.Debug.LogError("Can't find agent");
                }
            }
            if (agent != null)
            {
                agent.enabled = false;
            }
            cam = CameraManager.Instance.UnityCamera.transform;
            inputManager = InputManager.Instance;
            weaponController.Hold();
            UpdateOnFallImpact();
            state = MoveState.Default;
            animController.JumpEvent += ChangeStaminaForJump;
            if ((bool)WaterEffect)
            {
                var emit = WaterEffect.emission;
                emit.enabled = false;
            }
        }

        public void UpdateStats()
        {
            float playerStat = stats.GetPlayerStat(StatsList.Health);
            Health.Setup(playerStat, playerStat);
            Health.RegenPerSecond = stats.GetPlayerStat(StatsList.HealthRegeneration);
            StaminaPerFlySprint = stats.GetPlayerStat(StatsList.SuperFlySprintStaminaCost);
            Defence.SetValue(DamageType.Bullet, stats.GetPlayerStat(StatsList.BulletsDefence));
            Defence.SetValue(DamageType.Energy, stats.GetPlayerStat(StatsList.EnergyDefence));
            Defence.SetValue(DamageType.Explosion, stats.GetPlayerStat(StatsList.ExplosionsDefence));
            stats.UpdateStats();
            animController.UpdatePlayerStats();
            weaponController.UpdatePlayerStats();
        }

        public void UpdateOnFallImpact()
        {
            if (animController.useSuperheroLandings)
            {
                animController.OnFallImpactCallback = OnFallImpactSuperhero;
            }
            else
            {
                animController.OnFallImpactCallback = OnFallImpact;
            }
        }

        private void OnFallImpact(Vector3 velocityVector)
        {
            float num = Mathf.Abs(velocityVector.y);
            if (num > 10f && !IsInWater)
            {
                float damage = (num - 10f) * (num - 10f) * rigidbody.mass / 75f;
                OnHit(DamageType.Collision, null, damage, base.transform.position, base.transform.up, 0f);
                if (!base.RDCollInvul)
                {
                    ReplaceOnRagdoll(canWakeUp: true);
                }
            }
            if (Time.timeSinceLevelLoad > 5f)
            {
                Fall fall = EffectManager.Instance.Create<Fall>();
                fall.ImpactVelocity = num;
                fall.Play();
            }
        }

        private void OnFallImpactSuperhero(Vector3 velocityVector)
        {
            Vector3 vector = Vector3.Scale(velocityVector, TriggerSpeedMults);
            if (Time.timeSinceLevelLoad > 2f && vector.magnitude >= TriggerSpeed && !IsInWater)
            {
                Fall fall = EffectManager.Instance.Create<Fall>();
                fall.ImpactVelocity = vector.magnitude;
                fall.Play();
                LandingExplosion();
            }
        }

        private void LandingExplosion()
        {
            if ((bool)LandingExplosionPrefab && UseSuperLandingExplosion)
            {
                GameObject fromPool = PoolManager.Instance.GetFromPool(LandingExplosionPrefab);
                Transform transform = fromPool.transform;
                Vector3 position = base.transform.position;
                float x = position.x + LandingExplosionOffset.x;
                float y = animController.SurfaceSensor.CurrGroundSurfaceHeight + LandingExplosionOffset.y;
                Vector3 position2 = base.transform.position;
                transform.position = new Vector3(x, y, position2.z + LandingExplosionOffset.z);
                Explosion component = fromPool.GetComponent<Explosion>();
                component.Init(this, new GameObject[1]
                {
                    base.gameObject
                });
            }
        }

        private void MoveToCarFunction(Vector2 moveInput)
        {
            if (MoveToCar)
            {
                MoveToCarTimer += Time.deltaTime;
                if (PlayerInteractionsManager.Instance.LastDrivableVehicle.Speed() > 7f)
                {
                    MoveToCarTimer = 0f;
                }
                if (MoveToCarTimer >= 10f && PlayerInteractionsManager.Instance.IsAbleToInteractWithVehicle())
                {
                    agent.enabled = false;
                    MoveToCar = false;
                    PlayerInteractionsManager.Instance.InteractionWithVehicle();
                    return;
                }
                if (Vector3.Distance(base.transform.position, PlayerInteractionsManager.Instance.LastDrivableVehicle.transform.position) > 15f)
                {
                    MoveToCar = false;
                    if (agent != null)
                    {
                        agent.enabled = false;
                    }
                }
                if (!moveInput.Equals(Vector2.zero))
                {
                    MoveToCar = false;
                    if (agent != null)
                    {
                        agent.enabled = false;
                    }
                    playerInteractionsManager.RemoveObstacles();
                }
                else if (agent != null)
                {
                    agent.enabled = true;
                    agent.SetDestination(CarToMove.position);
                    float sqrMagnitude = (CarToMove.position - base.transform.position).sqrMagnitude;
                    Vector3 _;
                    if (sqrMagnitude <= 0.5f && PlayerInteractionsManager.Instance.IsAbleToInteractWithVehicle() && !PlayerInteractionsManager.Instance.LastDrivableVehicle.IsDoorBlockedOffset(BlockedLayerMask, base.transform, out _))
                    {
                        agent.enabled = false;
                        MoveToCar = false;
                        PlayerInteractionsManager.Instance.InteractionWithVehicle();
                    }
                    move = SmoothVelocityVector((agent.steeringTarget - base.transform.position).normalized);
                }
            }
            else
            {
                if (!IsFlying)
                {
                    camForward = Vector3.Scale(cam.forward, new Vector3(1f, 0f, 1f)).normalized;
                }
                else
                {
                    camForward = cam.forward;
                }
                move = moveInput.y * camForward + moveInput.x * cam.right;
                MoveToCarTimer = 0f;
                agent.enabled = false;
            }
        }

        public void AbortMoveToCar()
        {
            MoveToCar = false;
        }

        public void ChangeStaminaForJump()
        {
            stats.stamina.SetAmount(0f - StaminaPerJump);
            StaminaRefillDelay = 1.5f;
        }

        public void ProceedStamina()
        {
            float num = IsFlying ? StaminaPerFlySprint : StaminaPerSprint;
            StaminaRefillDelay -= Time.deltaTime;
            if (StaminaRefillDelay <= 0f && !inputs.sprint && !inputs.jump && animController.GetForwardAmount() < 0.5f && !IsSwiming)
            {
                stats.stamina.DoFixedUpdate();
            }
            if (IsSwiming)
            {
                stats.stamina.SetAmount((0f - StaminaPerSwim) * Time.deltaTime);
            }
            if (inputs.sprint && stats.stamina.Current < num)
            {
                //if (AdManager.Instance.IsRewardVideoLoaded())
                //{
                //    AdManager.Instance.ShowRewardVideo((bool isDone) =>
                //    {
                //        if (!isDone)
                //        {
                //            inputs.sprint = false;
                //            stats.stamina.StatDisplay.OnChanged(num);
                //        }
                //        else
                //        {
                //            stats.stamina.Set(stats.stamina.Max);
                //        }
                //    });
                //}
                //else
                //{
                //    inputs.sprint = false;
                //    stats.stamina.StatDisplay.OnChanged(num);
                //}
            }
            if (inputs.eyeLaser && stats.stamina.Current < num)
            {
                //if (AdManager.Instance.IsRewardVideoLoaded())
                //{
                //    AdManager.Instance.ShowRewardVideo((bool isDone) =>
                //    {
                //        if (!isDone)
                //        {
                //            inputs.eyeLaser = false;
                //            stats.stamina.StatDisplay.OnChanged(num);
                //        }
                //        else
                //        {
                //            stats.stamina.Set(stats.stamina.Max);
                //        }
                //    });
                //}
                //else
                //{
                //    inputs.eyeLaser = false;
                //    stats.stamina.StatDisplay.OnChanged(num);
                //}
            }

            if (inputs.sprint && animController.GetForwardAmount() > 0f)
            {
                stats.stamina.SetAmount((0f - num) * Time.deltaTime);
                StaminaRefillDelay = 1.5f;
            }
            if (inputs.sprint)
            {
                inputs.moveInput = new Vector2(inputs.moveInput.x, 1f);
            }
            if (inputs.eyeLaser)
            {
                stats.stamina.SetAmount((0f - num) * Time.deltaTime);
                StaminaRefillDelay = 1.5f;
                PlayerStartUsingEyeLaser?.Invoke();
                //inputs.moveInput = new Vector2(inputs.moveInput.x, 1f);
                //todo : use i laser
            }
            else
            {
                PlayerStoptUsingEyeLaser?.Invoke();
            }
            // if (stats.stamina.Current < StaminaPerJump && HelpfullAdsManager.Instance != null && AdManager.Instance.IsRewardVideoLoaded())
            // {
            // 	//HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Stamina, null);
            // 	AdManager.Instance.ShowRewardVideo((bool isDone) =>
            // 	{
            // 		if(isDone)
            // 			stats.stamina.Set(stats.stamina.Max);
            // 	});
            // }
            if (stats.stamina.Current < stats.stamina.Max * 0.15f)
            {
                if (!StaminaAduioSource.isPlaying)
                {
                    StaminaAduioSource.Play();
                }
                // StatBar statBar = stats.stamina.StatDisplay as StatBar;
                // if (statBar != null)
                // {
                // 	statBar.Blink(statBar.BarColor);
                // }
            }
            else if (StaminaAduioSource.isPlaying)
            {
                StaminaAduioSource.Stop();
            }
            if (WeaponController.CurrentWeapon is EnergyAmmoRangedWeapon)
            {
                WeaponController.UpdateAmmoText((WeaponController.CurrentWeapon as EnergyAmmoRangedWeapon)?.AmmoCountText);
            }
        }

        public override void ReplaceOnRagdoll(bool canWakeUp, out GameObject initRagdoll, bool isDrowning = false)
        {
            CameraEffect cameraEffect = EffectManager.Instance.Create(Game.Character.CameraEffects.Type.SprintShake);
            cameraEffect.Stop();
            if (!animController.KeepFlyingAfterRagdoll)
            {
                ResetMoveState();
            }
            base.ReplaceOnRagdoll(canWakeUp, out initRagdoll, isDrowning);
        }

        public override void Footsteps()
        {
            if (!IsFlying)
            {
                if (FlyingSound)
                {
                    AudioSource.Stop();
                    AudioSource.pitch = 1f;
                    AudioSource.volume = SoundManager.instance.GetSoundValue();
                    FlyingSound = false;
                }
                base.Footsteps();
                return;
            }
            float a = Mathf.Abs(animController.GetForwardAmount());
            float b = Mathf.Abs(animController.GetStrafeAmount());
            float num = Mathf.Max(a, b);
            float maxHigh = animController.MaxHigh;
            Vector3 position = base.transform.position;
            float num2 = 1f - (maxHigh - position.y) / animController.MaxHigh;
            num2 *= num2;
            float num3 = num2 * 3f / 4f;
            if (!AudioSource.isPlaying)
            {
                int num4 = UnityEngine.Random.Range(0, PlayerManager.Instance.DefaulPlayer.FlySounds.Length);
                AudioSource.clip = PlayerManager.Instance.DefaulPlayer.FlySounds[num4];
                AudioSource.Play();
                FlyingSound = true;
            }
            AudioSource.pitch = num3 + num;
            AudioSource.volume = (num2 + num) * SoundManager.instance.GetSoundValue();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (ToHighToFly)
            {
                OnHit(DamageType.Instant, null, animController.OverHighDamagePerTic * Time.deltaTime, Vector3.zero, Vector3.zero, 0f);
                DangerIndicator.Instance.Activate("You are too hight. Сome down!");
            }
            else
            {
                DangerIndicator.Instance.Deactivate();
            }
            IsSwiming = CheckSwiming();
            IsDrowning = CheckDrowning();
            if (WaterEffect != null)
            {
                if (IsInWater || (animController.SurfaceSensor.AboveWater && IsFlying))
                {
                    WaterEffect.Play();
                    Transform transform = WaterEffect.transform;
                    Vector3 position = base.transform.position;
                    float x = position.x;
                    float currWaterSurfaceHeight = animController.SurfaceSensor.CurrWaterSurfaceHeight;
                    Vector3 position2 = base.transform.position;
                    transform.position = new Vector3(x, currWaterSurfaceHeight, position2.z);
                }
                else
                {
                    WaterEffect.Stop();
                }
            }
            if (Remote)
            {
                //Debug.LogError("remote ne");
                return;
            }
            mode = CameraManager.Instance.GetCurrentCameraMode();
            inputs.crouch = inputManager.GetInput(InputType.Crouch, defaultValue: false);
            inputs.moveInput = inputManager.GetInput(InputType.Move, Vector2.zero);
            inputs.sprint = inputManager.GetInput(InputType.Sprint, defaultValue: false);
            inputs.eyeLaser = inputManager.GetInput(InputType.EyeLaser, defaultValue: false);
            inputs.reset = inputManager.GetInput(InputType.Reset, defaultValue: false);
            inputs.jump = inputManager.GetInput(InputType.Jump, defaultValue: false);
            inputs.shootRope = inputManager.GetInput(InputType.ShootRope, defaultValue: false);
            inputs.fly = inputManager.GetInput(InputType.Fly, defaultValue: false);
            ProceedStamina();
            Health.DoFixedUpdate();
            MoveToCarFunction(inputs.moveInput);
            Footsteps();
            if (move.magnitude > 1f)
            {
                move.Normalize();
            }
            bool input = inputManager.GetInput(InputType.Walk, defaultValue: false);
            float num = WalkByDefault ? ((!input) ? 0.5f : 1f) : ((!input) ? 1f : 0.5f);
            move *= num;
            if (base.IsDead)
            {
                CameraManager.Instance.SetMode(Game.Character.Modes.Type.Dead);
            }
            if (inputs.reset)
            {
                CameraManager.Instance.ResetCameraMode();
                Resurrect();
            }
            bool flag = inputManager.GetInput(InputType.Fire, defaultValue: false) && !base.IsDead && animController.CanShootInCurrentState;
            bool flag2 = inputManager.GetInput(InputType.Aim, defaultValue: false) && !base.IsDead && !inputs.shootRope && (animController.AnimOnGround || IsFlying) && weaponController.CurrentWeapon is RangedWeapon;
            if (TargetManager.Instance.UseAutoAim)
            {
                TargetManager.Instance.MoveCrosshair = (!flag && (bool)TargetManager.Instance.AutoAimTarget);
                TargetManager.Instance.ColorCrosshair = TargetManager.Instance.MoveCrosshair;
            }
            AttackState attackState = UpdateAttackState(flag);
            if (attackState.CanAttack)
            {
                Attack();
            }
            MoveState moveState = state;
            if (inputs.sprint && !inputs.fly && !needToSwitchDefMoveState)
            {
                if (!IsFlying)
                {
                    if (animController.AnimOnGround)
                    {
                        state = MoveState.Sprint;
                    }
                    else
                    {
                        state = MoveState.Default;
                    }
                }
                else
                {
                    state = MoveState.FlySprint;
                }
            }
            else if ((attackState.Aim || flag2) && !needToSwitchDefMoveState)
            {
                if (IsFlying)
                {
                    state = MoveState.FlyAim;
                }
                else
                {
                    state = MoveState.Aim;
                }
            }
            else if (inputs.crouch && !needToSwitchDefMoveState)
            {
                state = MoveState.Crouch;
            }
            else if (animController.UseSuperFly && inputs.fly && animController.CanStartSuperFly)
            {
                if (Time.time - moveStateSwitchTime > 0.3f)
                {
                    if (defMoveState == MoveState.Default)
                    {
                        defMoveState = MoveState.Fly;
                        needToSwitchDefMoveState = true;
                    }
                    else if (defMoveState == MoveState.Fly)
                    {
                        ResetMoveState();
                        needToSwitchDefMoveState = true;
                    }
                    moveStateSwitchTime = Time.time;
                }
            }
            else
            {
                state = defMoveState;
                needToSwitchDefMoveState = false;
            }
            if (state == MoveState.Fly && moveState != MoveState.Fly)
            {
                Controls.SetControlsSubPanel(ControlsType.Character, 1);
            }
            else if (state == MoveState.Default && moveState != 0)
            {
                Controls.SetControlsSubPanel(ControlsType.Character);
            }
            if (animController.FlyNearWalls && !nearWalls)
            {
                mode.SetCameraConfigMode("SuperFlyNearWalls");
                nearWalls = true;
            }
            else if (!animController.FlyNearWalls && nearWalls)
            {
                if (state == MoveState.FlySprint)
                {
                    mode.SetCameraConfigMode("SuperFlySprint");
                }
                else
                {
                    mode.SetCameraConfigMode("SuperFly");
                }
                nearWalls = false;
            }
            if (moveState != state)
            {
                switch (state)
                {
                    case MoveState.Fly:
                        mode.SetCameraConfigMode("SuperFly");
                        break;
                    case MoveState.FlySprint:
                        mode.SetCameraConfigMode("SuperFlySprint");
                        break;
                    case MoveState.Default:
                        mode.SetCameraConfigMode("Default");
                        break;
                    case MoveState.Crouch:
                        mode.SetCameraConfigMode("Crouch");
                        break;
                    case MoveState.Aim:
                    case MoveState.FlyAim:
                        if (!attackState.RangedAttackState.Equals(RangedAttackState.None))
                        {
                            mode.SetCameraConfigMode("Aim");
                        }
                        else if (!attackState.RangedAttackState.Equals(MeleeAttackState.None))
                        {
                            mode.SetCameraConfigMode("MeleeAim");
                        }
                        break;
                    case MoveState.Sprint:
                        {
                            mode.SetCameraConfigMode("Sprint");
                            CameraEffect cameraEffect = EffectManager.Instance.Create(Game.Character.CameraEffects.Type.SprintShake);
                            cameraEffect.Loop = true;
                            cameraEffect.Play();
                            break;
                        }
                }
                if (moveState == MoveState.Sprint)
                {
                    CameraEffect cameraEffect2 = EffectManager.Instance.Create(Game.Character.CameraEffects.Type.SprintShake);
                    cameraEffect2.Stop();
                }
            }
            attackState.Aim = (state == MoveState.Aim || state == MoveState.FlyAim);
            Aim(attackState.Aim);
            lookPos = base.transform.position + LookVector * 100f;
            UnityEngine.Debug.DrawRay(base.transform.position, (lookPos - base.transform.position) * 100f, Color.red);
            if (MoveToCar || playerInteractionsManager.inVehicle || IsSwiming)
            {
                attackState.RangedAttackState = RangedAttackState.None;
                attackState.MeleeAttackState = MeleeAttackState.None;
                attackState.Aim = false;
                attackState.CanAttack = false;
            }
            animController.Move(new Input
            {
                camMove = move,
                crouch = (state == MoveState.Crouch),
                inputMove = inputs.moveInput,
                jump = (inputs.jump && !MoveToCar),
                lookPos = lookPos,
                die = base.IsDead,
                reset = inputs.reset,
                smoothAimRotation = false,
                aimTurn = false,
                sprint = (inputs.sprint && !MoveToCar),
                AttackState = attackState,
                shootRope = inputs.shootRope,
                fly = IsFlying,
                eyeLaser = inputs.eyeLaser
            });
        }

        public void JumpOutFromVehicle(bool canWakeUp, float animationTimeLength, bool isReplaceOnRagdoll = true, bool lookOnVehicleFirst = true, bool jumpInAir = false, bool revertYRotateOnFinish = false, DrivableVehicle vehicle = null)
        {
            if (!IsTransformer)
            {
                StartCoroutine(JumpOut(canWakeUp, animationTimeLength, isReplaceOnRagdoll, lookOnVehicleFirst, jumpInAir, revertYRotateOnFinish));
                if (vehicle != null)
                {
                    vehicle.ResetDriver();
                }
            }
        }

        public void CastSkill(int id)
        {
            animController.CastSkill(id);
        }

        protected override void Swim()
        {
            IsSwiming = CheckSwiming();
        }

        public bool CheckDrowning()
        {
            bool flag = animController.SurfaceSensor.InWater && !IsFlying && (IsTransformer || PlayerInteractionsManager.Instance.inVehicle || (IsSwiming && stats.stamina.Current <= 0f));
            if (flag)
            {
                OnHit(DamageType.Water, null, DamagePerDrow * Time.deltaTime, Vector3.zero, Vector3.zero, 0f);
            }
            return flag;
        }

        private bool CheckSwiming()
        {
            float currWaterSurfaceHeight = animController.SurfaceSensor.CurrWaterSurfaceHeight;
            Vector3 position = base.transform.position;
            float num = currWaterSurfaceHeight - position.y;
            bool flag = num > Mathf.Abs(SwimOffset);
            if (IsFlying || IsTransformer || !IsInWater || (IsInWater && !flag))
            {
                return false;
            }
            base.Swim();
            return true;
        }

        private IEnumerator JumpOut(bool canWakeUp, float animationTimeLength, bool isReplaceOnRagdoll, bool lookOnVehicleFirst, bool jumpInAir, bool revertYRotateOnFinish)
        {
            animController.tweakStart = false;
            animController.enabled = true;
            animController.ExitAnimStart();
            GetComponent<Animator>().enabled = true;
            DrivableVehicle vehicle = PlayerInteractionsManager.Instance.LastDrivableVehicle;
            VehicleType currentVehicleType = vehicle.GetVehicleType();
            animController.GetInOutVehicle(currentVehicleType, false, false, true, true, jumpInAir);
            yield return new WaitForFixedUpdate();
            if ((bool)vehicle.VehiclePoints.JumpOutPosition && !base.IsDead)
            {
                base.transform.position = vehicle.VehiclePoints.JumpOutPosition.position;
            }
            CameraManager.Instance.SetCameraTarget(GetHips());
            yield return new WaitForSeconds(animationTimeLength);
            base.transform.parent = null;
            collider.enabled = true;
            rigidbody.isKinematic = false;
            base.enabled = true;
            CharacterSensor.gameObject.SetActive(value: true);
            animController.ExitAnimEnd();
            if (isReplaceOnRagdoll)
            {
                ReplaceOnRagdoll(canWakeUp);
                Transform rdHips = GetRagdollHips();
                CameraManager.Instance.SetCameraTarget(rdHips);
                Vector3 force = PlayerInteractionsManager.Instance.LastDrivableVehicle.MainRigidbody.velocity * 3f + Vector3.up * 0.2f;
                rdHips.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
            }
            if (revertYRotateOnFinish)
            {
                base.transform.forward = -base.transform.forward;
                CameraManager.Instance.UnityCamera.transform.forward = base.transform.forward;
            }
            dontGoThroughThings.enabled = true;
            ResetRotation();
        }

        public void ResetRotation()
        {
            Transform transform = base.transform;
            Vector3 eulerAngles = base.transform.eulerAngles;
            transform.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
        }

        public void GetInOutVehicle(bool isInOut, bool force, DrivableVehicle vehicle, bool crash = false)
        {
            bool flag = !isInOut;
            bool flag2 = isInOut;
            animController.enabled = flag;
            collider.enabled = !flag2;
            if (!IsTransformer)
            {
                rigidbody.isKinematic = flag2;
            }
            dontGoThroughThings.enabled = flag;
            CharacterSensor.gameObject.SetActive(flag);
            if (flag2)
            {
                base.enabled = false;
                weaponController.HideWeapon();
            }
            else
            {
                GetComponent<Animator>().enabled = true;
                if (!vehicle.HasExitAnimation() || crash)
                {
                    weaponController.ShowWeapon();
                }
            }
            Vector3 offset = Vector3.zero;
            if (flag2 && (MoveToCarTimer >= 10f || !vehicle.HasEnterAnimation()))
            {
                EnterToVehicleInstantly(vehicle);
                MoveToCarTimer = 0f;
            }
            else if (flag && (vehicle.IsDoorBlockedOffset(BlockedLayerMask, base.transform, out offset, flag2) || crash || !vehicle.HasExitAnimation()))
            {
                ForcedOutOfTheVehicle(offset, vehicle);
            }
            else
            {
                vehicle.AnimateGetInOut = true;
                bool flag3 = vehicle.PointOnTheLeft(base.transform.position + offset);
                if (flag && !IsTransformer)
                {
                    base.transform.position = vehicle.GetExitPosition(flag3);
                }
                animController.GetInOutVehicle(vehicle.GetVehicleType(), flag2, force, flag3);
            }
            if (flag)
            {
                vehicle.ResetDriver();
            }
            weaponController.CheckReloadOnWakeUp();
            if (PlayerGetInOutVehicleEvent != null)
            {
                PlayerGetInOutVehicleEvent(isInOut);
            }
        }

        private void ApplyOffset(Vector3 offset)
        {
            if (base.gameObject.activeSelf)
            {
                StartCoroutine(ApplyOffsetDelay(offset));
            }
        }

        private IEnumerator ApplyOffsetDelay(Vector3 offset)
        {
            yield return new WaitForEndOfFrame();
            base.transform.position += offset;
        }

        private void EnterToVehicleInstantly(DrivableVehicle vehicle)
        {
            vehicle.AnimateGetInOut = false;
            GetComponent<Animator>().enabled = false;
            if (!IsTransformer)
            {
                base.transform.position = vehicle.VehiclePoints.EnterFromPositions[0].position;
                base.transform.rotation = Quaternion.identity;
            }
            if (!vehicle.IsControlsPlayerAnimations())
            {
                animController.SetGetInTrigger(vehicle);
                animController.MainAnimator.enabled = false;
                PlayerInteractionsManager.Instance.MoveCharacterToSitPosition(PlayerInteractionsManager.Instance.CharacterHips, PlayerInteractionsManager.Instance.DriverHips, 1f);
                PlayerInteractionsManager.Instance.TweakingSkeleton(PlayerInteractionsManager.Instance.CharacterHips, PlayerInteractionsManager.Instance.DriverHips, 1f);
            }
        }

        public void ProceedDriverStatus(DrivableVehicle vehicle, bool isGettingIn, float delayedActivateTime = 0f)
        {
            if (isGettingIn)
            {
                initiatedDriverStatusGO = PoolManager.Instance.GetFromPool(DriverStatusPrefab);
                PoolManager.Instance.AddBeforeReturnEvent(initiatedDriverStatusGO, delegate
                {
                    DriverStatus component = initiatedDriverStatusGO.GetComponent<DriverStatus>();
                    DrivableVehicle drivableVehicle2 = vehicle;
                    component.DamageEvent -= drivableVehicle2.OnDriverStatusDamageEvent;
                    initiatedDriverStatusGO = null;
                });
                initiatedDriverStatusGO.transform.parent = vehicle.gameObject.transform;
                initiatedDriverStatusGO.transform.localPosition = vehicle.VehicleSpecificPrefab.PlayerDriverStatusPosition;
                initiatedDriverStatusGO.transform.localEulerAngles = vehicle.VehicleSpecificPrefab.PlayerDriverStatusRotation;
                InitiatedDriverStatus = initiatedDriverStatusGO.GetComponent<DriverStatus>();
                DriverStatus initiatedDriverStatus = InitiatedDriverStatus;
                DrivableVehicle drivableVehicle = vehicle;
                initiatedDriverStatus.DamageEvent += drivableVehicle.OnDriverStatusDamageEvent;
                InitiatedDriverStatus.Init(base.gameObject, vehicle.DriverIsVulnerable);
                vehicle.CurrentDriver = InitiatedDriverStatus;
                if (delayedActivateTime > 0f)
                {
                    initiatedDriverStatusGO.SetActive(value: false);
                    Invoke("DelayedDriverStatusActivator", delayedActivateTime);
                }
            }
            else if (!isGettingIn && (bool)InitiatedDriverStatus)
            {
                PoolManager.Instance.ReturnToPool(InitiatedDriverStatus);
            }
        }

        private void DelayedDriverStatusActivator()
        {
            if (initiatedDriverStatusGO != null)
            {
                initiatedDriverStatusGO.SetActive(value: true);
            }
        }

        private void ForcedOutOfTheVehicle(Vector3 offset, DrivableVehicle vehicle)
        {
            transform.parent = null;
            GetComponent<Animator>().Rebind();
            vehicle.AnimateGetInOut = false;
            enabled = true;
            transform.position = vehicle.transform.position + offset;
            animController.ExitAnimStart();
            animController.ExitAnimEnd();
        }

        private Vector3 SmoothVelocityVector(Vector3 v)
        {
            velocityFilter.AddSample(new Vector2(v.x, v.z));
            Vector2 value = velocityFilter.GetValue();
            return new Vector3(value.x, 0f, value.y).normalized;
        }

        public void AddAmmoForCurrentWeapon()
        {
            AmmoManager.Instance.AddAmmo(weaponController.CurrentWeapon.AmmoType);
        }

        public bool CheckIsPlayerWeapon(Weapon weapon)
        {
            return weaponController.CheckIsThisWeaponControllerWeapon(weapon);
        }

        public void ShowWeapon()
        {
            weaponController.ShowWeapon();
        }

        public void AddHealth(float amount)
        {
            Health.Change(amount);
            if (Health.Current > Health.Max)
            {
                Health.Current = Health.Max;
            }
            if ((bool)InitiatedDriverStatus)
            {
                InitiatedDriverStatus.GetComponent<DriverStatus>().Health.Change(amount);
            }
            if (currentRagdoll != null)
            {
                RagdollStatus componentInChildren = currentRagdoll.GetComponentInChildren<RagdollStatus>();
                componentInChildren.Health.Change(amount);
            }
        }

        protected override void OnCollisionEnter(Collision col)
        {
            int layer = col.collider.gameObject.layer;
            if (layer != LayerMask.NameToLayer("SmallDynamic") && !IsSwiming && (!animController.useSuperheroLandings || layer == LayerMask.NameToLayer("BigDynamic") || layer == LayerMask.NameToLayer("SmallDynamic")))
            {
                base.OnCollisionEnter(col);
            }
        }

        protected override void OnCollisionSpecific(Collision col)
        {
            if (!IsTransformer && animController.NeedCollisionCheckInCurrentState)
            {
                base.OnCollisionSpecific(col);
                if (!base.RDCollInvul)
                {
                    CameraManager.Instance.SetCameraTarget(currentRagdoll.transform.Find("metarig").Find("hips"));
                }
            }
        }

        public void BakeRootModelOnPose(GameObject newPose)
        {
            CopyTransformRecurse(newPose.transform, rootModel);
        }

        protected override void OnDie()
        {
            if (isChoosingDieOrAlive) return;
            isChoosingDieOrAlive = true;
            if (HelpfullAdsManager.Instance != null /*&& AdManager.Instance.IsRewardVideoLoaded()*/)
            {
                //UICanvasController.Instance.CloseAllOnePositionPanel(AbsUICanvas.Position.Middle);
                HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.Heal, OnDieAction);
            }
            else
            {
                if (!Dead)
                {
                    OnDieAction(notDie: false);
                }
            }

        }

        private void OnDieAction(bool notDie)
        {
            if (!notDie)
            {
                Dead = true;
                UICanvasController.Instance.CloseAllPanel();
                if (PlayerInteractionsManager.Instance.inVehicle)
                {
                    PlayerInteractionsManager.Instance.DieInCar();
                }
                else
                {
                    base.OnDie();
                    PlayerDieManager.Instance.OnPlayerDie();
                }
                if (IsTransformer)
                {
                    Transformer.DeInit();
                }
            }
            else
            {
                isChoosingDieOrAlive = false;
            }
        }

        protected override void OnDieSpecific()
        {
            if (!currentRagdoll)
            {
                ReplaceOnRagdoll(false, IsSwiming);
                return;
            }
            RagdollWakeUper componentInChildren = currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
            if (componentInChildren != null)
            {
                componentInChildren.DeInitRagdoll(mainObjectDead: true, callOnDieEvent: false);
            }
        }

        public void ResetRagdoll()
        {
            if (currentRagdoll == null) return;
            RagdollWakeUper componentInChildren = currentRagdoll.GetComponentInChildren<RagdollWakeUper>();
            if (componentInChildren != null)
            {
                componentInChildren.DeInitRagdoll(base.IsDead, callOnDieEvent: false, instantly: true);
            }
        }

        public override void Resurrect()
        {
            //Debug.LogError("go here");
            base.Resurrect();
            ClearCurrentRagdoll();
            StopAllCoroutines();
            MoveToCar = false;
            base.gameObject.SetActive(value: true);
            base.gameObject.transform.parent = null;
            collider.enabled = true;
            rigidbody.isKinematic = false;
            rigidbody.velocity = Vector3.zero;
            CharacterSensor.gameObject.SetActive(value: true);
            base.enabled = true;
            Dead = false;
            Health.Setup(.5f);
            isChoosingDieOrAlive = false;
            stats.stamina.Setup();
            CheckReloadOnWakeUp();
            Animator component = GetComponent<Animator>();
            component.enabled = true;
            component.Rebind();
            animController.enabled = true;
            animController.rope.Disable();
            animController.Reset();
            animController.ClimbEnd();
            CameraManager.Instance.SetCameraTarget(base.transform);
            CameraManager.Instance.ResetCameraMode();
            CameraManager.Instance.ActivateModeOnStart.SetCameraConfigMode("Default");
            CameraManager.Instance.GetCurrentCameraMode().Reset();
            Game.MiniMap.MiniMap.Instance.SetTarget(base.gameObject);
            TrafficManager.Instance.ResetTransformersSpawnTime();
            ResetMoveState();
            Controls.SetControlsSubPanel(ControlsType.Character);
            animController.SurfaceSensor.Reset();
            animController.ResetCollisionNormal();
            if (IsTransformer)
            {
                Transformer.Init();
                animController.ExitAnimEnd();
            }
        }

        public void ResetMoveState()
        {
            defMoveState = MoveState.Default;
            animController.ResetDrag();
        }

        public void LostCurrentWeapon()
        {
            if (GameManager.ShowDebugs)
            {
                UnityEngine.Debug.Log("Need implement this function in weapon controlle");
            }
        }

        protected override void DropPickup()
        {
        }

        protected override void VelocityCheck()
        {
            if (animController.NeedSpeedCheckInCurrentState)
            {
                base.VelocityCheck();
            }
        }
    }
}
