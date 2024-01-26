using UnityEngine;

namespace Game.GlobalComponent
{
	public class ControlableObjectRespawner : ObjectRespawner
	{
		public void SetNewObject(GameObject newObject, RespawnedObjectType newObjectType = RespawnedObjectType.None, bool instantReturn = true)
		{
			if (!(newObject == null))
			{
				if (controlledObject != null && instantReturn)
				{
					PoolManager.Instance.ReturnToPool(controlledObject);
				}
				ObjectPrefab = newObject;
				if (newObjectType != RespawnedObjectType.None)
				{
					ObjectType = newObjectType;
				}
				lostControllTime = Time.time - RespawnTime * 2f;
				if (base.isActiveAndEnabled)
				{
					OnEnable();
				}
			}
		}

		public GameObject GetControlledObject()
		{
			return controlledObject;
		}
	}
}
