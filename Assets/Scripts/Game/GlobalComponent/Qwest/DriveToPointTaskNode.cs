using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class DriveToPointTaskNode : TaskNode
	{
		[Header("Will use its own position if null")]
		[Space]
		[Separator("Specific")]
		public Transform PointPosition;

		[Header("Use null for any specific")]
		public VehicleList VehicleName;

		public float PointRadius = 2f;

		public VehicleType VehicleType = VehicleType.Any;

		public override BaseTask ToPo()
		{
			DriveToPointTask driveToPointTask = new DriveToPointTask();
			driveToPointTask.SpecificVehicleName = VehicleName;
			driveToPointTask.VehicleType = VehicleType;
			driveToPointTask.PointRadius = PointRadius;
			driveToPointTask.PointPosition = ((!(PointPosition == null)) ? PointPosition.position : base.transform.position);
			DriveToPointTask driveToPointTask2 = driveToPointTask;
			ToPoBase(driveToPointTask2);
			return driveToPointTask2;
		}

		private void OnDrawGizmos()
		{
			if (IsDebug)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere((!(PointPosition == null)) ? PointPosition.position : base.transform.position, PointRadius);
			}
		}
	}
}
