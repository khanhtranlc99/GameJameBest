using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class NPCMeleeHitAssistant : StateMachineBehaviour
	{
		private SmartHumanoidWeaponController weaponController;

		private int meleeAttackState;

		private bool hitted;
		

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			WeaponControllerInitialize(animator);
			meleeAttackState = animator.GetInteger("Melee");
			hitted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (!hitted)
			{
				weaponController.MeleeWeaponAttack(meleeAttackState);
				hitted = true;
			}
		}

		private void WeaponControllerInitialize(Animator animator)
		{
			if (weaponController == null)
			{
				BaseNPC component = animator.GetComponent<BaseNPC>();
				SmartHumanoidController smartHumanoidController = component.CurrentController as SmartHumanoidController;
				if (smartHumanoidController != null)
				{
					weaponController = smartHumanoidController.WeaponController;
					PoolManager.Instance.AddBeforeReturnEvent(smartHumanoidController, delegate
					{
						weaponController = null;
					});
				}
			}
		}
	}
}
