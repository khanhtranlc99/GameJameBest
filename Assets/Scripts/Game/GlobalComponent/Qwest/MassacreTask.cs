using Game.Character.CharacterController;
using Game.Character.Extras;
using Game.Items;
using Game.Traffic;
using Game.Vehicle;
using Game.Weapons;
using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class MassacreTask : BaseTask
	{
		public int WeaponItemID;

		public int RequiredVictimsCount = 50;

		private int currVictimsCount;

		private int defaultWeaponID;

		public string MarksTypeNPC;

		public int MarksCount;

		private GameObject searchGo;

		public override void TaskStart()
		{
			if (PlayerManager.Instance.PlayerIsDefault)
			{
				base.TaskStart();
				GameEventManager.Instance.MassacreTaskActive = true;
				GameItemWeapon gameItemWeapon = ItemsManager.Instance.GetItem(WeaponItemID) as GameItemWeapon;
				if (gameItemWeapon != null)
				{
					WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
					int slotIndex = defaultWeaponController.WeaponSet.FirstSlotOfType(defaultWeaponController.GetTargetSlot(gameItemWeapon.Weapon));
					defaultWeaponID = defaultWeaponController.EquipWeapon(gameItemWeapon, slotIndex, temporary: true);
					defaultWeaponController.ChooseSlot(slotIndex);
					defaultWeaponController.LockWeponSet();
				}
				EntityManager instance = EntityManager.Instance;
				instance.PlayerKillEvent = (EntityManager.PlayerKill)Delegate.Combine(instance.PlayerKillEvent, new EntityManager.PlayerKill(CheckForComplite));
				searchGo = new GameObject
				{
					name = "SearchProcessing_" + GetType().Name
				};
				SearchProcess<HitEntity> searchProcess = new SearchProcess<HitEntity>();
				searchProcess.countMarks = MarksCount;
				searchProcess.markType = MarksTypeNPC;
				SearchProcess<HitEntity> process = searchProcess;
				SearchProcessing searchProcessing = searchGo.AddComponent<SearchProcessing>();
				searchProcessing.process = process;
				searchProcessing.Init();
			}
		}

		public void CheckForComplite(HitEntity enemy)
		{
			if (!(enemy is VehicleStatus))
			{
				currVictimsCount++;
				if (currVictimsCount >= RequiredVictimsCount)
				{
					CurrentQwest.MoveToTask(NextTask);
					FactionsManager.Instance.ChangePlayerRelations(Faction.Police, 0f - FactionsManager.Instance.GetPlayerRelationsValue(Faction.Police));
					TrafficManager.Instance.CalmDownCops();
					EntityManager.Instance.ReturnAllLivingRagdollsAroundPoint(PlayerManager.Instance.Player.transform.position, 100f);
				}
			}
		}

		public override void Finished()
		{
			base.Finished();
			GameEventManager.Instance.MassacreTaskActive = false;
			GameItemWeapon gameItemWeapon = ItemsManager.Instance.GetItem(WeaponItemID) as GameItemWeapon;
			if (gameItemWeapon != null)
			{
				WeaponController defaultWeaponController = PlayerManager.Instance.DefaultWeaponController;
				defaultWeaponController.UnlockWeponSet();
				int slotIndex = defaultWeaponController.WeaponSet.FirstSlotOfType(defaultWeaponController.GetTargetSlot(gameItemWeapon.Weapon));
				defaultWeaponController.UnEquipWeapon(slotIndex);
				GameItemWeapon gameItemWeapon2 = ItemsManager.Instance.GetItem(defaultWeaponID) as GameItemWeapon;
				if (gameItemWeapon2 != null)
				{
					defaultWeaponController.EquipWeapon(gameItemWeapon2, slotIndex);
				}
			}
			currVictimsCount = 0;
			EntityManager instance = EntityManager.Instance;
			instance.PlayerKillEvent = (EntityManager.PlayerKill)Delegate.Remove(instance.PlayerKillEvent, new EntityManager.PlayerKill(CheckForComplite));
			if ((bool)searchGo)
			{
				UnityEngine.Object.Destroy(searchGo);
			}
		}

		public override string TaskStatus()
		{
			return base.TaskStatus() + "\nKilled  " + currVictimsCount + "/" + RequiredVictimsCount;
		}

		private bool CheckConditionNPC(HitEntity npc)
		{
			return !(npc is VehicleStatus);
		}
	}
}
