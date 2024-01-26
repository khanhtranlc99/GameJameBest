using Game.GlobalComponent.Qwest;
using Game.Managers;
using System;
using UnityEngine;

namespace Game.Character.Stats
{
	public class LevelManager : MonoBehaviour
	{
		public int ExpForFirstLevel = 1000;

		public float PercenOfExpForNextLevel = 0.1f;

		public static LevelManager Instance;

		[HideInInspector]
		public int ExpForNextLevel;

		public Action OnLevelUpAction;

		public void AddExperience(int exp, bool useVIPmult = false)
		{
			PlayerInfoManager.Instance.ChangeInfoValue(PlayerInfoType.Experience, exp, useVIPmult);
			if (PlayerInfoManager.Experience >= ExpForNextLevel)
			{
				LevelUp();
				int exp2 = PlayerInfoManager.Experience - ExpForNextLevel;
				ExpForNextLevel = CalculateExpForLvl(PlayerInfoManager.Level, ExpForFirstLevel);
				PlayerInfoManager.Experience = 0;
				AddExperience(exp2);
			}
		}

		private void Awake()
		{
			Instance = this;
			ExpForNextLevel = CalculateExpForLvl(PlayerInfoManager.Level, ExpForFirstLevel);
		}

		private int CalculateExpForLvl(int lvl, int exp)
		{
			while (true)
			{
				int num = (int)((float)exp + (float)exp * PercenOfExpForNextLevel);
				if (lvl == 0)
				{
					break;
				}
				lvl--;
				exp = num;
			}
			return exp;
		}

		private void LevelUp()
		{
			PlayerInfoManager.UpgradePoints++;
			PlayerInfoManager.Level++;
			int level = PlayerInfoManager.Level;
			GameEventManager.Instance.Event.GetLevelEvent(level);
			if (OnLevelUpAction != null)
			{
				OnLevelUpAction();
			}
			if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("new lvl: " + level);
			}
		}
	}
}
