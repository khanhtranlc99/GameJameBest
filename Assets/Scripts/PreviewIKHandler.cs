using UnityEngine;

public class PreviewIKHandler : MonoBehaviour
{
	private Animator anim;

	public float ikWeightHand = 1f;

	public float ikWeightElbow = 1f;

	public bool DoIKAnimation;

	public Transform leftIKHandTarget;

	public Transform leftIKHandHint;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	public void TurnThisShitOff()
	{
		DoIKAnimation = false;
	}

	public void TurnThisShitOn()
	{
		DoIKAnimation = true;
	}

	private void OnAnimatorIK()
	{
		if (DoIKAnimation)
		{
			anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeightHand);
			anim.SetIKPosition(AvatarIKGoal.LeftHand, leftIKHandTarget.position);
			//anim.SetIKRotation(AvatarIKGoal.LeftHand, leftIKHandTarget.rotation);
			anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ikWeightElbow);
			anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftIKHandHint.position);
		}
	}
}
