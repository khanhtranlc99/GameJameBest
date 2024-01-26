using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(StashManager))]
	public class StashHelper : MonoBehaviour
	{
		public void UpdateObjects()
		{
			GetComponent<StashManager>().UpdateObjects();
		}
	}
}
