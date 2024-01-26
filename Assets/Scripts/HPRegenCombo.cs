using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

public class HPRegenCombo : ComboManager.BaseComboEffect
{
	[Space(10f)]
	public float PerKillHPRegen = 0.05f;

	public float MaxAdditionalHPRegen = 0.5f;

	private Player player;

	private new void Start()
	{
		player = PlayerInteractionsManager.Instance.Player;
	}

	public override void StartEffect(ComboManager.ComboInfo comboInfo)
	{
		float num = PerKillHPRegen * (float)comboInfo.ComboMultiplier;
		if (num > MaxAdditionalHPRegen)
		{
			num = MaxAdditionalHPRegen;
		}
		player.Health.Current += num * player.Health.Max;
		if (DebugLog)
		{
			UnityEngine.Debug.Log(num * player.Health.Max);
		}
	}

	public override void StopEffect()
	{
	}
}
