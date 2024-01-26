using UnityEngine;

public class EffectGrowingArea : EffectArea
{
	[Separator("EffectGrowingArea parameters")]
	[Space(10f)]
	public bool DebugLog_EGArea;

	[Tooltip("Works only with sphere colliders")]
	[Space(10f)]
	public float MinRadius = 1f;

	[Tooltip("Works only with sphere colliders")]
	public float MaxRadius = 1f;

	[Tooltip("Works only with sphere colliders")]
	public float GrowPerSecond;

	[Space(5f)]
	public bool StartFromMin = true;

	private float defaultRadius;

	private float currRadius;

	private SphereCollider sphereTriggerCollider;

	protected override void Awake()
	{
		base.Awake();
		defaultRadius = ((!StartFromMin) ? MaxRadius : MinRadius);
		sphereTriggerCollider = (TriggerCollider as SphereCollider);
		currRadius = defaultRadius;
		SetAreaRadius(currRadius);
	}

	public override void Activate()
	{
		base.Activate();
		currRadius = defaultRadius;
		SetAreaRadius(currRadius);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (effectIsActive)
		{
			AutoChangeSize();
		}
	}

	private void AutoChangeSize()
	{
		if (GrowPerSecond != 0f && !(currRadius < MinRadius) && !(currRadius > MaxRadius))
		{
			currRadius += GrowPerSecond * Time.deltaTime;
			SetAreaRadius(currRadius);
		}
	}

	private void SetAreaRadius(float radius)
	{
		if ((bool)sphereTriggerCollider)
		{
			sphereTriggerCollider.radius = radius;
		}
	}
}
