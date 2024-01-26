using Game.GlobalComponent.Qwest;
using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class TimeAds : HelpfulAds
	{
		[Range(0f, 1f)]
		public float ProcentTime = 0.25f;

		public override HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.Time;
		}

		public override void HelpAccepted()
		{
			base.HelpAccepted();
			QwestTimerManager.Instance.AddAdditionalTimeOfProcentMaxTime(ProcentTime);
		}
	}
}
