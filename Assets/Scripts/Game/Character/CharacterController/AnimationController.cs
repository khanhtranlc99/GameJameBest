using Game.Character.CharacterController.Enums;
using Game.Character.Modes;
using Game.Character.Stats;
using Game.Effects;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using Game.Weapons;
using RopeNamespace;
using System;
using System.Collections.Generic;
using Root.Scripts.Helper;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public class AnimationController : MonoBehaviour
	{
		public delegate void OnFallImpact(Vector3 velocity);

		public delegate void SuperFlyDelegate();

		public delegate void Jumping();

		private const float MaxmaxHeightObstacle = 3f;

		private const float ExitTimeMultipler = 0.7f;

		private const int BaseLayer = 0;

		private const float Half = 0.5f;

		[SerializeField]
		private float jumpPower = 12f;

		[SerializeField]
		private float airSpeed = 6f;

		[SerializeField]
		public float superheroAirSpeed = 5f;

		[SerializeField]
		public float superheroFastAirSpeed = 10f;

		[SerializeField]
		private float maxGlideAngle = 45f;

		[SerializeField]
		private float airControl = 2f;

		[SerializeField]
		[Range(0f, 10f)]
		public float gravityMultiplier = 1f;

		[Range(0f, 1f)]
		[SerializeField]
		private float moveToSprintMultiplier = 0.8f;

		[Range(0.1f, 3f)]
		[SerializeField]
		private float moveSpeedMultiplier = 1f;

		[SerializeField]
		[Range(0.1f, 3f)]
		private float animSpeedMultiplier = 1f;

		[SerializeField]
		private AdvancedSettings advancedSettings;

		private bool isAiming;

		public LayerMask ObstaclesLayerMask;

		public OnFallImpact OnFallImpactCallback;

		public WeaponController weaponController;

		[HideInInspector]
		public bool tweakStart;

		public bool AnimOnGround;

		public SurfaceSensor SurfaceSensor;

		public Animator MainAnimator;

		public bool useIKLook = true;

		public bool DebugLog;

		[Separator("Specific for Transformer")]
		public GameObject SmashPrefab;

		public Vector3 SmashOffset = new Vector3(0f, 0f, 1f);

		public SuperFlyDelegate StartSuperFlyEvent;

		public SuperFlyDelegate StopSuperFlyEvent;

		private float originalHeight;

		private float lastAirTime;

		private CapsuleCollider capsule;

		private Input input;

		private float forwardAmount;

		private float turnAmount;

		private int turnAnimMult = 1;

		private float strafeAmount;

		private Vector3 lookPos;

		private AnimState animState;

		private Obstacle obstacle;

		private Vector3 obstacleVelocity;

		private bool climbTrigger;

		private int climbTriggerHash;

		private float climbVelocity;

		private bool startClimbEvent;

		private Vector3 velocity;

		private IComparer<RaycastHit> rayHitComparer;

		private bool isGetinOutState;

		private Rigidbody rbody;

		private Collider coll;

		private bool meleePerformed;

		private bool IsSuperFlying;

		private bool ropeIsActive;

		private bool isPlayer;

		private Player player;

		private float fistsAttackSpeedMultipler = 1f;

		private float meleeWeaponAttackSpeedMultipler = 1f;

		private float defaultDrag = 1f;

		private Vector3 prevFallVelocity = Vector3.zero;

		private float FUCounts;

		private bool resetNearWalls;

		private bool previousIsAnimOnGround;

		private int SimpleObjectsLH;

		private int ComplexObjectsLH;

		private int SmallDynamicLH;

		private int ForwardHash;

		private int TurnHash;

		private int CrouchHash;

		private int OnGroundHash;

		private int JumpHash;

		private int JumpLegHash;

		private int StrafeHash;

		private int StrafeDirHash;

		private int SprintHash;

		private int ResetHash;

		private int DieHash;

		private int ClimbLowHash;

		private int ClimbMediumHash;

		private int ClimbHighHash;

		private int DoMeleeHash;

		private int MeleeHash;

		private int RangedWeaponTypeHash;

		private int RangedWeaponShootHash;

		private int RangedWeaponRechargeHash;

		private int GetInVehicleHash;

		private int GetOutVehicleHash;

		private int JumpOutVehicleHash;

		private int ForceGetVehicleHash;

		private int MeleeWeaponTypeHash;

		private int IsMovingHash;

		private int MeleeWeaponAttackSpeedHash;

		private int StartInCarHash;

		private int DeadInCarHash;

		private int VehicleTypeHash;

		private int CharacterLeftFromVehicleHash;

		private int NearWallHash;

		private int IsOnWaterHash;

		private int ShootRopeFlyHash;

		private int ShootRopeDragHash;

		private int ReShootRopeHash;

		private int UseSuperheroLandingsHash;

		private int FlyingHash;

		private int RopeFailHash;

		private int GlidingHash;

		private int IsFlyingHash;

		private int CloakForwardHash;

		private int CloakStrafeHash;

		private int GroundedSpeedMultiplierHash;
		private int castingSkillHash;
		private int idSkillHash;

		private AnimState defAnimState;

		[Separator("Wall climbing parameters")]
		public bool EnableClimbWalls;

		private bool _hasWall;

		private ClimbingSensors climbingSensors;

		private Vector3 WallNormal = Vector3.zero;

		private bool tweening;

		private bool reverseTurn;

		public float ClimbingSpeed = 2f;

		private float ClimbingForwardSpeed = 3f;

		public float JumpOffTheWallForce = 500f;

		public float climbDotParameter = 0.5f;

		[Separator("Rope shooting parameters")]
		public GameObject[] ShootRopeButtons;

		public GameObject SprintButton;

		public bool useRope;

		public bool useSuperheroLandings;

		public LayerMask WallsLayerMask;

		public LayerMask DoNotShootThroughThisLayerMask;

		public LayerMask DragLayerMask;

		public float maxRopeXZDistance = 100f;

		public float ropeStartForce = 1000f;

		public float ropeForce = 800f;

		public GameObject CharacterModel;

		public bool useGravityMults;

		[SerializeField]
		[Range(0f, 10f)]
		public float jumpGravityMultiplier = 1f;

		[SerializeField]
		[Range(0f, 10f)]
		public float fallGravityMultiplier = 1f;

		public float MaxFallSpeed = 110f;

		private float startTime;

		private float flyStartDelay = 0.4f;

		private RaycastHit target;

		public float dragDelay = 0.5f;

		public Rope rope;

		public float ropeExpandTime = 0.2f;

		public float ropeStraighteningTime = 0.2f;

		private float dragVerticalSpeedComponent = 5f;

		private Vector3 targetOffset = new Vector3(0f, 1f, 0f);

		public float cooldownTime = 1f;

		private float cooldownStartTime;

		public float StaminaCost;

		private RopeShootState ropeState = RopeShootState.Default;

		public float jumpToFallSpeed = 3f;

		public float fallToGlideSpeed = 10f;

		private bool flyCameraEnabled;

		private Vector3 movingTargetOffset;

		private bool _gliding;

		public float MinHeightForGlide = 5f;

		[Separator("SuperFly parameters")]
		public bool UseSuperFly;

		[Space(5f)]
		public GameObject[] FlyInputs = new GameObject[2];

		public float FlyInputsCD = 1f;

		[Space(5f)]
		public float MaxAngleNearTheGround = 5f;

		[Space(5f)]
		public float SuperFlyDefaultSpeed = 10f;

		[HideInInspector]
		public float SuperFlySpeed = 10f;

		public float SuperFlyBackwardsSpeed = 5f;

		public float SuperFlyStrafeSpeed = 5f;

		[Space(5f)]
		public float SuperFlyDefaultSprintSpeed = 30f;

		[HideInInspector]
		public float SuperFlySprintSpeed = 30f;

		[Space(5f)]
		public bool KeepFlyingAfterRagdoll = true;

		public bool RDExpInvulOnFly;

		public bool RDCollInvulOnFly;

		[Space(5f)]
		public bool UseRopeWhileFlying;

		[Space(5f)]
		public float FlyRotationLerpMult = 10f;

		[Space(5f)]
		public float MaxHigh = 100f;

		public float OverHighDamagePerTic = 25f;

		private float FlyInputsCDstart;

		private Vector3 collisionNormal;

		private Transform cameraTransform;

		private bool flyNearWalls;

		private float nearWallsLastChangeTime;

		private float activateTimer;

		public bool IsGettingInOrOut => isGetinOutState;

		public Vector2 SpeedMults
		{
			get
			{
				return new Vector2(moveSpeedMultiplier, animSpeedMultiplier);
			}
			set
			{
				moveSpeedMultiplier = value.x;
				animSpeedMultiplier = value.y;
			}
		}

		public bool IsWallClimbing => animState == AnimState.WallClimb;

		public bool IsRopeFlying => ropeIsActive;

		public bool CanStartSuperFly => !IsWallClimbing && !IsRopeFlying;

		public float AirSpeed
		{
			get
			{
				return airSpeed;
			}
			set
			{
				airSpeed = value;
			}
		}

		public Transform lookTarget
		{
			get;
			set;
		}

		public AnimState CurAnimState => animState;

		public bool CanShootInCurrentState => !ropeIsActive && animState != AnimState.WallClimb && (IsSuperFlying || player.IsSwiming || AnimOnGround);

		public bool NeedSpeedCheckInCurrentState => !IsSuperFlying && !useSuperheroLandings && !useRope && AnimOnGround && isGetinOutState;

		public bool NeedCollisionCheckInCurrentState => !IsRopeFlying && !IsGliding && (!IsSuperFlying || !RDCollInvulOnFly);

		public bool FlyNearWalls => flyNearWalls;

		private bool HasWall
		{
			get
			{
				return _hasWall;
			}
			set
			{
				if (value && !_hasWall)
				{
					CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("WallClimb");
				}
				else if (!value && _hasWall)
				{
					CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Default");
				}
				_hasWall = value;
			}
		}

		private bool IsGliding
		{
			get
			{
				return _gliding;
			}
			set
			{
				_gliding = value;
			}
		}

		private Vector3 TargetDirection => (target.point - rbody.transform.position).normalized;

		private float TargetDistance => (target.point - rbody.transform.position).magnitude;

		public event Jumping JumpEvent;

		public bool IsAming()
		{
			return isAiming;
		}

		public void Initialization()
		{
			ComponentInitialize();
			SetUpAnimator();
			cameraTransform = CameraManager.Instance.UnityCamera.transform;
			SimpleObjectsLH = LayerMask.NameToLayer("SimpleStaticObject");
			ComplexObjectsLH = LayerMask.NameToLayer("ComplexStaticObject");
			SmallDynamicLH = LayerMask.NameToLayer("SmallDynamic");
		}

		public void InicializeWithoutDestroyChildAnimator()
		{
			ComponentInitialize();
		}

		public void SetAnimator(Animator newAnimator)
		{
			MainAnimator = newAnimator;
		}

		public void StartTweak()
		{
			tweakStart = true;
		}

		public void ExitAnimEnd()
		{
			if (isGetinOutState && (bool)player)
			{
				player.enabled = true;
				player.collider.enabled = true;
				player.ShowWeapon();
				if (!player.IsTransformer)
				{
					player.transform.parent = null;
				}
				player.ResetRotation();
				isGetinOutState = false;
			}
		}

		public float GetForwardAmount()
		{
			return forwardAmount;
		}

		public float GetStrafeAmount()
		{
			return strafeAmount;
		}

		public void Smash()
		{
			if ((bool)SmashPrefab)
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool(SmashPrefab);
				fromPool.transform.position = base.transform.position + base.transform.right * SmashOffset.x + base.transform.up * SmashOffset.y + base.transform.forward * SmashOffset.z;
				Explosion component = fromPool.GetComponent<Explosion>();
				component.Init(GetComponent<Human>(), new GameObject[1]
				{
					base.gameObject
				});
			}
		}

		public void CastSkill(int idSkill)
		{
			MainAnimator.SetInteger(idSkillHash,idSkill);
			MainAnimator.SetTrigger(castingSkillHash);
		}

		public void ExitAnimStart()
		{
			if (!isGetinOutState)
			{
				Player component = GetComponent<Player>();
				if ((bool)component)
				{
					component.enabled = false;
				}
				isGetinOutState = true;
			}
		}

		public void Reset()
		{
			MainAnimator.StopPlayback();
			input.AttackState.Aim = false;
			input.crouch = false;
			input.die = false;
			input.jump = false;
			input.shootRope = false;
			AnimOnGround = true;
			animState = defAnimState;
			HasWall = false;
			forwardAmount = 0f;
			strafeAmount = 0f;
			turnAmount = 0f;
			velocity = Vector3.zero;
			prevFallVelocity = Vector3.zero;
			SurfaceSensor.AboveGround = true;
			IsSuperFlying = false;
			Move(input);
		}

		public void SetCollisionInvulStatus(bool status)
		{
			RDCollInvulOnFly = status;
			if (!status && IsSuperFlying)
			{
				player.RDCollInvul = false;
			}
		}

		public void SetExplosionInvulStatus(bool status)
		{
			RDExpInvulOnFly = status;
			if (!status && IsSuperFlying)
			{
				player.RDExpInvul = false;
			}
		}

		public void ResetDrag()
		{
			rbody.drag = defaultDrag;
		}

		public void SetCapsuleToVertical()
		{
			if (capsule != null && capsule.direction != 1)
			{
				capsule.direction = 1;
				capsule.center = new Vector3(0f, 1f, 0f);
			}
		}

		public void SetCapsuleToHorizontal()
		{
			if (capsule != null)
			{
				capsule.direction = 2;
				capsule.center = new Vector3(0f, 0.55f, 0f);
			}
		}

		public void Move(Input controllerInput)
		{
			if (!MainAnimator)
			{
				return;
			}
			input = controllerInput;
			if (isPlayer)
			{
				ManageSuperFlying();
			}
			UpdateAnimState();
			if (!IsSuperFlying)
			{
				CheckForObstacles();
			}
			lookPos = input.lookPos;
			velocity = rbody.velocity;
			ConvertMoveInput();
			TurnTowardsCameraForward();
			ScaleCapsule();
			ApplyExtraTurnRotation();
			AnimOnGroundCheck();
			SetFriction();
			if (AnimOnGround || player.IsSwiming)
			{
				if (ShouldStickToTheGround() && (!isPlayer || (isPlayer && !IsSuperFlying)))
				{
					HandleGroundedVelocities();
				}
			}
			else if (!isPlayer || (isPlayer && !IsSuperFlying))
			{
				HandleAirborneVelocities();
			}
			if (isPlayer)
			{
				if (EnableClimbWalls && !IsSuperFlying)
				{
					WallClimbCheck();
				}
				if ((useRope || (IsSuperFlying && UseRopeWhileFlying)) && (animState != AnimState.Fly || !input.sprint))
				{
					HandleRopeStage();
				}
			}
			UpdateAnimator();
			rbody.velocity = velocity;
		}

		public void UpdatePlayerStats()
		{
			if (isPlayer)
			{
				meleeWeaponAttackSpeedMultipler = player.stats.GetPlayerStat(StatsList.MeleeWeaponAttackSpeed);
				SuperFlySpeed = SuperFlyDefaultSpeed * player.stats.GetPlayerStat(StatsList.SuperFlySpeedMult);
				SuperFlyBackwardsSpeed = (SuperFlyStrafeSpeed = SuperFlySpeed / 2f);
				SuperFlySprintSpeed = SuperFlyDefaultSprintSpeed * player.stats.GetPlayerStat(StatsList.SuperFlySpeedMult);
			}
		}

		public void OnAnimatorMove()
		{
			if (isGetinOutState)
			{
				return;
			}
			Vector3 lookDelta = base.transform.InverseTransformDirection(lookPos - base.transform.position);
			switch (animState)
			{
			case AnimState.MoveAim:
			{
				float num = Mathf.Atan2(lookDelta.x, lookDelta.z) * 57.29578f;
				if (input.smoothAimRotation)
				{
					num *= Time.deltaTime * 10f;
				}
				base.transform.Rotate(0f, num, 0f);
				rbody.rotation = base.transform.rotation;
				break;
			}
			case AnimState.Obstacle:
				if (startClimbEvent)
				{
					base.transform.position = Vector3.SmoothDamp(base.transform.position, obstacle.WallPoint + Vector3.up * 0.1f, ref obstacleVelocity, Time.deltaTime * climbVelocity);
				}
				break;
			case AnimState.WallClimb:
			{
				Vector3 vector = MainAnimator.velocity;
				vector = rbody.transform.InverseTransformVector(vector);
				vector.z = ((!reverseTurn) ? ClimbingForwardSpeed : 0f);
				rbody.velocity = rbody.transform.TransformVector(vector);
				break;
			}
			}
			if (IsSuperFlying)
			{
				SuperFlyMovement(lookDelta);
			}
			else
			{
				rbody.rotation = MainAnimator.rootRotation;
				if ((AnimOnGround || player.IsSwiming) && Time.deltaTime > 0f)
				{
					Vector3 vector2 = MainAnimator.deltaPosition * moveSpeedMultiplier / Time.deltaTime;
					vector2.y = 0f;
					rbody.velocity = vector2;
				}
			}
			isAiming = (((animState == AnimState.MoveAim || animState == AnimState.FlyAim) && input.AttackState.RangedAttackState != RangedAttackState.Recharge) ? true : false);
		}

		public void ClimbEnd()
		{
			startClimbEvent = false;
			animState = defAnimState;
			rbody.useGravity = true;
			capsule.enabled = true;
		}

		public void NotAnimatedGetInOutVehicle(bool on, GameObject hidedObject)
		{
			hidedObject.SetActive(!on);
		}

		public void SetGetInTrigger(DrivableVehicle vehicle)
		{
			//Debug.LogError("get in");
			MainAnimator.SetTrigger(GetInVehicleHash);
			MainAnimator.SetInteger(VehicleTypeHash, (int)vehicle.GetVehicleType());
		}

		public void GetInOutVehicle(VehicleType vehicleType, bool isGettingIn, bool force, bool isLeft, bool jump = false, bool jumpInAir = false)
		{
			MainAnimator.SetInteger(VehicleTypeHash, (int)vehicleType);
			MainAnimator.SetBool(CharacterLeftFromVehicleHash, isLeft);
			MainAnimator.SetBool(JumpOutVehicleHash, jump);
			//MainAnimator.SetTrigger(GetInVehicleHash);
			if (isGettingIn)
			{
				//Debug.LogError("get in");
				MainAnimator.SetBool(GetOutVehicleHash,false);
				MainAnimator.SetTrigger(GetInVehicleHash);
				MainAnimator.SetBool(ForceGetVehicleHash, force);
			}
			else if (jump)
			{
				MainAnimator.SetBool(GetOutVehicleHash,true);
				if (jumpInAir)
				{
					MainAnimator.SetBool(OnGroundHash, value: false);
				}

			}
			else
			{
				//Debug.LogError("get out");
				//MainAnimator.SetBool("GetInside", value: false);
				MainAnimator.SetBool(GetOutVehicleHash,true);
				MainAnimator.SetBool(ForceGetVehicleHash, force);

			}
			float delay = Time.deltaTime;
			if (MainAnimator.GetCurrentAnimatorClipInfo(0).Length > 0)
			{
				delay = MainAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.7f;
			}
			if (!isGettingIn && MainAnimator.gameObject.activeSelf)
			{
				isGetinOutState = true;
				ExitAnimEndDelay(delay);
			}
		}

		public void StartInCar(VehicleType vehicleType, bool force, bool driverDead)
		{
			MainAnimator.SetInteger(VehicleTypeHash, (int)vehicleType);
			MainAnimator.SetBool(ForceGetVehicleHash, force);
			MainAnimator.SetBool(DeadInCarHash, driverDead);
			MainAnimator.SetTrigger(StartInCarHash);
			isGetinOutState = true;
			ExitAnimEndDelay(MainAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length * 0.7f);
		}

		public void ResetCollisionNormal()
		{
			collisionNormal = Vector3.zero;
			resetNearWalls = true;
		}

		private void ComponentInitialize()
		{
			rbody = GetComponent<Rigidbody>();
			defaultDrag = rbody.drag;
			coll = GetComponent<Collider>();
			weaponController = GetComponent<WeaponController>();
			player = GetComponentInParent<Player>();
			if (SurfaceSensor == null)
			{
				SurfaceSensor = GetComponentInChildren<SurfaceSensor>();
			}
			if ((bool)player)
			{
				isPlayer = true;
			}
			if (!MainAnimator)
			{
				MainAnimator = GetComponentInChildren<Animator>();
			}
			capsule = (coll as CapsuleCollider);
			if (capsule != null)
			{
				originalHeight = capsule.height;
				capsule.center = Vector3.up * originalHeight * 0.5f;
			}
			else
			{
				UnityEngine.Debug.LogError(" collider cannot be cast to CapsuleCollider");
			}
			rayHitComparer = new RayHitComparer();
			GenerateAnimatorHashes();
			climbingSensors = GetComponentInChildren<ClimbingSensors>();
		}

		private void GenerateAnimatorHashes()
		{
			ForwardHash = Animator.StringToHash("Forward");
			TurnHash = Animator.StringToHash("Turn");
			CrouchHash = Animator.StringToHash("Crouch");
			OnGroundHash = Animator.StringToHash("OnGround");
			JumpHash = Animator.StringToHash("Jump");
			JumpLegHash = Animator.StringToHash("JumpLeg");
			StrafeHash = Animator.StringToHash("Strafe");
			StrafeDirHash = Animator.StringToHash("StrafeDir");
			SprintHash = Animator.StringToHash("Sprint");
			ResetHash = Animator.StringToHash("Reset");
			DieHash = Animator.StringToHash("Die");
			ClimbLowHash = Animator.StringToHash("ClimbLow");
			ClimbMediumHash = Animator.StringToHash("ClimbMedium");
			ClimbHighHash = Animator.StringToHash("ClimbHigh");
			DoMeleeHash = Animator.StringToHash("DoMelee");
			MeleeHash = Animator.StringToHash("Melee");
			RangedWeaponTypeHash = Animator.StringToHash("RangedWeaponType");
			RangedWeaponShootHash = Animator.StringToHash("RangedWeaponShoot");
			RangedWeaponRechargeHash = Animator.StringToHash("RangedWeaponRecharge");
			GetInVehicleHash = Animator.StringToHash("GetIn");
			GetOutVehicleHash = Animator.StringToHash("GetOut");
			JumpOutVehicleHash = Animator.StringToHash("JumpOut");
			ForceGetVehicleHash = Animator.StringToHash("ForceGet");
			MeleeWeaponTypeHash = Animator.StringToHash("MeleeWeaponType");
			IsMovingHash = Animator.StringToHash("IsMoving");
			MeleeWeaponAttackSpeedHash = Animator.StringToHash("MeleeWeaponAttackSpeed");
			StartInCarHash = Animator.StringToHash("StartInCar");
			DeadInCarHash = Animator.StringToHash("DeadInCar");
			VehicleTypeHash = Animator.StringToHash("VehicleType");
			CharacterLeftFromVehicleHash = Animator.StringToHash("CharacterLeftFromVehicle");
			NearWallHash = Animator.StringToHash("NearWall");
			IsOnWaterHash = Animator.StringToHash("IsOnWater");
			ShootRopeFlyHash = Animator.StringToHash("ShootRopeFly");
			ShootRopeDragHash = Animator.StringToHash("ShootRopeDrag");
			UseSuperheroLandingsHash = Animator.StringToHash("UseSuperheroLandings");
			FlyingHash = Animator.StringToHash("Flying");
			RopeFailHash = Animator.StringToHash("RopeFail");
			GlidingHash = Animator.StringToHash("Gliding");
			IsFlyingHash = Animator.StringToHash("IsFlying");
			GroundedSpeedMultiplierHash = Animator.StringToHash("GroundedSpeedMultiplier");
			castingSkillHash =  Animator.StringToHash("CASTINGSKILL");
			idSkillHash =  Animator.StringToHash("ID_SKILL");
		}

		private void UpdateAnimState()
		{
			if (defAnimState == AnimState.Move && IsSuperFlying)
			{
				defAnimState = AnimState.Fly;
			}
			else if (defAnimState == AnimState.Fly && !IsSuperFlying)
			{
				defAnimState = AnimState.Move;
			}
			switch (animState)
			{
			case AnimState.Obstacle:
				break;
			case AnimState.WallClimb:
			case AnimState.GetInOutVehicle:
				break;
			case AnimState.Jump:
				if (IsSuperFlying)
				{
					animState = AnimState.Fly;
				}
				break;
			case AnimState.Move:
			case AnimState.MoveAim:
			case AnimState.Crouch:
			case AnimState.Fly:
			case AnimState.FlyAim:
				if (input.die)
				{
					animState = AnimState.Death;
				}
				else if (input.crouch && animState != AnimState.Fly && animState != AnimState.FlyAim)
				{
					animState = AnimState.Crouch;
				}
				else if (input.AttackState.Aim)
				{
					if (animState != AnimState.Fly && animState != AnimState.FlyAim)
					{
						animState = AnimState.MoveAim;
					}
					else
					{
						animState = AnimState.FlyAim;
					}
				}
				else
				{
					animState = defAnimState;
				}
				break;
			case AnimState.Death:
				if (input.reset)
				{
					animState = defAnimState;
				}
				break;
			}
		}

		private bool HasRobotInFront()
		{
			Ray ray = new Ray(base.transform.position + Vector3.up * player.NPCShootVectorOffset.magnitude, base.transform.forward);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 4f, TargetManager.Instance.ShootingLayerMask))
			{
				return hitInfo.collider.tag == "Player" || hitInfo.collider.tag == "Robot";
			}
			return false;
		}

		private void CheckForObstacles()
		{
			if (!CanClimb())
			{
				return;
			}
			obstacle = ObstacleHelper.FindObstacle(base.transform.position, base.transform.forward, 3f, 3f, ObstaclesLayerMask);
			if (obstacle.Type != 0 && obstacle.Type != ObstacleType.ObstacleHigh)
			{
				animState = AnimState.Obstacle;
				climbTrigger = true;
				startClimbEvent = false;
				switch (obstacle.Type)
				{
				case ObstacleType.ObstacleLow:
					climbVelocity = advancedSettings.climbVelocityLow;
					climbTriggerHash = ClimbLowHash;
					break;
				case ObstacleType.ObstacleMedium:
					climbVelocity = advancedSettings.climbVelocityMedium;
					climbTriggerHash = ClimbMediumHash;
					break;
				case ObstacleType.ObstacleHigh:
					climbVelocity = advancedSettings.climbVelocityHigh;
					climbTriggerHash = ClimbHighHash;
					break;
				}
				Quaternion quaternion = Quaternion.LookRotation(-obstacle.WallNormal);
				Tweener instance = Tweener.Instance;
				Transform transform = base.transform;
				Vector3 eulerAngles = quaternion.eulerAngles;
				instance.RotateTo(transform, Quaternion.Euler(0f, eulerAngles.y, 0f), 0.3f);
			}
		}

		private void ManageSuperFlying()
		{
			if (UseSuperFly)
			{
				if (player.IsFlying && IsGliding)
				{
					SetGliding(setGliding: false);
				}
				if (player.IsFlying && !IsSuperFlying)
				{
					StartSuperFlying();
				}
				if (!player.IsFlying && IsSuperFlying)
				{
					StopSuperFlying();
				}
			}
		}

		private void StartSuperFlying()
		{
			PlayerManager.Instance.Player.AbortMoveToCar();
			rbody.drag = 1f;
			if (RDCollInvulOnFly)
			{
				player.RDCollInvul = true;
			}
			if (RDExpInvulOnFly)
			{
				player.RDExpInvul = true;
			}
			FlyInputsCDstart = SwitchButtonsState(FlyInputs, FlyInputsCDstart, FlyInputsCD);
			IsSuperFlying = true;
			if (StartSuperFlyEvent != null)
			{
				StartSuperFlyEvent();
			}
		}

		private void StopSuperFlying()
		{
			ResetDrag();
			ResetCollisionNormal();
			if (RDCollInvulOnFly)
			{
				player.RDCollInvul = false;
			}
			if (RDExpInvulOnFly)
			{
				player.RDExpInvul = false;
			}
			FlyInputsCDstart = SwitchButtonsState(FlyInputs, FlyInputsCDstart, FlyInputsCD);
			IsSuperFlying = false;
			if (StopSuperFlyEvent != null)
			{
				StopSuperFlyEvent();
			}
			Transform transform = base.transform;
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			transform.rotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
			rbody.rotation = base.transform.rotation;
			SetCapsuleToVertical();
		}

		private float SwitchButtonsState(GameObject[] buttons, float CDstartTime, float CD = 1f)
		{
			if (Time.time >= CDstartTime + CD)
			{
				foreach (GameObject gameObject in buttons)
				{
					gameObject.SetActive(!gameObject.gameObject.activeSelf);
				}
				CDstartTime = Time.time;
			}
			return CDstartTime;
		}

		private void AutoResetButtonsState(GameObject[] buttons, float CDstartTime, float CD = 1f)
		{
			if (!buttons[0].gameObject.activeSelf && Time.time >= CDstartTime + CD)
			{
				foreach (GameObject gameObject in buttons)
				{
					gameObject.SetActive(value: true);
				}
			}
		}

		private void WallClimbCheck()
		{
			if (!AnimOnGround)
			{
				CheckClimbSensors();
			}
			else
			{
				HasWall = false;
			}
			if (animState == AnimState.WallClimb && input.jump && WallNormal != Vector3.zero)
			{
				JumpOffTheWall();
			}
			else if (animState == AnimState.WallClimb && (AnimOnGround || !HasWall))
			{
				animState = AnimState.Move;
			}
		}

		private void JumpOffTheWall()
		{
			animState = defAnimState;
			climbingSensors.DisableSensorsForJumpOffTheWall();
			rbody.AddForce(WallNormal * JumpOffTheWallForce, ForceMode.Impulse);
		}

		private void CheckClimbSensors()
		{
			bool shouldClimbToTop = false;
			if ((bool)climbingSensors)
			{
				bool hasWall = false;
				climbingSensors.CheckWall(out hasWall, out shouldClimbToTop);
				HasWall = hasWall;
			}
			if (input.jump && HasWall)
			{
				Vector3 vector = rbody.velocity;
				if (vector.y > 0f)
				{
					weaponController.ActivateFists();
					return;
				}
			}
			if (animState == AnimState.WallClimb && shouldClimbToTop && !tweening)
			{
				tweening = true;
				Vector3 position = climbingSensors.TopPoint.localPosition + Vector3.forward / 1.5f;
				Tweener.Instance.MoveTo(rbody.transform, rbody.transform.TransformPoint(position), 0.2f, delegate
				{
					tweening = false;
				});
			}
			else if (HasWall && !AnimOnGround && !tweening && !input.jump && ropeState != 0)
			{
				ropeState = RopeShootState.Default;
				animState = AnimState.WallClimb;
				rbody.useGravity = false;
				strafeAmount = input.inputMove.x;
				forwardAmount = input.inputMove.y;
				if (rope.RopeEnabled)
				{
					rope.Disable();
				}
			}
		}

		private void FaceWall(Collision collisionInfo)
		{
			if (animState != AnimState.WallClimb)
			{
				return;
			}
			WallNormal = Vector3.zero;
			reverseTurn = false;
			ContactPoint[] contacts = collisionInfo.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				reverseTurn = (reverseTurn || (WallNormal != Vector3.zero && (double)Vector3.Dot(WallNormal, contactPoint.normal) < 0.8));
				if (IsClimbAngle(contactPoint.normal))
				{
					WallNormal += contactPoint.normal;
				}
			}
			if (WallNormal != Vector3.zero && HasWall)
			{
				WallNormal.Normalize();
				rbody.transform.rotation = Quaternion.LookRotation(-WallNormal, Vector3.up);
			}
		}

		private bool IsClimbAngle(Vector3 normal)
		{
			return Mathf.Abs(Vector3.Dot(Vector3.up, normal)) < climbDotParameter;
		}

		private void ActivateObjectWithDelay(GameObject obj, float delay)
		{
			if (!obj.activeInHierarchy)
			{
				if (activateTimer < delay)
				{
					activateTimer += Time.fixedDeltaTime;
					return;
				}
				obj.SetActive(value: true);
				activateTimer = 0f;
			}
		}

		private void HideShowButtons()
		{
			if (ropeIsActive || animState == AnimState.WallClimb)
			{
				SprintButton.SetActive(value: false);
			}
			else
			{
				SprintButton.SetActive(value: true);
			}
			if ((player.stats.stamina.Current >= StaminaCost || (player.stats.stamina.Current < StaminaCost /*&& AdManager.Instance.IsRewardVideoLoaded()*/)) && animState != AnimState.MoveAim)
			{
				GameObject[] shootRopeButtons = ShootRopeButtons;
				foreach (GameObject obj in shootRopeButtons)
				{
					ActivateObjectWithDelay(obj, 0.5f);
				}
				return;
			}
			GameObject[] shootRopeButtons2 = ShootRopeButtons;
			foreach (GameObject gameObject in shootRopeButtons2)
			{
				gameObject.SetActive(value: false);
			}
			activateTimer = 0f;
		}

		private void HandleRopeStage()
		{
			if (!IsGliding)
			{
				CharacterModel.transform.localRotation = Quaternion.identity;
			}
			if (!ropeIsActive)
			{
				ropeState = RopeShootState.Default;
			}
			if (flyCameraEnabled && (AnimOnGround || animState == AnimState.WallClimb))
			{
				CameraManager.Instance.ResetCameraMode();
				if (HasWall)
				{
					CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("WallClimb");
				}
				flyCameraEnabled = false;
			}
			HideShowButtons();
			if (input.shootRope && Time.time - cooldownStartTime > cooldownTime && !ropeIsActive)
			{
				if (animState == AnimState.WallClimb)
				{
					JumpOffTheWall();
				}
				ropeIsActive = true;
			}
			if (!ropeIsActive)
			{
				return;
			}
			switch (ropeState)
			{
			case RopeShootState.Default:
				if (animState == AnimState.WallClimb)
				{
					RopeFlyCancel();
				}
				if (input.shootRope)
				{
					//if(player.stats.stamina.Current >= StaminaCost )
						ShootRope();
					//else
					//{
					//	AdManager.Instance.ShowRewardVideo((bool isDone) =>
					//	{
					//		if (isDone)
					//		{
					//			player.stats.stamina.Set(player.stats.stamina.Max);
					//			ShootRope();
					//		}
					//	});
					//}
				}
				break;
			case RopeShootState.DragStarting:
				Drag();
				break;
			case RopeShootState.FlyStarting:
				RopeFlyStart();
				break;
			case RopeShootState.Flying:
				RopeFlying();
				break;
			}
		}

		private bool RopeAnimationsFinished()
		{
			AnimatorStateInfo currentAnimatorStateInfo = MainAnimator.GetCurrentAnimatorStateInfo(0);
			return !currentAnimatorStateInfo.IsName("RopeDrag") && !currentAnimatorStateInfo.IsName("Fail");
		}

		private void ShootRope()
		{
			if (!RopeAnimationsFinished())
			{
				ropeState = RopeShootState.Default;
				ropeIsActive = false;
			}
			else
			{
				if (!(Time.time - cooldownStartTime > cooldownTime))
				{
					return;
				}
				cooldownStartTime = Time.time;
				Ray ray = CameraManager.Instance.UnityCamera.ScreenPointToRay(TargetManager.Instance.RopeAimPosition);
				float num = Vector3.Dot(ray.direction, Vector3.up);
				float maxDistance = maxRopeXZDistance / (float)Math.Cos(Math.Asin(num));
				RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, (int)WallsLayerMask | (int)DragLayerMask | (int)DoNotShootThroughThisLayerMask);
				Array.Sort(array, rayHitComparer);
				Transform transform = rbody.transform;
				Vector3 direction = ray.direction;
				float x = direction.x;
				Vector3 direction2 = ray.direction;
				transform.rotation = Quaternion.LookRotation(new Vector3(x, 0f, direction2.z), Vector3.up);
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					if (LayerInLayerMask(raycastHit.collider.transform.gameObject.layer, DoNotShootThroughThisLayerMask))
					{
						break;
					}
					if (!raycastHit.collider.gameObject.CompareTag("Player") && raycastHit.distance > 1f)
					{
						target = raycastHit;
						if (!IsSuperFlying && LayerInLayerMask(target.collider.transform.gameObject.layer, WallsLayerMask) && IsClimbAngle(raycastHit.normal) && EnableClimbWalls)
						{
							target.point -= targetOffset;
							rope.ShootTarget(target.point + targetOffset, ropeExpandTime, ropeStraighteningTime);
							BeginRopeFlyStage();
							PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.RopeShoot);
							return;
						}
						if (LayerInLayerMask(target.collider.transform.gameObject.layer, DragLayerMask) && !IsWallClimbing && !IsGliding)
						{
							PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.RopeShoot);
							movingTargetOffset = target.point - target.collider.transform.position;
							rope.ShootMovingTarget(target.collider.transform, movingTargetOffset, ropeExpandTime, ropeStraighteningTime);
							BeginDragStage();
							return;
						}
					}
				}
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.RopeShoot);
				rope.ShootFail(ray.direction, maxDistance, ropeExpandTime, ropeStraighteningTime);
				if (AnimOnGround || IsSuperFlying)
				{
					MainAnimator.SetTrigger(RopeFailHash);
				}
				ropeState = RopeShootState.Default;
				ropeIsActive = false;
			}
		}

		private void BeginRopeFlyStage()
		{
			if (ropeState != RopeShootState.Default)
			{
				return;
			}
			rbody.useGravity = false;
			player.stats.stamina.SetAmount(0f - StaminaCost);
			startTime = Time.time;
			ropeState = RopeShootState.FlyStarting;
			MainAnimator.SetTrigger(ShootRopeFlyHash);
			if (AnimOnGround)
			{
				velocity.y = 10f;
				AnimOnGround = false;
				if (this.JumpEvent != null)
				{
					this.JumpEvent();
				}
			}
		}

		private void BeginDragStage()
		{
			if (ropeState == RopeShootState.Default)
			{
				player.stats.stamina.SetAmount(0f - StaminaCost);
				startTime = Time.time;
				ropeState = RopeShootState.DragStarting;
				MainAnimator.SetTrigger(ShootRopeDragHash);
			}
		}

		private void RopeFlyStart()
		{
			if (Time.time - startTime > flyStartDelay)
			{
				if (!HasWall)
				{
					rbody.velocity = Vector3.zero;
					velocity = Vector3.zero;
					CameraManager.Instance.SetMode(Game.Character.Modes.Type.Fly, fastTranslation: true);
					flyCameraEnabled = true;
					rbody.AddForce(TargetDirection * ropeStartForce, ForceMode.Impulse);
					ropeState = RopeShootState.Flying;
				}
				else
				{
					MainAnimator.ResetTrigger(ShootRopeFlyHash);
					animState = AnimState.WallClimb;
					rope.Disable();
					ropeIsActive = false;
				}
			}
		}

		private void RopeFlying()
		{
			rbody.transform.LookAt(target.point);
			rbody.AddForce(TargetDirection * ropeForce, ForceMode.Force);
			weaponController.ActivateFists();
			Vector3 vector = rbody.velocity;
			float x = vector.x;
			Vector3 vector2 = rbody.velocity;
			float y = vector2.y;
			Vector3 vector3 = rbody.velocity;
			velocity = new Vector3(x, y, vector3.z);
			if (AnimOnGround || HasWall || input.jump)
			{
				RopeFlyCancel();
			}
			else if (TargetDistance < rbody.velocity.magnitude * Time.deltaTime * 10f)
			{
				RopeFlyFinish();
			}
			else if (input.shootRope && Time.time - cooldownStartTime > cooldownTime)
			{
				RopeFlyReshoot();
			}
		}

		private void RopeFlyFinish()
		{
			velocity = Vector3.zero;
			rbody.velocity = Vector3.zero;
			ropeState = RopeShootState.Default;
			animState = AnimState.WallClimb;
			ropeIsActive = false;
			rbody.velocity = Vector3.zero;
			base.transform.position = target.point + target.normal * 0.5f;
			base.transform.rotation = Quaternion.LookRotation(-target.normal, Vector3.up);
			rope.Disable();
		}

		private void RopeFlyCancel()
		{
			Vector3 forward = rbody.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			if (forward != Vector3.zero)
			{
				base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
			}
			ropeState = RopeShootState.Default;
			ropeIsActive = false;
			rope.Disable();
		}

		private void RopeFlyReshoot()
		{
			ropeState = RopeShootState.Default;
			ShootRope();
		}

		private void Drag()
		{
			if (!(Time.time - startTime > dragDelay))
			{
				return;
			}
			rope.Decrease();
			ropeState = RopeShootState.Default;
			ropeIsActive = false;
			GameObject gameObject = target.transform.gameObject;
			PseudoDynamicObject component = gameObject.GetComponent<PseudoDynamicObject>();
			Human component2 = gameObject.GetComponent<Human>();
			HumanoidStatusNPC component3 = gameObject.GetComponent<HumanoidStatusNPC>();
			Rigidbody componentInParent = gameObject.GetComponentInParent<Rigidbody>();
			Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
			if ((bool)rigidbody)
			{
				rigidbody = ((rigidbody.gameObject.layer != SmallDynamicLH) ? null : rigidbody);
			}
			if (component != null)
			{
				component.ReplaceOnDynamic();
				component.GetComponent<Rigidbody>().AddForceAtPosition(-TargetDirection * ForceScale(TargetDistance) + Vector3.up * dragVerticalSpeedComponent, target.point, ForceMode.VelocityChange);
			}
			else if ((bool)component2)
			{
				GameObject initRagdoll;
				component2.ReplaceOnRagdoll(true, out initRagdoll);
				Rigidbody componentInChildren = initRagdoll.GetComponentInChildren<Rigidbody>();
				Push(componentInChildren, 6f, 6f, Vector3.zero);
			}
			else if ((bool)component3)
			{
				if (component3.Ragdollable)
				{
					BaseControllerNPC controller;
					component3.BaseNPC.ChangeController(BaseNPC.NPCControllerType.Smart, out controller);
					SmartHumanoidController smartHumanoidController = controller as SmartHumanoidController;
					if (smartHumanoidController != null)
					{
						smartHumanoidController.AddTarget(player);
						smartHumanoidController.InitBackToDummyLogic();
					}
					GameObject _;
					component3.ReplaceOnRagdoll(true, out  _);
					Rigidbody component4 = component3.GetRagdollHips().GetComponent<Rigidbody>();
					Push(component4, 6f, 6f, Vector3.zero);
				}
			}
			else if ((bool)rigidbody)
			{
				Transform transform = rigidbody.transform;
				int num = 20;
				while (!transform.gameObject.name.Equals("hips") && num > 0)
				{
					transform = transform.parent;
					num--;
				}
				Vector3 offset = transform.position - rigidbody.transform.position;
				Rigidbody component5 = transform.GetComponent<Rigidbody>();
				if ((bool)component5)
				{
					rigidbody = component5;
					RagdollWakeUper componentInChildren2 = component5.GetComponentInChildren<RagdollWakeUper>();
					if ((bool)componentInChildren2 && componentInChildren2.CurrentState != RagdollState.Ragdolled)
					{
						componentInChildren2.SetRagdollWakeUpStatus(wakeUp: false);
					}
				}
				Push(rigidbody, 6f, 6f, offset);
			}
			else
			{
				if (!componentInParent)
				{
					return;
				}
				DrivableVehicle component6 = componentInParent.transform.GetComponent<DrivableVehicle>();
				if ((bool)component6)
				{
					DrivableCar drivableCar = component6 as DrivableCar;
					if ((bool)drivableCar)
					{
						WheelCollider[] wheels = drivableCar.wheels;
						foreach (WheelCollider wheelCollider in wheels)
						{
							wheelCollider.brakeTorque = 0f;
						}
					}
				}
				DrivableMotorcycle component7 = componentInParent.GetComponent<DrivableMotorcycle>();
				if ((bool)component7 && (bool)component7.DummyDriver)
				{
					component7.MainRigidbody.constraints = RigidbodyConstraints.None;
					component7.DummyDriver.DropRagdoll(player, component7.transform.up, canWakeUp: false, isOnWater: false, waterHight: 0f);
				}
				Push(componentInParent, 1.5f, 1f, movingTargetOffset);
			}
		}

		private float ForceScale(float distance)
		{
			return -0.01f * distance * distance + distance * 1f - 1f;
		}

		private void Push(Rigidbody r, float directionMultiplier, float upMultiplier, Vector3 offset)
		{
			r.velocity = Vector3.zero;
			Vector3 vector = target.collider.transform.position + (target.point - target.collider.transform.position);
			r.AddForceAtPosition(-TargetDirection * ForceScale(TargetDistance) * directionMultiplier + Vector3.up * dragVerticalSpeedComponent * upMultiplier, r.transform.position + offset, ForceMode.VelocityChange);
		}

		private bool LayerInLayerMask(int layer, LayerMask mask)
		{
			return (mask.value & (1 << layer)) == 1 << layer;
		}

		private void OnCollisionEnter(Collision collisionInfo)
		{
			if (ropeState == RopeShootState.Flying && LayerInLayerMask(coll.gameObject.layer, WallsLayerMask))
			{
				target.point = collisionInfo.contacts[0].point;
				target.normal = collisionInfo.contacts[0].normal;
				if (IsClimbAngle(target.normal))
				{
					RopeFlyFinish();
				}
				else
				{
					RopeFlyCancel();
				}
			}
			int layer = collisionInfo.collider.gameObject.layer;
			if (IsSuperFlying && (layer == ComplexObjectsLH || layer == SimpleObjectsLH))
			{
				collisionNormal = collisionInfo.impulse.normalized;
			}
		}

		private void OnCollisionStay(Collision collisionInfo)
		{
			FaceWall(collisionInfo);
		}

		private void OnCollisionExit(Collision collisionInfo)
		{
			int layer = collisionInfo.collider.gameObject.layer;
			if (IsSuperFlying && (layer == ComplexObjectsLH || layer == SimpleObjectsLH))
			{
				collisionNormal = Vector3.zero;
			}
		}

		private bool EnoughHeightForGlide()
		{
			if (IsGliding)
			{
				return true;
			}
			Ray ray = new Ray(base.transform.position + Vector3.up * 0.1f, -Vector3.up);
			RaycastHit[] array = Physics.RaycastAll(ray, MinHeightForGlide, advancedSettings.GroundLayerMask);
			Array.Sort(array, rayHitComparer);
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (!raycastHit.collider.isTrigger)
				{
					return false;
				}
			}
			return true;
		}

		private void ConvertMoveInput()
		{
			if (!input.sprint)
			{
				input.camMove *= moveToSprintMultiplier;
				input.inputMove *= moveToSprintMultiplier;
			}
			Vector3 vector = base.transform.InverseTransformDirection(input.camMove);
			if (input.AttackState.Aim || (IsSuperFlying && !input.sprint))
			{
				forwardAmount = input.inputMove.y;
				turnAmount = 0f;
				strafeAmount = input.inputMove.x;
			}
			else
			{
				forwardAmount = vector.z;
				turnAmount = Mathf.Atan2(vector.x, vector.z);
				strafeAmount = vector.x;
			}
		}

		private void TurnTowardsCameraForward()
		{
			if (Mathf.Abs(forwardAmount) < 0.01f)
			{
				Vector3 vector = base.transform.InverseTransformDirection(input.lookPos - base.transform.position);
				float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				if (Mathf.Abs(num) > advancedSettings.autoTurnThresholdAngle)
				{
					turnAmount += num * advancedSettings.autoTurnSpeed * 0.001f;
				}
			}
		}

		private void PreventStandingInLowHeadroom()
		{
			if (!input.crouch)
			{
				Ray ray = new Ray(rbody.position + Vector3.up * capsule.radius * 0.5f, Vector3.up);
				float maxDistance = originalHeight - capsule.radius * 0.5f;
				if (Physics.SphereCast(ray, capsule.radius * 0.5f, maxDistance))
				{
					animState = AnimState.Crouch;
				}
			}
		}

		private void ScaleCapsule()
		{
			float num = 1f;
			switch (animState)
			{
			case AnimState.Crouch:
				if (AnimOnGround)
				{
					num = advancedSettings.crouchHeightFactor;
				}
				break;
			case AnimState.Fly:
				num = advancedSettings.FlyHeightFactor;
				break;
			case AnimState.FlyAim:
				num = advancedSettings.FlyHeightFactor;
				break;
			}
			float num2 = originalHeight * num;
			Vector3 rhs = Vector3.up * originalHeight * num * 0.5f;
			if (Math.Abs(capsule.height - num2) > 0.01f)
			{
				capsule.height = Mathf.MoveTowards(capsule.height, num2, Time.deltaTime * 4f);
			}
			if (capsule.center != rhs)
			{
				capsule.center = Vector3.MoveTowards(capsule.center, rhs, Time.deltaTime * 2f);
			}
		}

		private void ApplyExtraTurnRotation()
		{
			if (CanRotate())
			{
				float num = Mathf.Lerp(advancedSettings.stationaryTurnSpeed, advancedSettings.movingTurnSpeed, forwardAmount);
				float yAngle = turnAmount * num * Time.deltaTime;
				base.transform.Rotate(0f, yAngle, 0f);
			}
		}

		private void AnimOnGroundCheck()
		{
			if (animState == AnimState.Obstacle)
			{
				return;
			}
			if (player.IsSwiming && !ropeIsActive)
			{
				rbody.useGravity = false;
				animState = AnimState.Move;
			}
			bool animOnGround = AnimOnGround;
			if (velocity.y < jumpPower * 0.5f || IsSuperFlying)
			{
				AnimOnGround = false;
				if (!IsSuperFlying && !player.IsSwiming)
				{
					rbody.useGravity = true;
				}
				AnimOnGround = SurfaceSensor.AboveGround;
				if (AnimOnGround)
				{
					if (ShouldStickToTheGround() && !IsSuperFlying)
					{
						Vector3 position = base.transform.position;
						float x = position.x;
						float currGroundSurfaceHeight = SurfaceSensor.CurrGroundSurfaceHeight;
						Vector3 position2 = base.transform.position;
						Vector3 end = new Vector3(x, currGroundSurfaceHeight, position2.z);
						UnityEngine.Debug.DrawLine(base.transform.position, end, Color.red);
						if (velocity.y <= 0f && !Physics.Raycast(base.transform.position, base.transform.forward, 0.7f, advancedSettings.GroundLayerMask))
						{
							float num = Mathf.Clamp(Vector3.Dot(velocity, rbody.transform.forward), 0.1f, float.PositiveInfinity);
							float currGroundSurfaceHeight2 = SurfaceSensor.CurrGroundSurfaceHeight;
							Vector3 position3 = rbody.transform.position;
							if (Mathf.Abs(currGroundSurfaceHeight2 - position3.y) > 0.2f)
							{
								num = Mathf.Clamp(num, advancedSettings.groundStickyEffect, float.PositiveInfinity);
							}
							rbody.position = Vector3.MoveTowards(GetComponent<Rigidbody>().position, end, Time.deltaTime * advancedSettings.groundStickyEffect * num);
							Rigidbody rigidbody = rbody;
							Vector3 eulerAngles = base.transform.eulerAngles;
							rigidbody.rotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
							if (isPlayer)
							{
								Transform transform = base.transform;
								Vector3 eulerAngles2 = base.transform.eulerAngles;
								transform.eulerAngles = new Vector3(0f, eulerAngles2.y, 0f);
							}
						}
						SetGliding(setGliding: false);
						rbody.useGravity = false;
					}
					if (animState == AnimState.Jump)
					{
						animState = defAnimState;
					}
				}
			}
			if (!AnimOnGround)
			{
				lastAirTime = Time.time;
			}
			if (!animOnGround && AnimOnGround && !IsSuperFlying && OnFallImpactCallback != null)
			{
				OnFallImpactCallback(prevFallVelocity);
				velocity.x = (velocity.z = 0f);
			}
			if (FUCounts >= 5f)
			{
				prevFallVelocity = base.transform.InverseTransformVector(velocity);
				FUCounts = 0f;
			}
		}

		private void SetFriction()
		{
			if (AnimOnGround && !IsSuperFlying)
			{
				if (input.camMove.magnitude < Mathf.Epsilon)
				{
					coll.material = advancedSettings.highFrictionMaterial;
					return;
				}
				coll.material = advancedSettings.zeroFrictionMaterial;
				rbody.constraints = (RigidbodyConstraints)80;
			}
			else
			{
				coll.material = advancedSettings.zeroFrictionMaterial;
				rbody.constraints = (RigidbodyConstraints)80;
			}
		}

		private void HandleGroundedVelocities()
		{
			velocity.y = 0f;
			if (input.camMove.magnitude < Mathf.Epsilon)
			{
				velocity.x = 0f;
				velocity.z = 0f;
			}
			if (input.jump && CanJump())
			{
				if (this.JumpEvent != null)
				{
					this.JumpEvent();
				}
				animState = AnimState.Jump;
				AnimOnGround = false;
				velocity = input.camMove * airSpeed;
				velocity.y = jumpPower;
			}
			CharacterModel.transform.localEulerAngles = Vector3.zero;
		}

		private bool CanJump()
		{
			bool flag = MainAnimator.GetCurrentAnimatorStateInfo(0).IsName("Grounded");
			bool flag2 = Time.time > lastAirTime + advancedSettings.jumpRepeatDelayTime;
			bool flag3 = !Physics.Raycast(capsule.bounds.max - base.transform.up * 0.2f, base.transform.up, 0.7f, advancedSettings.GroundLayerMask);
			if (DebugLog)
			{
				UnityEngine.Debug.DrawRay(capsule.bounds.max - base.transform.up * 0.2f, base.transform.up * 0.7f, Color.yellow, 10f);
			}
			return flag2 && flag && flag3 && animState != AnimState.Crouch && animState != AnimState.Death && animState != AnimState.MoveAim && animState != AnimState.Fly && animState != AnimState.FlyAim && animState != AnimState.Obstacle;
		}

		private bool CanClimb()
		{
			return input.jump && animState != AnimState.Crouch && animState != AnimState.Death && animState != AnimState.Obstacle && animState != AnimState.Jump && animState != AnimState.Fly && animState != AnimState.FlyAim && animState != AnimState.WallClimb && !ropeIsActive && AnimOnGround;
		}

		private bool ShouldStickToTheGround()
		{
			return animState != AnimState.WallClimb && animState != AnimState.Jump && !ropeIsActive && animState != AnimState.Fly && animState != AnimState.FlyAim && !player.IsSwiming;
		}

		private bool CanRotate()
		{
			return animState == AnimState.Move || animState == AnimState.Crouch || animState == AnimState.Fly;
		}

		private void FixedUpdate()
		{
			FUCounts += 1f;
			if (UseSuperFly)
			{
				AutoResetButtonsState(FlyInputs, FlyInputsCDstart, FlyInputsCD);
			}
			if (!previousIsAnimOnGround && AnimOnGround)
			{
				player.ResetRotation();
				previousIsAnimOnGround = true;
			}
			else if (previousIsAnimOnGround && !AnimOnGround)
			{
				previousIsAnimOnGround = false;
			}
		}

		private void HandleAirborneVelocities()
		{
			rbody.useGravity = (!ropeIsActive && animState != AnimState.WallClimb);
			if (!useGravityMults)
			{
				velocity = Vector3.Lerp(b: new Vector3(input.camMove.x * airSpeed, velocity.y, input.camMove.z * airSpeed), a: velocity, t: Time.deltaTime * airControl);
			}
			else if (animState == AnimState.Move)
			{
				gravityMultiplier = fallGravityMultiplier;
			}
			else
			{
				gravityMultiplier = jumpGravityMultiplier;
			}
			if (rbody.useGravity)
			{
				Vector3 a = Physics.gravity * gravityMultiplier - Physics.gravity;
				rbody.AddForce(a * player.MainRigidbody().mass);
			}
			if (isPlayer)
			{
				velocity += rbody.transform.forward * 0.1f;
				Vector3 vector = rbody.velocity;
				if (vector.y < 0f - jumpToFallSpeed && animState == AnimState.Jump)
				{
					animState = AnimState.Move;
				}
				Vector3 vector2 = rbody.velocity;
				if (vector2.y < 0f - fallToGlideSpeed && EnoughHeightForGlide())
				{
					SetGliding();
				}
				else
				{
					SetGliding(setGliding: false);
				}
				if (velocity.y < 0f - MaxFallSpeed)
				{
					velocity.y = 0f - MaxFallSpeed;
				}
			}
		}

		private void SetGliding(bool setGliding = true)
		{
			if (setGliding)
			{
				if (!IsGliding)
				{
					CameraManager.Instance.SetMode(Game.Character.Modes.Type.Fly);
					flyCameraEnabled = true;
					IsGliding = true;
				}
				velocity.x = 0f;
				velocity.z = 0f;
				Vector3 forward = rbody.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				Vector3 vector = new Vector3(input.camMove.x * superheroFastAirSpeed, 0f, input.camMove.z * superheroFastAirSpeed);
				velocity += forward * superheroAirSpeed;
				velocity += vector;
			}
			else if (IsGliding)
			{
				CameraManager.Instance.ResetCameraMode();
				flyCameraEnabled = false;
				IsGliding = false;
			}
		}

		private void UpdateAnimator()
		{
			if (!MainAnimator.isInitialized)
			{
				return;
			}
			MainAnimator.SetBool(FlyingHash, ropeState == RopeShootState.Flying || ropeState == RopeShootState.FlyStarting);
			MainAnimator.applyRootMotion = (AnimOnGround || IsSuperFlying || player.IsSwiming);
			MainAnimator.SetBool(NearWallHash, HasWall);
			MainAnimator.SetBool(GlidingHash, IsGliding);
			MainAnimator.SetBool(UseSuperheroLandingsHash, useSuperheroLandings);
			MainAnimator.SetBool(StrafeHash, input.AttackState.Aim);
			MainAnimator.SetFloat(StrafeDirHash, strafeAmount);
			MainAnimator.SetFloat(ForwardHash, forwardAmount, 0.1f, Time.deltaTime);
			MainAnimator.SetBool(IsOnWaterHash, player.IsSwiming);
			MainAnimator.SetBool(SprintHash, input.sprint);
			MainAnimator.SetBool(IsFlyingHash, IsSuperFlying);
			if (player.IsTransformer)
			{
				if (player.IsDrowning)
				{
					MainAnimator.SetBool(IsOnWaterHash, value: false);
					MainAnimator.SetFloat(GroundedSpeedMultiplierHash, advancedSettings.UnderWaterSpeedMult);
				}
				else
				{
					MainAnimator.SetFloat(GroundedSpeedMultiplierHash, advancedSettings.DefaultSpeedMult);
				}
			}
			if (strafeAmount != 0f || (double)forwardAmount > 0.1 || forwardAmount < -0.1f)
			{
				MainAnimator.SetBool(IsMovingHash, value: true);
			}
			else
			{
				MainAnimator.SetBool(IsMovingHash, value: false);
			}
			if (!input.AttackState.Aim)
			{
				MainAnimator.SetFloat(TurnHash, (float)turnAnimMult * turnAmount, 0.1f, Time.deltaTime);
				MainAnimator.SetBool(CrouchHash, animState == AnimState.Crouch);
			}
			if (input.reset)
			{
				MainAnimator.SetTrigger(ResetHash);
				MainAnimator.ResetTrigger(DieHash);
			}
			if (input.die)
			{
				MainAnimator.SetTrigger(DieHash);
				MainAnimator.ResetTrigger(ResetHash);
			}
			MainAnimator.SetBool(OnGroundHash, AnimOnGround);
			if (!AnimOnGround && !player.IsSwiming)
			{
				MainAnimator.SetFloat(JumpHash, velocity.y);
			}
			if (climbTrigger)
			{
				MainAnimator.SetTrigger(climbTriggerHash);
				climbTrigger = false;
			}
			float num = Mathf.Repeat(MainAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + advancedSettings.runCycleLegOffset, 1f);
			float value = (float)((num < 0.5f) ? 1 : (-1)) * forwardAmount;
			if (AnimOnGround)
			{
				MainAnimator.SetFloat(JumpLegHash, value);
			}
			if (animSpeedMultiplier >= 1f && input.camMove.magnitude > 0f)
			{
				if (AnimOnGround && !IsSuperFlying)
				{
					MainAnimator.speed = animSpeedMultiplier;
				}
				else
				{
					MainAnimator.speed = 1f;
				}
			}
			useIKLook = true;
			MainAnimator.SetBool("DoSmash", value: false);
			if (input.AttackState.MeleeAttackState != MeleeAttackState.None)
			{
				if ((bool)SmashPrefab && !HasRobotInFront())
				{
					useIKLook = false;
					MainAnimator.SetBool(DoMeleeHash, value: false);
					MainAnimator.SetBool("DoSmash", value: true);
				}
				else
				{
					meleePerformed = true;
					MainAnimator.SetBool(DoMeleeHash, value: true);
					MainAnimator.SetFloat(MeleeWeaponAttackSpeedHash, meleeWeaponAttackSpeedMultipler);
					MainAnimator.SetInteger(MeleeHash, (int)input.AttackState.MeleeAttackState);
					MainAnimator.SetInteger(MeleeWeaponTypeHash, (int)input.AttackState.MeleeWeaponType);
				}
				TargetManager.Instance.HideCrosshair = true;
			}
			else if (meleePerformed)
			{
				MainAnimator.SetBool(DoMeleeHash, value: false);
				meleePerformed = false;
			}
			else
			{
				TargetManager.Instance.HideCrosshair = false;
			}
			switch (input.AttackState.RangedAttackState)
			{
			case RangedAttackState.Shoot:
				MainAnimator.SetBool(RangedWeaponShootHash, value: true);
				MainAnimator.SetInteger(RangedWeaponTypeHash, (int)input.AttackState.RangedWeaponType);
				MainAnimator.SetBool(RangedWeaponRechargeHash, value: false);
				break;
			case RangedAttackState.Recharge:
				MainAnimator.SetBool(RangedWeaponShootHash, value: false);
				MainAnimator.SetInteger(RangedWeaponTypeHash, (int)input.AttackState.RangedWeaponType);
				MainAnimator.SetBool(RangedWeaponRechargeHash, value: true);
				break;
			default:
				MainAnimator.SetBool(RangedWeaponShootHash, value: false);
				MainAnimator.SetInteger(RangedWeaponTypeHash, (int)input.AttackState.RangedWeaponType);
				MainAnimator.SetBool(RangedWeaponRechargeHash, value: false);
				break;
			}
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if ((bool)MainAnimator && useIKLook && input.AttackState != null)
			{
				LookAtWeights lookAtWeights;
				Vector2 vector;
				if (input.AttackState.Aim || input.AttackState.CanAttack)
				{
					lookAtWeights = ((!IsSuperFlying) ? advancedSettings.AimlookAtWeights : advancedSettings.FlyAimlookAtWeights);
					vector = PlayerManager.Instance.WeaponController.CurrentWeapon.AimOffset;
				}
				else
				{
					lookAtWeights = advancedSettings.IdlelookAtWeights;
					vector = Vector2.zero;
				}
				MainAnimator.SetLookAtWeight(lookAtWeights.weight, lookAtWeights.bodyWeight, lookAtWeights.headWeight, lookAtWeights.eyesWeight, lookAtWeights.clampWeight);
				if (lookTarget != null)
				{
					lookPos = lookTarget.position;
				}
				MainAnimator.SetLookAtPosition(lookPos + base.transform.right * vector.x * Vector3.Distance(lookPos, base.transform.position) + Vector3.up * vector.y * Vector3.Distance(lookPos, base.transform.position));
				UnityEngine.Debug.DrawLine(base.transform.position, lookPos, Color.blue);
			}
		}

		private void SetUpAnimator()
		{
			MainAnimator = GetComponent<Animator>();
			Animator[] componentsInChildren = GetComponentsInChildren<Animator>();
			foreach (Animator animator in componentsInChildren)
			{
				if (animator != MainAnimator && animator.avatar != null)
				{
					MainAnimator.avatar = animator.avatar;
					UnityEngine.Object.Destroy(animator);
					break;
				}
			}
			UpdatePlayerStats();
		}

		private void SuperFlyMovement(Vector3 lookDelta)
		{
			Quaternion rhs = Quaternion.Euler(0f, 0f, 0f);
			Quaternion rhs2 = Quaternion.Euler(0f, 0f, 0f);
			Quaternion rotation = base.transform.rotation;
			Vector3 vector = cameraTransform.forward;
			rbody.useGravity = false;
			if (collisionNormal != Vector3.zero)
			{
				flyNearWalls = true;
			}
			else
			{
				flyNearWalls = false;
			}
			if (resetNearWalls)
			{
				flyNearWalls = true;
				resetNearWalls = false;
			}
			if (!input.sprint)
			{
				if (Mathf.Abs(input.inputMove.y) >= 0.1f || Mathf.Abs(input.inputMove.x) >= 0.1f || input.AttackState.Aim)
				{
					float num = Mathf.Atan2(lookDelta.x, lookDelta.z) * 57.29578f;
					if (input.smoothAimRotation)
					{
						num *= Time.deltaTime * 10f;
					}
					rhs = Quaternion.Euler(0f, num, 0f);
				}
			}
			else
			{
				float num2 = Mathf.Atan2(lookDelta.y, lookDelta.z) * 57.29578f;
				if (input.smoothAimRotation)
				{
					num2 *= Time.deltaTime * 10f;
				}
				rhs2 = Quaternion.Euler(0f - num2, 0f, 0f);
			}
			Vector3 eulerAngles = (base.transform.rotation * rhs * rhs2).eulerAngles;
			if (AnimOnGround || SurfaceSensor.AboveWater || SurfaceSensor.InWater)
			{
				if (eulerAngles.x > MaxAngleNearTheGround)
				{
					eulerAngles.x = MaxAngleNearTheGround;
				}
				if (vector.y < 0f && input.inputMove.y > 0f)
				{
					vector.y = 0f;
				}
			}
			if (SurfaceSensor.InWater)
			{
				Transform transform = base.transform;
				Vector3 position = base.transform.position;
				Vector3 position2 = base.transform.position;
				float x = position2.x;
				float currWaterSurfaceHeight = SurfaceSensor.CurrWaterSurfaceHeight;
				Vector3 position3 = base.transform.position;
				transform.position = Vector3.Lerp(position, new Vector3(x, currWaterSurfaceHeight, position3.z), Time.deltaTime * 2f);
			}
			if (flyNearWalls)
			{
				turnAnimMult = -1;
				float num3 = Vector3.Dot(vector, collisionNormal);
				if (num3 < 0f && input.inputMove.y > 0f)
				{
					vector = Vector3.ProjectOnPlane(vector, collisionNormal);
					vector = vector.normalized;
				}
				UnityEngine.Debug.DrawRay(base.transform.position, vector * 2f, Color.yellow);
				UnityEngine.Debug.DrawRay(base.transform.position, collisionNormal, Color.yellow);
			}
			else
			{
				turnAnimMult = 1;
				rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
			}
			float d;
			float d2;
			if (!input.sprint)
			{
				d = ((!(input.inputMove.y >= 0f)) ? SuperFlyBackwardsSpeed : SuperFlySpeed);
				d2 = SuperFlyStrafeSpeed;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(0f, eulerAngles.y, 0f), Time.deltaTime * FlyRotationLerpMult);
				SetCapsuleToVertical();
			}
			else
			{
				d = SuperFlySprintSpeed;
				d2 = 0f;
				base.transform.rotation = rotation;
				SetCapsuleToHorizontal();
			}
			rbody.rotation = base.transform.rotation;
			if (Mathf.Abs(input.inputMove.y) >= 0.1f || Mathf.Abs(input.inputMove.x) >= 0.1f)
			{
				if (!input.sprint)
				{
					rbody.velocity = vector * forwardAmount * d + cameraTransform.right * strafeAmount * d2;
				}
				else if (!AnimOnGround && !SurfaceSensor.AboveWater && !SurfaceSensor.InWater)
				{
					rbody.velocity = base.transform.forward * forwardAmount * d;
				}
				else
				{
					rbody.velocity = vector * forwardAmount * d;
				}
			}
		}

		private void ClimbStart()
		{
			startClimbEvent = true;
			rbody.useGravity = false;
			capsule.enabled = false;
		}

		private void ExitAnimEndDelay(float delay)
		{
			Invoke("ExitAnimEnd", delay);
		}
	}
}
