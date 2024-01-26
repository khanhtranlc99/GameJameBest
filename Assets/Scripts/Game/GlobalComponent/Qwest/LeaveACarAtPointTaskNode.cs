using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class LeaveACarAtPointTaskNode : DriveToPointTaskNode
	{
		[TextArea]
		public string AtPointDialog;

		public float LeaveRange = 10f;

		public override BaseTask ToPo()
		{
			LeaveACarAtPointTask leaveACarAtPointTask = new LeaveACarAtPointTask();
			leaveACarAtPointTask.SpecificVehicleName = VehicleName;
			leaveACarAtPointTask.VehicleType = VehicleType;
			leaveACarAtPointTask.PointRadius = PointRadius;
			leaveACarAtPointTask.Range = LeaveRange;
			leaveACarAtPointTask.AtPointDialog = AtPointDialog;
			leaveACarAtPointTask.PointPosition = ((!(PointPosition == null)) ? PointPosition.position : base.transform.position);
			LeaveACarAtPointTask leaveACarAtPointTask2 = leaveACarAtPointTask;
			ToPoBase(leaveACarAtPointTask2);
			return leaveACarAtPointTask2;
		}
	}
}
