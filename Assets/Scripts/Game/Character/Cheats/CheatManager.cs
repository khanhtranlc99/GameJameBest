using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.GlobalComponent.Qwest;
using Game.Managers;
using Game.MiniMap;
using UnityEngine;

namespace Game.Character.Cheats
{
	public class CheatManager : MonoBehaviour
	{
		public GameObject cheatPanel;

		public int expForAdd = 1000000;

		public int healthForAdd = 1000000;

		public int moneyForAdd = 1000000;

		public int GemsForAdd = 10000;

		private Player player;

		private Game.MiniMap.MiniMap miniMap;

		private void Awake()
		{
			if (Debug.isDebugBuild)
			{
				cheatPanel.SetActive(value: true);
			}
			else
			{
				cheatPanel.SetActive(value: false);
			}
		}

		private void Start()
		{
			player = PlayerInteractionsManager.Instance.Player;
			miniMap = Game.MiniMap.MiniMap.Instance;
		}

		public void AddExperience()
		{
			LevelManager.Instance.AddExperience(expForAdd);
		}

		public void AddHealth()
		{
			player.Health.Setup(healthForAdd, healthForAdd);
		}

		public void ResetStamina()
		{
			player.stats.stamina.Setup();
		}

		public void AddMoney()
		{
			PlayerInfoManager.Money += moneyForAdd;
		}

		public void AddGems()
		{
			PlayerInfoManager.Gems += GemsForAdd;
		}

		public void GetWeapon()
		{
			AddMoney();
		}

		public void CompleteTask()
		{
			GameEventManager instance = GameEventManager.Instance;
			Qwest markedQwest = instance.MarkedQwest;
			if (markedQwest != null)
			{
				markedQwest.MoveToTask(markedQwest.GetCurrentTask().NextTask);
			}
			else if (GameManager.ShowDebugs)
			{
				UnityEngine.Debug.Log("no marked qwest");
			}
		}

		public void Teleport()
		{
			Vector3 position = miniMap.UserMark.transform.position;
			Vector3 origin = new Vector3(position.x, position.y + 1000f, position.z);
			Ray ray = new Ray(origin, -Vector3.up);
			RaycastHit hitInfo;
			Physics.Raycast(ray, out hitInfo, float.PositiveInfinity);
			player.transform.position = hitInfo.point;
		}
	}
}
