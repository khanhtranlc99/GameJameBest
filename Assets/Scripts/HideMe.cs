using Game.Character;
using UnityEngine;

public class HideMe : MonoBehaviour
{
	public GameObject[] Models;

	private bool isHiden;

	private void Update()
	{
		if (!isHiden)
		{
			if (PlayerInteractionsManager.Instance.sitInVehicle)
			{
				GameObject[] models = Models;
				foreach (GameObject gameObject in models)
				{
					gameObject.SetActive(value: false);
					isHiden = true;
				}
			}
		}
		else if (!PlayerInteractionsManager.Instance.sitInVehicle)
		{
			GameObject[] models2 = Models;
			foreach (GameObject gameObject2 in models2)
			{
				gameObject2.SetActive(value: true);
				isHiden = false;
			}
		}
	}
}
