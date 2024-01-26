using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	[Serializable]
	public class PlayerRelations
	{
		public const float HostileRelationsTreshold = -10f;

		public const float FriendlyRelationsTreshold = 10f;

		public const float OneStarWeight = 2f;

		public Faction NpcFaction;

		[Tooltip("<-10 - Враги; >10 - Друзья; Иначе нейтралы")]
		public float RelationValue;

		public Relations CurrentRelations
		{
			get
			{
				if (RelationValue <= -10f)
				{
					return Relations.Hostile;
				}
				if (RelationValue >= 10f)
				{
					return Relations.Friendly;
				}
				return Relations.Neutral;
			}
			set
			{
				switch (value)
				{
				case Relations.Neutral:
					RelationValue = 0f;
					break;
				case Relations.Hostile:
					RelationValue = -10f;
					break;
				case Relations.Friendly:
					RelationValue = 10f;
					break;
				}
			}
		}

		public void ChangeRelationValue(float value)
		{
			RelationValue += value;
		}
	}
}
