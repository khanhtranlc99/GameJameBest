using Game.GlobalComponent;
using Game.Traffic;
using UnityEngine;

namespace Game.Enemy
{
	public class PedestrianHumanoidController : SimpleHumanoidController
	{
		private const float StayTimeout = 4f;

		private SlowUpdateProc slowUpdateProc;

		private RoadPoint currentFromPoint;

		private RoadPoint currentToPoint;

		private int currentLine;

		private float stayingTime;

		public override void Init(BaseNPC controlledNPC)
		{
			base.Init(controlledNPC);
			if (currentFromPoint == null)
			{
				currentFromPoint = TrafficManager.Instance.FindClosestPedestrianPoint(controlledNPC.transform.position);
				if (currentFromPoint != null)
				{
					currentLine = Random.Range(0, currentFromPoint.LineCount) + 1;
					currentToPoint = TrafficManager.BestDestinationPoint(currentFromPoint);
					RecalcMovePoint();
				}
			}
		}

		public override void DeInit()
		{
			currentFromPoint = null;
			currentToPoint = null;
			base.DeInit();
		}

		public void InitPedestrianPath(RoadPoint fromPoint, RoadPoint toPoint, int line)
		{
			currentFromPoint = fromPoint;
			currentToPoint = toPoint;
			currentLine = line;
			RecalcMovePoint();
		}

		private void Awake()
		{
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 0.5f);
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (currentFromPoint != null)
			{
				if (!base.IsMoving && ObstacleSensor.CanMove)
				{
					TrafficManager.Instance.GetNextRoute(ref currentFromPoint, ref currentToPoint, ref currentLine);
					RecalcMovePoint();
				}
				if (stayingTime > 4f)
				{
					RoadPoint roadPoint = currentToPoint;
					currentToPoint = currentFromPoint;
					currentFromPoint = roadPoint;
					RecalcMovePoint();
				}
				if (!base.IsMoving)
				{
					stayingTime += slowUpdateProc.DeltaTime;
				}
				else
				{
					stayingTime = 0f;
				}
			}
		}

		private void RecalcMovePoint()
		{
			Vector3 _;
				Vector3 endLine;
			TrafficManager.Instance.CalcTargetSidewalkPoint(currentFromPoint, currentToPoint, currentLine, out _, out endLine);
			SetMovePoint(endLine);
			stayingTime = 0f;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(MovePoint, 0.5f);
		}
	}
}
