using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Character.Extras
{
	internal class EntityManager
	{
		public delegate void PlayerKill(HitEntity enemy);

		private const float CallOverallReloadTime = 2f;

		private static EntityManager instance;

		public PlayerKill PlayerKillEvent;

		private readonly HashSet<HitEntity> enemies;

		private readonly HashSet<HitEntity> players;

		private readonly HashSet<HitEntity> killedEntities;

		private readonly HashSet<RagdollStatus> livingRagdolls;

		private Dictionary<HitEntity, float> overallCallers = new Dictionary<HitEntity, float>();

		public static EntityManager Instance => instance ?? (instance = new EntityManager());

		public HitEntity Player => players.FirstOrDefault((HitEntity player) => player is Human && !((Human)player).Remote && (bool)player);

		private EntityManager()
		{
			enemies = new HashSet<HitEntity>();
			players = new HashSet<HitEntity>();
			killedEntities = new HashSet<HitEntity>();
			livingRagdolls = new HashSet<RagdollStatus>();
		}

		public void SingleAlarm(HitEntity disturber, HitEntity victim)
		{
			if ((bool)victim && (bool)disturber)
			{
				BaseStatusNPC baseStatusNPC = victim as BaseStatusNPC;
				if (baseStatusNPC != null)
				{
					baseStatusNPC.OnStatusAlarm(disturber);
				}
			}
		}

		public void OverallAlarm(HitEntity disturber, HitEntity victim, Vector3 position, float range)
		{
			if (disturber == null)
			{
				return;
			}
			if (!overallCallers.ContainsKey(disturber))
			{
				overallCallers.Add(disturber, Time.time - 2f);
				PoolManager.Instance.AddBeforeReturnEvent(disturber, delegate
				{
					overallCallers.Remove(disturber);
				});
			}
			if (Time.time < overallCallers[disturber] + 2f)
			{
				return;
			}
			BaseStatusNPC[] array = FindAllClosestNPCs(position, range, onPlayerSignlineOnly: true);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != disturber)
				{
					if (array[i].BaseNPC.SpecificNpcLinks.SmartActionType == ActionType.Coward || array[i].Faction == Faction.Police)
					{
						array[i].OnStatusAlarm(disturber);
					}
					if (!(victim == null) && !(array[i] == victim) && FactionsManager.Instance.GetRelations(array[i].Faction, victim.Faction) == Relations.Friendly)
					{
						array[i].OnStatusAlarm(disturber);
					}
				}
			}
			overallCallers[disturber] = Time.time;
		}

		public void Register(HitEntity enemy)
		{
			enemies.Add(enemy);
			killedEntities.Remove(enemy);
		}

		public void RegisterLivingRagdoll(RagdollStatus ragdoll)
		{
			livingRagdolls.Add(ragdoll);
		}

		public void UnregisterRagdoll(RagdollStatus ragdoll)
		{
			livingRagdolls.Remove(ragdoll);
		}

		public void RegisterPlayer(HitEntity player)
		{
			players.Add(player);
		}

		public void OnDeath(HitEntity enemy)
		{
			Player player = PlayerInteractionsManager.Instance.Player;
			if (enemy.LastHitOwner == player && FactionsManager.Instance.GetRelations(player.Faction, enemy.Faction) != 0)
			{
				LevelManager.Instance.AddExperience((int)enemy.ExperienceForAKill, useVIPmult: true);
			}
			if (PlayerKillEvent != null && enemy.LastHitOwner == player)
			{
				PlayerKillEvent(enemy);
			}
			killedEntities.Add(enemy);
			enemies.Remove(enemy);
		}

		public bool EntityWasKilled(HitEntity entity)
		{
			return killedEntities.Contains(entity);
		}

		public HitEntity Find(Vector3 pos, float radius, string ignoreTag)
		{
			float num = radius * radius;
			HitEntity result = null;
			float num2 = float.MaxValue;
			foreach (HitEntity enemy in enemies)
			{
				if ((bool)enemy && !enemy.gameObject.CompareTag(ignoreTag))
				{
					float sqrMagnitude = (pos - enemy.transform.position).sqrMagnitude;
					if (sqrMagnitude < num && sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
						result = enemy;
					}
				}
			}
			return result;
		}

		public BaseStatusNPC[] FindAllClosestNPCs(Vector3 aroundPosition, float radius, bool onPlayerSignlineOnly = false)
		{
			List<BaseStatusNPC> list = new List<BaseStatusNPC>();
			foreach (HitEntity enemy in enemies)
			{
				BaseStatusNPC baseStatusNPC = enemy as BaseStatusNPC;
				if (baseStatusNPC != null && baseStatusNPC.gameObject.activeSelf && baseStatusNPC.Faction != Faction.Player && Vector3.Distance(aroundPosition, baseStatusNPC.transform.position) <= radius)
				{
					if (!onPlayerSignlineOnly)
					{
						list.Add(baseStatusNPC);
					}
					else if (PlayerManager.Instance.OnPlayerSignline(baseStatusNPC, radius))
					{
						list.Add(baseStatusNPC);
					}
				}
			}
			return list.ToArray();
		}

		public void ReturnToPoolAllEnemiesAroundPoint(Vector3 point, float radius)
		{
			foreach (HitEntity enemy in enemies)
			{
				if (!(enemy == null) && enemy.gameObject.activeSelf && !(Vector3.Distance(enemy.transform.position, point) > radius))
				{
					BaseStatusNPC baseStatusNPC = enemy as BaseStatusNPC;
					if (baseStatusNPC != null)
					{
						PoolManager.Instance.ReturnToPool(baseStatusNPC);
					}
					else
					{
						VehicleStatus vehicleStatus = enemy as VehicleStatus;
						if (vehicleStatus != null)
						{
							PoolManager.Instance.ReturnToPool(vehicleStatus.transform.parent.gameObject);
						}
					}
				}
			}
			ReturnAllLivingRagdollsAroundPoint(point, radius);
		}

		public void ReturnAllLivingRagdollsAroundPoint(Vector3 point, float radius)
		{
			foreach (RagdollStatus livingRagdoll in livingRagdolls)
			{
				if (!(livingRagdoll == null) && (!livingRagdoll.gameObject.activeSelf || Vector3.Distance(livingRagdoll.transform.position, point) > radius) && livingRagdoll.gameObject.activeInHierarchy)
				{
					livingRagdoll.StartCoroutine(DeInitRagdoll(livingRagdoll));
				}
			}
		}

		private IEnumerator DeInitRagdoll(RagdollStatus ragdoll)
		{
			yield return new WaitForSeconds(0.2f);
			ragdoll.wakeUper.DeInitRagdoll(mainObjectDead: true);
		}
	}
}
