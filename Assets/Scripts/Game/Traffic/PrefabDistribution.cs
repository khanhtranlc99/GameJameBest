using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Traffic
{
	[Serializable]
	public class PrefabDistribution
	{
		[Serializable]
		public class Chance
		{
			public GameObject Prefab;

			public float Percent;
		}

		public List<Chance> Chances = new List<Chance>();

		public static PrefabDistribution AverageDistanceDistribution(IDictionary<PrefabDistribution, float> distributionToDistance)
		{
			float num = distributionToDistance.Values.Sum((float distance) => 1f / distance);
			float num2 = 1f / num;
			PrefabDistribution[] array = new PrefabDistribution[distributionToDistance.Count];
			int num3 = 0;
			foreach (KeyValuePair<PrefabDistribution, float> item in distributionToDistance)
			{
				PrefabDistribution key = item.Key;
				float value = item.Value;
				float num4 = 1f / value;
				PrefabDistribution prefabDistribution = new PrefabDistribution();
				foreach (Chance chance2 in key.Chances)
				{
					Chance chance = new Chance();
					chance.Percent = chance2.Percent;
					chance.Prefab = chance2.Prefab;
					prefabDistribution.Chances.Add(chance);
				}
				array[num3] = prefabDistribution;
				prefabDistribution.Normalize();
				foreach (Chance chance3 in prefabDistribution.Chances)
				{
					chance3.Percent = chance3.Percent * num2 * num4;
					if (chance3.Percent < 0.1f)
					{
						chance3.Percent = 0f;
					}
				}
				num3++;
			}
			return AverageDistribution(array).Normalize();
		}

		public static PrefabDistribution AverageDistribution(params PrefabDistribution[] distributions)
		{
			PrefabDistribution prefabDistribution = new PrefabDistribution();
			foreach (PrefabDistribution prefabDistribution2 in distributions)
			{
				foreach (Chance chance in prefabDistribution2.Chances)
				{
					bool flag = false;
					foreach (Chance chance2 in prefabDistribution.Chances)
					{
						if (chance2.Prefab.Equals(chance.Prefab))
						{
							chance2.Percent += chance.Percent;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						prefabDistribution.Chances.Add(new Chance
						{
							Percent = chance.Percent,
							Prefab = chance.Prefab
						});
					}
				}
			}
			return prefabDistribution.Normalize();
		}

		public GameObject GetRandomPrefab()
		{
			float max = Chances.Sum((Chance chance) => chance.Percent);
			float num = UnityEngine.Random.Range(0f, max);
			float num2 = 0f;
			for (int i = 0; i < Chances.Count; i++)
			{
				if (num2 + Chances[i].Percent > num)
				{
					return Chances[i].Prefab;
				}
				num2 += Chances[i].Percent;
			}
			return Chances[Chances.Count - 1].Prefab;
		}

		public PrefabDistribution Normalize()
		{
			float num = Chances.Sum((Chance chance) => chance.Percent);
			float num2 = 100f / num;
			foreach (Chance chance in Chances)
			{
				chance.Percent *= num2;
				chance.Percent = Mathf.Round(chance.Percent * 100f) / 100f;
			}
			return this;
		}

		public string GetStatusForLog()
		{
			string text = string.Empty;
			foreach (Chance chance in Chances)
			{
				text += $"{chance.Percent}% - {chance.Prefab.name}\n";
			}
			return text;
		}
	}
}
