using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.Managers.Shop.Prefabs
{
	public class StatViewVehicles : StatView
	{
		[SerializeField]
		private Image fillImage;

		private float fillImageMaxValue;

		public void SetMaxAmount(float maxValue)
		{
			fillImageMaxValue = maxValue;
		}

		public override void SetValue(float value)
		{
			fillImage.fillAmount = value / fillImageMaxValue;
			base.value.text = string.Empty + value;
		}
	}
}
