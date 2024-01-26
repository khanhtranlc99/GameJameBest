using Game.GlobalComponent;
using System;
using Game.UI;
using RopeHeroViceCity.Scripts.Gampelay;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public class CharacterStat
	{
		private const int SlowUpdatePeriod = 1;

		public string Name;

		public float Max = 100f;

		public float Current;

		public float RegenPerSecond;

		private float regenStore;

		private SlowUpdateProc slowUpdateProc;

		public CharacterStatDisplay StatDisplay;
		public Action<float> OnValueChange;
		public float RegenModifier
		{
			get
			{
				if (regenStore == 0f)
				{
					regenStore = RegenPerSecond * Time.fixedDeltaTime;
				}
				return regenStore;
			}
		}

		public void Setup(float percentFloat =1)
		{
			Setup(Max*percentFloat, Max);
		}

		public void Setup(float current, float max)
		{
			if (Max != max && max > 0f)
			{
				Max = max;
			}
			if (Current != current)
			{
				Current = current;
			}
			if (StatDisplay != null)
			{
				StatDisplay.Setup(Max, Current);
			}
		}

		public void DoFixedUpdate()
		{
			if (RegenModifier != 0f)
			{
				InitSlowUpdateProc();
				if (Current >= Max)
				{
					Current = Max;
					return;
				}
				Current += RegenModifier;
				slowUpdateProc.ProceedOnFixedUpdate();
			}
		}

		public void Change(float amount)
		{
			Current += amount;
			if (Current > Max)
			{
				Current = Max;
			}
			if (Current < 0f)
			{
				Current = 0f;
			}
			if (StatDisplay != null)
			{
				StatDisplay.SetCurrent(Current);
				StatDisplay.OnChanged(amount);
			}
			OnValueChange?.Invoke(Current);
		}

		public void SetAmount(float amount)
		{
			Current += amount;
			if (Current > Max)
			{
				Current = Max;
			}
			if (Current < 0f)
			{
				Current = 0f;
			}
			if (StatDisplay != null)
			{
				StatDisplay.SetCurrent(Current);
			}
			OnValueChange?.Invoke(Current);
		}

		public void Set(float value)
		{
			Current = value;
			if (Current > Max)
			{
				Current = Max;
			}
			if (Current < 0f)
			{
				Current = 0f;
			}
			if (StatDisplay != null)
			{
				StatDisplay.SetCurrent(Current);
			}
			OnValueChange?.Invoke(Current);
		}

		private void InitSlowUpdateProc()
		{
			if (slowUpdateProc == null)
			{
				slowUpdateProc = new SlowUpdateProc(SlowUpdate, 1f);
			}
		}

		private void SlowUpdate()
		{
			if (StatDisplay != null)
			{
				StatDisplay.SetCurrent(Current);
			}
		}
	}
}
