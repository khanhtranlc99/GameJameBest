using Game.Character.Config;
using UnityEngine;

namespace Game.Character.CollisionSystem
{
	[RequireComponent(typeof(CollisionConfig))]
	public class CameraCollision : MonoBehaviour
	{
		private const float DistanceToSwitchNearClipPlanes = 2f;

		private const float LowDistanceNearClipPlanes = 0.07f;

		private Camera unityCamera;

		private Game.Character.Config.Config config;

		private TargetCollision targetCollision;

		private SimpleCollision simpleCollision;

		private VolumetricCollision volumetricCollision;

		private SphericalCollision sphericalCollision;

		private bool Enabled;

		private float standartNeapClipPlane;

		private float cameraNearClipPlanes;

		public static CameraCollision Instance
		{
			get;
			private set;
		}

		private void Awake()
		{
			Instance = this;
			Enabled = true;
			unityCamera = CameraManager.Instance.UnityCamera;
			config = GetComponent<CollisionConfig>();
			targetCollision = new TargetCollision(config);
			simpleCollision = new SimpleCollision(config);
			sphericalCollision = new SphericalCollision(config);
			volumetricCollision = new VolumetricCollision(config);
		}

		private void Start()
		{
			standartNeapClipPlane = config.GetFloat("NearClipPlane");
			unityCamera.nearClipPlane = standartNeapClipPlane;
			cameraNearClipPlanes = standartNeapClipPlane;
		}

		private ViewCollision GetCollisionAlgorithm(string algorithm)
		{
			switch (algorithm)
			{
			case "Simple":
				return simpleCollision;
			case "Spherical":
				return sphericalCollision;
			case "Volumetric":
				return volumetricCollision;
			default:
				return null;
			}
		}

		public void SetCollisionConfig(string modeName)
		{
			config.SetCameraMode(modeName);
		}

		public void Enable(bool status)
		{
			Enabled = status;
		}

		public void ProcessCollision(Vector3 cameraTarget, Vector3 targetHead, Vector3 dir, float distance, out float collisionTarget, out float collisionDistance)
		{
			if (!Enabled)
			{
				collisionTarget = 1f;
				collisionDistance = distance;
			}
			else
			{
				collisionTarget = targetCollision.CalculateTarget(targetHead, cameraTarget);
				ViewCollision collisionAlgorithm = GetCollisionAlgorithm(config.GetSelection("CollisionAlgorithm"));
				Vector3 cameraTarget2 = cameraTarget * collisionTarget + targetHead * (1f - collisionTarget);
				collisionDistance = collisionAlgorithm.Process(cameraTarget2, dir, distance);
			}
			ControllCameraNearClipPlanes(collisionDistance);
		}

		public float GetRaycastTolerance()
		{
			return config.GetFloat("RaycastTolerance");
		}

		public float GetClipSpeed()
		{
			return config.GetFloat("ClipSpeed");
		}

		public float GetTargetClipSpeed()
		{
			return config.GetFloat("TargetClipSpeed");
		}

		public float GetReturnSpeed()
		{
			return config.GetFloat("ReturnSpeed");
		}

		public float GetReturnTargetSpeed()
		{
			return config.GetFloat("ReturnTargetSpeed");
		}

		public float GetHeadOffset()
		{
			return config.GetFloat("HeadOffset");
		}

		public ViewCollision.CollisionClass GetCollisionClass(Collider coll)
		{
			string @string = config.GetString("IgnoreCollisionTag");
			string string2 = config.GetString("TransparentCollisionTag");
			return ViewCollision.GetCollisionClass(coll, @string, string2);
		}

		private void ControllCameraNearClipPlanes(float collisionDistance)
		{
			float num = (!(collisionDistance < 2f)) ? standartNeapClipPlane : 0.07f;
			if (num != cameraNearClipPlanes)
			{
				unityCamera.nearClipPlane = num;
				cameraNearClipPlanes = num;
			}
		}
	}
}
