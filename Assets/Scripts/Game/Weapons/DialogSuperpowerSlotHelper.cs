using UnityEngine;

namespace Game.Weapons
{
	public class DialogSuperpowerSlotHelper : DialogSlotHelper
	{
		[Space(10f)]
		public bool Active;

		public override void UpdateSlot(bool IsAvailable, Sprite sprite, bool highlighted)
		{
			CheckItem();
			base.UpdateSlot(IsAvailable, sprite, highlighted);
		}

		private void CheckItem()
		{
		}
	}
}
