using Game.Character;
using Game.GlobalComponent.Quality;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class UnderwaterEffect : MonoBehaviour
	{
		private static UnderwaterEffect instance;

		public Color FogColor;

		public float FogDensity;

		public float FogStartDistance;

		private Color noramalColor;

		private Camera gameCamera;

		private float depth = -1000f;

		private bool effectEnabled;

		public static UnderwaterEffect Instance
		{
			get
			{
				if (!instance)
				{
					instance = UnityEngine.Object.FindObjectOfType<UnderwaterEffect>();
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			noramalColor = RenderSettings.fogColor;
		}

		private void Start()
		{
			gameCamera = CameraManager.Instance.UnityCamera;
		}

		public void SetDepth(float d)
		{
			depth = d;
		}

		private void UnderwaterCameraEffect()
		{
			Vector3 position = gameCamera.transform.position;
			if (position.y + 0.1f <= depth)
			{
				EnableEffect();
				return;
			}
			DisableEffect();
			depth = -1000f;
		}

		public void EnableEffect()
		{
			if (!effectEnabled)
			{
				if (gameCamera == null)
				{
					gameCamera = CameraManager.Instance.UnityCamera;
				}
				RenderSettings.fogEndDistance = FogStartDistance;
				RenderSettings.fogStartDistance = FogStartDistance / 4f;
				RenderSettings.fogColor = FogColor;
				gameCamera.clearFlags = CameraClearFlags.Color;
				gameCamera.backgroundColor = FogColor;
				effectEnabled = true;
			}
		}

		public void DisableEffect()
		{
			if (effectEnabled)
			{
				RenderSettings.fogColor = noramalColor;
				QualityManager.ChangeFog();
				if ((bool)gameCamera)
				{
					gameCamera.clearFlags = CameraClearFlags.Skybox;
				}
				effectEnabled = false;
			}
		}

		private void Update()
		{
			UnderwaterCameraEffect();
		}
	}
}
