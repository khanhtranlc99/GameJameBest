using System;
using System.Collections.Generic;

namespace Game.Character.Stats
{
	[Serializable]
	public class StatsManager
	{
		public static StatsManager Instance;

		public int TimeForFullStaminaRegeneration = 60;

		public CharacterStat stamina = new CharacterStat();

		public List<StatsMas> upgradeValues = new List<StatsMas>();

		public static void SaveStat(StatsList name, int value)
		{
			BaseProfile.StoreValue(value, name.ToString());
			if (Instance != null)
			{
				Instance.Init();
			}
		}

		public static int GetStat(StatsList name)
		{
			return BaseProfile.ResolveValue(name.ToString(), 0);
		}

		public void Init()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			UpdateSpentPoints();
			UpdateStats();
		}

		public float GetPlayerStat(StatsList stat)
		{
			float result = 0f;
			for (int i = 0; i < upgradeValues.Count; i++)
			{
				StatsMas statsMas = upgradeValues[i];
				if (statsMas.stat == stat)
				{
					result = statsMas.ActualValue;
				}
			}
			return result;
		}

		public void UpdateStats()
		{
			float playerStat = GetPlayerStat(StatsList.Stamina);
			float regenPerSecond = playerStat / (float)TimeForFullStaminaRegeneration;
			stamina.Setup(playerStat, playerStat);
			stamina.RegenPerSecond = regenPerSecond;
		}

		private void UpdateSpentPoints()
		{
			foreach (StatsMas upgradeValue in upgradeValues)
			{
				upgradeValue.SpentPoints = GetStat(upgradeValue.stat);
			}
		}
	}
}
