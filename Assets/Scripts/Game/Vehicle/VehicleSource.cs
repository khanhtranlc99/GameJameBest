using UnityEngine;

namespace Game.Vehicle
{
	public class VehicleSource : MonoBehaviour
	{
		private BoxCollider sourceCollider;

		private DrivableVehicle owner;

		public BoxCollider SourceCollider
		{
			get
			{
				if (sourceCollider == null)
				{
					sourceCollider = GetComponent<BoxCollider>();
				}
				return sourceCollider;
			}
		}

		public DrivableVehicle RootVehicle
		{
			get
			{
				if (owner == null)
				{
					owner = GetComponentInParent<DrivableVehicle>();
				}
				return owner;
			}
			set
			{
				owner = value;
			}
		}
	}
}
