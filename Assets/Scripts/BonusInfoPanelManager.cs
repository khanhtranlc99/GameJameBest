using System.Collections.Generic;
using UnityEngine;

public class BonusInfoPanelManager : MonoBehaviour
{
	private static BonusInfoPanelManager instance;

	public GameObject InfoPrimitivePrefab;

	public GameObject GetBonusButton;

	public GameObject ContentContainer;

	public static BonusInfoPanelManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<BonusInfoPanelManager>();
			}
			return instance;
		}
	}

	public void ShowInfo(Dictionary<string, Sprite> output, bool isAvailable)
	{
		int childCount = ContentContainer.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			UnityEngine.Object.Destroy(ContentContainer.transform.GetChild(i));
		}
	}
}
