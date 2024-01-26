using Game.GlobalComponent;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Items
{
	public class PowerUpTimeStatusPanel : MonoBehaviour
	{
		private const int CheckActualPowerUpPeriod = 2;

		public RectTransform ContentContainer;

		public PowerUpTimeStatusItem StatusItemPrefab;

		public AspectRatioFitter RatioFitter;

		public GridLayoutGroup GridLayoutGroup;

		public float AdditionalAspectRatio = 0.5f;

		[Tooltip("0 - infinity")]
		public int MaxShowedItemsCount;

		[Tooltip("0 - not consider time")]
		public int MinRemainingDurationToShow;

		private readonly List<GameItemPowerUp> trackedPowerUps = new List<GameItemPowerUp>();

		private readonly Dictionary<GameItemPowerUp, PowerUpTimeStatusItem> powerUpToStatus = new Dictionary<GameItemPowerUp, PowerUpTimeStatusItem>();

		private SlowUpdateProc slowUpdateProc;

		private Image image;

		private float lastAspectRatio;

		private void Awake()
		{
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 2f);
			image = GetComponent<Image>();
			lastAspectRatio = RatioFitter.aspectRatio;
		}

		private void OnEnable()
		{
			CheckActualPowerUp();
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			CheckActualPowerUp();
		}

		private void FillContent()
		{
			for (int i = 0; i < StuffManager.ActivePowerUps.Count; i++)
			{
				if (MaxShowedItemsCount > 0 && trackedPowerUps.Count >= MaxShowedItemsCount)
				{
					break;
				}
				GameItemPowerUp gameItemPowerUp = StuffManager.ActivePowerUps[i];
				if (!trackedPowerUps.Contains(gameItemPowerUp) && (MinRemainingDurationToShow <= 0 || gameItemPowerUp.RemainingDuration <= MinRemainingDurationToShow))
				{
					PowerUpTimeStatusItem fromPool = PoolManager.Instance.GetFromPool(StatusItemPrefab);
					fromPool.transform.parent = ContentContainer;
					fromPool.transform.localScale = Vector3.one;
					fromPool.Init(gameItemPowerUp);
					trackedPowerUps.Add(gameItemPowerUp);
					powerUpToStatus.Add(gameItemPowerUp, fromPool);
				}
			}
		}

		private void CheckActualPowerUp()
		{
			for (int i = 0; i < trackedPowerUps.Count; i++)
			{
				GameItemPowerUp gameItemPowerUp = trackedPowerUps[i];
				if (!StuffManager.ActivePowerUps.Contains(gameItemPowerUp))
				{
					PowerUpTimeStatusItem o = powerUpToStatus[gameItemPowerUp];
					PoolManager.Instance.ReturnToPool(o);
					trackedPowerUps.Remove(gameItemPowerUp);
					powerUpToStatus.Remove(gameItemPowerUp);
				}
			}
			if (StuffManager.ActivePowerUps.Count != trackedPowerUps.Count)
			{
				FillContent();
			}
			bool flag = trackedPowerUps.Count > 0;
			if ((bool)image)
			{
				image.enabled = flag;
			}
			if ((bool)GridLayoutGroup)
			{
				GridLayoutGroup.enabled = flag;
			}
			if (!RatioFitter)
			{
				return;
			}
			RatioFitter.enabled = flag;
			if (flag)
			{
				float num = (float)trackedPowerUps.Count + AdditionalAspectRatio;
				if (Math.Abs(lastAspectRatio - num) > 0f)
				{
					RatioFitter.aspectRatio = num;
				}
			}
		}
	}
}
