using Game.Character.CharacterController;
using Game.Character.Input;
using UnityEngine;

namespace Game.Character.Extras
{
	public class EnemyAI : HitEntity
	{
		protected enum AnimationState
		{
			Idle,
			Walk,
			Run,
			Attack,
			Dead
		}

		public float AttackDistance = 2f;

		public float AttackTimeout = 1f;

		public bool Aggresive;

		protected UnityEngine.AI.NavMeshAgent agent;

		protected float attackTimer;

		protected HitEntity player;

		protected InputFilter velocityFilter;

		protected float targetSpeed;

		protected static int corpseCounter;

		protected float deadTimeout;

		protected float agentSpeed;

		protected AnimationState animState;

		protected override void Start()
		{
			base.Start();
			agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			animState = AnimationState.Idle;
			player = EntityManager.Instance.Player;
			velocityFilter = new InputFilter(10, 1f);
		}

		protected virtual void UpdateAnimState()
		{
		}

		protected override void Update()
		{
			if (base.IsDead)
			{
				deadTimeout += Time.deltaTime;
				if (corpseCounter > 10 && deadTimeout > 5f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					corpseCounter--;
				}
				return;
			}
			if (!player || player.IsDead)
			{
				animState = AnimationState.Idle;
			}
			UpdateAnimState();
			if (!player)
			{
				return;
			}
			float sqrMagnitude = (player.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude < AttackDistance * AttackDistance)
			{
				animState = AnimationState.Attack;
				agent.isStopped = true;
				attackTimer -= Time.deltaTime;
				if (attackTimer < 0f)
				{
					player.OnHit(DamageType.Bullet, this, 10f, base.transform.position + base.transform.forward + Vector3.up * 0.5f, Vector3.zero, 0f);
					attackTimer = AttackTimeout;
				}
				return;
			}
			Vector3 vector = player.transform.position - (player.transform.position - base.transform.position).normalized;
			vector = player.transform.position;
			agent.SetDestination(vector);
			if (agent.hasPath)
			{
				if (Aggresive)
				{
					animState = AnimationState.Run;
				}
				else
				{
					animState = AnimationState.Walk;
				}
				Vector3 normalized = (-(base.transform.position - agent.steeringTarget)).normalized;
				Vector3 a = SmoothVelocityVector(normalized);
				base.transform.position += a * agentSpeed * Time.deltaTime;
				normalized.y = 0f;
				if (normalized != Vector3.zero)
				{
					base.transform.forward = normalized;
				}
			}
			else
			{
				animState = AnimationState.Idle;
			}
		}

		protected Vector3 SmoothVelocityVector(Vector3 v)
		{
			velocityFilter.AddSample(new Vector2(v.x, v.z));
			Vector2 value = velocityFilter.GetValue();
			return new Vector3(value.x, 0f, value.y).normalized;
		}

		protected override void OnDie()
		{
			base.OnDie();
			animState = AnimationState.Dead;
			UpdateAnimState();
			GetComponent<Collider>().enabled = false;
			agent.enabled = false;
			corpseCounter++;
			deadTimeout = 0f;
		}

		public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			base.OnHit(damageType, owner, damage, hitPos, hitVector, defenceReduction);
			Aggresive = true;
		}

		protected override void FixedUpdate()
		{
			if (animState == AnimationState.Attack)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(player.transform.position - base.transform.position), Time.deltaTime * 10f);
			}
		}
	}
}
