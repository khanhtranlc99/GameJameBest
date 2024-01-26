using Game.Character;
using UnityEngine;

namespace Game.UI
{
	public class LevelUpedButton : MonoBehaviour
	{
		private void Awake()
		{
			Activator(PlayerInfoManager.UpgradePoints);
			PlayerInfoManager.Instance.AddOnValueChangedEvent(PlayerInfoType.UpgradePoints, Activator);
		}

		private void Activator(int upgradePoints)
		{
			base.gameObject.SetActive(upgradePoints > 0);
		}
	}
}
