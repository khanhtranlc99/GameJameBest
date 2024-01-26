using UnityEngine;

namespace Game.Items
{
	//[CreateAssetMenu(fileName = "ItemAccessoryData", menuName = "RopeData/ItemData/Accessory", order = 100)]
	public class GameItemAccessory : GameItemSkin
	{
		[Space(10f)]
		public GameObject ModelPrefab;

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return ModelPrefab == (GameObject)parametrs[0];
		}
	}
}
