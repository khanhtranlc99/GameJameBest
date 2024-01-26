using Game.Character.Stats;
using System;
using UnityEngine;

namespace Game.Character
{
	public class OnLevelUpEffectEnabler : MonoBehaviour
	{
		public GameObject Effect;

		private void Awake()
		{
			Effect.SetActive(value: false);
			LevelManager instance = LevelManager.Instance;
			if(instance!=null)
				instance.OnLevelUpAction = (Action)Delegate.Combine(instance.OnLevelUpAction, new Action(Activate));
		}

		private void Activate()
		{
			if (!PlayerInteractionsManager.Instance.inVehicle)
			{
				Effect.SetActive(value: true);
			}
		}

		private void OnDestroy()
		{
			LevelManager instance = LevelManager.Instance;
			instance.OnLevelUpAction = (Action)Delegate.Remove(instance.OnLevelUpAction, new Action(Activate));
		}
	}
}
