using UnityEngine;
using UnityEngine.UI;

public class OfflineAdsBanner : MonoBehaviour
{
	public Sprite Land;

	public Sprite Port;

	public void SetOrientation(ScreenOrientation orientation)
	{
		Image component = GetComponent<Image>();
		if (!(component == null))
		{
			switch (orientation)
			{
			case ScreenOrientation.Portrait:
				component.sprite = Port;
				break;
			case ScreenOrientation.LandscapeLeft:
				component.sprite = Land;
				break;
			}
		}
	}

	public void Click()
	{
		IAPController.Buy(IAPController.Item.NoAds);
	}
}
