using UnityEngine;

namespace Game.Traffic
{
	public class RoadPoint
	{
		public Vector3 Point;

		public RoadLink[] RoadLinks;

		public int LineCount;

		public float SpeedLimit = float.MaxValue;

		public float GetSpaceLineWidthToNode(RoadPoint toNode)
		{
			float result = 0f;
			RoadLink[] roadLinks = RoadLinks;
			foreach (RoadLink roadLink in roadLinks)
			{
				if (roadLink.Link == toNode)
				{
					result = roadLink.SpacerLineWidth;
					break;
				}
			}
			return result;
		}
	}
}
