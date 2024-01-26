using UnityEngine.UI;

namespace UnitySampleAssets.CrossPlatformInput
{
	public class InputAxisScrollbar : Scrollbar
	{
		public string axis;

		protected override void OnEnable()
		{
			base.OnEnable();
			base.onValueChanged.AddListener(HandleInput);
		}

		private void HandleInput(float arg0)
		{
			CrossPlatformInputManager.SetAxis(axis, base.value * 2f - 1f);
		}
	}
}
