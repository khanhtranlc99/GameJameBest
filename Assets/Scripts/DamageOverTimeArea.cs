using Game.Character.CharacterController;
using Game.Enemy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageOverTimeArea : EffectGrowingArea
{
	[Separator("DamageOverTimeArea parameters")]
	public bool DebugLog_DoTArea;

	[Space(10f)]
	public float TicPeriod = 1f;

	public float DamagePerTic = 100f;

	public DamageType DamageType;

	private List<HitEntity> affectedHitEntities = new List<HitEntity>();

	private HitEntity[] ignoreHitEntities;

	private float lastTicTime;

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Time.time - lastTicTime >= TicPeriod)
		{
			HitThemAll();
			lastTicTime = Time.time;
		}
	}

	public void SetIgnorable(HitEntity[] ignorableObjects)
	{
		ignoreHitEntities = ignorableObjects;
	}

	public override void Activate()
	{
		base.Activate();
		UpdateAffectedHitEntities();
	}

	public override void Deactivate()
	{
		base.Deactivate();
		affectedHitEntities.Clear();
	}

	protected override void StartEffect()
	{
	}

	private void HitThemAll()
	{
		List<HitEntity> list = new List<HitEntity>();
		foreach (HitEntity affectedHitEntity in affectedHitEntities)
		{
			if (!(affectedHitEntity == null) && affectedHitEntity.isActiveAndEnabled)
			{
				affectedHitEntity.OnHit(DamageType, areaInitiator, DamagePerTic, Vector3.zero, Vector3.zero, 0f);
				if (affectedHitEntity.IsDead)
				{
					list.Add(affectedHitEntity);
				}
			}
		}
		UpdateAffectedHitEntities(list, delete: true);
	}

	protected void UpdateAffectedHitEntities()
	{
		foreach (Collider affectedCollider in affectedColliders)
		{
			HitEntity component = affectedCollider.GetComponent<HitEntity>();
			if (component != null)
			{
				UpdateAffectedHitEntities(component);
			}
		}
	}

	protected void UpdateAffectedHitEntities(HitEntity incHitEntity, bool delete = false)
	{
		if (incHitEntity is BodyPartDamageReciever)
		{
			incHitEntity = (incHitEntity as BodyPartDamageReciever).RerouteEntity;
		}
		if (delete)
		{
			affectedHitEntities.Remove(incHitEntity);
		}
		else if ((ignoreHitEntities == null || !ignoreHitEntities.Contains(incHitEntity)) && !affectedHitEntities.Contains(incHitEntity))
		{
			affectedHitEntities.Add(incHitEntity);
		}
	}

	protected void UpdateAffectedHitEntities(List<HitEntity> incHitEntities, bool delete = false)
	{
		foreach (HitEntity incHitEntity in incHitEntities)
		{
			UpdateAffectedHitEntities(incHitEntity, delete);
		}
	}

	protected new virtual void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		HitEntity component = other.GetComponent<HitEntity>();
		if (component != null)
		{
			UpdateAffectedHitEntities(component);
		}
	}

	protected new virtual void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);
		HitEntity component = other.GetComponent<HitEntity>();
		if (component != null)
		{
			UpdateAffectedHitEntities(component, delete: true);
		}
	}
}
