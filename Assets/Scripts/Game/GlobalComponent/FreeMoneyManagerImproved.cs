using Game.GlobalComponent.HelpfulAds;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class FreeMoneyManagerImproved : FreeMoneyManager
	{
		public bool HideOnDeactivate;

		private Button button;

		protected override void Awake()
		{
			//slowUpdateProc = new SlowUpdateProc((SlowUpdateProc.SlowUpdateDelegate)((FreeMoneyManager)this).SlowUpdate, 1f);
			button = MoneyButton.GetComponent<Button>();
			if (button == null)
			{
				button = MoneyButton.GetComponentInChildren<Button>();
			}
		}

		public override void ButtonClick()
		{
			if (ByGems)
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeGems, null);
			}
			else
			{
				HelpfullAdsManager.Instance.OfferAssistance(HelpfullAdsType.FreeMoney, null);
			}
			if (HideOnDeactivate)
			{
				MoneyButton.SetActive(value: false);
			}
			else if (button != null)
			{
				button.interactable = false;
			}
		}

		protected override void SlowUpdate()
		{
			//bool isReady = HelpfullAdsManager.Instance.IsReady;
			//if (HideOnDeactivate)
			//{
			//	MoneyButton.SetActive(isReady);
			//}
			//else if (button != null)
			//{
			//	button.interactable = isReady;
			//}
		}
	}
}
