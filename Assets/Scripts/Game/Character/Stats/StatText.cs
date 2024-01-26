using UnityEngine;
using UnityEngine.UI;

namespace Game.Character.Stats
{
	public class StatText : CharacterStatDisplay
	{
		[SerializeField]
		private Text text;

		private Text Text
		{
			get
			{
				if (text == null)
				{
					text = GetComponent<Text>();
					if (text == null)
					{
						UnityEngine.Debug.LogError("Can't find Text");
						base.enabled = false;
					}
				}
				return text;
			}
		}

		protected override void UpdateDisplayValue()
		{
			if (Text != null)
			{
				Text.text = current.ToString("F0");
			}
		}

		public override void OnChanged(float amount)
		{
		}
	}
}
