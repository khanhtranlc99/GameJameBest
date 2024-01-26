using Game.GlobalComponent;
using System;
using System.Collections.Generic;
using RopeHeroViceCity.Scripts.Gampelay;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.CharacterController
{
	public class FactionsManager : MonoBehaviour
	{
		private const string PlayerRelationsFactionsArrayName = "PlayerRelationsFactions";

		private const string PlayerRelationsValuesArrayName = "PlayerRelationsValues";

		private const float PolliceAttentionTime = 20f;

		private const float PoliceRelationWarmingValue = 0.5f;

		private static FactionsManager instance;

		public List<PlayerRelations> PlayerDefaultRelations = new List<PlayerRelations>();

		public List<NpcRelations> NpcRelationsList = new List<NpcRelations>();



		private List<PlayerRelations> playerRelationsList = new List<PlayerRelations>();

		private readonly List<HitEntity> victimsList = new List<HitEntity>();

		private SlowUpdateProc slowUpdateProc;

		private float lastCrimeTime;

		private readonly List<Faction> ignoredFactionsToSave = new List<Faction>
		{
			Faction.Police,
			Faction.Civilian
		};

		public static FactionsManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("FactionsManager is not initialized");
				}
				return instance;
			}
		}

		public static void ClearPlayerRelations()
		{
			BaseProfile.ClearArray<string>("PlayerRelationsFactions");
			BaseProfile.ClearArray<string>("PlayerRelationsValues");
		}

		public void SavePlayerRelations()
		{
			List<int> list = new List<int>();
			List<float> list2 = new List<float>();
			foreach (PlayerRelations playerRelations in playerRelationsList)
			{
				if (!ignoredFactionsToSave.Contains(playerRelations.NpcFaction))
				{
					list.Add((int)playerRelations.NpcFaction);
					list2.Add(playerRelations.RelationValue);
				}
			}
			BaseProfile.StoreArray(list.ToArray(), "PlayerRelationsFactions");
			BaseProfile.StoreArray(list2.ToArray(), "PlayerRelationsValues");
		}

		public Relations GetRelations(Faction entityFaction, Faction targetFaction)
		{
			if (entityFaction == Faction.NoneFaction || targetFaction == Faction.NoneFaction)
			{
				return Relations.Neutral;
			}
			if (entityFaction == targetFaction)
			{
				return Relations.Friendly;
			}
			if (entityFaction == Faction.Player)
			{
				return GetPlayerRelations(targetFaction);
			}
			if (targetFaction == Faction.Player)
			{
				return GetPlayerRelations(entityFaction);
			}
			return GetNpcRelations(entityFaction, targetFaction);
		}

		public void ChangePlayerRelations(Faction faction, float value)
		{
			FindPlayerRelations(faction).ChangeRelationValue(value);
			if (faction == Faction.Police)
			{
				UpdateCopRelationSlider();
			}
		}

		public void ChangePlayerRelations(Faction faction, Relations newRelations)
		{
			FindPlayerRelations(faction).CurrentRelations = newRelations;
			if (faction == Faction.Police)
			{
				UpdateCopRelationSlider();
			}
		}

		public void ChangeFriendlyFactionsRelation(Faction rootFaction, float amount)
		{
			foreach (NpcRelations npcRelations in NpcRelationsList)
			{
				if (npcRelations.TheirRelations == Relations.Friendly && npcRelations.FirstFaction != npcRelations.SecondFaction)
				{
					Faction faction = Faction.NoneFaction;
					if (npcRelations.FirstFaction == rootFaction)
					{
						faction = npcRelations.SecondFaction;
					}
					else if (npcRelations.SecondFaction == rootFaction)
					{
						faction = npcRelations.FirstFaction;
					}
					if (faction != 0)
					{
						ChangePlayerRelations(faction, amount);
					}
				}
			}
		}

		public void ChangeFriendlyFactionsRelation(Faction rootFaction, Relations newRelations)
		{
			foreach (NpcRelations npcRelations in NpcRelationsList)
			{
				if (npcRelations.TheirRelations == Relations.Friendly && npcRelations.FirstFaction != npcRelations.SecondFaction)
				{
					Faction faction = Faction.NoneFaction;
					if (npcRelations.FirstFaction == rootFaction)
					{
						faction = npcRelations.SecondFaction;
					}
					else if (npcRelations.SecondFaction == rootFaction)
					{
						faction = npcRelations.FirstFaction;
					}
					if (faction != 0)
					{
						ChangePlayerRelations(faction, newRelations);
					}
				}
			}
		}

		public void CommitedACrime()
		{
			if (GetPlayerRelations(Faction.Police) != Relations.Hostile)
			{
				ChangePlayerRelations(Faction.Police, -1f);
				ChangeFriendlyFactionsRelation(Faction.Police, -1f);
			}
			lastCrimeTime = Time.time;
		}

		public void PlayerAttackHuman(HitEntity victim)
		{
			if (!victimsList.Contains(victim) && GetNpcRelations(victim.Faction, Faction.Police) == Relations.Friendly)
			{
				victimsList.Add(victim);
				CommitedACrime();
				PoolManager.Instance.AddBeforeReturnEvent(victim, delegate
				{
					victimsList.Remove(victim);
				});
			}
		}

		public Relations GetPlayerRelations(Faction faction)
		{
			return FindPlayerRelations(faction).CurrentRelations;
		}

		public float GetPlayerRelationNormalized(Faction faction)
		{
			float playerRelationsValue = GetPlayerRelationsValue(faction);
			if (playerRelationsValue >= 0f)
			{
				return playerRelationsValue / 10f;
			}
			return (0f - playerRelationsValue) / -10f;
		}

		public float GetPlayerRelationsValue(Faction faction)
		{
			return FindPlayerRelations(faction).RelationValue;
		}

		private void Awake()
		{
			instance = this;

			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 1f);
			LoadPlayerRelations();
			LoadDefaultFactionsValues();
		}

		private void SlowUpdate()
		{
			if (Time.time > lastCrimeTime + 20f && GetPlayerRelationsValue(Faction.Police) < 0f)
			{
				ChangePlayerRelations(Faction.Police, 0.5f);
				ChangeFriendlyFactionsRelation(Faction.Police, 0.5f);
			}
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void LoadPlayerRelations()
		{
			int[] array = BaseProfile.ResolveArray<int>("PlayerRelationsFactions");
			float[] array2 = BaseProfile.ResolveArray<float>("PlayerRelationsValues");
			if (array == null || array.Length == 0)
			{
				return;
			}
			playerRelationsList.Clear();
			for (int i = 0; i < array.Length; i++)
			{
				Faction item = (Faction)array[i];
				if (!ignoredFactionsToSave.Contains(item))
				{
					playerRelationsList.Add(new PlayerRelations
					{
						NpcFaction = (Faction)array[i],
						RelationValue = array2[i]
					});
				}
			}
		}

		private void LoadDefaultFactionsValues()
		{
			foreach (PlayerRelations playerDefaultRelation in PlayerDefaultRelations)
			{
				PlayerRelations playerRelations = null;
				foreach (PlayerRelations playerRelations2 in playerRelationsList)
				{
					if (playerRelations2.NpcFaction == playerDefaultRelation.NpcFaction)
					{
						playerRelations = playerRelations2;
						break;
					}
				}
				if (playerRelations == null)
				{
					playerRelationsList.Add(playerDefaultRelation);
				}
			}
		}

		private void UpdateCopRelationSlider()
		{
			EventManager.Instance.UpdatePlayerFactionValue(GetPlayerRelationsValue(Faction.Police));
		}

		private PlayerRelations FindPlayerRelations(Faction faction)
		{
			PlayerRelations playerRelations = null;
			for (int i = 0; i < playerRelationsList.Count; i++)
			{
				PlayerRelations playerRelations2 = playerRelationsList[i];
				if (playerRelations2.NpcFaction == faction)
				{
					playerRelations = playerRelations2;
					break;
				}
			}
			if (playerRelations == null)
			{
				PlayerRelations playerRelations3 = new PlayerRelations();
				playerRelations3.NpcFaction = faction;
				playerRelations = playerRelations3;
				playerRelationsList.Add(playerRelations);
			}
			return playerRelations;
		}

		private Relations GetNpcRelations(Faction firstFac, Faction secondFac)
		{
			NpcRelations npcRelations = null;
			for (int i = 0; i < NpcRelationsList.Count; i++)
			{
				NpcRelations npcRelations2 = NpcRelationsList[i];
				if ((npcRelations2.FirstFaction == firstFac && npcRelations2.SecondFaction == secondFac) || (npcRelations2.FirstFaction == secondFac && npcRelations2.SecondFaction == firstFac))
				{
					npcRelations = npcRelations2;
					break;
				}
			}
			if (npcRelations == null)
			{
				NpcRelations npcRelations3 = new NpcRelations();
				npcRelations3.FirstFaction = firstFac;
				npcRelations3.SecondFaction = secondFac;
				npcRelations = npcRelations3;
				NpcRelationsList.Add(npcRelations);
			}
			return npcRelations.TheirRelations;
		}
	}
}
