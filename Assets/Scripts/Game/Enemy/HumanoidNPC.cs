using UnityEngine;

namespace Game.Enemy
{
	public class HumanoidNPC : BaseNPC
	{
		public override void Init()
		{
			base.Init();
		}

		public override void DeInit()
		{
			base.DeInit();
		}

		public void Smash()
		{
			SmartHumanoidController smartHumanoidController = (SmartHumanoidController)base.CurrentController;
			if (smartHumanoidController != null)
			{
				smartHumanoidController.AnimationController.Smash(StatusNpc, new GameObject[1]
				{
					base.gameObject
				});
			}
		}
	}
}
