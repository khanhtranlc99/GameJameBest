using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;

public class FireEyesTracer : LaserTracer
{
	private LineRenderer secondLaser;

	protected override void OnDisable()
	{
		if (!(secondLaser == null))
		{
			base.OnDisable();
			secondLaser.positionCount = (0);
			PoolManager.Instance.ReturnToPool(secondLaser);
			secondLaser = null;
		}
	}

	protected override void Update()
	{
		if (!(secondLaser == null))
		{
			base.Update();
			if (Time.time > lastLaserTime + LifeTime)
			{
				secondLaser.positionCount = (0);
			}
		}
	}

	protected override void ShootLaser()
	{
		if (base.isActiveAndEnabled)
		{
			FireEyes fireEyes = (FireEyes)currentRangedWeapon;
			if (fireEyes != null)
			{
				base.ShootLaser();
				LineFromMuzzle(fireEyes.SecondMuzzle, ref secondLaser);
			}
		}
	}
}
