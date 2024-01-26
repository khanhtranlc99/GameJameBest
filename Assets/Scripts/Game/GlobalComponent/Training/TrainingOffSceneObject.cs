using System.Collections;
using UnityEngine;

namespace Game.GlobalComponent.Training
{
	public class TrainingOffSceneObject : TrainingBase
	{
		private IEnumerator Start()
		{
			TrainingManager.Instance.InitOffSceneTraining(this);
			yield return new WaitForSecondsRealtime(0.5f);
			if (ObjectForActive.gameObject.activeInHierarchy)
			{
				ObjectForActive.GetComponent<TrainingObjectTracker>().TrackedObjectActivate();
			}
		}
	}
}
