using System;
using UnityEngine;

namespace Game.Character.Stats
{
	[Serializable]
	public class AdditionalFeature
	{
		[SerializeField]
		private Sprite sprite;

		[SerializeField]
		private string description;

		public AdditionalFeature(Sprite sprite, string description)
		{
			this.sprite = sprite;
			this.description = description;
		}

		public Sprite GetSprite()
		{
			return sprite;
		}

		public string GetDescription()
		{
			return description;
		}
	}
}
