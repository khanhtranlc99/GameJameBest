using Game.GlobalComponent;
using UnityEngine;

public class PlaySoundTypeOnAnimation : StateMachineBehaviour
{
	public TypeOfSound soundType;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		PointSoundManager.Instance.PlaySoundAtPoint(animator.transform.position, soundType);
	}
}
