using Game.Items;
using Game.Shop;

namespace Game.Character.Superpowers
{
	public static class PlayerAbilityManager
	{
		public static GameItemAbility[] ActiveAbilities = new GameItemAbility[3];

		public static GameItemAbility[] PasiveAbilities = new GameItemAbility[5];

		public static void EnableEbilities()
		{
			GameItemAbility[] activeAbilities = ActiveAbilities;
			foreach (GameItemAbility gameItemAbility in activeAbilities)
			{
				if (gameItemAbility != null)
				{
					gameItemAbility.Enable();
				}
			}
			GameItemAbility[] pasiveAbilities = PasiveAbilities;
			foreach (GameItemAbility gameItemAbility2 in pasiveAbilities)
			{
				if (gameItemAbility2 != null)
				{
					gameItemAbility2.Enable();
				}
			}
		}

		public static int GetSlotIndex(GameItemAbility ability)
		{
			int result = -1;
			if (ability.IsActive)
			{
				for (int i = 0; i < ActiveAbilities.Length; i++)
				{
					GameItemAbility gameItemAbility = ActiveAbilities[i];
					if (!(gameItemAbility == null) && gameItemAbility.ID == ability.ID)
					{
						result = i;
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < PasiveAbilities.Length; j++)
				{
					GameItemAbility gameItemAbility2 = PasiveAbilities[j];
					if (!(gameItemAbility2 == null) && gameItemAbility2.ID == ability.ID)
					{
						result = j;
						break;
					}
				}
			}
			return result;
		}

		public static void SaveAbilities()
		{
			for (int i = 0; i < ActiveAbilities.Length; i++)
			{
				if (ActiveAbilities[i] != null)
				{
					PlayerStoreProfile.CurrentLoadout.ActiveSuperpowers[i] = ActiveAbilities[i].ID;
				}
				else
				{
					PlayerStoreProfile.CurrentLoadout.ActiveSuperpowers[i] = 0;
				}
			}
			for (int j = 0; j < PasiveAbilities.Length; j++)
			{
				if (PasiveAbilities[j] != null)
				{
					PlayerStoreProfile.CurrentLoadout.PasiveSuperpowers[j] = PasiveAbilities[j].ID;
				}
				else
				{
					PlayerStoreProfile.CurrentLoadout.PasiveSuperpowers[j] = 0;
				}
			}
			PlayerStoreProfile.SaveLoadout();
		}

		public static void LoadAbilities()
		{
			int[] activeSuperpowers = PlayerStoreProfile.CurrentLoadout.ActiveSuperpowers;
			int[] pasiveSuperpowers = PlayerStoreProfile.CurrentLoadout.PasiveSuperpowers;
			for (int i = 0; i < activeSuperpowers.Length; i++)
			{
				if (activeSuperpowers[i] != 0)
				{
					ActiveAbilities[i] = (GameItemAbility)ItemsManager.Instance.GetItem(activeSuperpowers[i]);
				}
			}
			for (int j = 0; j < pasiveSuperpowers.Length; j++)
			{
				if (pasiveSuperpowers[j] != 0)
				{
					PasiveAbilities[j] = (GameItemAbility)ItemsManager.Instance.GetItem(pasiveSuperpowers[j]);
				}
			}
		}

		public static bool IsAbilityAdded(GameItemAbility ability)
		{
			if (ability.IsActive)
			{
				GameItemAbility[] activeAbilities = ActiveAbilities;
				foreach (GameItemAbility gameItemAbility in activeAbilities)
				{
					if ((bool)gameItemAbility && gameItemAbility.ID == ability.ID)
					{
						return true;
					}
				}
			}
			if (!ability.IsActive)
			{
				GameItemAbility[] pasiveAbilities = PasiveAbilities;
				foreach (GameItemAbility gameItemAbility2 in pasiveAbilities)
				{
					if ((bool)gameItemAbility2 && gameItemAbility2.ID == ability.ID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void AddAbility(GameItemAbility ability, int slotNumber)
		{
			GameItemAbility gameItemAbility = null;
			if (ability.IsActive)
			{
				gameItemAbility = ActiveAbilities[slotNumber];
				ActiveAbilities[slotNumber] = ability;
			}
			else
			{
				gameItemAbility = PasiveAbilities[slotNumber];
				PasiveAbilities[slotNumber] = ability;
			}
			if (gameItemAbility != null)
			{
				gameItemAbility.Disable();
			}
			ability.Enable();
			SaveAbilities();
		}

		public static void RemoveAbility(GameItemAbility ability)
		{
			if (ability.IsActive)
			{
				for (int i = 0; i < ActiveAbilities.Length; i++)
				{
					GameItemAbility gameItemAbility = ActiveAbilities[i];
					if (!(gameItemAbility == null) && gameItemAbility.ID == ability.ID)
					{
						ActiveAbilities.SetValue(null, i);
						ability.Disable();
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < PasiveAbilities.Length; j++)
				{
					GameItemAbility gameItemAbility2 = PasiveAbilities[j];
					if (!(gameItemAbility2 == null) && gameItemAbility2.ID == ability.ID)
					{
						PasiveAbilities.SetValue(null, j);
						ability.Disable();
						break;
					}
				}
			}
			SaveAbilities();
		}

		public static bool SkinCanBeRemoved(GameItemSkin skin, GameItemAbility ability)
		{
			for (int i = 0; i < ActiveAbilities.Length; i++)
			{
				if (ActiveAbilities[i] == null || ActiveAbilities[i] == ability || ActiveAbilities[i].RelatedSkins.Length <= 0)
				{
					continue;
				}
				for (int j = 0; j < ActiveAbilities[i].RelatedSkins.Length; j++)
				{
					if (ActiveAbilities[i].RelatedSkins[j] == skin)
					{
						return false;
					}
				}
			}
			for (int k = 0; k < PasiveAbilities.Length; k++)
			{
				if (PasiveAbilities[k] == null || PasiveAbilities[k] == ability || PasiveAbilities[k].RelatedSkins.Length <= 0)
				{
					continue;
				}
				for (int l = 0; l < PasiveAbilities[k].RelatedSkins.Length; l++)
				{
					if (PasiveAbilities[k].RelatedSkins[l] == skin)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
