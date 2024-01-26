using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public class StatsMas
	{
		public string name;

		public StatsList stat;

		public float[] values = new float[6];

		[HideInInspector]
		public int SpentPoints;

		public float ActualValue => values[SpentPoints];
	}
}
