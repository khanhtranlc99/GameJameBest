using UnityEngine;

namespace Game.Vehicle
{
	public class OnCollisionEnterRerouting : MonoBehaviour
	{
		public DrivableVehicle Reciever;

		private void OnCollisionEnter(Collision col)
		{
			Reciever.OnCollisionEnter(col);
		}
	}
}
