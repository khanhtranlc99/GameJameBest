using Game.Character.Config;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(EmptyConfig))]
	public class EmptyCameraMode : CameraMode
	{
		public override Type Type => Type.None;

		public override void Init()
		{
			base.Init();
			UnityCamera.transform.LookAt(cameraTarget);
			config = GetComponent<EmptyConfig>();
		}
	}
}
