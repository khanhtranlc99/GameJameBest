using Game.Character.CharacterController;
using Game.Effects;
using Game.Enemy;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Weapons
{
	public class HomingProjectile : Projectile
	{
		[Tooltip("In local space")]
		[Separator("Homing projectile")]
		public Vector3 FirstPoint;

		public float Speed = 10f;

		private float homingStep;

		private Rigidbody rigidbody;

		private HitEntity currentOwner;

		private Vector3 currentTargetPos;

		private Vector3 endPosition;

		private Vector3 startPosition;

		private HitEntity currentTarget;

		private bool homingToFirstPos;

		private bool homingToTarget;

		private bool haveTarget;

		private bool havePosition;

		private SphereCollider sphereCollider;

		private bool isSimple;

		public override void Init(HitEntity projectileOwner)
		{
			Init(projectileOwner, 0f, -1f, DamageType.Explosion, 0f);
		}

		public override void Init(HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f)
		{
			if (!rigidbody)
			{
				rigidbody = GetComponent<Rigidbody>();
			}
			if (Trail != null)
			{
				Trail.SetActive(value: true);
			}
			if (!sphereCollider)
			{
				sphereCollider = GetComponent<SphereCollider>();
			}
			startPosition = base.transform.position;
			currentOwner = projectileOwner;
			HitDamageType = damageType;
			DefenceIgnorance = damageReduction;
			ProjectileDamage = projectileDamage;
			currentTargetPos = base.transform.position + FirstPoint;
			havePosition = true;
			homingToFirstPos = true;
			homingStep = 0f;
			isSimple = true;
		}

		public void Init(Vector3 position, HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f)
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
			if (!rigidbody)
			{
				rigidbody = GetComponent<Rigidbody>();
			}
			if (Trail != null)
			{
				Trail.SetActive(value: true);
			}
			if (!sphereCollider)
			{
				sphereCollider = GetComponent<SphereCollider>();
			}
			startPosition = base.transform.position;
			endPosition = position;
			currentOwner = projectileOwner;
			currentTargetPos = base.transform.position + base.transform.right * FirstPoint.x + base.transform.up * FirstPoint.y + base.transform.forward * FirstPoint.z;
			havePosition = true;
			homingToFirstPos = true;
			homingStep = 0f;
		}

		public void Init(HitEntity target, HitEntity projectileOwner, float explosionMaxDamage = -1f, float projectileDamage = -1f, DamageType damageType = DamageType.Explosion, float damageReduction = 0f)
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
			if (!rigidbody)
			{
				rigidbody = GetComponent<Rigidbody>();
			}
			if (Trail != null)
			{
				Trail.SetActive(value: true);
			}
			if (!sphereCollider)
			{
				sphereCollider = GetComponent<SphereCollider>();
			}
			startPosition = base.transform.position;
			currentTarget = ((!(target is BodyPartDamageReciever)) ? target : (target as BodyPartDamageReciever).RerouteEntity);
			currentOwner = projectileOwner;
			currentTargetPos = base.transform.position + base.transform.right * FirstPoint.x + base.transform.up * FirstPoint.y + base.transform.forward * FirstPoint.z;
			haveTarget = true;
			homingToFirstPos = true;
			homingStep = 0f;
		}

		public override void DeInit()
		{
			if (Trail != null)
			{
				Trail.SetActive(value: false);
			}
			if (rigidbody != null)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
			if (sphereCollider != null)
			{
				sphereCollider.enabled = false;
			}
			PoolManager.Instance.ReturnToPool(base.gameObject);
			currentOwner = null;
			havePosition = false;
			haveTarget = false;
			homingToFirstPos = false;
			homingToTarget = false;
			isSimple = false;
		}

		public override void onHit()
		{
			GameObject fromPool = PoolManager.Instance.GetFromPool(ExplosionPrefab);
			fromPool.transform.position = base.transform.position;
			Explosion component = fromPool.GetComponent<Explosion>();
			component.Init(currentOwner, ExplosionDamage, ExplosionRange);
			DeInit();
		}

		private void OnCollisionEnter(Collision c)
		{
			HitEntity component = c.collider.gameObject.GetComponent<HitEntity>();
			if (component != null)
			{
				component.OnHit(DamageType.Explosion, currentOwner, ProjectileDamage, c.contacts[0].point, c.contacts[0].normal, 0f);
			}
			onHit();
		}

		private void FixedUpdate()
		{
			if (isSimple)
			{
				float num = Vector3.Distance(startPosition, base.transform.position);
				if (num >= 1.2f)
				{
					sphereCollider.enabled = true;
				}
				return;
			}
			if (homingToFirstPos)
			{
				homingStep += Time.deltaTime * Speed;
				float num2 = Vector3.Distance(startPosition, currentTargetPos);
				base.transform.position = Vector3.Lerp(startPosition, currentTargetPos, homingStep / num2);
				base.transform.LookAt(currentTargetPos);
				if (Vector3.Distance(base.transform.position, currentTargetPos) <= 0.1f)
				{
					homingToFirstPos = false;
					homingToTarget = true;
					startPosition = base.transform.position;
					homingStep = 0f;
					sphereCollider.enabled = true;
				}
			}
			if (homingToTarget && haveTarget)
			{
				homingStep += Time.deltaTime * Speed;
				Vector3 vector = currentTarget.transform.position + currentTarget.NPCShootVectorOffset;
				float num3 = Vector3.Distance(startPosition, vector);
				base.transform.position = Vector3.Lerp(startPosition, vector, homingStep / num3);
				UnityEngine.Debug.DrawLine(startPosition, vector, Color.cyan, 1f);
				base.transform.LookAt(currentTarget.transform);
			}
			else if (havePosition && !haveTarget && homingToTarget)
			{
				base.transform.LookAt(endPosition);
				float num4 = Vector3.Distance(startPosition, endPosition);
				homingStep += Time.deltaTime * Speed;
				base.transform.position = Vector3.Lerp(startPosition, endPosition, homingStep / num4);
				if ((double)Vector3.Distance(base.transform.position, endPosition) <= 0.01)
				{
					havePosition = false;
				}
			}
		}
	}
}
