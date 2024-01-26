using Game.Character.CharacterController;
using Game.Character.Config;
using System;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class SimpleCollision : ViewCollision
	{
		private Ray ray;

		private RaycastHit[] hits;

		private readonly RayHitComparer rayHitComparer;

		public SimpleCollision(Game.Character.Config.Config config)
			: base(config)
		{
			rayHitComparer = new RayHitComparer();
		}

		public override float Process(Vector3 cameraTarget, Vector3 dir, float distance)
		{
			float value = distance;
			float @float = config.GetFloat("RaycastTolerance");
			float float2 = config.GetFloat("MinDistance");
			float num = float.PositiveInfinity;
			ray.origin = cameraTarget;
			ray.direction = -dir;
			hits = Physics.RaycastAll(ray, distance + @float);
			Array.Sort(hits, rayHitComparer);
			string @string = config.GetString("IgnoreCollisionTag");
			string string2 = config.GetString("TransparentCollisionTag");
			RaycastHit[] array = hits;
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				CollisionClass collisionClass = ViewCollision.GetCollisionClass(raycastHit.collider, @string, string2);
				if (raycastHit.distance < num && collisionClass == CollisionClass.Collision)
				{
					num = raycastHit.distance;
					value = raycastHit.distance - @float;
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
