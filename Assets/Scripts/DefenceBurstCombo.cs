using Game.Character;
using Game.Character.CharacterController;

public class DefenceBurstCombo : ComboManager.BaseComboEffect
{
	[Separator("DefenceBurstCombo")]
	public Defence DefencePerStack;

	private Defence originalDefence;

	private Player player;

	protected override void Start()
	{
		base.Start();
		player = PlayerInteractionsManager.Instance.Player;
		originalDefence = player.Defence;
	}

	public override void AddStackEffect()
	{
		player.Defence = originalDefence + DefencePerStack * currStacks;
	}

	public override void ClearStacksEffects()
	{
		player.Defence = originalDefence;
	}
}
