using UnityEngine;

namespace Game.GlobalComponent
{
	public class PlaySoundOnAnimation : StateMachineBehaviour
	{
		public AudioClip sound;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			PointSoundManager.Instance.PlayCustomClipAtPoint(animator.transform.position, sound);
		}
	}
}
