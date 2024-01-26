using Game.UI;
using System;
using RopeHeroViceCity.UI_Features.UI_Descriptor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Items
{
	public class PowerUpTimeStatusItem : MonoBehaviour
	{
		private const int MaxTimeDiff = 4;

		private const int DescriptionShowedTime = 3;

		public Image ShowedImage;

		public Image Filler;

		public Text TimerText;

		public bool ShowDescriptionOnClick;

		private RectTransform myRectTransform;

		private float remainingTime;

		private int lastRemMin;

		private int lastRemSec;

		public GameItemPowerUp CurrentPowerUp
		{
			get;
			private set;
		}

		public void ShowDescription()
		{
			if (ShowDescriptionOnClick)
			{
				string str = " (" + GetRemainingMinutes().ToString("00") + ":" + GetRemainingSeconds().ToString("00") + ")";
				UIDescriptor.OpenPopup(myRectTransform, CurrentPowerUp.ShopVariables.Description + str, 3f);
			}
		}

		public void Init(GameItemPowerUp displayedPowerUp)
		{
			CurrentPowerUp = displayedPowerUp;
			ShowedImage.sprite = displayedPowerUp.ShopVariables.ItemIcon;
			remainingTime = displayedPowerUp.RemainingDuration;
		}

		private void Awake()
		{
			myRectTransform = (base.transform as RectTransform);
		}

		private void Update()
		{
			SyncTime();
			UpdateTimer();
			UpdateFiller();
		}

		private void SyncTime()
		{
			remainingTime -= Time.deltaTime;
			if (Math.Abs(remainingTime - (float)CurrentPowerUp.RemainingDuration) > 4f)
			{
				remainingTime = CurrentPowerUp.RemainingDuration;
			}
		}

		private void UpdateTimer()
		{
			if (TimerText == null)
			{
				return;
			}
			bool flag = false;
			string text = string.Empty;
			int remainingMinutes = GetRemainingMinutes();
			if (remainingMinutes > 0)
			{
				if (remainingMinutes != lastRemMin)
				{
					text = remainingMinutes + "m";
					lastRemMin = remainingMinutes;
					flag = true;
				}
			}
			else
			{
				int remainingSeconds = GetRemainingSeconds();
				if (remainingSeconds != lastRemSec)
				{
					text = remainingSeconds.ToString("00");
					lastRemSec = remainingSeconds;
					flag = true;
				}
			}
			if (flag)
			{
				TimerText.text = text;
			}
		}

		private int GetRemainingMinutes()
		{
			return (int)Mathf.Floor(remainingTime / 60f);
		}

		private int GetRemainingSeconds()
		{
			return (int)Mathf.Clamp(Mathf.Floor(remainingTime % 60f), 0f, 60f);
		}

		private void UpdateFiller()
		{
			if (!(Filler == null))
			{
				float fillAmount = 1f - Mathf.Clamp01(remainingTime / (float)CurrentPowerUp.Duration);
				Filler.fillAmount = fillAmount;
			}
		}
	}
}
