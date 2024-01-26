using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Code.Game.Object_Respawner
{
	public class ObjectBossRespawner : ObjectRespawner
	{
		public string QuestName;

		public string TaskName;

		private new void OnEnable()
		{
			InvokeRepeating("CheckTheTask", 0f, 1f);
		}

		private void CheckTheTask()
		{
			if (ObjectPrefab.GetComponent<HumanoidStatusNPC>() != null && ObjectPrefab.GetComponent<HumanoidStatusNPC>().Faction == Faction.Boss)
			{
				GameEventManager instance = GameEventManager.Instance;
				string text = string.Empty;
				foreach (Qwest activeQwest in instance.ActiveQwests)
				{
					if (activeQwest.Name == QuestName && activeQwest.GetCurrentTask().TaskText == TaskName)
					{
						text = activeQwest.GetCurrentTask().TaskText;
						UnityEngine.Debug.Log(text);
					}
				}
				if (text == TaskName)
				{
					CancelInvoke("CheckTheTask");
					base.OnEnable();
				}
			}
		}
	}
}
