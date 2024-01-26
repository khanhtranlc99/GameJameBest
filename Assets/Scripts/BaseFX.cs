using UnityEngine;

public class BaseFX : MonoBehaviour
{
	public bool DebugLog;

	[Space(10f)]
	[Tooltip("Set 0 for manual control")]
	public float EffectDuration = 5f;

	[Tooltip("Is duration stack or just renew?")]
	public bool StackableDuration;

	protected float activationTime;

	protected bool effectIsActive;

	protected float timeToStop;

	private int currStacks;

	public bool IsActive
	{
		get
        {
			return effectIsActive;
		}
	}
	

	protected virtual void FixedUpdate()
	{
		if (effectIsActive)
		{
			CheckAutoStop();
		}
	}

	protected void CheckAutoStop()
	{
		if (EffectDuration != 0f && timeToStop != 0f && Time.time >= timeToStop)
		{
			StopEffect();
		}
	}

	public virtual void StartEffect()
	{
		effectIsActive = true;
		if (timeToStop == 0f)
		{
			timeToStop = Time.time;
		}
		timeToStop = ((!StackableDuration) ? (Time.time + EffectDuration) : (timeToStop + EffectDuration));
	}

	public virtual void StopEffect()
	{
		effectIsActive = false;
		timeToStop = 0f;
	}

	public virtual void ActivateFX()
	{
	}

	public virtual void DeactivateFX()
	{
	}
}
