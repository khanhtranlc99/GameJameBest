using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.Managers;
using UnityEngine;

public class ExperienceBurstCombo : ComboManager.BaseComboEffect
{
	[Space(10f)]
	public float PerKillExpMult = 0.05f;

	public float MaxAdditionalExpMult = 1f;

	private Player player;

	private new void Start()
	{
		player = PlayerInteractionsManager.Instance.Player;
	}

	public override void StartEffect(ComboManager.ComboInfo comboInfo)
	{
		float num = PerKillExpMult * (float)comboInfo.ComboMultiplier;
		if (num > MaxAdditionalExpMult)
		{
			num = MaxAdditionalExpMult;
		}
		LevelManager.Instance.AddExperience((int)(num * comboInfo.LastVictim.ExperienceForAKill));
		if (DebugLog && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log(num * comboInfo.LastVictim.ExperienceForAKill);
		}
	}

	public override void StopEffect()
	{
	}
}
