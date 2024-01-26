using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PoolEventProcessor
	{
		public delegate void PoolEvent(GameObject poolingObject);

		private readonly IDictionary<GameObject, List<PoolEvent>> beforeReturnToPoolEvent = new Dictionary<GameObject, List<PoolEvent>>();

		public void AddBeforeReturnToPoolEvent(GameObject gameObject, PoolEvent poolEvent)
		{
			List<PoolEvent> list;
			if (!beforeReturnToPoolEvent.ContainsKey(gameObject))
			{
				list = new List<PoolEvent>();
				beforeReturnToPoolEvent.Add(gameObject, list);
			}
			else
			{
				list = beforeReturnToPoolEvent[gameObject];
			}
			if (!list.Contains(poolEvent))
			{
				list.Add(poolEvent);
			}
		}

		public void InvokeBeforeReturnToPoolEvents(GameObject gameObject)
		{
			List<PoolEvent> value;
			beforeReturnToPoolEvent.TryGetValue(gameObject, out value);
			if (value == null || value.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < value.Count; i++)
			{
				PoolEvent poolEvent = value[value.Count - 1 - i];
				if (poolEvent != null)
				{
					try
					{
						poolEvent(gameObject);
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogError(ex.Message + "\n" + ex.StackTrace);
					}
				}
			}
			value.Clear();
		}
	}
}
