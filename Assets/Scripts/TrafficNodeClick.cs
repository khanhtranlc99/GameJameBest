using Game.Traffic;
using UnityEngine;

public class TrafficNodeClick : MonoBehaviour
{
	public bool isWork;

	[Tooltip("Press S to add selected node in source node")]
	public Node sourceNode;

	[Tooltip("Press S to add selected node in linked node")]
	public Node linkedNode;

	public bool sourceNodeIsSet;

	public bool linkedNodeIsSet;

	private NodeLink newLink;

	[InspectorButton("CreateLink")]
	public string linkNodes;

	[InspectorButton("ClearSources")]
	public string clearNodes;

	[InspectorButton("DeleteLink")]
	public string deleteLink;

	public void CreateLink()
	{
		if (sourceNodeIsSet && linkedNodeIsSet)
		{
			newLink.Link = linkedNode;
			sourceNode.NodeLinks.Add(newLink);
			MonoBehaviour.print("New link added in: " + sourceNode.gameObject.name + " to: " + linkedNode.gameObject.name);
			ClearSources();
		}
	}

	public void ClearSources()
	{
		sourceNode = null;
		linkedNode = null;
		sourceNodeIsSet = false;
		linkedNodeIsSet = false;
		newLink = new NodeLink();
	}

	public void DeleteLink()
	{
		if (sourceNodeIsSet)
		{
			if (sourceNode.NodeLinks.Count > 0)
			{
				sourceNode.NodeLinks.RemoveAt(sourceNode.NodeLinks.Count - 1);
				MonoBehaviour.print("Last link in: " + sourceNode.gameObject.name + "was been deleted.");
			}
			else
			{
				MonoBehaviour.print(sourceNode.gameObject.name + " don't have links.");
			}
		}
	}
}
