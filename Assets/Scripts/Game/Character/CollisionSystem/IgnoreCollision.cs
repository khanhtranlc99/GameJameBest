using Game.Character.Modes;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	public class IgnoreCollision : MonoBehaviour
	{
		[Header("If empty, works for all mods")]
		public Type[] NeededCameraMods;

		public bool IsWorkingForCurrentCamera
		{
			get
			{
				if (NeededCameraMods == null || NeededCameraMods.Length == 0)
				{
					return true;
				}
				Type type = CameraManager.Instance.GetCurrentCameraMode().Type;
				for (int i = 0; i < NeededCameraMods.Length; i++)
				{
					Type type2 = NeededCameraMods[i];
					if (type2 == type)
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}
