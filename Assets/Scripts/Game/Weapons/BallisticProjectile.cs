using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.Effects;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Weapons
{
	public class BallisticProjectile : Projectile
	{
		[Separator("Ballistic projectile")]
		public ParticleSystem DetachedTrail;

		public float DetachedTrailLifeTime;

		protected Rigidbody rigidBody;

		protected HitEntity currentOwner;

		private float finalExplosionDamage;

		private float finalExplosionForce;

		public override void Init(HitEntity projectileOwner)
		{
			Init(projectileOwner, 0f, -1f, DamageType.Explosion, 0f);
		}

		public override void Init(HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f)
		{
			currentOwner = projectileOwner;
			HitDamageType = damageType;
			DefenceIgnorance = damageReduction;
			if (explosionMaxDamage != -1f)
			{
				ExplosionDamage = explosionMaxDamage;
			}
			if (projectileDamage != -1f)
			{
				ProjectileDamage = projectileDamage;
			}
			finalExplosionDamage = ExplosionDamage;
			finalExplosionForce = ExplosionForce;
			if (currentOwner is Player)
			{
				Player player = currentOwner as Player;
				finalExplosionDamage *= player.stats.GetPlayerStat(StatsList.ExplosionDamageMult);
				finalExplosionForce *= player.stats.GetPlayerStat(StatsList.ExplosionForceMult);
			}
			if (!rigidBody)
			{
				rigidBody = GetComponent<Rigidbody>();
			}
			if (Trail != null)
			{
				Trail.SetActive(value: true);
			}
			if ((bool)DetachedTrail)
			{
				var newTrail = PoolManager.Instance.GetFromPool(DetachedTrail);
				newTrail.transform.parent = base.transform;
				newTrail.transform.localPosition = Vector3.zero;
				newTrail.transform.localEulerAngles = Vector3.zero;
				var emit = newTrail.emission;
				emit.enabled = true;
				PoolManager.Instance.AddBeforeReturnEvent(base.gameObject, delegate
				{
					newTrail.transform.parent = null;
					emit.enabled = false;
					PoolManager.Instance.AddBeforeReturnEvent(newTrail, delegate
					{
					});
					PoolManager.Instance.ReturnToPoolWithDelay(newTrail, DetachedTrailLifeTime);
				});
			}
		}

		public override void onHit()
		{
			GameObject fromPool = PoolManager.Instance.GetFromPool(ExplosionPrefab);
			fromPool.transform.position = base.transform.position;
			Explosion component = fromPool.GetComponent<Explosion>();
			component.Init(currentOwner, finalExplosionDamage, ExplosionRange, finalExplosionForce);
			DeInit();
		}

		public override void DeInit()
		{
			if (Trail != null)
			{
				Trail.SetActive(value: false);
			}
			rigidBody.velocity = Vector3.zero;
			rigidBody.angularVelocity = Vector3.zero;
			currentOwner = null;
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		protected virtual void OnCollisionEnter(Collision col)
		{
			HitEntity component = col.collider.gameObject.GetComponent<HitEntity>();
			if (component != null)
			{
				component.OnHit(HitDamageType, currentOwner, ProjectileDamage, col.contacts[0].point, col.contacts[0].normal, DefenceIgnorance);
			}
			onHit();
		}
	}
}
