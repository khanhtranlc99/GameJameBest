using Game.Character;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestPoint : MonoBehaviour
	{
		public BaseTask Task;

		private void OnTriggerEnter(Collider col)
		{
			if ((bool)col.GetComponent<CharacterSensor>())
			{
				GameEventManager.Instance.Event.PointReachedEvent(base.transform.position, Task);
				Task = null;
				PoolManager.Instance.ReturnToPool(this);
			}
		}
	}
}
