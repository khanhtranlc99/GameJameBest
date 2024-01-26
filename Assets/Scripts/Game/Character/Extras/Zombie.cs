using Game.Character.Demo;
using UnityEngine;

namespace Game.Character.Extras
{
	public class Zombie : EnemyAI
	{
		private Animator animator;

		private bool injured;

		protected override void Start()
		{
			base.Start();
			animator = GetComponent<Animator>();
			animator.applyRootMotion = false;
			injured = (Random.value > 0.5f);
		}

		protected override void UpdateAnimState()
		{
			float b = base.targetSpeed;
			switch (animState)
			{
			case AnimationState.Idle:
				b = 0f;
				agent.isStopped = true;
				break;
			case AnimationState.Walk:
				b = 0.5f;
				break;
			case AnimationState.Run:
				b = 1f;
				break;
			case AnimationState.Attack:
				b = 0.5f;
				break;
			case AnimationState.Dead:
				base.targetSpeed = 0f;
				agent.isStopped = true;
				break;
			}
			base.targetSpeed = Mathf.Lerp(base.targetSpeed, b, Time.deltaTime * 5f);
			float targetSpeed = base.targetSpeed;
			agentSpeed = 0f;
			if (base.targetSpeed <= 0.5f)
			{
				agentSpeed = base.targetSpeed / 0.5f;
			}
			else
			{
				agentSpeed = (base.targetSpeed - 0.5f) / 0.5f * 3.5f + 0.5f;
			}
			animator.SetFloat("Speed", targetSpeed);
			animator.SetBool("Injured", injured);
		}

		protected override void OnDie()
		{
			base.OnDie();
			if ((bool)ZombieHUD.Instance)
			{
				ZombieHUD.Instance.ZombieKilled();
			}
			animator.SetBool("Die", value: true);
			EnemyAI.corpseCounter++;
			deadTimeout = 0f;
		}
	}
}
