using UnityEngine;

namespace Game.MiniMap
{
	public class MarkButton : MonoBehaviour
	{
		private MarkForMiniMap mark;

		public void Init(MarkForMiniMap m)
		{
			mark = m;
		}

		public void OnMarkClick()
		{
			mark.MarckOnClick();
		}
	}
}
