using Game.Character;
using System.Collections;
using UnityEngine;

namespace Game.Vehicle
{
	public class BikeAnimatorController : MonoBehaviour
	{
		private const float maxAnimatorSpid = 4f;

		private const float tweakingTimeout = 0.3f;

		public float TimeToSit;

		private bool sit;

		private Animator animator;

		private BikeController bikeController;

		private bool isTweaking;

		private PlayerInteractionsManager playerInteractionsManager;

		private float tweakingTimer;

		public virtual void Init(Animator anim)
		{
			tweakingTimer = 0f;
			playerInteractionsManager = PlayerInteractionsManager.Instance;
			bikeController = GetComponent<BikeController>();
			bikeController.crashed = true;
			animator = anim;
			animator.enabled = true;
		}

		public virtual void DeInit()
		{
			animator.Rebind();
			animator.enabled = false;
			sit = false;
		}

		private void Update()
		{
			if (PlayerInteractionsManager.Instance.inVehicle && !sit)
			{
				StartCoroutine(Sitting());
				sit = true;
			}
			SetSteerAnimationState((bikeController.steerInput + 1f) / 2f);
			SetDirectionAnimationState((int)bikeController.MotorInput);
			SetSpeedAnimationState(bikeController.Speed * 4f / bikeController.maxSpeed);
			SetReversAnimationState(bikeController.reversing);
		}

		private IEnumerator Sitting()
		{
			isTweaking = true;
			yield return new WaitForSeconds(0.3f + Time.deltaTime * 2f);
			SetEnterAnimationState(value: true);
			yield return new WaitForSeconds(TimeToSit);
			SetSitAnimationState(value: true);
			bikeController.crashed = false;
		}

		private void LateUpdate()
		{
			if (isTweaking)
			{
				tweakingTimer += Time.deltaTime;
				playerInteractionsManager.TweakingSkeleton(playerInteractionsManager.CharacterHips, playerInteractionsManager.DriverHips, tweakingTimer);
				if (tweakingTimer >= 0.3f)
				{
					playerInteractionsManager.SwitchSkeletons(on: true);
					tweakingTimer = 0f;
					isTweaking = false;
				}
			}
		}

		private void SetSteerAnimationState(float value)
		{
			animator.SetFloat("Steer", value);
		}

		private void SetSpeedAnimationState(float value)
		{
			animator.SetFloat("Speed", value);
		}

		private void SetDirectionAnimationState(int value)
		{
			animator.SetInteger("Direction", value);
		}

		private void SetSitAnimationState(bool value)
		{
			animator.SetBool("Sit", value);
		}

		private void SetEnterAnimationState(bool value)
		{
			animator.SetBool("Enter", value);
		}

		private void SetReversAnimationState(bool value)
		{
			animator.SetBool("Revers", value);
		}
	}
}
