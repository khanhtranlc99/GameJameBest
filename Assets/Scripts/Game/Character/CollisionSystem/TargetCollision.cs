using Game.Character.CharacterController;
using Game.Character.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class TargetCollision
	{
		private const string Ignorecollisiontag = "IgnoreCollisionTag";

		private const string Transparentcollisiontag = "TransparentCollisionTag";

		private const string Targetsphereradius = "TargetSphereRadius";

		private readonly List<RaycastHit> hits = new List<RaycastHit>();

		private readonly RayHitComparer rayHitComparer;

		private readonly Game.Character.Config.Config config;

		private readonly RaycastHit[] currentHits = new RaycastHit[10];

		public TargetCollision(Game.Character.Config.Config config)
		{
			rayHitComparer = new RayHitComparer();
			this.config = config;
		}

		public float CalculateTarget(Vector3 targetHead, Vector3 cameraTarget)
		{
			string @string = config.GetString("IgnoreCollisionTag");
			string string2 = config.GetString("TransparentCollisionTag");
			float @float = config.GetFloat("TargetSphereRadius");
			float num = 1f;
			Vector3 normalized = (cameraTarget - targetHead).normalized;
			Ray ray = new Ray(targetHead, normalized);
			hits.Clear();
			int num2 = Physics.RaycastNonAlloc(ray, currentHits, normalized.magnitude + @float);
			for (int i = 0; i < num2; i++)
			{
				hits.Add(currentHits[i]);
			}
			hits.Sort(rayHitComparer);
			float num3 = float.PositiveInfinity;
			bool flag = false;
			for (int j = 0; j < hits.Count; j++)
			{
				RaycastHit raycastHit = hits[j];
				ViewCollision.CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
				if (raycastHit.distance < num3 && collisionClass == ViewCollision.CollisionClass.Collision)
				{
					num3 = raycastHit.distance;
					num = raycastHit.distance - @float;
					flag = true;
				}
			}
			if (flag)
			{
				return Mathf.Clamp01(num / (targetHead - cameraTarget).magnitude);
			}
			return 1f;
		}
	}
}
