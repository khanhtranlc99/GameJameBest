using UnityEngine;

namespace Game.Character.CharacterController
{
	public class ObstacleHelper
	{
		private const float HeightObstacleLow = 1f;

		private const float HeightObstacleMid = 1.5f;

		private const float HeightObstacleHigh = 3f;

		private const float HeightCharacter = 2f;

		public static Obstacle FindObstacle(Vector3 pos, Vector3 dir, float maxDistance, float maxHeight, LayerMask layerMask)
		{
			Ray ray = new Ray(pos + Vector3.up * 0.5f, dir);
			RaycastHit[] array = Physics.RaycastAll(ray, maxDistance, layerMask);
			UnityEngine.Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow, 1f);
			float num = float.PositiveInfinity;
			Vector3 a = Vector3.zero;
			Vector3 wallNormal = Vector3.zero;
			bool flag = false;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (raycastHit.distance < num && !raycastHit.collider.isTrigger && Vector3.Dot(raycastHit.normal, Vector3.up) < 0.2f)
				{
					num = raycastHit.distance;
					a = raycastHit.point;
					wallNormal = raycastHit.normal;
					flag = true;
				}
			}
			if (flag)
			{
				float distance = num;
				ray.origin = a + dir * 0.25f + Vector3.up * maxHeight;
				ray.direction = Vector3.up * -1f;
				array = Physics.RaycastAll(ray, maxHeight);
				UnityEngine.Debug.DrawRay(ray.origin, ray.direction * maxHeight, Color.yellow, 1f);
				num = float.PositiveInfinity;
				Vector3 vector = Vector3.zero;
				bool flag2 = false;
				RaycastHit[] array3 = array;
				for (int j = 0; j < array3.Length; j++)
				{
					RaycastHit raycastHit2 = array3[j];
					if (raycastHit2.distance < num && !raycastHit2.collider.isTrigger)
					{
						num = raycastHit2.distance;
						vector = raycastHit2.point;
						flag2 = true;
					}
				}
				if (flag2)
				{
					Obstacle result = default(Obstacle);
					result.Distance = distance;
					result.Height = num;
					result.WallPoint = vector;
					result.WallNormal = wallNormal;
					result.Type = GetType(pos, vector);
					return result;
				}
			}
			Obstacle result2 = default(Obstacle);
			result2.Type = ObstacleType.None;
			return result2;
		}

		private static ObstacleType GetType(Vector3 ground, Vector3 wall)
		{
			float num = wall.y - ground.y;
			RaycastHit hitInfo;
			if (Physics.Raycast(wall, Vector3.up, out hitInfo) && Vector3.Distance(hitInfo.point, wall) < 2f)
			{
				return ObstacleType.None;
			}
			if (num < 1f)
			{
				return ObstacleType.ObstacleLow;
			}
			if (num < 1.5f)
			{
				return ObstacleType.ObstacleMedium;
			}
			if (num < 3f)
			{
				return ObstacleType.ObstacleHigh;
			}
			return ObstacleType.None;
		}
	}
}
