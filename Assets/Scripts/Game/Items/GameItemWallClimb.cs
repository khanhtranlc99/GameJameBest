using Game.Character.CharacterController;
using UnityEngine;
namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemWallClimbData", menuName = "RopeData/ItemData/WallClimb", order = 100)]
	public class GameItemWallClimb : GameItemAbility
	{
		public override void Enable()
		{
			PlayerManager.Instance.AnimationController.EnableClimbWalls = true;
		}

		public override void Disable()
		{
			PlayerManager.Instance.AnimationController.EnableClimbWalls = false;
			PlayerManager.Instance.AnimationController.Reset();
		}
	}
}
