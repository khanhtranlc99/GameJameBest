using Game.Traffic;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Game.Race.RaceTrackManager
{
	[RequireComponent(typeof(Node))]
	public class RaceNodeHelper : MonoBehaviour
	{
		public static void CloneNode()
		{
		}

		public static void LinkNodes()
		{
		}

		public static void UnlinkNodes()
		{
		}

		private static NodeLink FindLink(List<NodeLink> linkList, Node exampleNode)
		{
			NodeLink result = null;
			foreach (NodeLink link in linkList)
			{
				if (link.Link == exampleNode)
				{
					result = link;
				}
			}
			return result;
		}
	}
}
