using Game.Character;
using Game.Vehicle;
using System.Collections;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class LeaveACarAtPointTask : BaseTask
	{
		public Vector3 PointPosition;

		public VehicleList SpecificVehicleName;

		public VehicleType VehicleType;

		public float Range;

		public float PointRadius;

		public string AtPointDialog;

		private QwestVehiclePoint point;

		public override void TaskStart()
		{
			base.TaskStart();
			if (!PlayerInteractionsManager.Instance.IsDrivingAVehicle())
			{
				CurrentQwest.MoveToTask(PrevTask);
			}
			point = PoolManager.Instance.GetFromPool<QwestVehiclePoint>(GameEventManager.Instance.QwestVehiclePointPrefab.gameObject);
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
			if (Equals(task) && (SpecificVehicleName.Equals(VehicleList.None) || vehicle.VehicleSpecificPrefab.Name.Equals(SpecificVehicleName)))
			{
				StartDialog(AtPointDialog);
			}
		}

		public override void PlayerDeadEvent(SuicideAchievment.DethType i = SuicideAchievment.DethType.None)
		{
			CurrentQwest.MoveToTask(PrevTask);
		}

		public override void GetOutVehicleEvent(DrivableVehicle vehicle)
		{
			if (Vector3.Distance(vehicle.MainRigidbody.position, PointPosition) < Range && (SpecificVehicleName.Equals(VehicleList.None) || vehicle.VehicleSpecificPrefab.Name.Equals(SpecificVehicleName)) && (VehicleType == VehicleType.Any || vehicle.GetVehicleType().Equals(VehicleType)))
			{
				if (PlayerInteractionsManager.Instance != null)
				{
					PlayerInteractionsManager.Instance.StartCoroutine(ReturnVehicleToPool(vehicle));
				}
				else if (!PlayerInteractionsManager.Instance.Player.IsTransformer && !PoolManager.Instance.ReturnToPool(vehicle.gameObject))
				{
					UnityEngine.Object.Destroy(vehicle.gameObject);
				}
				CurrentQwest.MoveToTask(NextTask);
			}
			else
			{
				CurrentQwest.MoveToTask(PrevTask);
			}
		}

		private IEnumerator ReturnVehicleToPool(DrivableVehicle vehicle)
		{
			while (PlayerInteractionsManager.Instance.Player.transform.parent == vehicle.transform)
			{
				yield return null;
			}
			if (!PlayerInteractionsManager.Instance.Player.IsTransformer && !PoolManager.Instance.ReturnToPool(vehicle.gameObject))
			{
				UnityEngine.Object.Destroy(vehicle.gameObject);
			}
			yield return null;
		}
	}
}
