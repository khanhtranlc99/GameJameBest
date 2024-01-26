using Game.GlobalComponent.HelpfulAds;
using Root.Scripts.Helper;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class FreeMoneyManager : MonoBehaviour
	{
		public bool ByGems;
		public MoneyAds helper;
		public bool showImmidiately;

		public GameObject MoneyButton;

		protected SlowUpdateProc slowUpdateProc;

		public float UpdateTime => HelpfullAdsManager.Instance.HelpTimerLength;

		protected virtual void Awake()
		{
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 1f);
		}

		protected void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		public virtual void ButtonClick()
		{
			if (showImmidiately && helper!=null)
			{
				//AdManager.Instance.ShowRewardVideo(OnWatchDone);
				return;
			}
			if (ByGems)
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeGems, null);
			}
			else
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeMoney, null);
			}
		}

		private void OnWatchDone(bool isDone)
		{
			if(!isDone) return;
			helper.HelpAccepted();
		}

		protected virtual void SlowUpdate()
		{
			//MoneyButton.SetActive(HelpfullAdsManager.Instance.IsReady);
		}
	}
}
