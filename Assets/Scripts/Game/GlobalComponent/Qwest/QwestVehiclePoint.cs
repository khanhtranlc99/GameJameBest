using Game.Character;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestVehiclePoint : MonoBehaviour
	{
		public BaseTask Task;

		public void SetRadius(float triggerRadius)
		{
			SphereCollider component = GetComponent<SphereCollider>();
			if (component != null)
			{
				component.radius = triggerRadius;
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			if ((bool)col.GetComponent<CharacterSource>() && PlayerInteractionsManager.Instance.IsDrivingAVehicle())
			{
				GameEventManager.Instance.Event.PointReachedByVehicleEvent(base.transform.position, Task, PlayerInteractionsManager.Instance.LastDrivableVehicle);
			}
		}
	}
}
