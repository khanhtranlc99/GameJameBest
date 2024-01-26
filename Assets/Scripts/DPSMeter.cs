using Game.Character.CharacterController;
using UnityEngine;

public class DPSMeter : HitEntity
{
	public float time = 10f;

	private float curDmg;

	private float timer;

	public bool TimerStarted;

	public override void OnHit(DamageType damageType, HitEntity owner, float damage, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
	{
		curDmg += damage;
		if (!TimerStarted)
		{
			TimerStarted = true;
		}
	}

	protected override void FixedUpdate()
	{
		if (TimerStarted)
		{
			if (timer < time)
			{
				timer += Time.fixedDeltaTime;
				return;
			}
			UnityEngine.Debug.Log("Total damage: " + curDmg + " DPS: " + curDmg / time, this);
			timer = 0f;
			curDmg = 0f;
		}
	}
}
