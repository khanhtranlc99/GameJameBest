using Game.Character;
using UnityEngine.UI;

namespace Game.UI
{
	public class PlayerIntInfoDisplay : PlayerInfoDisplayBase
	{
		public PlayerInfoType InfoType;

		public Text TextLink;

		public string BeforeText;

		protected override PlayerInfoType GetInfoType()
		{
			return InfoType;
		}

		protected override void Display()
		{
			if ((bool)TextLink)
			{
				TextLink.text = BeforeText + InfoValue;
			}
		}
	}
}
