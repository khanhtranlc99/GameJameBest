using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class NPCShootingAssistant : StateMachineBehaviour
	{
		private SmartHumanoidWeaponController weaponController;

		private bool shooted;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			WeaponControllerInitialize(animator);
			if (weaponController != null)
			{
				animator.SetFloat("ShootSpeed", 1f / weaponController.CurrentWeapon.AttackDelay);
			}
			shooted = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (Time.timeScale == 0f)
			{
				return;
			}
			if (!shooted)
			{
				if (weaponController != null)
				{
					weaponController.AttackWithWeapon();
				}
				shooted = true;
			}
			if (stateInfo.normalizedTime > 0.9f)
			{
				shooted = false;
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
