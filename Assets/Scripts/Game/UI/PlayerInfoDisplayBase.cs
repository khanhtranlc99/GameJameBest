using Game.Character;
using UnityEngine;

namespace Game.UI
{
	public abstract class PlayerInfoDisplayBase : MonoBehaviour
	{
		protected int InfoValue;

		protected abstract PlayerInfoType GetInfoType();

		protected abstract void Display();

		private void Awake()
		{
			PlayerInfoManager.Instance.AddOnValueChangedEvent(GetInfoType(), OnChangeValue);
			InfoValue = PlayerInfoManager.Instance.GetInfoValue(GetInfoType());
			Display();
		}

		private void OnChangeValue(int newValue)
		{
			InfoValue = newValue;
			Display();
		}
	}
}
