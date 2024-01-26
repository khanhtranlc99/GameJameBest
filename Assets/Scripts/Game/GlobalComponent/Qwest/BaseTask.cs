using Game.Character.CharacterController;
using Game.DialogSystem;
using Game.Items;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class BaseTask : IEvent
	{
		public string TaskText;

		public BaseTask NextTask;

		public BaseTask PrevTask;

		public float AdditionalTimer;

		public string DialogData;

		public string EndDialogData;

		protected Transform Target;

		protected Qwest CurrentQwest;

		public virtual void TaskStart()
		{
			StartDialog(DialogData);
		}

		public virtual void Init(Qwest qwest)
		{
			CurrentQwest = qwest;
		}

		public virtual void Cancel()
		{
			Target = null;
		}

		public virtual void Finished()
		{
			StartDialog(EndDialogData);
			Target = null;
		}

		public virtual void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
		}

		public virtual void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
		{
		}

		public virtual void PickedQwestItemEvent(Vector3 position, QwestPickupType pickupType, BaseTask relatedTask)
		{
		}

		public virtual void PointReachedEvent(Vector3 position, BaseTask task)
		{
		}

		public virtual void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle)
		{
		}

		public virtual void GetIntoVehicleEvent(DrivableVehicle vehicle)
		{
		}

		public virtual void GetOutVehicleEvent(DrivableVehicle vehicle)
		{
		}

		public virtual void PickUpCollectionEvent(string collectionName)
		{
		}

		public virtual void GetLevelEvent(int level)
		{
		}

		public virtual void GetShopEvent()
		{
		}

		public virtual void VehicleDrawingEvent(DrivableVehicle vehicle)
		{
		}

		public virtual void BuyItemEvent(GameItem item)
		{
		}

		public virtual string TaskStatus()
		{
			return TaskText;
		}

		public virtual Transform TaskTarget()
		{
			return Target;
		}

		protected void StartDialog(string dialog)
		{
			if (!string.IsNullOrEmpty(dialog) && dialog.Length > Qwest.MinCharDialogLength)
			{
				DialogManager.Instance.StartDialog(dialog);
			}
		}
	}
}
