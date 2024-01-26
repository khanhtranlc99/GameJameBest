using Game.Shop;
using System;
using Game.Character;
using RopeHeroViceCity.Scripts.Gampelay;
using UnityEngine;

namespace Game.Items
{
	public class GameItemPowerUp : GameItem
	{
		private const int updateCD = 3;

		private const string durationStorePrefix = "duration";

		private const string cdStartStorePrefix = "cdStart";

		[Tooltip("In seconds")]
		public int Duration = 60;

		[Tooltip("In seconds")]
		public long Cooldawn = 600L;

		private long cdStartTime;

		private float remainingUpdateCD = 3f;

		public override bool CanBeEquiped => RemainingDuration > 0 || !TimeManager.OnCoolDown(cdStartTime, Cooldawn);

		public bool Paused
		{
			get;
			protected set;
		}

		public int RemainingDuration
		{
			get;
			protected set;
		}

		public int RemainingCooldawn
		{
			get;
			protected set;
		}

		public bool isActive
		{
			get;
			protected set;
		}

		private void Awake()
		{
			// ShopManager.Instance.ShopOpeningEvent = Pause;
			// ShopManager.Instance.ShopCloseningEvent = Сontinue;
			EventManager.Instance.ShopOpeningEvent.AddListener(Pause);
			EventManager.Instance.ShopClosingEvent.AddListener(Сontinue);
		}

		private void FixedUpdate()
		{
			if (isActive && !Paused)
			{
				remainingUpdateCD -= Time.deltaTime;
				if (remainingUpdateCD <= 0f)
				{
					RemainingDuration -= 3;
					remainingUpdateCD = 3f;
					BaseProfile.StoreValue(RemainingDuration, "duration" + ID);
				}
				if (RemainingDuration <= 0)
				{
					Deactivate();
				}
			}
		}

		public void Pause()
		{
			Paused = true;
		}

		public void Сontinue()
		{
			Paused = false;
		}

		public override void UpdateItem()
		{
			if (PlayerInfoManager.Instance.BoughtAlredy(this))
			{
				cdStartTime = BaseProfile.GetValue<long>("cdStart" + ID);
				RemainingDuration = BaseProfile.GetValue<int>("duration" + ID);
				RemainingCooldawn = TimeManager.RemainingCooldawn(cdStartTime, Cooldawn);
				if (!isActive && !TimeManager.OnCoolDown(cdStartTime, Cooldawn))
				{
					ResetDuration();
				}
			}
		}

		public override void OnBuy()
		{
			cdStartTime = DateTime.Now.ToFileTime() - Cooldawn;
			ResetDuration();
			BaseProfile.StoreValue(cdStartTime, "cdStart" + ID);
			BaseProfile.StoreValue(RemainingDuration, "duration" + ID);
		}

		private void ResetDuration()
		{
			RemainingDuration = Duration * PlayerInfoManager.Instance.GetBIValue(ID, ShopVariables.gemPrice);
			BaseProfile.StoreValue(RemainingDuration, "duration" + ID);
		}

		public virtual void Activate()
		{
			if (!isActive)
			{
				isActive = true;
				StuffManager.ActivePowerUps.Add(this);
			}
		}

		public virtual void Deactivate()
		{
			if (isActive)
			{
				cdStartTime = DateTime.Now.ToFileTime();
				BaseProfile.StoreValue(cdStartTime, "cdStart" + ID);
				BaseProfile.StoreValue(RemainingDuration, "duration" + ID);
				isActive = false;
				StuffManager.ActivePowerUps.Remove(this);
			}
		}
	}
}
