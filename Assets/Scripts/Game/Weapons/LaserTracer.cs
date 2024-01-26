using Game.GlobalComponent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapons
{
	public class LaserTracer : MonoBehaviour
	{
		public LineRenderer LaserPrefab;

		public float SegmentLength = 5f;

		public float LaserScatter = 0.2f;

		public float LifeTime = 0.5f;

		protected RangedWeapon currentRangedWeapon;

		private LineRenderer currentLaser;

		protected float lastLaserTime;

		private void Awake()
		{
			currentRangedWeapon = GetComponent<RangedWeapon>();
			RangedWeapon rangedWeapon = currentRangedWeapon;
			rangedWeapon.AfterAttackEvent = (Weapon.AttackEvent)Delegate.Combine(rangedWeapon.AfterAttackEvent, new Weapon.AttackEvent(AttackEvent));
		}

		protected virtual void OnDisable()
		{
			if (!(currentLaser == null))
			{
				currentLaser.positionCount = (0);
				PoolManager.Instance.ReturnToPool(currentLaser);
				currentLaser = null;
			}
		}

		protected virtual void Update()
		{
			if (!(currentLaser == null) && Time.time > lastLaserTime + LifeTime)
			{
				currentLaser.positionCount = (0);
			}
		}

		private void AttackEvent(Weapon weapon)
		{
			ShootLaser();
			lastLaserTime = Time.time;
		}

		protected virtual void ShootLaser()
		{
			if (base.isActiveAndEnabled)
			{
				LineFromMuzzle(currentRangedWeapon.Muzzle, ref currentLaser);
			}
		}

		protected void LineFromMuzzle(Transform currMuzzle, ref LineRenderer LaserRenderer)
		{
			Vector3 normalized = currentRangedWeapon.LastHitDirectionVector.normalized;
			if (LaserRenderer == null)
			{
				LaserRenderer = PoolManager.Instance.GetFromPool(LaserPrefab);
			}
			List<Vector3> list = new List<Vector3>();
			list.Add(currMuzzle.position);
			float num = (!(currentRangedWeapon.LastHitPosition == Vector3.zero)) ? Vector3.Distance(currMuzzle.position, currentRangedWeapon.LastHitPosition) : 100f;
			Vector3 item = normalized * num + currMuzzle.position;
			float num2 = num / SegmentLength;
			for (int i = 1; (float)i < num2; i++)
			{
				Vector3 b = currMuzzle.right * UnityEngine.Random.Range(0f - LaserScatter, LaserScatter) + currMuzzle.up * UnityEngine.Random.Range(0f - LaserScatter, LaserScatter);
				Vector3 item2 = normalized * (SegmentLength * (float)i) + currMuzzle.position + b;
				list.Add(item2);
			}
			list.Add(item);
			LaserRenderer.positionCount = (list.Count);
			LaserRenderer.SetPositions(list.ToArray());
		}
	}
}
