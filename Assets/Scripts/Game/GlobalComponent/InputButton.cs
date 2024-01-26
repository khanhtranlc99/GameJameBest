using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Button))]
	public class InputButton : MonoBehaviour
	{
		public string InputButtonName;

		private Button button;

		private void Update()
		{
			if (button == null)
			{
				button = GetComponent<Button>();
			}
			if (Controls.GetButtonDown(InputButtonName))
			{
				button.onClick.Invoke();
			}
		}
	}
}
