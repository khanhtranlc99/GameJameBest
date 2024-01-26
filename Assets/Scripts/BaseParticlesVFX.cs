using UnityEngine;

public class BaseParticlesVFX : BaseFX
{
	[Separator("BaseParticlesVFX")]
	public bool DebugLog_BaseParticlesVFX;

	[Space(10f)]
	public ParticleSystem[] ParticleEffects;

	public override void StartEffect()
	{
		base.StartEffect();
		ActivateFX();
	}

	public override void StopEffect()
	{
		base.StopEffect();
		DeactivateFX();
	}

	public override void ActivateFX()
	{
		ParticleSystem[] particleEffects = ParticleEffects;
		foreach (ParticleSystem particleSystem in particleEffects)
		{
			particleSystem.Play();
		}
	}

	public override void DeactivateFX()
	{
		ParticleSystem[] particleEffects = ParticleEffects;
		foreach (ParticleSystem particleSystem in particleEffects)
		{
			particleSystem.Stop();
		}
	}
}
