using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public struct StatIcon
	{
		[SerializeField]
		private Sprite icon;

		[SerializeField]
		private StatsList statType;

		[SerializeField]
		private string statShowName;

		public Sprite Icon => icon;

		public StatsList StatType => statType;

		public string ShowedName => statShowName;
	}
}
