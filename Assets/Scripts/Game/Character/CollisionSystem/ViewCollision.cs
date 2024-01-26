using Game.Character.Config;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public abstract class ViewCollision
	{
		public enum CollisionClass
		{
			Collision,
			Trigger,
			Ignore,
			IgnoreTransparent
		}

		protected readonly Game.Character.Config.Config config;

		protected ViewCollision(Game.Character.Config.Config config)
		{
			this.config = config;
		}

		public abstract float Process(Vector3 cameraTarget, Vector3 cameraDir, float distance);

		public static CollisionClass GetCollisionClass(Collider collider, string ignoreTag, string transparentTag)
		{
			CollisionClass result = CollisionClass.Collision;
			if (collider.isTrigger)
			{
				result = CollisionClass.Trigger;
			}
			else
			{
				IgnoreCollision component = collider.gameObject.GetComponent<IgnoreCollision>();
				if (((bool)component && component.IsWorkingForCurrentCamera) || collider.gameObject.CompareTag(ignoreTag))
				{
					result = CollisionClass.Ignore;
				}
				else if ((bool)collider.gameObject.GetComponent<TransparentCollision>() || collider.gameObject.CompareTag(transparentTag))
				{
					result = CollisionClass.IgnoreTransparent;
				}
			}
			return result;
		}

		protected void UpdateTransparency(Collider collider)
		{
			TransparencyManager.Instance.UpdateObject(collider.gameObject);
		}
	}
}
