using UnityEngine;

public class TrailedComboVFX : BaseComboVFX
{
	[Separator("TrailedComboVFX")]
	public bool DebugLog_TrailedComboVFX;

	[Space(10f)]
	public TrailRenderer[] Trails;

	public override void ActivateFX()
	{
		base.ActivateFX();
		TrailRenderer[] trails = Trails;
		foreach (TrailRenderer trailRenderer in trails)
		{
			trailRenderer.gameObject.SetActive(value: true);
		}
	}

	public override void DeactivateFX()
	{
		base.DeactivateFX();
		TrailRenderer[] trails = Trails;
		foreach (TrailRenderer trailRenderer in trails)
		{
			trailRenderer.gameObject.SetActive(value: false);
		}
	}
}
