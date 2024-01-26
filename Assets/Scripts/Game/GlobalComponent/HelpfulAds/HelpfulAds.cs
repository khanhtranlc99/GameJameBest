using UnityEngine;

namespace Game.GlobalComponent.HelpfulAds
{
	public class HelpfulAds : MonoBehaviour
	{
		public string Message;

		public virtual HelpfullAdsType HelpType()
		{
			return HelpfullAdsType.None;
		}

		public virtual void HelpAccepted()
		{
		}
	}
}
