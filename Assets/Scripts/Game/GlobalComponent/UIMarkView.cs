using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class UIMarkView : UIMarkViewBase
	{
		[SerializeField]
		private Text m_DistanceLabel;

		[SerializeField]
		private Color m_ColorA;

		[SerializeField]
		private Color m_ColorB;

		[SerializeField]
		private AnimationCurve m_Curve;

		private int lastDistance;

		private void Update()
		{
			base.IconColor = Color.Lerp(m_ColorA, m_ColorB, m_Curve.Evaluate(Time.unscaledTime));
		}

		public void UpdateDistanceLabel(float dist)
		{
			if (lastDistance != (int)dist)
			{
				lastDistance = (int)dist;
				m_DistanceLabel.text = lastDistance + " M";
			}
		}
	}
}
