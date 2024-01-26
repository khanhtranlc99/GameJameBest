using Game.GlobalComponent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Traffic
{
	public class RoadMapPreprocessor : MonoBehaviour
	{
		private HashSet<RoadPoint> points = new HashSet<RoadPoint>();

		private HashSet<Node> usedNodes = new HashSet<Node>();

		private HashSet<RoadPoint> usedRPs = new HashSet<RoadPoint>();

		private IDictionary<Node, RoadPoint> nodeToPoints = new Dictionary<Node, RoadPoint>();

		private string serializedMap;

		private string PrebuildRoadPoints(Node[] nodes)
		{
			if (nodes == null)
			{
				return string.Empty;
			}
			usedNodes.Clear();
			nodeToPoints.Clear();
			points.Clear();
			foreach (Node node in nodes)
			{
				NodeBypass(node);
			}
			usedRPs.Clear();
			foreach (RoadPoint value in nodeToPoints.Values)
			{
				RoadPointDistanceBypass(value);
			}
			return MiamiSerializier.JSONSerialize(points.ToArray());
		}
		public RoadPoint[] GetRoadPointArray(Node[] nodes)
		{
			if (nodes == null)
			{
				return null;
			}
			usedNodes.Clear();
			nodeToPoints.Clear();
			points.Clear();
			foreach (Node node in nodes)
			{
				NodeBypass(node);
			}
			usedRPs.Clear();
			foreach (RoadPoint value in nodeToPoints.Values)
			{
				RoadPointDistanceBypass(value);
			}
			return points.ToArray();
		}

		private void NodeBypass(Node node)
		{
			if (!usedNodes.Contains(node) && node.NodeLinks != null && node.NodeLinks.Count != 0)
			{
				usedNodes.Add(node);
				RoadPoint roadPoint = new RoadPoint();
				roadPoint.Point = node.transform.position;
				roadPoint.LineCount = node.LineCount;
				roadPoint.SpeedLimit = ((!node.SpeedLimit.Equals(float.PositiveInfinity)) ? node.SpeedLimit : float.MaxValue);
				roadPoint.RoadLinks = new RoadLink[node.NodeLinks.Count];
				points.Add(roadPoint);
				nodeToPoints.Add(node, roadPoint);
				int num = 0;
				foreach (Node linkedNodes in node.GetLinkedNodesList())
				{
					if (!usedNodes.Contains(linkedNodes))
					{
						NodeBypass(linkedNodes);
					}
					RoadPoint link = nodeToPoints[linkedNodes];
					roadPoint.RoadLinks[num] = new RoadLink
					{
						Link = link,
						SpacerLineWidth = node.GetSpaceLineWidthToNode(linkedNodes)
					};
					num++;
				}
			}
		}

		private void RoadPointDistanceBypass(RoadPoint rp)
		{
			if (usedRPs.Contains(rp))
			{
				return;
			}
			usedRPs.Add(rp);
			int num = 0;
			for (int i = 0; i < rp.RoadLinks.Length; i++)
			{
				RoadPoint link = rp.RoadLinks[i].Link;
				if (!usedRPs.Contains(link))
				{
					float num2 = Vector3.Distance(link.Point, rp.Point);
					if (num2 > TrafficManager.NodesMaxDistance)
					{
						int num3 = (int)(num2 / TrafficManager.NodesMaxDistance);
						float d = num2 / (float)(num3 + 1);
						Vector3 normalized = (link.Point - rp.Point).normalized;
						RoadPoint roadPoint = rp;
						float spacerLineWidth = (link.GetSpaceLineWidthToNode(rp) + rp.GetSpaceLineWidthToNode(link)) / 2f;
						for (int j = 0; j < num3; j++)
						{
							RoadPoint roadPoint2 = new RoadPoint();
							usedRPs.Add(roadPoint2);
							points.Add(roadPoint2);
							roadPoint2.LineCount = rp.LineCount;
							roadPoint2.Point = rp.Point + normalized * d * (j + 1);
							roadPoint2.RoadLinks = new RoadLink[2];
							roadPoint2.RoadLinks[0] = new RoadLink
							{
								Link = roadPoint,
								SpacerLineWidth = spacerLineWidth
							};
							if (roadPoint.Equals(rp))
							{
								roadPoint.RoadLinks[i].Link = roadPoint2;
								roadPoint.RoadLinks[i].SpacerLineWidth = spacerLineWidth;
							}
							else
							{
								roadPoint.RoadLinks[1] = new RoadLink
								{
									Link = roadPoint2,
									SpacerLineWidth = spacerLineWidth
								};
							}
							roadPoint = roadPoint2;
						}
						for (int k = 0; k < link.RoadLinks.Length; k++)
						{
							if (link.RoadLinks[k].Link.Equals(rp))
							{
								link.RoadLinks[k].Link = roadPoint;
								link.RoadLinks[k].SpacerLineWidth = spacerLineWidth;
								roadPoint.RoadLinks[1] = new RoadLink
								{
									Link = link,
									SpacerLineWidth = spacerLineWidth
								};
								break;
							}
						}
					}
				}
				num++;
			}
		}

		public void Rebuild()
		{
		}

		private static void CureNodes(Node[] nodes)
		{
			foreach (Node node in nodes)
			{
				List<NodeLink> nodeLinks = node.NodeLinks;
				List<int> list = new List<int>();
				int num = 0;
				foreach (NodeLink item in nodeLinks)
				{
					if (item.Link == null || item.Link == node)
					{
						list.Add(num);
					}
					else
					{
						List<Node> linkedNodesList = item.Link.GetLinkedNodesList();
						if (!linkedNodesList.Contains(node))
						{
							item.Link.NodeLinks.Add(new NodeLink
							{
								Link = node,
								SpacerLineWidth = item.SpacerLineWidth
							});
						}
					}
					num++;
				}
				for (int num2 = list.Count - 1; num2 >= 0; num2--)
				{
					int index = list[num2];
					nodeLinks.RemoveAt(index);
				}
			}
		}
	}
}
