using Game.Character.CollisionSystem;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.Modes
{
	public abstract class CameraMode : MonoBehaviour
	{
		public Transform Target;

		public bool ShowTargetDummy;

		public bool EnableLiveGUI;

		public string DefaultConfiguration = "Default";

		protected CameraCollision collision;

		protected InputManager InputManager;

		protected Camera UnityCamera;

		protected Game.Character.Config.Config config;

		protected Vector3 cameraTarget;

		protected float targetDistance;

		protected bool disableInput;

		private GameObject targetDummy;

		public abstract Type Type
		{
			get;
		}

		public Game.Character.Config.Config Configuration => config;

		public virtual void Init()
		{
			CameraManager instance = CameraManager.Instance;
			UnityCamera = instance.UnityCamera;
			InputManager = InputManager.Instance;
			if (!Target)
			{
				Target = instance.CameraTarget;
			}
			if ((bool)Target)
			{
				cameraTarget = Target.position;
				targetDistance = (UnityCamera.transform.position - Target.position).magnitude;
			}
			collision = CameraCollision.Instance;
		}

		public virtual void OnActivate()
		{
		}

		public virtual void OnDeactivate()
		{
		}

		public virtual void SetCameraTarget(Transform target)
		{
			Target = target;
		}

		public virtual void SetCameraConfigMode(string modeName)
		{
			config.SetCameraMode(modeName);
		}

		public void EnableCollision(bool status)
		{
			if ((bool)collision)
			{
				collision.Enable(status);
			}
		}

		public virtual void Reset()
		{
		}

		public void EnableOrthoCamera(bool status)
		{
			if (status != UnityCamera.orthographic)
			{
				if (status)
				{
					UnityCamera.orthographic = true;
					UnityCamera.orthographicSize = (UnityCamera.transform.position - cameraTarget).magnitude / 2f;
				}
				else
				{
					UnityCamera.orthographic = false;
					UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * UnityCamera.orthographicSize * 2f;
				}
			}
		}

		public bool IsOrthoCamera()
		{
			return UnityCamera.orthographic;
		}

		public void CreateTargetDummy()
		{
			targetDummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			targetDummy.name = "TargetDummy";
			targetDummy.transform.parent = base.gameObject.transform;
			SphereCollider component = targetDummy.GetComponent<SphereCollider>();
			if ((bool)component)
			{
				UnityEngine.Object.Destroy(component);
			}
			Material material = new Material(Shader.Find("Diffuse"));
			material.color = Color.magenta;
			targetDummy.GetComponent<MeshRenderer>().sharedMaterial = material;
			targetDummy.transform.position = cameraTarget;
			targetDummy.SetActive(ShowTargetDummy);
		}

		protected Vector3 GetTargetHeadPos()
		{
			float d = collision.GetHeadOffset();
			Game.Character.Input.Input input = InputManager.GetInput(InputType.Crouch);
			if (input.Valid && (bool)input.Value)
			{
				d = 1.2f;
			}
			if ((bool)Target)
			{
				return Target.position + Vector3.up * d;
			}
			return cameraTarget + Vector3.up * d;
		}

		protected void UpdateTargetDummy()
		{
			Game.Character.Utils.Debug.SetActive(targetDummy, ShowTargetDummy);
			if ((bool)targetDummy)
			{
				float num = (UnityCamera.transform.position - targetDummy.transform.position).magnitude;
				if (num > 70f)
				{
					num = 70f;
				}
				float num2 = num / 70f;
				targetDummy.transform.localScale = new Vector3(num2, num2, num2);
				targetDummy.transform.position = cameraTarget;
			}
		}

		public virtual void GameUpdate()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.O))
			{
				EnableOrthoCamera(!UnityCamera.orthographic);
			}
			UpdateTargetDummy();
			if ((bool)config)
			{
				config.EnableLiveGUI(EnableLiveGUI);
				config.Update();
			}
			if (config.IsBool("Orthographic"))
			{
				EnableOrthoCamera(config.GetBool("Orthographic"));
			}
		}

		public virtual void FixedStepUpdate()
		{
		}

		public virtual void PostUpdate()
		{
		}

		protected float GetZoomFactor()
		{
			float num = 1f;
			num = ((!UnityCamera.orthographic) ? (UnityCamera.transform.position - cameraTarget).magnitude : UnityCamera.orthographicSize);
			if (num > 1f)
			{
				return num / (1f + Mathf.Log(num));
			}
			return num;
		}

		protected void DebugDraw()
		{
			UnityEngine.Debug.DrawLine(UnityCamera.transform.position, cameraTarget, Color.red, 1f);
			UnityEngine.Debug.DrawRay(cameraTarget, UnityCamera.transform.up, Color.green, 1f);
			UnityEngine.Debug.DrawRay(cameraTarget, UnityCamera.transform.right, Color.yellow, 1f);
		}

		private void OnGUI()
		{
			string[] results = Game.Character.Utils.Profiler.GetResults();
			int num = 10;
			int num2 = Screen.width - 300;
			string[] array = results;
			foreach (string text in array)
			{
				GUI.Label(new Rect(num2, num, 500f, 30f), text);
				num += 20;
			}
		}
	}
}
