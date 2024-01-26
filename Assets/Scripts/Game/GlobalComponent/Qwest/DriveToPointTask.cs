using Game.Character;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class DriveToPointTask : BaseTask
	{
		public Vector3 PointPosition;

		public VehicleList SpecificVehicleName;

		public float PointRadius;

		public VehicleType VehicleType;

		private QwestVehiclePoint point;

		public override void TaskStart()
		{
			if (!PlayerInteractionsManager.Instance.IsDrivingAVehicle())
			{
				CurrentQwest.MoveToTask(PrevTask);
				return;
			}
			base.TaskStart();
			point = PoolManager.Instance.GetFromPool(GameEventManager.Instance.QwestVehiclePointPrefab);
			PoolManager.Instance.AddBeforeReturnEvent(point, delegate
			{
				Target = null;
				if (point != null)
				{
					point.Task = null;
					point = null;
				}
			});
			point.SetRadius(PointRadius);
			point.Task = this;
			point.transform.parent = GameEventManager.Instance.transform;
			point.transform.position = PointPosition;
			Target = point.transform;
		}

		public override void Finished()
		{
			if (point != null)
			{
				PoolManager.Instance.ReturnToPool(point);
			}
			base.Finished();
		}

		public override void PointReachedByVehicleEvent(Vector3 position, BaseTask task, DrivableVehicle vehicle)
		{
			if (Equals(task) && (SpecificVehicleName.Equals(VehicleList.None) || vehicle.VehicleSpecificPrefab.Name.Equals(SpecificVehicleName)) && (VehicleType == VehicleType.Any || vehicle.GetVehicleType().Equals(VehicleType)))
			{
				PoolManager.Instance.ReturnToPool(point);
				CurrentQwest.MoveToTask(NextTask);
			}
		}

		public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
			CurrentQwest.MoveToTask(PrevTask);
		}

		public override void GetOutVehicleEvent(DrivableVehicle vehicle)
		{
			CurrentQwest.MoveToTask(PrevTask);
		}
	}
}
