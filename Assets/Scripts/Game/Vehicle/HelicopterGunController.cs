using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Weapons;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Vehicle
{
	public class HelicopterGunController : MonoBehaviour
	{
		private const string MinigunShootStateName = "Fire";

		private const string LauncherShootStateName = "Fire2";

		private const int MinigunXMaxAngle = 45;

		private const int MinigunYMaxAngle = 70;

		private const int LauncherXMaxAngle = 10;

		private const int LauncherYMaxAngle = 70;

		public RangedWeapon MinigunPrefab;

		public RangeWeaponProjectile LauncherPrefab;

		private GameObject[] launcherPoints;

		private int currentLauncherPointIndex;

		private RangedWeapon minigunInstance;

		private RangeWeaponProjectile launcherInstance;

		private DrivableHelicopter currentHelicopter;

		private HelicopterController helicopterController;

		private VehicleStatus vehStatus;

		private Image shootIndicator;

		private AudioSource audioSource;

		public void Init(DrivableHelicopter drivableHelicopter, HitEntity driver)
		{
			launcherPoints = drivableHelicopter.RocketsPoints;
			currentHelicopter = drivableHelicopter;
			helicopterController = (drivableHelicopter.controller as HelicopterController);
			vehStatus = drivableHelicopter.GetComponentInChildren<VehicleStatus>();
			minigunInstance = PoolManager.Instance.GetFromPool(MinigunPrefab);
			GameObject minigunPoint = drivableHelicopter.MinigunPoint;
			minigunInstance.transform.position = minigunPoint.transform.position;
			minigunInstance.transform.parent = minigunPoint.transform;
			minigunInstance.Muzzle = minigunPoint.transform;
			minigunInstance.Init();
			minigunInstance.SetWeaponOwner(driver);
			minigunInstance.InflictDamageEvent = InflictDamageEvent;
			minigunInstance.PerformAttackEvent = OverallWeaponAttackEvent;
			minigunInstance.SetScatterCalculateFromMuzzle();
			launcherInstance = PoolManager.Instance.GetFromPool(LauncherPrefab);
			launcherInstance.transform.position = launcherPoints[0].transform.position;
			launcherInstance.transform.parent = launcherPoints[0].transform;
			launcherInstance.Muzzle = launcherPoints[0].transform;
			launcherInstance.Init();
			launcherInstance.SetWeaponOwner(driver);
			launcherInstance.PerformAttackEvent = LauncherAttackEvent;
			RangeWeaponProjectile rangeWeaponProjectile = launcherInstance;
			rangeWeaponProjectile.PerformAttackEvent = (Weapon.AttackEvent)Delegate.Combine(rangeWeaponProjectile.PerformAttackEvent, new Weapon.AttackEvent(OverallWeaponAttackEvent));
			launcherInstance.SetScatterCalculateFromMuzzle();
		}

		public void DeInit()
		{
			minigunInstance.DeInit();
			launcherInstance.DeInit();
			PoolManager.Instance.ReturnToPool(minigunInstance);
			PoolManager.Instance.ReturnToPool(launcherInstance);
		}

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void FixedUpdate()
		{
			if (helicopterController.InitializedGetter && !helicopterController.IsGrounded)
			{
				SetShootIndicator();
				Shooting();
			}
		}

		private void InflictDamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			victim.OnHit(DamageType.Bullet, owner, weapon.Damage, hitPos, hitVector, defenceReduction);
		}

		private void OverallWeaponAttackEvent(Weapon weapon)
		{
			weapon.PlayAttackSound(audioSource);
		}

		private void LauncherAttackEvent(Weapon weapon)
		{
			currentLauncherPointIndex++;
			if (currentLauncherPointIndex >= launcherPoints.Length)
			{
				currentLauncherPointIndex = 0;
			}
		}

		private void SetShootIndicator()
		{
			if (!shootIndicator)
			{
				shootIndicator = HelicopterControlPanel.Instance.ReloadIndicator;
			}
			shootIndicator.fillAmount = launcherInstance.GetRechargeStatus();
		}

		private void Shooting()
		{
			if (Controls.GetButton("Fire"))
			{
				ShootFromWeapon(minigunInstance, 45f, 70f);
			}
			if (Controls.GetButton("Fire2"))
			{
				launcherInstance.Muzzle = launcherPoints[currentLauncherPointIndex].transform;
				ShootFromWeapon(launcherInstance, 10f, 70f);
			}
		}

		private void ShootFromWeapon(RangedWeapon weapon, float xMaxAngle, float yMaxAngle)
		{
			Vector3 shotDirVector;
			CastResult castResult = TargetManager.Instance.ShootFromCamera(weapon.WeaponOwner, weapon.ScatterVector, out shotDirVector, weapon.AttackDistance, weapon);
			shotDirVector = currentHelicopter.transform.InverseTransformDirection(shotDirVector);
			shotDirVector.x = Mathf.Clamp(shotDirVector.x, 0f - xMaxAngle, xMaxAngle);
			shotDirVector.y = Mathf.Clamp(shotDirVector.y, 0f - yMaxAngle, yMaxAngle);
			shotDirVector.z = Mathf.Abs(shotDirVector.z);
			shotDirVector = currentHelicopter.transform.TransformDirection(shotDirVector);
			weapon.Attack(vehStatus.GetVehicleDriver(), shotDirVector);
		}
	}
}
