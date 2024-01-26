using Game.Managers;
using UnityEngine;

namespace Game.Weapons
{
	public class MeleeHitAssistant : StateMachineBehaviour
	{
		private WeaponController weaponController;

		private int previousInteger;

		private int meleeAttackState;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			previousInteger = 0;
			meleeAttackState = animator.GetInteger("Melee");
			if (weaponController == null)
			{
				WeaponControllerInitialize(animator);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (previousInteger == 0)
			{
				previousInteger++;
				if (weaponController == null)
				{
					WeaponControllerInitialize(animator);
				}
				if (weaponController != null)
				{
					weaponController.MeleeWeaponAttack(meleeAttackState);
				}
			}
		}

		private void WeaponControllerInitialize(Animator animator)
		{
			weaponController = animator.GetComponent<WeaponController>();
			if (!weaponController && GameManager.ShowDebugs)
			{
				UnityEngine.Debug.LogError("Can't find WeaponController");
			}
		}
	}
}
