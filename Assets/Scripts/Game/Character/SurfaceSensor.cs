using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	public class SurfaceSensor : WaterSensor
	{
		[Space(10f)]
		public bool PSS_DebugLog;

		[Space(10f)]
		public LayerMask SurfaceLayerMask;

		public Vector3 TransformPositionOffset = Vector3.zero;

		public float CheckingRayLenght = 3f;

		private readonly List<RaycastHit> surfaceRaycastHits = new List<RaycastHit>();

		private readonly RaycastHit[] currentHits = new RaycastHit[10];

		private float groundHeight;

		public float CurrGroundSurfaceHeight => groundHeight;

		public SurfaceStatePack CurrSurfaceStatePack => currSurfaceStatePack;

		public bool AboveGround
		{
			get
			{
				return currSurfaceStatePack.AboveGround;
			}
			set
			{
				currSurfaceStatePack.AboveGround = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			groundHeight = waterHeight;
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		protected override void CheckSurface()
		{
			base.CheckSurface();
			UnityEngine.Debug.DrawRay(base.transform.position + TransformPositionOffset, -Vector3.up * CheckingRayLenght, Color.blue);
			surfaceRaycastHits.Clear();
			int num = Physics.RaycastNonAlloc(base.transform.position + TransformPositionOffset, -Vector3.up, currentHits, CheckingRayLenght, SurfaceLayerMask);
			for (int i = 0; i < num; i++)
			{
				surfaceRaycastHits.Add(currentHits[i]);
			}
			currSurfaceStatePack.SetTypePack(false, false, currSurfaceStatePack.InWater);
			if (surfaceRaycastHits.Count > 0)
			{
				for (int j = 0; j < surfaceRaycastHits.Count; j++)
				{
					RaycastHit raycastHit = surfaceRaycastHits[j];
					if (raycastHit.collider.gameObject.layer == WaterSensor.WaterLayerNumber)
					{
						if (!base.InWater)
						{
							if (!currSurfaceStatePack.AboveWater)
							{
								Vector3 point = raycastHit.point;
								base.waterHeight = point.y;
							}
							float waterHeight = base.waterHeight;
							Vector3 point2 = raycastHit.point;
							float waterHeight2;
							if (waterHeight > point2.y)
							{
								waterHeight2 = base.waterHeight;
							}
							else
							{
								Vector3 point3 = raycastHit.point;
								waterHeight2 = point3.y;
							}
							base.waterHeight = waterHeight2;
						}
						currSurfaceStatePack.AboveWater = true;
					}
					else
					{
						if (!currSurfaceStatePack.AboveGround)
						{
							Vector3 point4 = raycastHit.point;
							groundHeight = point4.y;
						}
						float num2 = groundHeight;
						Vector3 point5 = raycastHit.point;
						float y;
						if (num2 > point5.y)
						{
							y = groundHeight;
						}
						else
						{
							Vector3 point6 = raycastHit.point;
							y = point6.y;
						}
						groundHeight = y;
						currSurfaceStatePack.AboveGround = true;
					}
				}
			}
			if (currSurfaceStatePack.InWater)
			{
				currSurfaceStatePack.AboveWater = false;
			}
		}
	}
}
