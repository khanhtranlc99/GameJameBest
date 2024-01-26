using Game.Character.CharacterController;
using Game.Effects;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Weapons
{
	public class RangeWeaponProjectile : RangedWeapon
	{
		private const int MinDistanceForShoot = 1;

		public GameObject Projectile;

		public float ProjectileSpeedRate = 50f;

		public float CurveRate;

		public Rigidbody BaseRigidbody;

		public override void Attack(HitEntity owner)
		{
			if (!PrepareAttack())
			{
				return;
			}
			Vector3 b = Vector3.zero;
			if (BaseRigidbody != null)
			{
				b = BaseRigidbody.velocity;
			}
			Vector3 shotDirVector;
			CastResult castResult = TargetManager.Instance.ShootFromCamera(owner, GetScatterVector(), out shotDirVector, AttackDistance, this, humanoidShoot: true);
			Vector3 vector = (!(castResult.TargetObject != null)) ? (TargetManager.Instance.Camera.transform.position + shotDirVector * 100f) : castResult.HitPosition;
			if (castResult.RayLength < 1f)
			{
				Projectile component = Projectile.GetComponent<Projectile>();
				if ((bool)component)
				{
					UnityEngine.Debug.Log(castResult.HitEntity, castResult.HitEntity);
					if ((bool)castResult.HitEntity)
					{
						castResult.HitEntity.OnHit(component.HitDamageType, base.WeaponOwner, component.ProjectileDamage, vector, shotDirVector, 0f);
					}
					GameObject fromPool = PoolManager.Instance.GetFromPool(component.ExplosionPrefab, vector, Quaternion.identity);
					Explosion component2 = fromPool.GetComponent<Explosion>();
					if ((bool)component2)
					{
						component2.Init(base.WeaponOwner, component.ExplosionDamage, component.ExplosionRange, component.ExplosionForce);
					}
				}
			}
			else
			{
				GameObject fromPool2 = PoolManager.Instance.GetFromPool(Projectile);
				fromPool2.transform.position = Muzzle.position;
				fromPool2.transform.rotation = Muzzle.rotation;
				Vector3 lhs = vector - Muzzle.position;
				if (Vector3.Dot(lhs, Muzzle.forward) < 0f)
				{
					lhs = shotDirVector;
				}
				fromPool2.GetComponent<Rigidbody>().AddForce((lhs.normalized + Vector3.up * CurveRate) * ProjectileSpeedRate + b, ForceMode.VelocityChange);
				fromPool2.GetComponent<Projectile>().Init(base.WeaponOwner, -1f, Damage, WeaponDamageType, DefenceIgnorence);
			}
			OnShootAlarm(owner, null);
		}

		public override void Attack(HitEntity owner, Vector3 direction)
		{
			if (PrepareAttack())
			{
				Vector3 b = Vector3.zero;
				if (BaseRigidbody != null)
				{
					b = BaseRigidbody.velocity;
				}
				GameObject fromPool = PoolManager.Instance.GetFromPool(Projectile);
				fromPool.transform.position = Muzzle.TransformPoint(0f, 0f, 0f);
				fromPool.transform.rotation = Muzzle.transform.rotation;
				Vector3 force = (direction.normalized + Vector3.up * CurveRate) * ProjectileSpeedRate + b;
				fromPool.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
				fromPool.GetComponent<Projectile>().Init(base.WeaponOwner, -1f, Damage, WeaponDamageType, DefenceIgnorence);
				OnShootAlarm(owner, null);
			}
		}

		public void HomingAttack(HitEntity owner, Vector3 targetPosition)
		{
			if (PrepareAttack())
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool(Projectile);
				fromPool.transform.position = Muzzle.TransformPoint(0f, 0f, 0f);
				fromPool.transform.rotation = Muzzle.transform.rotation;
				fromPool.GetComponent<HomingProjectile>().Init(targetPosition, base.WeaponOwner, -1f, Damage, WeaponDamageType, DefenceIgnorence);
				OnShootAlarm(owner, null);
			}
		}

		public void HomingAttack(HitEntity owner, HitEntity target)
		{
			if (PrepareAttack() && !(owner == target))
			{
				GameObject fromPool = PoolManager.Instance.GetFromPool(Projectile);
				fromPool.transform.position = Muzzle.TransformPoint(0f, 0f, 0f);
				fromPool.transform.rotation = Muzzle.transform.rotation;
				fromPool.GetComponent<HomingProjectile>().Init(target, base.WeaponOwner, -1f, Damage, WeaponDamageType, DefenceIgnorence);
				OnShootAlarm(owner, target);
			}
		}
	}
}
