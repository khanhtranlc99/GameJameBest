using Game.Items;
using UnityEngine;

//[CreateAssetMenu(fileName = "ItemClotherData", menuName = "RopeData/ItemData/Clother", order = 100)]
public class GameItemClothes : GameItemSkin
{
	[Space(10f)]
	public Material[] SkinMaterials;

	public Mesh SkinMesh;

	public GameItemAccessory[] RelatedAccesories;

	public bool HideByDefault;
}
