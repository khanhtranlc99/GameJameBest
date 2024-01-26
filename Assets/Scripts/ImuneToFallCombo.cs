using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

public class ImuneToFallCombo : ComboManager.BaseComboEffect
{
	[Separator("ImuneToFallCombo")]
	public bool CollisionInvul;

	[Space(5f)]
	public bool ExplosionInvul;

	private Player player;

	private bool originalCollisionInvul;

	private bool originalExplosionInvul;

	protected override void Start()
	{
		base.Start();
		player = PlayerInteractionsManager.Instance.Player;
		originalCollisionInvul = player.RDCollInvul;
		originalExplosionInvul = player.RDExpInvul;
	}

	public override void AddStackEffect()
	{
		if (CollisionInvul)
		{
			player.RDCollInvul = true;
		}
		if (ExplosionInvul)
		{
			player.RDExpInvul = true;
		}
	}

	public override void ClearStacksEffects()
	{
		player.RDCollInvul = originalCollisionInvul;
		player.RDExpInvul = originalExplosionInvul;
	}
}
