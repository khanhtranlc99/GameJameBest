using UnityEngine;

public class PrevewAnimationController : MonoBehaviour
{
	public enum PreviewAnimType
	{
		None = -1,
		Idle,
		Rifle,
		Pistol,
		Melee,
		Shotgun,
		Minigun,
		RPG,
		Braslet,
		Body,
		Hands,
		Legs,
		Knife,
		BaseBoll,
		ShowMeThatShit,
		AXE,
		IronFly,
		ShootRope,
		SuperLanding,
		ClimbWalls,
		SuperKick
	}

	private const string AnimationStateName = "AnimationType";

	private Animator animator;

	private PreviewAnimType currenAnimType;

	private int animationStateHash;

	public PreviewAnimType animationToPlay = PreviewAnimType.Rifle;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		animationStateHash = Animator.StringToHash("AnimationType");
		currenAnimType = (PreviewAnimType)animator.GetInteger(animationStateHash);
	}

	public void SetPreviewAnimation(PreviewAnimType animaType)
	{
		if (animaType != currenAnimType)
		{
			animator.SetInteger(animationStateHash, (int)animaType);
			currenAnimType = animaType;
		}
	}
}
