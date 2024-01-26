using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Character.CharacterController
{
	[Serializable]
	public class Defence
	{
		[Serializable]
		public class DefencePrimitive
		{
			public DamageType DamageType;

			[Tooltip("0 - no defence; 1 - 100% defence")]
			public float DefenceMultiplier;

			public DefencePrimitive()
			{
				DamageType = DamageType.Instant;
				DefenceMultiplier = 0f;
			}

			public DefencePrimitive(DamageType damageType, float mult)
			{
				DamageType = damageType;
				DefenceMultiplier = mult;
			}
		}

		[SerializeField]
		protected List<DefencePrimitive> defencePrimitives = new List<DefencePrimitive>();

		[Space(30f)]
		public float MinLimit;

		public float MaxLimit = 1f;

		public void Init(int capacity = 10)
		{
			defencePrimitives.Capacity = capacity;
		}

		public float GetValue(DamageType type)
		{
			float result = 0f;
			if (defencePrimitives.Any())
			{
				foreach (DefencePrimitive defencePrimitive in defencePrimitives)
				{
					if (defencePrimitive.DamageType == type)
					{
						result = defencePrimitive.DefenceMultiplier;
					}
				}
				return result;
			}
			return result;
		}

		public void SetValue(DamageType type, float defValue = 0f)
		{
			defValue = Mathf.Clamp(defValue, MinLimit, MaxLimit);
			if (defencePrimitives.Any())
			{
				foreach (DefencePrimitive defencePrimitive in defencePrimitives)
				{
					if (defencePrimitive.DamageType == type)
					{
						defencePrimitive.DefenceMultiplier = defValue;
						return;
					}
				}
			}
			defencePrimitives.Add(new DefencePrimitive(type, defValue));
		}

		public void Set(Defence additionalDefence)
		{
			MinLimit = additionalDefence.MinLimit;
			MaxLimit = additionalDefence.MaxLimit;
			foreach (DefencePrimitive defencePrimitive in additionalDefence.defencePrimitives)
			{
				SetValue(defencePrimitive.DamageType, defencePrimitive.DefenceMultiplier);
			}
		}

		public void Add(Defence additionalDefence)
		{
			MinLimit += additionalDefence.MinLimit;
			MaxLimit += additionalDefence.MaxLimit;
			foreach (DefencePrimitive defencePrimitive in additionalDefence.defencePrimitives)
			{
				SetValue(defencePrimitive.DamageType, GetValue(defencePrimitive.DamageType) + defencePrimitive.DefenceMultiplier);
			}
		}

		public void Sub(Defence additionalDefence)
		{
			MinLimit += additionalDefence.MinLimit;
			MaxLimit += additionalDefence.MaxLimit;
			foreach (DefencePrimitive defencePrimitive in additionalDefence.defencePrimitives)
			{
				SetValue(defencePrimitive.DamageType, GetValue(defencePrimitive.DamageType) - defencePrimitive.DefenceMultiplier);
			}
		}

		public static Defence operator +(Defence val1, Defence val2)
		{
			Defence defence = new Defence();
			defence.Set(val1);
			defence.Add(val2);
			return defence;
		}

		public static Defence operator -(Defence val1, Defence val2)
		{
			Defence defence = new Defence();
			defence.Set(val1);
			defence.Sub(val2);
			return defence;
		}

		public static Defence operator *(Defence val1, float val2)
		{
			Defence defence = new Defence();
			defence.Set(val1);
			foreach (DefencePrimitive defencePrimitive in defence.defencePrimitives)
			{
				defence.SetValue(defencePrimitive.DamageType, defence.GetValue(defencePrimitive.DamageType) * val2);
			}
			return defence;
		}
	}
}
