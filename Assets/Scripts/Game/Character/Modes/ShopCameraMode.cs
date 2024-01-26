using UnityEngine;

namespace Game.Character.Modes
{
	public class ShopCameraMode : CameraMode
	{
		public float Indent = -2f;

		public float CameraSpeed = 0.01f;

		private GameObject positionFinder;

		public override Type Type => Type.Shop;

		public override void Init()
		{
			base.Init();
			PositionFinderInit();
		}

		private void PositionFinderInit()
		{
			if (!positionFinder)
			{
				positionFinder = new GameObject("Position Finder");
			}
			positionFinder.transform.parent = Target.parent;
			positionFinder.transform.localRotation = Target.localRotation;
			Transform transform = positionFinder.transform;
			Vector3 localPosition = Target.transform.localPosition;
			float y = localPosition.y;
			Vector3 localPosition2 = Target.transform.localPosition;
			transform.localPosition = new Vector3(-2f, y, localPosition2.z);
			positionFinder.transform.LookAt(Target);
		}

		public override void SetCameraTarget(Transform target)
		{
			base.SetCameraTarget(target);
			PositionFinderInit();
		}

		public override void GameUpdate()
		{
			if ((bool)positionFinder)
			{
				UnityCamera.transform.position = Vector3.Lerp(UnityCamera.transform.position, positionFinder.transform.position, CameraSpeed);
				UnityCamera.transform.rotation = Quaternion.Slerp(UnityCamera.transform.rotation, positionFinder.transform.rotation, CameraSpeed);
			}
		}

		public override void SetCameraConfigMode(string modeName)
		{
		}
	}
}
