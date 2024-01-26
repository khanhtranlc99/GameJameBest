using Game.Items;
using UnityEngine;

public class StuffHelper : MonoBehaviour
{
	public Transform HatPlaceholder;

	public Transform GlassPlaceholder;

	public Transform MaskPlaceholder;

	[Space(5f)]
	public Transform LeftBraceletPlaceholder;

	public Transform RightBraceletPlaceholder;

	[Space(5f)]
	public Transform LeftHucklePlaceholder;

	public Transform RightHucklePlaceholder;

	[Space(5f)]
	public Transform LeftPalmPlaceholder;

	public Transform RightPalmPlaceholder;

	[Space(5f)]
	public Transform LeftToePlaceholder;

	public Transform RightToePlaceholder;

	[Space(10f)]
	public SlotRenderer SlotRenderers;

	[Space(10f)]
	public GameItemClothes[] DefaultClotheses;

	[Separator("For moto specific")]
	public Transform LeftUpperArm;

	public Transform LeftForeArm;

	public Transform RightUpperArm;

	public Transform RightForeArm;

	public Transform LeftHand;

	public Transform RightHand;
	
	//New Logic . 
	public SkinnedMeshRenderer meshCharacter;
	public Material[] allNewSkin;
	public GameObject[] allNewAccessories;

	public Transform GetPlaceholder(SkinSlot slot)
	{
		Transform result = null;
		switch (slot)
		{
		case SkinSlot.Glass:
			result = GlassPlaceholder;
			break;
		case SkinSlot.Hat:
			result = HatPlaceholder;
			break;
		case SkinSlot.Mask:
			result = MaskPlaceholder;
			break;
		case SkinSlot.LeftBracelet:
			result = LeftBraceletPlaceholder;
			break;
		case SkinSlot.RightBracelet:
			result = RightBraceletPlaceholder;
			break;
		case SkinSlot.LeftHuckle:
			result = LeftHucklePlaceholder;
			break;
		case SkinSlot.RightHuckle:
			result = RightHucklePlaceholder;
			break;
		case SkinSlot.LeftPalm:
			result = LeftPalmPlaceholder;
			break;
		case SkinSlot.RightPalm:
			result = RightPalmPlaceholder;
			break;
		case SkinSlot.LeftToe:
			result = LeftToePlaceholder;
			break;
		case SkinSlot.RightToe:
			result = RightToePlaceholder;
			break;
		}
		return result;
	}
	

	public void UpdateNewSkin(int index)
	{
		if (index < 0 || index >= allNewSkin.Length)
			index = 0;
		meshCharacter.material = allNewSkin[index];
		//int index = mappingSkinIdWithIndex(id);
		// for (int i = 0; i < allNewSkin.Length; i++)
		// {
		// 	
		// 	//allNewSkin[i].SetActive(i==index);
		// }
	}	public void UpdateNewAcessories(int index)
	{
		//int index = mappingSkinIdWithIndex(id);
		//Debug.LogError("index "+index);
		// for (int i = 0; i < allNewAccessories.Length; i++)
		// {
		// 	allNewAccessories[i].SetActive(i==index);
		// }
	}
}
