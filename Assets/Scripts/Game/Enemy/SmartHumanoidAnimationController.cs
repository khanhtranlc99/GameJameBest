using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.Effects;
using Game.GlobalComponent;
using Game.Vehicle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy
{
	public class SmartHumanoidAnimationController : MonoBehaviour
	{
		[Serializable]
		public struct Input
		{
			public Vector3 CamMove;

			public Vector3 InputMove;

			public Vector3 LookPos;

			public bool SmoothAimRotation;

			public bool AimTurn;

			public AttackState AttackState;
		}

		public delegate void OnFallImpact(float velocity);

		private const float MaxCheckGroundVerticalVelocity = 5f;

		private const float AirSpeed = 6f;

		private const float AirControl = 2f;

		private const float AnimSpeedMultiplier = 1f;

		public bool DebugLog;

		public float AutoTurnThresholdAngle = 100f;

		public float AutoTurnSpeed = 2f;

		public float StationaryTurnSpeed = 180f;

		public float MovingTurnSpeed = 360f;

		public float MoveSpeedMultiplier = 1f;

		public LayerMask GroundLayerMask;

		public float GroundStickyEffect = 1f;

		public PhysicMaterial ZeroFrictionMaterial;

		public PhysicMaterial HighFrictionMaterial;

		public float GravityMultiplier = 2f;

		public LookAtWeights LookAtWeights;

		public OnFallImpact OnFallImpactCallback;

		private Animator controlledAnimator;

		private Rigidbody rbody;

		private Collider mainCollider;

		private Transform rootTransform;

		private Input input;

		private RayHitComparer rayHitComparer;

		private Vector3 velocity;

		private Vector3 lookPos;

		private float turnAmount;

		private float forwardAmount;

		private float strafeAmount;

		private bool onGround;

		private readonly List<RaycastHit> groundHits = new List<RaycastHit>();

		private RaycastHit[] currentHits = new RaycastHit[5];

		private int forwardHash;

		private int turnHash;

		private int onGroundHash;

		private int strafeHash;

		private int strafeDirHash;

		private int doMeleeHash;

		private int meleeHash;

		private int doSmashHash;

		private int rangedWeaponTypeHash;

		private int rangedWeaponShootHash;

		private int rangedWeaponRechargeHash;

		private int forceGetVehicleHash;

		private int meleeWeaponTypeHash;

		private int isMovingHash;

		private int startInCarHash;

		private int deadInCarHash;

		private int vehicleTypeHash;

		public GameObject SmashPrefab;

		public Transform LookTarget
		{
			get;
			set;
		}

		public bool CantMove => controlledAnimator.GetBool(forceGetVehicleHash);

		public void Init(BaseNPC controlledNPC)
		{
			controlledAnimator = controlledNPC.NPCAnimator;
			rbody = controlledNPC.NPCRigidbody;
			mainCollider = controlledNPC.RootModel.GetComponent<Collider>();
			rootTransform = controlledNPC.transform;
		}

		public void DeInit()
		{
			rbody.useGravity = true;
			controlledAnimator.applyRootMotion = true;
			mainCollider.material = null;
			controlledAnimator = null;
			rbody = null;
			mainCollider = null;
			rootTransform = null;
		}

		public void Move(Input newInput)
		{
			if ((bool)controlledAnimator)
			{
				input = newInput;
				lookPos = input.LookPos;
				velocity = rbody.velocity;
				ConvertMoveInput();
				TurnTowardsLookPos();
				ApplyExtraTurnRotation();
				GroundCheck();
				SetFriction();
				if (onGround)
				{
					HandleGroundedVelocities();
				}
				else
				{
					HandleAirborneVelocities();
				}
				UpdateAnimator();
				rbody.velocity = velocity;
			}
		}

		public void StartInCar(VehicleType vehicleType, bool force, bool driverDead)
		{
			controlledAnimator.SetInteger(vehicleTypeHash, (int)vehicleType);
			controlledAnimator.SetBool(forceGetVehicleHash, force);
			controlledAnimator.SetBool(deadInCarHash, driverDead);
			controlledAnimator.SetTrigger(startInCarHash);
		}

		private void Awake()
		{
			rayHitComparer = new RayHitComparer();
			GenerateAnimatorHashes();
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if ((bool)controlledAnimator)
			{
				controlledAnimator.SetLookAtWeight(LookAtWeights.weight, LookAtWeights.bodyWeight, LookAtWeights.headWeight, LookAtWeights.eyesWeight, LookAtWeights.clampWeight);
				if (LookTarget != null)
				{
					lookPos = LookTarget.position;
				}
				controlledAnimator.SetLookAtPosition(lookPos);
			}
		}

		private void OnAnimatorMove()
		{
			rbody.rotation = controlledAnimator.rootRotation;
			if (onGround && Time.deltaTime > 0f)
			{
				Vector3 vector = controlledAnimator.deltaPosition * MoveSpeedMultiplier / Time.deltaTime;
				vector.y = 0f;
				rbody.velocity = vector;
			}
		}

		private void ConvertMoveInput()
		{
			Vector3 vector = rootTransform.InverseTransformDirection(input.CamMove);
			turnAmount = Mathf.Atan2(vector.x, vector.z);
			forwardAmount = vector.z;
			strafeAmount = vector.x;
			if (input.AttackState != null && input.AttackState.Aim)
			{
				forwardAmount = input.InputMove.y;
				strafeAmount = input.InputMove.x;
			}
		}

		private void TurnTowardsLookPos()
		{
			if (Mathf.Abs(forwardAmount) < 0.01f)
			{
				Vector3 vector = rootTransform.InverseTransformDirection(input.LookPos - rootTransform.position);
				float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				if (Mathf.Abs(num) > AutoTurnThresholdAngle)
				{
					turnAmount += num + AutoTurnSpeed * 0.001f;
				}
			}
		}

		private void ApplyExtraTurnRotation()
		{
			float num = Mathf.Lerp(StationaryTurnSpeed, MovingTurnSpeed, forwardAmount);
			float yAngle = turnAmount * num * Time.deltaTime;
			rootTransform.Rotate(0f, yAngle, 0f);
		}

		private void GroundCheck()
		{
			Ray ray = new Ray(rootTransform.position + Vector3.up * 0.1f, -Vector3.up);
			int num = Physics.RaycastNonAlloc(ray, currentHits, 0.5f, GroundLayerMask);
			groundHits.Clear();
			for (int i = 0; i < num; i++)
			{
				groundHits.Add(currentHits[i]);
			}
			groundHits.Sort(rayHitComparer);
			bool flag = !onGround;
			if (velocity.y < 2.5f)
			{
				onGround = false;
				rbody.useGravity = true;
				for (int j = 0; j < groundHits.Count; j++)
				{
					RaycastHit raycastHit = groundHits[j];
					if (!raycastHit.collider.isTrigger)
					{
						onGround = true;
						if (velocity.y <= 0f && !Physics.Raycast(rootTransform.position, rootTransform.forward, 0.4f, GroundLayerMask))
						{
							rbody.position = Vector3.MoveTowards(rbody.position, raycastHit.point, Time.deltaTime * GroundStickyEffect);
							Rigidbody rigidbody = rbody;
							Vector3 eulerAngles = rootTransform.eulerAngles;
							rigidbody.rotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
						}
						rbody.useGravity = false;
						break;
					}
				}
			}
			if (flag && onGround && OnFallImpactCallback != null)
			{
				OnFallImpactCallback(Mathf.Abs(velocity.y));
			}
		}

		private void SetFriction()
		{
			if (onGround)
			{
				mainCollider.material = ((!(input.CamMove.magnitude < Mathf.Epsilon)) ? ZeroFrictionMaterial : HighFrictionMaterial);
			}
			else
			{
				mainCollider.material = ZeroFrictionMaterial;
			}
		}

		private void HandleGroundedVelocities()
		{
			velocity.y = 0f;
			if (input.CamMove.magnitude < Mathf.Epsilon)
			{
				velocity.x = 0f;
				velocity.z = 0f;
			}
		}

		private void HandleAirborneVelocities()
		{
			velocity = Vector3.Lerp(b: new Vector3(input.CamMove.x * 6f, velocity.y, input.CamMove.z * 6f), a: velocity, t: Time.deltaTime * 2f);
			rbody.useGravity = true;
			Vector3 force = Physics.gravity * GravityMultiplier - Physics.gravity;
			rbody.AddForce(force);
		}

		private void UpdateAnimator()
		{
			controlledAnimator.applyRootMotion = onGround;
			controlledAnimator.SetBool(strafeHash, input.AttackState.Aim);
			controlledAnimator.SetFloat(strafeDirHash, strafeAmount);
			controlledAnimator.SetFloat(forwardHash, forwardAmount, 0.1f, Time.deltaTime);
			bool value = (double)Mathf.Abs(strafeAmount) >= 0.1 || forwardAmount > 0.1f || forwardAmount < -0.1f;
			controlledAnimator.SetBool(isMovingHash, value);
			if (!input.AttackState.Aim)
			{
				controlledAnimator.SetFloat(turnHash, turnAmount, 0.1f, Time.deltaTime);
			}
			controlledAnimator.SetBool(onGroundHash, onGround);
			controlledAnimator.speed = ((!onGround || !(input.CamMove.magnitude > 0f)) ? 1f : 1f);
			controlledAnimator.SetBool(doMeleeHash, value: false);
			if (base.gameObject.CompareTag("Robot"))
			{
				controlledAnimator.SetBool(doSmashHash, value: false);
			}
			if (input.AttackState.MeleeAttackState != MeleeAttackState.None)
			{
				controlledAnimator.SetBool(doMeleeHash, value: true);
				if (base.gameObject.CompareTag("Robot") && (bool)SmashPrefab && !HasRobotInFront())
				{
					controlledAnimator.SetBool(doSmashHash, value: true);
					return;
				}
				controlledAnimator.SetInteger(meleeHash, (int)input.AttackState.MeleeAttackState);
				controlledAnimator.SetInteger(meleeWeaponTypeHash, (int)input.AttackState.MeleeWeaponType);
			}
			else
			{
				bool value2 = input.AttackState.RangedAttackState == RangedAttackState.Shoot;
				bool value3 = input.AttackState.RangedAttackState == RangedAttackState.Recharge;
				controlledAnimator.SetInteger(rangedWeaponTypeHash, (int)input.AttackState.RangedWeaponType);
				controlledAnimator.SetBool(rangedWeaponShootHash, value2);
				controlledAnimator.SetBool(rangedWeaponRechargeHash, value3);
			}
		}

		private bool HasRobotInFront()
		{
			Ray ray = new Ray(base.transform.position + Vector3.up * 3f, base.transform.forward);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 5f, TargetManager.Instance.ShootingLayerMask) && (hitInfo.collider.CompareTag("Player") || hitInfo.collider.CompareTag("Robot")))
			{
				return true;
			}
			return false;
		}

		private void GenerateAnimatorHashes()
		{
			forwardHash = Animator.StringToHash("Forward");
			turnHash = Animator.StringToHash("Turn");
			onGroundHash = Animator.StringToHash("OnGround");
			strafeHash = Animator.StringToHash("Strafe");
			strafeDirHash = Animator.StringToHash("StrafeDir");
			doMeleeHash = Animator.StringToHash("DoMelee");
			meleeHash = Animator.StringToHash("Melee");
			if (CompareTag("Robot"))
			{
				doSmashHash = Animator.StringToHash("DoSmash");
			}
			rangedWeaponTypeHash = Animator.StringToHash("RangedWeaponType");
			rangedWeaponShootHash = Animator.StringToHash("RangedWeaponShoot");
			rangedWeaponRechargeHash = Animator.StringToHash("RangedWeaponRecharge");
			forceGetVehicleHash = Animator.StringToHash("ForceGet");
			meleeWeaponTypeHash = Animator.StringToHash("MeleeWeaponType");
			isMovingHash = Animator.StringToHash("IsMoving");
			startInCarHash = Animator.StringToHash("StartInCar");
			deadInCarHash = Animator.StringToHash("DeadInCar");
			vehicleTypeHash = Animator.StringToHash("VehicleType");
		}

		public void Smash(HitEntity owner, GameObject[] ignored)
		{
			if ((bool)SmashPrefab)
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool(SmashPrefab);
				fromPool.transform.position = base.transform.position + base.transform.forward;
				Explosion component = fromPool.GetComponent<Explosion>();
				component.Init(owner, ignored);
			}
		}
	}
}
