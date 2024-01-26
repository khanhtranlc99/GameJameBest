using Game.Weapons;
using UnityEngine;
using UnityEngine.Animations;
//using UnityEngine.Experimental.Director;

namespace Game.Character
{
	public class ShootingAssistant : StateMachineBehaviour
	{
		private WeaponController weaponController;

		private bool shooted;
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (weaponController == null)
			{
				WeaponControllerInitialize(animator);
			}
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
			weaponController = animator.GetComponent<WeaponController>();
			if (!weaponController)
			{
				UnityEngine.Debug.LogError("Can't find WeaponController");
			}
		}
	}
}
