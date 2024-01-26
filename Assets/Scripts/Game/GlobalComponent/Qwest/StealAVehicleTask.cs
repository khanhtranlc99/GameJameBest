using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class StealAVehicleTask : BaseTask
	{
		public VehicleList SpecificVehicleName;

		public VehicleType VehicleType;

		public int countVisualMarks;

		public string markVisualType;

		private GameObject searchGo;

		public override void TaskStart()
		{
			searchGo = new GameObject
			{
				name = "SearchProcessing_" + GetType().Name
			};
			SearchProcess<DrivableVehicle> searchProcess = new SearchProcess<DrivableVehicle>(CheckCondition);
			searchProcess.countMarks = countVisualMarks;
			searchProcess.markType = markVisualType;
			SearchProcess<DrivableVehicle> process = searchProcess;
			SearchProcessing searchProcessing = searchGo.AddComponent<SearchProcessing>();
			searchProcessing.process = process;
			searchProcessing.Init();
			base.TaskStart();
		}

		public override void Finished()
		{
			if ((bool)searchGo)
			{
				UnityEngine.Object.Destroy(searchGo);
			}
			base.Finished();
		}

		public override void GetIntoVehicleEvent(DrivableVehicle vehicle)
		{
			if (CheckCondition(vehicle))
			{
				CurrentQwest.MoveToTask(NextTask);
			}
		}

		private bool CheckCondition(DrivableVehicle vehicle)
		{
			return (SpecificVehicleName.Equals(VehicleList.None) || SpecificVehicleName.Equals(vehicle.VehicleSpecificPrefab.Name)) && (VehicleType == VehicleType.Any || vehicle.GetVehicleType().Equals(VehicleType));
		}
	}
}
