using Game.Character;
using Game.Character.Stats;

namespace Game.Weapons
{
	public class EnergyAmmoContainer : JointAmmoContainer
	{
		public bool IsPlayer;

		private CharacterStat stamina;

		private void Awake()
		{
			if (IsPlayer)
			{
				stamina = PlayerInteractionsManager.Instance.Player.stats.stamina;
			}
		}

		public override int GetAmmoCount()
		{
			if (IsPlayer)
			{
				return (int)stamina.Current;
			}
			return base.GetAmmoCount();
		}
	}
}
