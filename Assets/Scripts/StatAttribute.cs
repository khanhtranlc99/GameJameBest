using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public struct StatAttribute
	{
		[SerializeField]
		private StatsList statType;

		[SerializeField]
		private float value;

		public float GetStatValue => value;

		public StatsList StatType => statType;

		public StatAttribute(StatsList statType, float value)
		{
			this.statType = statType;
			this.value = value;
		}

		public void SetStatValue(float value)
		{
			this.value = value;
		}
	}
}
