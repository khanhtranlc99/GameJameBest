using Game.Items;
using UnityEngine;
//[CreateAssetMenu(fileName = "ItemItemPackData", menuName = "RopeData/ItemData/ItemPack", order = 100)]
public class GameItemPack : GameItem
{
	public GameItem[] PackedItems;

	public override bool SameParametrWithOther(object[] parametrs)
	{
		return PackedItems == (GameItem[])parametrs[0];
	}
}
