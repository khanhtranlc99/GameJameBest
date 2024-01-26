using UnityEngine;

namespace Game.Enemy
{
	public class ForceGetOutReset : StateMachineBehaviour
	{
		private bool reseted;

		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			base.OnStateMachineEnter(animator, stateMachinePathHash);
			reseted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime > 0.9f && !reseted)
			{
				animator.SetBool("ForceGet", value: false);
				reseted = true;
			}
		}
	}
}
