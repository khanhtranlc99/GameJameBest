using Game.Character.CharacterController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EffectArea : MonoBehaviour
{
	[Separator("EffectArea parameters")]
	public bool DebugLog;

	[Space(10f)]
	public Collider TriggerCollider;

	[Space(5f)]
	[Tooltip("0 for permanent activity")]
	public float EffectDuration;

	[Space(5f)]
	public LayerMask AffectedLayers;

	[Space(5f)]
	public BaseFX VisualEffects;

	protected List<Collider> affectedColliders = new List<Collider>();

	protected bool effectIsActive;

	private Collider[] ignoreColliders;

	protected float activationTime;

	protected HitEntity areaInitiator;

	public bool IsActive
	{
		get
        {
			return effectIsActive;
		}
	}
	

	protected virtual void Awake()
	{
		if (TriggerCollider == null)
		{
			TriggerCollider = GetComponent<Collider>();
		}
		TriggerCollider.isTrigger = true;
		TriggerCollider.enabled = false;
		if (!(VisualEffects == null))
		{
			VisualEffects.EffectDuration = EffectDuration;
			VisualEffects.StartEffect();
		}
	}

	protected virtual void FixedUpdate()
	{
		if (effectIsActive && EffectDuration != 0f && Time.time - activationTime >= EffectDuration)
		{
			Deactivate();
		}
	}

	public void SetIgnorable(Collider[] ignorableObjects)
	{
		ignoreColliders = ignorableObjects;
	}

	public virtual void Activate()
	{
		if (!effectIsActive)
		{
			effectIsActive = true;
			activationTime = Time.time;
			TriggerCollider.enabled = true;
			UpdateAffectedColliders(CheckTrigger(TriggerCollider));
			StartEffect();
			if (VisualEffects != null)
			{
				VisualEffects.StartEffect();
			}
		}
	}

	public virtual void Activate(HitEntity initiator)
	{
		areaInitiator = initiator;
		Activate();
	}

	public virtual void Deactivate()
	{
		if (effectIsActive)
		{
			TriggerCollider.enabled = false;
			affectedColliders.Clear();
			effectIsActive = false;
			StopEffect();
			if (VisualEffects != null)
			{
				VisualEffects.StopEffect();
			}
		}
	}

	protected virtual void StartEffect()
	{
		if (DebugLog)
		{
			foreach (Collider affectedCollider in affectedColliders)
			{
				UnityEngine.Debug.Log(affectedCollider.gameObject.name);
			}
		}
	}

	protected virtual void StopEffect()
	{
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (((1 << other.gameObject.layer) & (int)AffectedLayers) != 0)
		{
			UpdateAffectedColliders(other);
		}
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		if (((1 << other.gameObject.layer) & (int)AffectedLayers) != 0)
		{
			UpdateAffectedColliders(other, delete: true);
		}
	}

	protected virtual Collider[] CheckTrigger(Collider sensorCollider)
	{
		if (sensorCollider is SphereCollider)
		{
			SphereCollider sphereCollider = sensorCollider as SphereCollider;
			return Physics.OverlapSphere(sphereCollider.center, sphereCollider.radius, AffectedLayers);
		}
		if (sensorCollider is BoxCollider)
		{
			BoxCollider boxCollider = sensorCollider as BoxCollider;
			return Physics.OverlapBox(boxCollider.center, boxCollider.bounds.extents, boxCollider.transform.rotation, AffectedLayers);
		}
		if (sensorCollider is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = sensorCollider as CapsuleCollider;
			Vector3 one = Vector3.one;
			if (capsuleCollider.direction == 0)
			{
				one.y = (one.z = 0f);
			}
			else if (capsuleCollider.direction == 1)
			{
				one.x = (one.z = 0f);
			}
			else
			{
				one.x = (one.y = 0f);
			}
			Vector3 b = one * capsuleCollider.height / 2f - one * capsuleCollider.radius;
			Vector3 a = base.transform.position + capsuleCollider.center;
			if (DebugLog)
			{
				UnityEngine.Debug.DrawLine(a + b, a - b, Color.red);
			}
			return Physics.OverlapCapsule(a + b, a - b, capsuleCollider.radius, AffectedLayers);
		}
		return Physics.OverlapSphere(base.transform.position, 1f, AffectedLayers);
	}

	protected void UpdateAffectedColliders(Collider incCollider, bool delete = false)
	{
		if (delete)
		{
			affectedColliders.Remove(incCollider);
		}
		else if ((ignoreColliders == null || !ignoreColliders.Contains(incCollider)) && !affectedColliders.Contains(incCollider))
		{
			affectedColliders.Add(incCollider);
		}
	}

	protected void UpdateAffectedColliders(Collider[] incColliders, bool delete = false)
	{
		foreach (Collider incCollider in incColliders)
		{
			UpdateAffectedColliders(incCollider, delete);
		}
	}

	private void OnEnable()
	{
		if ((double)EffectDuration < 0.001)
		{
			Activate();
		}
	}

	private void OnDisable()
	{
		if ((double)EffectDuration < 0.001)
		{
			Deactivate();
		}
	}
}
