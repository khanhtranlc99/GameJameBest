using Game.Character.CharacterController.Enums;
using UnityEngine;

namespace Game.Weapons
{
	public class MinigunRangedWeapon : RangedWeapon
	{
		[Separator("Minigun specific parametrs")]
		public GameObject RotatedObject;

		public Transform RotationAxisY;

		public float RotateToShooting = 10f;

		public float CrankingCounter = 1f;

		private float currentRotateSpeed;

		public override RangedAttackState GetRangedAttackState()
		{
			LastGetStateTime = Time.time;
			if (currentRotateSpeed <= RotateToShooting)
			{
				currentRotateSpeed += Time.deltaTime * CrankingCounter;
				lastAttackTime = Time.time;
				return RangedAttackState.Idle;
			}
			return base.GetRangedAttackState();
		}

		public override void DeInit()
		{
			base.DeInit();
			currentRotateSpeed = 0f;
		}

		private void FixedUpdate()
		{
			if (!(currentRotateSpeed <= 0f))
			{
				if (RotationAxisY == null)
				{
					RotatedObject.transform.Rotate(currentRotateSpeed, 0f, 0f);
				}
				else
				{
					RotatedObject.transform.Rotate(RotationAxisY.up, currentRotateSpeed, Space.World);
				}
				if (Time.time > lastAttackTime + AttackDelay + 1f)
				{
					currentRotateSpeed -= Time.deltaTime * CrankingCounter;
				}
			}
		}
	}
}
