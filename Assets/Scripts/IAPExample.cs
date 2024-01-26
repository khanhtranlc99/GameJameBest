using UnityEngine;

public class IAPExample : MonoBehaviour
{
	private void Start()
	{
		IAPController.Items[IAPController.Item.Money100].Callback = delegate(string receipt, bool succeeded)
		{
			if (!succeeded)
			{
			}
		};
		IAPController.Items[IAPController.Item.Money200].Callback = delegate(string receipt, bool succeeded)
		{
			if (!succeeded)
			{
			}
		};
	}
}
