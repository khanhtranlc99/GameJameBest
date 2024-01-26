using Game.Character.CharacterController;
using Game.Character.CharacterController.Enums;
using Game.Character.Input;
using Game.GlobalComponent;
using Game.Managers;
using Game.PickUps;
using Game.Vehicle;
using Game.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy
{
	public class SmartHumanoidController : BaseControllerNPC
	{
		private const float TargetHeightOffset = 1.4f;

		private const float LerpMult = 2f;

		private const float RunOutSpeedCounter = 2.5f;

		private const float TargetCornersError = 1f;

		private const float AimDelay = 2f;

		private const float StrafeTime = 1f;

		private const float BackToTargetSearchingTime = 0.5f;

		private const float BackToDummyTime = 3f;

		private const float EvadeTime = 1f;

		private const int RunOutEvadeTime = 5;

		private const int AirAttackDistanceCounter = 2;

		private const float SlowUpdatePeriod = 0.5f;

		private const float FlyingCheckHigh = 1f;

		[Separator("Smart Controller Parametrs")]
		public bool DebugLog;

		public ActionType NPCActionType;

		public float WalkSpeed = 0.5f;

		public float RunSpeed = 1f;

		public float SprintDurationTime = 2f;

		public float RangeAttackDistance = 15f;

		public float MeleeWeaponDistance = 2f;

		public float MeleeAttackDistance = 3.4f;

		[Tooltip("Делает непися бешеным. Он валит всех, кто не друг.")]
		public bool IsFCKingManiac;

		public bool VehiclesAsTargets;

		[Separator("Smart Controller Links")]
		public SmartHumanoidAnimationController AnimationController;

		public SmartHumanoidWeaponController WeaponController;

		public Collider AutoAimTrigger;

		public UnityEngine.AI.NavMeshAgent NavMeshAgent;

		private List<HitEntity> personalTargets = new List<HitEntity>();

		private List<HitEntity> targetInSightSensor = new List<HitEntity>();

		private List<HitEntity> secondaryTargetInSightSensor = new List<HitEntity>();

		private BaseStatusNPC currentStatus;

		private Transform rootTransform;

		private ActionState currentActionState;

		private bool attackEnemy;

		private bool moveToDestination;

		private HitEntity enemyTarget;

		private float rangeToNearestTarget;

		private InputFilter velocityFilter;

		private float moveSpeed = 1f;

		private float smoothedSpeed;

		private SmartHumanoidAnimationController.Input input;

		private float startRunTime;

		private Vector3 enemyTargetPosition;

		private float attackDistance;

		private bool previousAim;

		private float lastAimTime;

		private bool smoothAim;

		private AttackState currentAttackState;

		private float strafeTimer;

		private float BackToTargetSearching;

		private bool changeOnDummy;

		private float SearchingStartTime;

		private bool frontBlocked;

		private bool rightBlocked;

		private bool leftBlocked;

		private Vector3 stashedMoveVector;

		private float evadeTimer;

		private int bigDynamicLM;

		private int smallDynamicLM;

		private Predicate<HitEntity> targetsToAttackPredicate;

		private Predicate<HitEntity> personaltargetsToAttackPredicate;

		private readonly List<HitEntity> personalTargetsToAttack = new List<HitEntity>();

		private readonly List<HitEntity> targetsToAttack = new List<HitEntity>();

		private readonly List<HitEntity> secondaryTargetsToAttack = new List<HitEntity>();

		private UnityEngine.AI.NavMeshPath path;

		private SlowUpdateProc slowUpdateProc;

		public float FollowDistance => attackDistance - 2f;

		public override void Init(BaseNPC controlledNPC)
		{
			base.Init(controlledNPC);
			NPCActionType = controlledNPC.SpecificNpcLinks.SmartActionType;
			SprintDurationTime = controlledNPC.SpecificNpcLinks.SmartSprintTime;
			currentStatus = controlledNPC.StatusNpc;
			BaseStatusNPC baseStatusNPC = currentStatus;
			baseStatusNPC.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Combine(baseStatusNPC.OnHitEvent, new HitEntity.HealthChagedEvent(AggroOnHitEvent));
			BaseStatusNPC baseStatusNPC2 = currentStatus;
			baseStatusNPC2.DiedEvent = (HitEntity.AliveStateChagedEvent)Delegate.Combine(baseStatusNPC2.DiedEvent, new HitEntity.AliveStateChagedEvent(DropGunOnDeath));
			rootTransform = controlledNPC.transform;
			AnimationController.Init(controlledNPC);
			WeaponController.Init(controlledNPC);
			attackDistance = MeleeAttackDistance;
			SwitchOnComfortWeapon(RangeAttackDistance);
			AutoAimTrigger.enabled = true;
			NavMeshAgent.enabled = true;
			foreach (Weapon weapon in WeaponController.Weapons)
			{
				if (weapon.Archetype == WeaponArchetype.Ranged)
				{
					try
					{
						RangeAttackDistance = ((RangedWeapon)weapon).RangedFireDistanceNPC;
					}
					catch (Exception)
					{
					}
				}
			}
			bigDynamicLM = 1 << LayerMask.NameToLayer("BigDynamic");
			smallDynamicLM = 1 << LayerMask.NameToLayer("SmallDynamic");
		}

		public override void DeInit()
		{
			if (changeOnDummy)
			{
				BaseStatusNPC baseStatusNPC = currentStatus;
				baseStatusNPC.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Remove(baseStatusNPC.OnHitEvent, new HitEntity.HealthChagedEvent(ResetSearchTimer));
				BackToTargetSearching = 0f;
				changeOnDummy = false;
			}
			BaseStatusNPC baseStatusNPC2 = currentStatus;
			baseStatusNPC2.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Remove(baseStatusNPC2.OnHitEvent, new HitEntity.HealthChagedEvent(AggroOnHitEvent));
			BaseStatusNPC baseStatusNPC3 = currentStatus;
			baseStatusNPC3.DiedEvent = (HitEntity.AliveStateChagedEvent)Delegate.Remove(baseStatusNPC3.DiedEvent, new HitEntity.AliveStateChagedEvent(DropGunOnDeath));
			currentStatus = null;
			rootTransform = null;
			previousAim = false;
			stashedMoveVector = Vector3.zero;
			evadeTimer = 0f;
			targetInSightSensor.Clear();
			secondaryTargetInSightSensor.Clear();
			currentActionState = ActionState.None;
			AnimationController.DeInit();
			WeaponController.DeInit();
			AutoAimTrigger.enabled = false;
			NavMeshAgent.enabled = false;
			path.ClearCorners();
			base.DeInit();
		}

		public void InitBackToDummyLogic()
		{
			if (base.IsInited && !changeOnDummy)
			{
				BaseStatusNPC baseStatusNPC = currentStatus;
				baseStatusNPC.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Combine(baseStatusNPC.OnHitEvent, new HitEntity.HealthChagedEvent(ResetSearchTimer));
				changeOnDummy = true;
				BackToTargetSearching = 0.5f;
			}
		}

		public void AddPersonalTarget(HitEntity newTarget)
		{
			if (base.IsInited && !(newTarget == null) && FactionsManager.Instance.GetRelations(currentStatus.Faction, newTarget.Faction) != 0)
			{
				if (!personalTargets.Contains(newTarget))
				{
					personalTargets.Add(newTarget);
				}
				AddTarget(newTarget);
			}
		}

		public void AddTarget(HitEntity newTarget, bool toSecondary = false)
		{
			if (!base.IsInited || newTarget == null || FactionsManager.Instance.GetRelations(currentStatus.Faction, newTarget.Faction) == Relations.Friendly || newTarget == currentStatus || newTarget.transform.IsChildOf(rootTransform))
			{
				return;
			}
			if (!toSecondary)
			{
				if (!targetInSightSensor.Contains(newTarget))
				{
					targetInSightSensor.Add(newTarget);
				}
			}
			else if (!secondaryTargetInSightSensor.Contains(newTarget))
			{
				secondaryTargetInSightSensor.Add(newTarget);
			}
		}

		public void RemoveTarget(HitEntity oldTarget, bool fromSighnLine = false)
		{
			if (base.IsInited && (!fromSighnLine || !(oldTarget is Player)))
			{
				if (targetInSightSensor.Contains(oldTarget) && currentActionState != ActionState.RunOut)
				{
					targetInSightSensor.Remove(oldTarget);
				}
				if (secondaryTargetInSightSensor.Contains(oldTarget))
				{
					secondaryTargetInSightSensor.Remove(oldTarget);
				}
			}
		}

		public void UpdateSensorInfo(SmartHumanoidCollisionTrigger.HumanoidSensorType sensorType)
		{
			switch (sensorType)
			{
			case SmartHumanoidCollisionTrigger.HumanoidSensorType.Front:
				frontBlocked = true;
				break;
			case SmartHumanoidCollisionTrigger.HumanoidSensorType.Left:
				leftBlocked = true;
				break;
			case SmartHumanoidCollisionTrigger.HumanoidSensorType.Right:
				rightBlocked = true;
				break;
			}
		}

		private void Awake()
		{
			velocityFilter = new InputFilter(10, 1f);
			personaltargetsToAttackPredicate = ((HitEntity target) => (target == null || currentStatus == null) ? false : (target != null && FactionsManager.Instance.GetRelations(currentStatus.Faction, target.Faction) != Relations.Friendly));
			targetsToAttackPredicate = ((HitEntity target) => (target == null) ? false : ((!IsFCKingManiac) ? (FactionsManager.Instance.GetRelations(currentStatus.Faction, target.Faction) == Relations.Hostile) : (FactionsManager.Instance.GetRelations(currentStatus.Faction, target.Faction) != Relations.Friendly)));
			path = new UnityEngine.AI.NavMeshPath();
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 0.5f);
		}

		protected override void Update()
		{
			if (base.IsInited && !AnimationController.CantMove)
			{
				base.Update();
				base.transform.localPosition = Vector3.zero;
				base.transform.localEulerAngles = Vector3.zero;
				currentAttackState = WeaponController.UpdateAttackState(attackEnemy);
				UpdateAttack(currentAttackState);
				ProceedMove();
			}
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void ProceedMove()
		{
			if (!base.IsInited || AnimationController.CantMove)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Vector3 inputMove = Vector3.zero;
			Vector3 lookPos = rootTransform.position + rootTransform.forward + Vector3.up * 1.4f;
			if (moveToDestination)
			{
				if (NavMeshAgent.hasPath)
				{
					vector = SmoothVelocityVector((NavMeshAgent.steeringTarget - rootTransform.position).normalized);
					if (!attackEnemy)
					{
						vector = CalculateMoveVectorWithObstaclesAvoid(vector, true, 1f);
					}
					inputMove.y = moveSpeed;
					lookPos = rootTransform.position + vector + Vector3.up * 1.4f;
				}
				if (currentActionState == ActionState.RunOut && enemyTarget != null)
				{
					vector = SmoothVelocityVector((rootTransform.position - enemyTarget.transform.position).normalized);
					vector = CalculateMoveVectorWithObstaclesAvoid(vector, true, 5f);
					inputMove.y = moveSpeed;
					lookPos = rootTransform.position + vector + Vector3.up * 1.4f;
				}
			}
			else
			{
				if (NavMeshAgent.isActiveAndEnabled && NavMeshAgent.isOnNavMesh)
				{
					NavMeshAgent.isStopped = true;
				}
				lookPos = enemyTargetPosition + Vector3.up * 1.4f;
				if ((bool)enemyTarget)
				{
					vector = (enemyTargetPosition - rootTransform.position).normalized;
					moveSpeed = 0.01f;
				}
			}
			if (strafeTimer > 0f)
			{
				inputMove = (rootTransform.position + rootTransform.right).normalized;
				vector = (enemyTargetPosition - rootTransform.position).normalized;
				strafeTimer -= Time.deltaTime;
			}
			smoothedSpeed = Mathf.Lerp(smoothedSpeed, inputMove.y, Time.deltaTime * 2f);
			inputMove.y = smoothedSpeed;
			input.CamMove = vector * moveSpeed;
			input.InputMove = inputMove;
			input.SmoothAimRotation = true;
			input.LookPos = lookPos;
			input.AimTurn = true;
			input.AttackState = currentAttackState;
			AnimationController.Move(input);
			DropSensorStatus();
		}

		private Vector3 CalculateMoveVectorWithObstaclesAvoid(Vector3 currentMoveVector, bool resetEvade, float evadeTime)
		{
			Vector3 vector = currentMoveVector;
			if (evadeTimer > 0f)
			{
				if (stashedMoveVector == Vector3.zero)
				{
					stashedMoveVector = vector;
				}
				vector = stashedMoveVector;
				if (resetEvade)
				{
					evadeTimer -= Time.deltaTime;
					if (evadeTimer <= 0f)
					{
						stashedMoveVector = Vector3.zero;
					}
				}
			}
			if (frontBlocked)
			{
				vector = (leftBlocked ? (vector + rootTransform.right.normalized) : ((!rightBlocked) ? (vector + rootTransform.right.normalized) : (vector - rootTransform.right.normalized)));
				vector.Normalize();
				stashedMoveVector = vector;
				evadeTimer = evadeTime;
			}
			return vector;
		}

		private HitEntity FindNearesEnemyTarget(out float minRange)
		{
			HitEntity hitEntity = null;
			minRange = float.PositiveInfinity;
			if (targetInSightSensor.Count == 0 && secondaryTargetInSightSensor.Count == 0 && personalTargets.Count == 0)
			{
				if (DebugLog)
				{
					UnityEngine.Debug.Log("Вокруг ни души");
				}
				return null;
			}
			personalTargetsToAttack.Clear();
			targetsToAttack.Clear();
			secondaryTargetsToAttack.Clear();
			for (int i = 0; i < personalTargets.Count; i++)
			{
				HitEntity hitEntity2 = personalTargets[i];
				if (personaltargetsToAttackPredicate(hitEntity2))
				{
					personalTargetsToAttack.Add(hitEntity2);
				}
			}
			for (int j = 0; j < personalTargetsToAttack.Count; j++)
			{
				HitEntity hitEntity3 = personalTargetsToAttack[j];
				if (hitEntity3 == null)
				{
					personalTargets.Remove(hitEntity3);
					targetInSightSensor.Remove(hitEntity3);
					secondaryTargetInSightSensor.Remove(hitEntity3);
					continue;
				}
				if (!hitEntity3.isActiveAndEnabled)
				{
					Player x = hitEntity3 as Player;
					bool flag = x != null;
					DriverStatus driverStatus = hitEntity3 as DriverStatus;
					bool flag2 = driverStatus != null && driverStatus.IsPlayer;
					RagdollStatus ragdollStatus = hitEntity3 as RagdollStatus;
					bool flag3 = ragdollStatus != null && ragdollStatus.wakeUper.OriginHitEntity == PlayerManager.Instance.Player;
					if (flag2 || flag || flag3)
					{
						AddPersonalTarget(PlayerManager.Instance.PlayerAsTarget);
					}
					if (!flag || (flag && !PlayerManager.Instance.IsGettingInOrOut))
					{
						personalTargets.Remove(hitEntity3);
						targetInSightSensor.Remove(hitEntity3);
						secondaryTargetInSightSensor.Remove(hitEntity3);
					}
				}
				if (hitEntity3.IsDead)
				{
					personalTargets.Remove(hitEntity3);
					targetInSightSensor.Remove(hitEntity3);
					secondaryTargetInSightSensor.Remove(hitEntity3);
					continue;
				}
				float num = (!(hitEntity3.MainCollider != null)) ? RangeToPoint(hitEntity3.transform.position) : RangeToPoint(hitEntity3.MainCollider.ClosestPointOnBounds(base.transform.position));
				if (num < minRange)
				{
					minRange = num;
					hitEntity = hitEntity3;
				}
			}
			if (hitEntity == null)
			{
				for (int k = 0; k < targetInSightSensor.Count; k++)
				{
					HitEntity hitEntity4 = targetInSightSensor[k];
					if (targetsToAttackPredicate(hitEntity4))
					{
						targetsToAttack.Add(hitEntity4);
					}
				}
				for (int l = 0; l < targetsToAttack.Count; l++)
				{
					HitEntity hitEntity5 = targetsToAttack[l];
					if (hitEntity5 == null)
					{
						personalTargets.Remove(hitEntity5);
						targetInSightSensor.Remove(hitEntity5);
						secondaryTargetInSightSensor.Remove(hitEntity5);
						continue;
					}
					if (!hitEntity5.isActiveAndEnabled)
					{
						Player x2 = hitEntity5 as Player;
						bool flag4 = x2 != null;
						DriverStatus driverStatus2 = hitEntity5 as DriverStatus;
						if ((driverStatus2 != null && driverStatus2.IsPlayer) || flag4)
						{
							AddTarget(PlayerManager.Instance.PlayerAsTarget);
						}
						if (!flag4 || (flag4 && !PlayerManager.Instance.IsGettingInOrOut))
						{
							personalTargets.Remove(hitEntity5);
							targetInSightSensor.Remove(hitEntity5);
							secondaryTargetInSightSensor.Remove(hitEntity5);
						}
					}
					if (hitEntity5.IsDead)
					{
						personalTargets.Remove(hitEntity5);
						targetInSightSensor.Remove(hitEntity5);
						secondaryTargetInSightSensor.Remove(hitEntity5);
						continue;
					}
					float num2 = (!(hitEntity5.MainCollider != null)) ? RangeToPoint(hitEntity5.transform.position) : RangeToPoint(hitEntity5.MainCollider.ClosestPointOnBounds(base.transform.position));
					if (num2 < minRange)
					{
						minRange = num2;
						hitEntity = hitEntity5;
					}
				}
			}
			if (hitEntity == null)
			{
				for (int m = 0; m < secondaryTargetInSightSensor.Count; m++)
				{
					HitEntity hitEntity6 = secondaryTargetInSightSensor[m];
					if (targetsToAttackPredicate(hitEntity6))
					{
						secondaryTargetsToAttack.Add(hitEntity6);
					}
				}
				for (int n = 0; n < secondaryTargetsToAttack.Count; n++)
				{
					HitEntity hitEntity7 = secondaryTargetsToAttack[n];
					if (hitEntity7 == null)
					{
						personalTargets.Remove(hitEntity7);
						targetInSightSensor.Remove(hitEntity7);
						secondaryTargetInSightSensor.Remove(hitEntity7);
						continue;
					}
					if (!hitEntity7.isActiveAndEnabled)
					{
						Player x3 = hitEntity7 as Player;
						bool flag5 = x3 != null;
						DriverStatus driverStatus3 = hitEntity7 as DriverStatus;
						if ((driverStatus3 != null && driverStatus3.IsPlayer) || flag5)
						{
							AddTarget(PlayerManager.Instance.PlayerAsTarget, toSecondary: true);
						}
						if (!flag5 || (flag5 && !PlayerManager.Instance.IsGettingInOrOut))
						{
							personalTargets.Remove(hitEntity7);
							targetInSightSensor.Remove(hitEntity7);
							secondaryTargetInSightSensor.Remove(hitEntity7);
						}
					}
					if (hitEntity7.IsDead)
					{
						personalTargets.Remove(hitEntity7);
						targetInSightSensor.Remove(hitEntity7);
						secondaryTargetInSightSensor.Remove(hitEntity7);
						continue;
					}
					float num3 = (!(hitEntity7.MainCollider != null)) ? RangeToPoint(hitEntity7.transform.position) : RangeToPoint(hitEntity7.MainCollider.ClosestPointOnBounds(base.transform.position));
					if (num3 < minRange)
					{
						minRange = num3;
						hitEntity = hitEntity7;
					}
				}
			}
			DriverStatus driverStatus4 = hitEntity as DriverStatus;
			if (driverStatus4 != null && driverStatus4.InArmoredVehicle)
			{
				DrivableVehicle componentInParent = driverStatus4.GetComponentInParent<DrivableVehicle>();
				if ((bool)componentInParent)
				{
					hitEntity = componentInParent.GetVehicleStatus();
				}
			}
			if (DebugLog)
			{
				if ((bool)hitEntity)
				{
					UnityEngine.Debug.Log("Выбрал целью " + hitEntity.name);
				}
				else
				{
					UnityEngine.Debug.Log("Не нашел подходящей цели");
				}
			}
			return hitEntity;
		}

		private void SearchForTarget()
		{
			if (SearchingStartTime == 0f)
			{
				SearchingStartTime = Time.time;
			}
			if (Time.time - SearchingStartTime >= 3f)
			{
				SearchingStartTime = 0f;
				CurrentControlledNpc.ChangeController(CurrentControlledNpc.QuietControllerType);
			}
			float num = UnityEngine.Random.Range(-1f, 1f);
			float num2 = UnityEngine.Random.Range(-1f, 1f);
			Vector3 position = base.transform.position;
			float x = position.x + num;
			Vector3 position2 = base.transform.position;
			float y = position2.y;
			Vector3 position3 = base.transform.position;
			Vector3 toPoint = new Vector3(x, y, position3.z + num2);
			if (CalculatePathToPoint(toPoint))
			{
				SetNavMeshPath(path);
				moveToDestination = true;
			}
		}

		public bool TargetInSightLine(HitEntity target)
		{
			LayerMask mask = AnimationController.GroundLayerMask;
			if (target is VehicleStatus || target is DriverStatus)
			{
				mask = ((int)mask & ~bigDynamicLM);
			}
			if (target is RagdollDamageReciever)
			{
				mask = ((int)mask & ~smallDynamicLM);
			}
			Vector3 start = base.transform.position + currentStatus.NPCShootVectorOffset;
			Vector3 end = target.transform.position + target.NPCShootVectorOffset;
			if (Physics.Linecast(start, end, mask))
			{
				return false;
			}
			return true;
		}

		private void UpdateEnemyState(float minRangeToTarget)
		{
			RaycastHit raycastHit = ProjectionOnGround(enemyTarget.transform.position + Vector3.up * 0.01f);
			enemyTargetPosition = raycastHit.point;
			float sqrMagnitude = (enemyTarget.transform.position - enemyTargetPosition).sqrMagnitude;
			bool flag = sqrMagnitude >= 1f;
			if (DebugLog)
			{
				UnityEngine.Debug.DrawLine(raycastHit.point, raycastHit.point + Vector3.up, Color.red, 3f);
			}
			if (WeaponController.CurrentArchetype == WeaponArchetype.Ranged)
			{
				attackDistance = (flag ? (RangeAttackDistance * 2f) : RangeAttackDistance);
			}
			if (!CalculatePathToPoint(enemyTargetPosition) || (flag && sqrMagnitude > attackDistance * attackDistance))
			{
				moveToDestination = false;
			}
			else
			{
				SetNavMeshPath(path);
				moveToDestination = true;
			}
			Vector3 b = (!enemyTarget.MainCollider) ? enemyTarget.transform.position : enemyTarget.MainCollider.ClosestPointOnBounds(rootTransform.position);
			float rangeToClosestTarget = Vector3.Distance(rootTransform.position, b);
			SwitchOnComfortWeapon(rangeToClosestTarget);
			if (minRangeToTarget < attackDistance * attackDistance && TargetInSightLine(enemyTarget))
			{
				attackEnemy = ((NavMeshAgent.steeringTarget - NavMeshAgent.destination).sqrMagnitude < 1f || flag);
				if (minRangeToTarget < FollowDistance * FollowDistance && attackEnemy)
				{
					moveToDestination = false;
				}
				CurrentControlledNpc.TryTalk(TalkingObject.PhraseType.Attack);
			}
			else
			{
				CurrentControlledNpc.TryTalk(TalkingObject.PhraseType.Alarm);
			}
			currentActionState = ActionState.Enemy;
		}

		private void UpdateEnemyLostState()
		{
			float sqrMagnitude = (rootTransform.position - enemyTargetPosition).sqrMagnitude;
			if (sqrMagnitude > 1f)
			{
				moveToDestination = true;
				moveSpeed = RunSpeed;
			}
			else
			{
				currentActionState = ActionState.None;
			}
		}

		private void UpdateAttack(AttackState attackState)
		{
			if (!attackState.Aim && previousAim)
			{
				lastAimTime = Time.time;
				smoothAim = true;
			}
			previousAim = attackState.Aim;
			if (smoothAim)
			{
				if (Time.time < lastAimTime + 2f)
				{
					attackState.Aim = true;
				}
				else
				{
					smoothAim = false;
				}
			}
			if (attackState.RangedAttackState == RangedAttackState.ShootInFriendly)
			{
				strafeTimer = 1f;
			}
			if (!WeaponController.CheckCurrentWeaponAmmoExist())
			{
				ChangeWeaponType(WeaponArchetype.Melee, delegate
				{
					attackDistance = MeleeAttackDistance;
					previousAim = false;
				});
			}
			if (attackState.CanAttack)
			{
				WeaponController.Attack(enemyTarget);
				if (DebugLog)
				{
					UnityEngine.Debug.Log("Атакую: " + enemyTarget.name);
				}
			}
		}

		private void SwitchOnComfortWeapon(float rangeToClosestTarget)
		{
			if (rangeToClosestTarget < RangeAttackDistance && rangeToClosestTarget > MeleeWeaponDistance && WeaponController.CurrentArchetype != 0)
			{
				ChangeWeaponType(WeaponArchetype.Ranged, delegate
				{
					attackDistance = RangeAttackDistance;
				});
			}
			else if (rangeToClosestTarget < MeleeWeaponDistance && WeaponController.CurrentArchetype != WeaponArchetype.Melee)
			{
				ChangeWeaponType(WeaponArchetype.Melee, delegate
				{
					attackDistance = MeleeAttackDistance;
					previousAim = false;
				});
			}
		}

		private void ChangeWeaponType(WeaponArchetype newWeaponArchetype, Action actionIfSuccess)
		{
			if (WeaponController.ActivateWeaponByType(newWeaponArchetype))
			{
				actionIfSuccess();
			}
		}

		private Vector3 SmoothVelocityVector(Vector3 v)
		{
			velocityFilter.AddSample(new Vector2(v.x, v.z));
			Vector2 value = velocityFilter.GetValue();
			return new Vector3(value.x, 0f, value.y).normalized;
		}

		private float RangeToPoint(Vector3 point)
		{
			return (rootTransform.position - point).sqrMagnitude;
		}

		private void ResetSearchTimer(HitEntity disturber)
		{
			BackToTargetSearching = 0.5f;
		}

		private void AggroOnHitEvent(HitEntity disturber)
		{
			AddPersonalTarget(disturber);
		}

		private void DropGunOnDeath()
		{
			if (currentStatus.isTransformer && GameManager.Instance.IsTransformersGame)
			{
				PickUpManager.Instance.GenerateEnergyOnPoint(rootTransform.position - rootTransform.right);
			}
			if (!GameManager.Instance.IsTransformersGame && WeaponController.CurrentWeapon != null && WeaponController.CurrentArchetype == WeaponArchetype.Ranged)
			{
				PickUpManager.Instance.GenerateBulletsOnPoint(rootTransform.position - rootTransform.right, WeaponController.CurrentWeapon.AmmoType);
			}
		}

		private void RunAwayFromTarget(HitEntity target)
		{
			if (currentActionState != ActionState.RunOut)
			{
				startRunTime = Time.time;
				CurrentControlledNpc.TryTalk(TalkingObject.PhraseType.RunOut);
			}
			if (!SectorManager.Instance.IsInActiveSector(rootTransform.position))
			{
				RemoveTarget(target);
				IsFCKingManiac = false;
				moveSpeed = WalkSpeed;
				return;
			}
			previousAim = false;
			moveToDestination = true;
			float num = 1f;
			if (Time.time <= startRunTime + SprintDurationTime)
			{
				num = 2.5f;
			}
			moveSpeed = RunSpeed * num;
			currentActionState = ActionState.RunOut;
		}

		private bool CalculatePathToPoint(Vector3 toPoint)
		{
			if (!NavMeshAgent.isOnNavMesh)
			{
				return false;
			}
			return NavMeshAgent.CalculatePath(toPoint, path);
		}

		private RaycastHit ProjectionOnGround(Vector3 targetPoint, float maxDistance = float.PositiveInfinity)
		{
			RaycastHit hitInfo;
			Physics.Raycast(targetPoint, Vector3.down, out hitInfo, maxDistance, PlayerManager.Instance.DefaulAnimationController.ObstaclesLayerMask);
			return hitInfo;
		}

		private void SetNavMeshPath(UnityEngine.AI.NavMeshPath path)
		{
			NavMeshAgent.SetPath(path);
			moveToDestination = true;
			moveSpeed = RunSpeed;
		}

		private void DropSensorStatus()
		{
			frontBlocked = false;
			leftBlocked = false;
			rightBlocked = false;
		}

		private void SlowUpdate()
		{
			enemyTarget = FindNearesEnemyTarget(out rangeToNearestTarget);
			attackEnemy = false;
			moveToDestination = false;
			if (DebugLog && (bool)enemyTarget)
			{
				UnityEngine.Debug.Log(enemyTarget.name);
			}
			if (enemyTarget != null)
			{
				ResetSearchTimer(null);
				switch (NPCActionType)
				{
				case ActionType.Coward:
					RunAwayFromTarget(enemyTarget);
					break;
				case ActionType.Neutral:
					if (currentStatus.Health.Current > currentStatus.Health.Max * 0.3f)
					{
						UpdateEnemyState(rangeToNearestTarget);
					}
					else
					{
						RunAwayFromTarget(enemyTarget);
					}
					break;
				case ActionType.Agressive:
					UpdateEnemyState(rangeToNearestTarget);
					break;
				}
			}
			else
			{
				if (changeOnDummy && BackToTargetSearching <= 0f)
				{
					SearchForTarget();
				}
				else if (BackToTargetSearching > 0f)
				{
					BackToTargetSearching -= Time.deltaTime;
				}
				if (currentActionState == ActionState.Enemy)
				{
					UpdateEnemyLostState();
				}
				else
				{
					currentActionState = ActionState.None;
				}
			}
		}
	}
}
