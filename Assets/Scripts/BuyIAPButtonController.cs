using UnityEngine;

public class BuyIAPButtonController : MonoBehaviour
{
	public IAPController.Item Item;

	public void Click()
	{
		IAPController.Buy(Item);
	}
}
