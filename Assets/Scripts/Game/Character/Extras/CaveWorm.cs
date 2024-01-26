using Game.Character.Demo;
using UnityEngine;

namespace Game.Character.Extras
{
	public class CaveWorm : EnemyAI
	{
		public AnimationClip ClipIdle;

		public AnimationClip ClipMove;

		public AnimationClip ClipAttack;

		public AnimationClip ClipDead;

		protected override void UpdateAnimState()
		{
			float b = targetSpeed;
			switch (animState)
			{
			case AnimationState.Idle:
				GetComponent<Animation>().clip = ClipIdle;
				GetComponent<Animation>().Play();
				agent.isStopped = true;
				b = 0f;
				break;
			case AnimationState.Walk:
				GetComponent<Animation>().clip = ClipMove;
				GetComponent<Animation>().Play();
				b = 2f;
				break;
			case AnimationState.Attack:
				GetComponent<Animation>().clip = ClipAttack;
				GetComponent<Animation>().Play();
				b = 0.5f;
				break;
			case AnimationState.Dead:
				GetComponent<Animation>().clip = ClipDead;
				GetComponent<Animation>().wrapMode = WrapMode.Once;
				GetComponent<Animation>().Play();
				agent.isStopped = true;
				b = 0.5f;
				break;
			}
			targetSpeed = Mathf.Lerp(targetSpeed, b, Time.deltaTime * 5f);
			agentSpeed = 0f;
			if (targetSpeed <= 0.5f)
			{
				agentSpeed = targetSpeed / 0.5f;
			}
			else
			{
				agentSpeed = (targetSpeed - 0.5f) / 0.5f * 3.5f + 0.5f;
			}
			agentSpeed = targetSpeed;
		}

		protected override void OnDie()
		{
			base.OnDie();
			if ((bool)ZombieHUD.Instance)
			{
				ZombieHUD.Instance.WormKilled();
			}
		}
	}
}
