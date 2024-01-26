using Game.Character.CharacterController;
using Game.Character.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class VolumetricCollision : ViewCollision
	{
		private const float offset = -0.5f;

		private const int maxRays = 10;

		private readonly List<RaycastHit> hits;

		private readonly Ray[] rays;

		private readonly RayHitComparer rayHitComparer;

		private readonly RaycastHit[] currentHits = new RaycastHit[10];

		public VolumetricCollision(Game.Character.Config.Config config)
			: base(config)
		{
			rayHitComparer = new RayHitComparer();
			hits = new List<RaycastHit>(40);
			rays = new Ray[10];
			for (int i = 0; i < rays.Length; i++)
			{
				rays[i] = new Ray(Vector3.zero, Vector3.zero);
			}
		}

		public override float Process(Vector3 cameraTarget, Vector3 dir, float distance)
		{
			float value = distance;
			float @float = config.GetFloat("RaycastTolerance");
			float float2 = config.GetFloat("MinDistance");
			Vector2 vector = config.GetVector2("ConeRadius");
			float float3 = config.GetFloat("ConeSegments");
			Vector3 a = cameraTarget - dir * distance;
			Vector3 a2 = Vector3.Cross(dir, Vector3.up);
			Vector3 vector2 = Vector3.zero;
			for (int i = 0; (float)i < float3; i++)
			{
				float angle = (float)i / float3 * 360f;
				Quaternion rotation = Quaternion.AngleAxis(angle, dir);
				Vector3 vector3 = cameraTarget + rotation * (a2 * vector.x);
				Vector3 a3 = a + rotation * (a2 * vector.y);
				vector2 = a3 - vector3;
				rays[i].origin = vector3;
				switch (i)
				{
				case 0:
					rays[i].direction = a3 - vector3 + Vector3.left;
					break;
				case 1:
					rays[i].direction = a3 - vector3 + Vector3.forward;
					break;
				case 2:
					rays[i].direction = a3 - vector3 + Vector3.right;
					break;
				case 3:
					rays[i].direction = a3 - vector3 + Vector3.back;
					break;
				}
			}
			float magnitude = vector2.magnitude;
			hits.Clear();
			for (int j = 0; j < rays.Length; j++)
			{
				Ray ray = rays[j];
				int num = Physics.RaycastNonAlloc(ray, currentHits, magnitude + @float);
				for (int k = 0; k < num; k++)
				{
					hits.Add(currentHits[k]);
				}
			}
			hits.Sort(rayHitComparer);
			float num2 = float.PositiveInfinity;
			string @string = config.GetString("IgnoreCollisionTag");
			string string2 = config.GetString("TransparentCollisionTag");
			for (int l = 0; l < hits.Count; l++)
			{
				RaycastHit raycastHit = hits[l];
				CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
				if (raycastHit.distance < num2 && collisionClass == CollisionClass.Collision)
				{
					num2 = raycastHit.distance;
					value = raycastHit.distance - @float + -0.5f;
				}
				if (collisionClass == CollisionClass.IgnoreTransparent)
				{
					UpdateTransparency(raycastHit.collider);
				}
			}
			return Mathf.Clamp(value, float2, distance);
		}
	}
}
