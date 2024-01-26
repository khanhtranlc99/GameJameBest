using Game.Vehicle;
using System;
using UnityEngine;

namespace Code.Game.Race
{
	[Serializable]
	public class Racer : IComparable
	{
		[SerializeField]
		private DrivableVehicle drivableVehicle;

		[SerializeField]
		private string driverName;

		private int lap;

		private int waypointIndex;

		private float distanceToPoint;

		private IComparable comparableImplementation;

		public Racer(DrivableVehicle drivableVehicle, string driverName)
		{
			this.drivableVehicle = drivableVehicle;
			this.driverName = driverName;
		}

		public void SetLap(int lap)
		{
			this.lap = lap;
		}

		public int GetLap()
		{
			return lap;
		}

		public void SetWaypointIndex(int waypointIndex)
		{
			this.waypointIndex = waypointIndex;
		}

		public void SetDistanceToPoint(float distanceToPoint)
		{
			this.distanceToPoint = distanceToPoint;
		}

		public DrivableVehicle GetDrivableVehicle()
		{
			return drivableVehicle;
		}

		public string GetRacerName()
		{
			return driverName;
		}

		public int CompareTo(object obj)
		{
			Racer racer = (Racer)obj;
			int num = -lap.CompareTo(racer.lap);
			if (num == 0)
			{
				num = -waypointIndex.CompareTo(racer.waypointIndex);
			}
			if (num == 0)
			{
				num = distanceToPoint.CompareTo(racer.distanceToPoint);
			}
			return num;
		}
	}
}
