using Game.Character.Extras;
using Game.Character.Input;
using UnityEngine;

namespace Game.Character.CharacterController
{
	[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
	[RequireComponent(typeof(AnimationController))]
	public class AIController : MonoBehaviour
	{
		public float AttackDistance = 5f;

		public float TargetChangeTolerance = 1f;

		public float Speed = 100f;

		public float TargetLookOffset = 1.4f;

		protected UnityEngine.AI.NavMeshAgent agent;

		protected AnimationController animController;

		private Vector3 targetPos;

		private InputManager inputManager;

		private Human human;

		private InputFilter velocityFilter;

		private HitEntity enemyTarget;

		private bool attackEnemy;

		private bool stickMove;

		public void Activate(bool status)
		{
			base.enabled = status;
			targetPos = base.transform.position;
			agent.enabled = status;
		}

		private void Awake()
		{
			agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			agent.enabled = true;
			animController = GetComponent<AnimationController>();
			inputManager = InputManager.Instance;
			human = GetComponent<Human>();
			velocityFilter = new InputFilter(10, 1f);
		}

		private void UpdateInput()
		{
			if (human.Remote || human.IsDead)
			{
				return;
			}
			Game.Character.Input.Input input = inputManager.GetInput(InputType.WaypointPos);
			if (input.Valid)
			{
				enemyTarget = EntityManager.Instance.Find((Vector3)input.Value, 2f, "Player");
				SetTarget((Vector3)input.Value);
				stickMove = false;
				return;
			}
			Game.Character.Input.Input input2 = inputManager.GetInput(InputType.Move);
			bool flag = inputManager.InputPreset == InputPreset.RPG;
			if (input2.Valid && flag)
			{
				Vector2 vector = (Vector2)input2.Value;
				Transform transform = CameraManager.Instance.UnityCamera.transform;
				Vector3 normalized = Vector3.Scale(transform.forward, new Vector3(1f, 0f, 1f)).normalized;
				Vector3 a = vector.y * normalized + vector.x * transform.right;
				stickMove = true;
				SetTarget(base.gameObject.transform.position + a * 10f);
			}
		}

		private void UpdateEnemyTarget()
		{
			attackEnemy = false;
			if (!enemyTarget)
			{
				return;
			}
			if ((bool)RTSProjector.Instance)
			{
				RTSProjector.Instance.Project(enemyTarget.transform.position, Color.red);
			}
			if (enemyTarget.IsDead)
			{
				enemyTarget = null;
				attackEnemy = false;
				agent.ResetPath();
				return;
			}
			float sqrMagnitude = (enemyTarget.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < AttackDistance * AttackDistance)
			{
				attackEnemy = true;
			}
		}

		private void Update()
		{
			UpdateInput();
			UpdateEnemyTarget();
			Vector3 vector = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Vector3 vector2 = base.transform.position + base.transform.forward + Vector3.up * TargetLookOffset;
			bool status = false;
			if (agent.hasPath)
			{
				if (attackEnemy)
				{
					status = true;
					human.Attack(enemyTarget);
					agent.isStopped = true;
					vector = SmoothVelocityVector((vector2 - base.transform.position).normalized);
					vector2 = enemyTarget.transform.position + Vector3.up * 1.6f;
				}
				else if ((base.transform.position - agent.destination).sqrMagnitude > 1f)
				{
					vector = SmoothVelocityVector((agent.steeringTarget - base.transform.position).normalized) * 1f;
					zero.y = 1f;
					vector2 = base.transform.position + vector + Vector3.up * TargetLookOffset;
				}
			}
			human.Aim(status);
			bool input = inputManager.GetInput(InputType.Reset, defaultValue: false);
			if (input)
			{
				CameraManager.Instance.ResetCameraMode();
				human.Resurrect();
			}
			bool isDead = human.IsDead;
			if (isDead)
			{
			}
			bool jump = false;
			animController.Move(new Input
			{
				camMove = vector,
				inputMove = zero,
				smoothAimRotation = true,
				crouch = false,
				jump = jump,
				lookPos = vector2,
				die = isDead,
				reset = input
			});
		}

		public void SetTarget(Vector3 target)
		{
			if ((target - targetPos).magnitude > TargetChangeTolerance)
			{
				targetPos = target;
				agent.SetDestination(targetPos);
				if (!enemyTarget && !stickMove && (bool)RTSProjector.Instance)
				{
					RTSProjector.Instance.Project(target, Color.white);
				}
			}
		}

		private Vector3 SmoothVelocityVector(Vector3 v)
		{
			velocityFilter.AddSample(new Vector2(v.x, v.z));
			Vector2 value = velocityFilter.GetValue();
			return new Vector3(value.x, 0f, value.y).normalized;
		}
	}
}
