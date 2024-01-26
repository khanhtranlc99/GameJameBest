using UnityEngine;

namespace Game.Character.Stats
{
	public abstract class CharacterStatDisplay : MonoBehaviour
	{
		public string nameStat;
		protected float current;

		protected float max = 100f;

		public bool DoSlowUpdate;

		public float SlowUpdateRate = 0.5f;

		protected virtual void Awake()
		{
			if (DoSlowUpdate)
			{
				InvokeRepeating("SlowUpdate", SlowUpdateRate, SlowUpdateRate);
			}
		}

		protected virtual void Start()
		{
		}

		protected virtual void OnEnable()
		{
		}

		public void Setup(float max, float current)
		{
			SetMax(max);
			SetCurrent(current);
		}

		public void SetCurrent(float value)
		{
			if (max > 0f)
			{
				current = Mathf.Clamp(value, 0f, max);
			}
			else
			{
				current = value;
			}
			SlowUpdate();
		}

		public void SetMax(float value)
		{
			if (value > 0f)
			{
				max = value;
			}
		}

		protected void SlowUpdate()
		{
			UpdateDisplayValue();
		}

		protected abstract void UpdateDisplayValue();

		public abstract void OnChanged(float amount);
	}
}
