using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Items
{
	public class GameItemAbility : GameItem
	{
		public GameItemSkin[] RelatedSkins;

		[HideInInspector]
		public bool IsActive;

		public override bool CanBeEquiped => PlayerManager.Instance.PlayerIsDefault;

		public virtual void Enable()
		{
		}

		public virtual void Disable()
		{
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return true;
		}
	}
}
