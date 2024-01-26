using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

public class SpeedBurstCombo : ComboManager.BaseComboEffect
{
	[Separator("SpeedBurstCombo")]
	public float SpeedPercentPerStack = 10f;

	private AnimationController animationController;

	private Vector2 originalSpeedMults;

	private float originalAirSpeed;

	private float originalClimbeSpeed;

	protected override void Start()
	{
		base.Start();
		animationController = PlayerInteractionsManager.Instance.Player.GetComponent<AnimationController>();
		originalSpeedMults = animationController.SpeedMults;
		originalAirSpeed = animationController.AirSpeed;
		originalClimbeSpeed = animationController.ClimbingSpeed;
	}

	public override void AddStackEffect()
	{
		animationController.SpeedMults = new Vector2(originalSpeedMults.x + (float)currStacks * originalSpeedMults.x * SpeedPercentPerStack / 100f, originalSpeedMults.y + (float)currStacks * originalSpeedMults.y * SpeedPercentPerStack / 100f);
		animationController.AirSpeed = originalAirSpeed + (float)currStacks * originalAirSpeed * SpeedPercentPerStack / 100f;
		animationController.ClimbingSpeed = originalClimbeSpeed + (float)currStacks * originalClimbeSpeed * SpeedPercentPerStack / 100f;
	}

	public override void ClearStacksEffects()
	{
		animationController.SpeedMults = originalSpeedMults;
		animationController.AirSpeed = originalAirSpeed;
		animationController.ClimbingSpeed = originalClimbeSpeed;
	}
}
