using Game.Items;
using System;
using UnityEngine;

[Serializable]
public class SlotRenderer
{
	[Space(10f)]
	public SkinnedMeshRenderer HeadRenderer;

	public SkinnedMeshRenderer FaceRenderer;

	[Space(10f)]
	public SkinnedMeshRenderer BodyRenderer;

	[Space(10f)]
	public SkinnedMeshRenderer ArmsRenderer;

	public SkinnedMeshRenderer ForearmsRenderer;

	public SkinnedMeshRenderer HandsRenderer;

	[Space(10f)]
	public SkinnedMeshRenderer ThighsRenderer;

	public SkinnedMeshRenderer ShinsRenderer;

	public SkinnedMeshRenderer FootsRenderer;

	public SkinnedMeshRenderer ExternalBodyRenderer;

	public SkinnedMeshRenderer ExternalFootsRenderer;

	public SkinnedMeshRenderer GetRenderer(SkinSlot slot)
	{
		switch (slot)
		{
		case SkinSlot.Head:
			return HeadRenderer;
		case SkinSlot.Face:
			return FaceRenderer;
		case SkinSlot.Body:
			return BodyRenderer;
		case SkinSlot.Arms:
			return ArmsRenderer;
		case SkinSlot.Forearms:
			return ForearmsRenderer;
		case SkinSlot.Hands:
			return HandsRenderer;
		case SkinSlot.Thighs:
			return ThighsRenderer;
		case SkinSlot.Shins:
			return ShinsRenderer;
		case SkinSlot.Foots:
			return FootsRenderer;
		case SkinSlot.ExternalBody:
			return ExternalBodyRenderer;
		case SkinSlot.ExternalFoots:
			return ExternalFootsRenderer;
		default:
			return null;
		}
	}
}
