using UnityEngine;

public class BaseComboVFX : BaseParticlesVFX
{
	[Separator("BaseComboVFX")]
	public bool DebugLog_BaseComboVFX;

	[Space(10f)]
	public ParticleSystem[] OneshotParticles;

	public override void ActivateFX()
	{
		base.ActivateFX();
		ParticleSystem[] oneshotParticles = OneshotParticles;
		foreach (ParticleSystem particleSystem in oneshotParticles)
		{
			particleSystem.Emit(1);
		}
	}
}
