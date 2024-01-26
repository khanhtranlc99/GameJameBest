using Game.Character.CharacterController;
using Game.Effects;
using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;

public class EffectAreaProjectile : BallisticProjectile
{
	[Separator("EffectAreaProjectile")]
	public GameObject AreaEffectPrefab;

	public float ReturnToPoolDelay = 5f;

	private GameObject effectArea;

	private EffectArea effectAreaScript;

	protected override void OnCollisionEnter(Collision col)
	{
		HitEntity component = col.collider.gameObject.GetComponent<HitEntity>();
		if (component != null)
		{
			component.OnHit(HitDamageType, currentOwner, ProjectileDamage, col.contacts[0].point, col.contacts[0].normal, DefenceIgnorance);
		}
		onHit(col);
	}

	public void onHit(Collision collision)
	{
		GameObject fromPool = PoolManager.Instance.GetFromPool(ExplosionPrefab);
		fromPool.transform.position = base.transform.position;
		Explosion component = fromPool.GetComponent<Explosion>();
		component.Init(currentOwner, ExplosionDamage, ExplosionRange);
		effectArea = PoolManager.Instance.GetFromPool(AreaEffectPrefab);
		effectArea.transform.position = base.transform.position;
		effectArea.transform.LookAt(new Vector3(0f, 0f, 0f), collision.impulse);
		effectAreaScript = effectArea.GetComponent<EffectArea>();
		effectAreaScript.Activate(currentOwner);
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
		PoolManager.Instance.ReturnToPoolWithDelay(effectArea, effectAreaScript.EffectDuration + ReturnToPoolDelay);
		PoolManager.Instance.ReturnToPool(base.gameObject);
	}
}
