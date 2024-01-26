using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class StealAVehicleTaskNode : TaskNode
	{
		[Header("Use null for any specific")]
		[Separator("Specific")]
		[Space]
		public VehicleList VehicleName;

		public VehicleType VehicleType = VehicleType.Any;

		public int MarksCount;

		[SelectiveString("MarkType")]
		public string MarksType;

		public override BaseTask ToPo()
		{
			StealAVehicleTask stealAVehicleTask = new StealAVehicleTask();
			stealAVehicleTask.VehicleType = VehicleType;
			stealAVehicleTask.SpecificVehicleName = VehicleName;
			stealAVehicleTask.countVisualMarks = MarksCount;
			stealAVehicleTask.markVisualType = MarksType;
			StealAVehicleTask stealAVehicleTask2 = stealAVehicleTask;
			ToPoBase(stealAVehicleTask2);
			return stealAVehicleTask2;
		}
	}
}
