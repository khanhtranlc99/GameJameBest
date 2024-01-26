using UnityEngine;

public class SelectiveStringAttribute : PropertyAttribute
{
	public readonly string ValidTagsContainerName;

	public SelectiveStringAttribute(string tagsContainerName)
	{
		ValidTagsContainerName = tagsContainerName;
	}
}
