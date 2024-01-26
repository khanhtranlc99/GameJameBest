using Code.Managers.QuestsManager.Tasks;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Code.Managers.QuestsManager.QwestMaker
{
	public class RaceTaskNode : TaskNode
	{
		[SerializeField]
		private int raceNumber;

		public override BaseTask ToPo()
		{
			RaceTask raceTask = new RaceTask();
			raceTask.RaceNumber = raceNumber;
			RaceTask raceTask2 = raceTask;
			ToPoBase(raceTask2);
			return raceTask2;
		}

		private void OnDrawGizmos()
		{
			if (IsDebug)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
				Gizmos.DrawSphere(base.transform.position, 2f);
			}
		}
	}
}
