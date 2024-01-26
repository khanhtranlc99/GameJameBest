using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Vehicle;
using Game.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Mech
{
	public class MechGunController : MonoBehaviour
	{
		private const string FirstWeaponShootStateName = "Fire";

		private const string SecondWeaponShootStateName = "Fire2";

		[Separator("Core")]
		public bool LimitedShotDirections;

		public int PermittedAngleForShooting = 25;

		public LayerMask WeaponLayerMask = -1;

		public LayerMask GroundLayerMask = -1;

		[Separator("Weapons")]
		public RangedWeapon RangedWeaponPrefab;

		public RangeWeaponProjectile LauncherPrefab;

		private AudioSource audioSource;

		private RangedWeapon rangedWeapon1;

		private RangedWeapon rangedWeapon2;

		private RangeWeaponProjectile rangeWeaponProjectile;

		private Image RechargeIndicator;

		private VehicleStatus vehStatus;

		private DrivableMech currentMech;

		public void Init(DrivableMech drivableMech, HitEntity driver)
		{
			currentMech = drivableMech;
			vehStatus = currentMech.GetComponentInChildren<VehicleStatus>();
			RechargeIndicator = MechControlPanel.Instance.ReloadIndicator;
			InitLauncher();
			InitRangedWeapons();
		}

		public void DeInit()
		{
			rangedWeapon1.DeInit();
			rangedWeapon2.DeInit();
			rangeWeaponProjectile.DeInit();
			PoolManager.Instance.ReturnToPool(rangedWeapon1);
			PoolManager.Instance.ReturnToPool(rangedWeapon2);
			PoolManager.Instance.ReturnToPool(rangeWeaponProjectile);
		}

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void Update()
		{
			Shooting();
		}

		private void WeaponAttackEvent(Weapon weapon)
		{
			PointSoundManager.Instance.PlayCustomClipAtPoint(rangeWeaponProjectile.transform.position, rangeWeaponProjectile.SoundAttack);
			WeaponManager.Instance.StartShootSFX(rangeWeaponProjectile.transform, rangeWeaponProjectile.ShotSfx);
			rangeWeaponProjectile.RechargeCall();
		}

		private void Shooting()
		{
			if (Controls.GetButton("Fire2"))
			{
				ShootFromRangeWeaponProjectile();
			}
			if (Controls.GetButton("Fire"))
			{
				ShootFromRangedWeapons();
			}
			UpdateRechargeIndicator();
		}

		private void UpdateRechargeIndicator()
		{
			if ((bool)RechargeIndicator)
			{
				RechargeIndicator.fillAmount = rangeWeaponProjectile.GetRechargeStatus();
			}
		}

		private void ShootFromRangeWeaponProjectile()
		{
			bool flag = rangeWeaponProjectile.Projectile.GetComponent<HomingProjectile>();
			Vector3 shotDirVector;
			CastResult castResult = TargetManager.Instance.ShootFromCamera(vehStatus.GetVehicleDriver(), rangeWeaponProjectile.ScatterVector, out shotDirVector, rangeWeaponProjectile.AttackDistance, rangeWeaponProjectile);
			if (flag)
			{
				HomingMissileLaunch(castResult.HitEntity, castResult.HitPosition);
			}
			else
			{
				rangeWeaponProjectile.Attack(vehStatus.GetVehicleDriver(), shotDirVector);
			}
		}

		private void ShootFromRangedWeapons()
		{
			Vector3 shotDirVector;
			Vector3 shotDirVector2;
			TargetManager.Instance.ShootFromCamera(vehStatus.GetVehicleDriver(), rangedWeapon1.ScatterVector, out shotDirVector, rangedWeapon1.AttackDistance, rangedWeapon1);
			TargetManager.Instance.ShootFromCamera(vehStatus.GetVehicleDriver(), rangedWeapon2.ScatterVector, out shotDirVector2, rangedWeapon2.AttackDistance, rangedWeapon2);
			if (DirectionPermittedForShooting(shotDirVector))
			{
				rangedWeapon1.Attack(vehStatus.GetVehicleDriver(), shotDirVector);
			}
			if (DirectionPermittedForShooting(shotDirVector2))
			{
				rangedWeapon2.Attack(vehStatus.GetVehicleDriver(), shotDirVector2);
			}
		}

		public bool DirectionPermittedForShooting(Vector3 shootingDirection)
		{
			if (!LimitedShotDirections)
			{
				return true;
			}
			bool flag = (double)Vector3.Dot(currentMech.transform.forward, shootingDirection.normalized) <= -0.2;
			Vector3 vector = Vector3.Cross(shootingDirection.normalized, currentMech.transform.forward);
			float num = Mathf.Asin(vector.y) * 57.29578f;
			if (flag && num >= (float)(-PermittedAngleForShooting) && num <= (float)PermittedAngleForShooting)
			{
				return true;
			}
			return false;
		}

		private void HomingMissileLaunch(HitEntity hitEntity, Vector3 targetPosition)
		{
			if ((bool)hitEntity)
			{
				rangeWeaponProjectile.HomingAttack(vehStatus.GetVehicleDriver(), hitEntity);
			}
			else
			{
				rangeWeaponProjectile.HomingAttack(vehStatus.GetVehicleDriver(), targetPosition);
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

		private void InitRangedWeapons()
		{
			rangedWeapon1 = PoolManager.Instance.GetFromPool(RangedWeaponPrefab);
			rangedWeapon2 = PoolManager.Instance.GetFromPool(RangedWeaponPrefab);
			GameObject lasergunPoint = currentMech.LasergunPoint1;
			GameObject lasergunPoint2 = currentMech.LasergunPoint2;
			rangedWeapon1.transform.parent = lasergunPoint.transform;
			rangedWeapon1.transform.localPosition = Vector3.zero;
			rangedWeapon1.transform.localRotation = Quaternion.identity;
			rangedWeapon2.transform.parent = lasergunPoint2.transform;
			rangedWeapon2.transform.localPosition = Vector3.zero;
			rangedWeapon2.transform.localRotation = Quaternion.identity;
			rangedWeapon1.Init();
			rangedWeapon2.Init();
			rangedWeapon1.SetWeaponOwner(vehStatus.GetVehicleDriver());
			rangedWeapon2.SetWeaponOwner(vehStatus.GetVehicleDriver());
			rangedWeapon1.InflictDamageEvent = InflictDamageEvent;
			rangedWeapon2.InflictDamageEvent = InflictDamageEvent;
			rangedWeapon1.PerformAttackEvent = OverallWeaponAttackEvent;
			rangedWeapon1.SetScatterCalculateFromMuzzle();
			rangedWeapon2.SetScatterCalculateFromMuzzle();
		}

		private void InitLauncher()
		{
			rangeWeaponProjectile = PoolManager.Instance.GetFromPool(LauncherPrefab).GetComponent<RangeWeaponProjectile>();
			rangeWeaponProjectile.transform.parent = currentMech.LauncherPoint1.transform.parent;
			rangeWeaponProjectile.transform.localPosition = Vector3.zero;
			rangeWeaponProjectile.transform.localEulerAngles = Vector3.zero;
			rangeWeaponProjectile.Muzzle = currentMech.LauncherPoint1.transform;
			rangeWeaponProjectile.Init();
			rangeWeaponProjectile.SetWeaponOwner(vehStatus.GetVehicleDriver());
			rangeWeaponProjectile.PerformAttackEvent = WeaponAttackEvent;
		}
	}
}
