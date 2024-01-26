using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using System;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(RTSConfig))]
	public class RTSCameraMode : CameraMode
	{
		private float rotX;

		private float rotY;

		private float targetZoom;

		private Plane groundPlane;

		private bool panning;

		private Vector3 panMousePosition;

		private Vector3 panCameraTarget;

		private Vector3 panCameraPos;

		private float activateTimeout;

		private Vector3 dragVelocity;

		private float dragSlowdown;

		private bool updatePanTarget;

		public override Type Type => Type.RTS;

		public override void Init()
		{
			base.Init();
			UnityCamera.transform.LookAt(cameraTarget);
			config = GetComponent<RTSConfig>();
			Game.Character.Utils.DebugDraw.Enabled = true;
		}

		public override void OnActivate()
		{
			base.OnActivate();
			config.SetCameraMode(DefaultConfiguration);
			targetDistance = config.GetFloat("Distance");
			groundPlane = new Plane(Vector3.up, config.GetFloat("GroundOffset"));
			if ((bool)Target)
			{
				cameraTarget = Target.position;
			}
			UpdateYAngle();
			UpdateXAngle(force: true);
			activateTimeout = 2f;
		}

		public override void SetCameraTarget(Transform target)
		{
			base.SetCameraTarget(target);
			if ((bool)target)
			{
				cameraTarget = target.position;
			}
		}

		private bool RotateCamera(Vector2 mousePosition)
		{
			if (config.GetBool("EnableRotation") && mousePosition.sqrMagnitude > Mathf.Epsilon)
			{
				rotX += config.GetFloat("RotationSpeed") * mousePosition.x * 0.01f;
				updatePanTarget = true;
				return true;
			}
			return false;
		}

		private void DragCamera(Vector2 mousePosition)
		{
			if (panning)
			{
				UnityCamera.transform.position = panCameraPos;
				Ray ray = UnityCamera.ScreenPointToRay(mousePosition);
				float enter = 0f;
				Vector3 a = (!groundPlane.Raycast(ray, out enter)) ? (ray.origin + ray.direction * targetDistance) : (ray.origin + ray.direction * enter);
				Vector3 b = a - panMousePosition;
				b.y = 0f;
				Vector3 point = panCameraTarget - b;
				ClampWithinMapBounds(cameraTarget, ref point, border: true);
				dragVelocity = point - cameraTarget;
				dragSlowdown = 1f;
				cameraTarget = point;
			}
			else
			{
				panCameraTarget = cameraTarget;
				panCameraPos = UnityCamera.transform.position;
				Ray ray2 = UnityCamera.ScreenPointToRay(mousePosition);
				Vector3 pos;
				if (GameInput.FindWaypointPosition(mousePosition, out pos))
				{
					groundPlane.distance = pos.y;
				}
				float enter2 = 0f;
				if (groundPlane.Raycast(ray2, out enter2))
				{
					panMousePosition = ray2.origin + ray2.direction * enter2;
				}
				else
				{
					panMousePosition = ray2.origin + ray2.direction * targetDistance;
				}
				panning = true;
			}
		}

		private void UpdatePanTarget()
		{
			if (!updatePanTarget)
			{
			}
		}

		private void UpdateDragMomentum()
		{
			if (dragVelocity.sqrMagnitude > Mathf.Epsilon)
			{
				dragSlowdown -= Time.deltaTime;
				if (dragSlowdown < 0f)
				{
					dragSlowdown = 0f;
				}
				dragVelocity *= dragSlowdown;
				cameraTarget += dragVelocity * Time.deltaTime * config.GetFloat("DragMomentum") * 100f;
				ClampWithinMapBounds(cameraTarget, ref cameraTarget, border: true);
			}
			Vector2 vector = config.GetVector2("MapCenter");
			Vector2 vector2 = config.GetVector2("MapSize");
			float @float = config.GetFloat("SoftBorder");
			float num = 0f;
			if (cameraTarget.x > vector.x + vector2.x / 2f)
			{
				num = (cameraTarget.x - (vector.x + vector2.x / 2f)) / @float;
				cameraTarget.x -= Time.deltaTime * 40f * num;
			}
			else if (cameraTarget.x < vector.x - vector2.x / 2f)
			{
				num = (0f - cameraTarget.x + vector.x - vector2.x / 2f) / @float;
				cameraTarget.x += Time.deltaTime * 40f * num;
			}
			if (cameraTarget.z > vector.y + vector2.y / 2f)
			{
				num = (cameraTarget.z - (vector.y + vector2.y / 2f)) / @float;
				cameraTarget.z -= Time.deltaTime * 40f * num;
			}
			else if (cameraTarget.z < vector.y - vector2.y / 2f)
			{
				num = (0f - cameraTarget.z + vector.y - vector2.y / 2f) / @float;
				cameraTarget.z += Time.deltaTime * 40f * num;
			}
		}

		private void MoveCamera(Vector2 move)
		{
			if (!(move.sqrMagnitude <= Mathf.Epsilon))
			{
				move *= 0.1f * config.GetFloat("MoveSpeed") * GetZoomFactor();
				Vector3 forward = UnityCamera.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				Vector3 right = UnityCamera.transform.right;
				right.y = 0f;
				right.Normalize();
				Vector3 b = forward * move.y + right * move.x;
				Vector3 point = cameraTarget + b;
				ClampWithinMapBounds(cameraTarget, ref point, border: false);
				cameraTarget = point;
			}
		}

		private void MoveCameraByScreenBorder(Vector2 mousePosition)
		{
			Vector2 vector = mousePosition;
			vector.y = (float)Screen.height - vector.y;
			float @float = config.GetFloat("ScreenBorderOffset");
			Vector2 zero = Vector2.zero;
			float value = 0f;
			if (vector.x <= @float)
			{
				zero.x = -1f;
				value = 1f - vector.x / @float;
			}
			else if (vector.x >= (float)Screen.width - @float)
			{
				zero.x = 1f;
				value = 1f - ((float)Screen.width - vector.x) / @float;
			}
			if (vector.y >= (float)Screen.height - @float)
			{
				zero.y = -1f;
				value = 1f - ((float)Screen.height - vector.y) / @float;
			}
			else if (vector.y <= @float)
			{
				zero.y = 1f;
				value = 1f - vector.y / @float;
			}
			if (zero.sqrMagnitude > Mathf.Epsilon)
			{
				zero.Normalize();
				value = Mathf.Clamp01(value);
				Vector2 vector2 = zero * Time.deltaTime * value * config.GetFloat("ScreenBorderSpeed") * GetZoomFactor();
				Vector3 forward = UnityCamera.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				Vector3 right = UnityCamera.transform.right;
				right.y = 0f;
				right.Normalize();
				Vector3 b = forward * vector2.y + right * vector2.x;
				Vector3 point = Vector3.Lerp(cameraTarget, cameraTarget + b, Time.deltaTime * 50f);
				ClampWithinMapBounds(cameraTarget, ref point, border: false);
				cameraTarget = point;
			}
		}

		private void UpdateFOV()
		{
			UnityCamera.fieldOfView = config.GetFloat("FOV");
		}

		private void UpdateYAngle()
		{
			Game.Character.Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
			float num = 0f;
			num = ((!UnityCamera.orthographic) ? ((targetDistance - config.GetFloat("DistanceMin")) / (config.GetFloat("DistanceMax") - config.GetFloat("DistanceMin"))) : ((UnityCamera.orthographicSize - config.GetFloat("OrthoMin")) / (config.GetFloat("OrthoMax") - config.GetFloat("OrthoMin"))));
			float num2 = config.GetFloat("AngleZoomMin") * (1f - num) + config.GetFloat("AngleY") * num;
			rotY = Mathf.Lerp(rotY, num2 * -1f * ((float)System.Math.PI / 180f), Time.deltaTime * 50f);
		}

		private void UpdateXAngle(bool force)
		{
			if (!config.GetBool("EnableRotation") || force || activateTimeout > 0f)
			{
				rotX = config.GetFloat("DefaultAngleX") * (-(float)System.Math.PI / 180f);
			}
		}

		private void UpdateDir()
		{
			Vector3 dir;
			Game.Character.Utils.Math.ToCartesian(rotX, rotY, out dir);
			UnityCamera.transform.forward = dir;
			UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
			UpdatePanTarget();
		}

		private void UpdateConfig()
		{
		}

		private void UpdateDistance()
		{
			if ((bool)Target && config.GetBool("FollowTargetY"))
			{
				 var cameraTarget =  base.cameraTarget;
				Vector3 position = Target.position;
				cameraTarget.y = position.y;
			}
		}

		private void UpdateZoom()
		{
			Game.Character.Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
			if (Mathf.Abs(targetZoom) > Mathf.Epsilon)
			{
				float num = targetZoom * 20f * Time.deltaTime;
				if (Mathf.Abs(num) > Mathf.Abs(targetZoom))
				{
					num = targetZoom;
				}
				Zoom(num);
				targetZoom -= num;
				updatePanTarget = true;
			}
		}

		private bool IsInMapBounds(Vector3 point)
		{
			Game.Character.Utils.Math.Swap(ref point.y, ref point.z);
			Vector2 vector = config.GetVector2("MapCenter");
			Vector2 vector2 = config.GetVector2("MapSize");
			return new Rect(vector.x - vector2.x / 2f, vector.y - vector2.y / 2f, vector2.x, vector2.y).Contains(point);
		}

		private void ClampWithinMapBounds(Vector3 currTarget, ref Vector3 point, bool border)
		{
			Vector2 vector = config.GetVector2("MapCenter");
			Vector2 vector2 = config.GetVector2("MapSize");
			if (config.GetBool("DisableHorizontal"))
			{
				point.x = currTarget.x;
			}
			if (config.GetBool("DisableVertical"))
			{
				point.z = currTarget.z;
			}
			float num = config.GetFloat("SoftBorder");
			if (!border)
			{
				num = 0f;
			}
			if (point.x > vector.x + vector2.x / 2f + num)
			{
				point.x = vector.x + vector2.x / 2f + num;
			}
			else if (point.x < vector.x - vector2.x / 2f - num)
			{
				point.x = vector.x - vector2.x / 2f - num;
			}
			if (point.z > vector.y + vector2.y / 2f + num)
			{
				point.z = vector.y + vector2.y / 2f + num;
			}
			else if (point.z < vector.y - vector2.y / 2f - num)
			{
				point.z = vector.y - vector2.y / 2f - num;
			}
		}

		public void Zoom(float amount)
		{
			if (!config.GetBool("EnableZoom"))
			{
				return;
			}
			float num = amount * config.GetFloat("ZoomSpeed");
			if (!(Mathf.Abs(num) > Mathf.Epsilon))
			{
				return;
			}
			if (UnityCamera.orthographic)
			{
				float zoomFactor = GetZoomFactor();
				num *= zoomFactor;
				UnityCamera.orthographicSize -= num;
				if (UnityCamera.orthographicSize < 0.01f)
				{
					UnityCamera.orthographicSize = 0.01f;
				}
				UnityCamera.orthographicSize = Mathf.Clamp(UnityCamera.orthographicSize, config.GetFloat("OrthoMin"), config.GetFloat("OrthoMax"));
			}
			else
			{
				float num2 = GetZoomFactor();
				if (num2 < 0.01f)
				{
					num2 = 0.01f;
				}
				num *= num2;
				Vector3 b = UnityCamera.transform.forward * num;
				Vector3 vector = UnityCamera.transform.position + b;
				if (!new Plane(UnityCamera.transform.forward, cameraTarget).GetSide(vector))
				{
					UnityCamera.transform.position = vector;
				}
			}
			targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
			targetDistance = Mathf.Clamp(targetDistance, config.GetFloat("DistanceMin"), config.GetFloat("DistanceMax"));
		}

		public override void Reset()
		{
			activateTimeout = 0.1f;
			targetZoom = 0f;
			targetDistance = config.GetFloat("Distance");
			if ((bool)Target)
			{
				cameraTarget = Target.position;
			}
		}

		public override void PostUpdate()
		{
			if (disableInput)
			{
				return;
			}
			if ((bool)InputManager)
			{
				updatePanTarget = false;
				UpdateConfig();
				UpdateDistance();
				UpdateFOV();
				if (InputManager.GetInput(InputType.Zoom).Valid)
				{
					targetZoom = (float)InputManager.GetInput(InputType.Zoom).Value;
				}
				UpdateZoom();
				UpdateYAngle();
				UpdateXAngle(force: false);
				bool flag = false;
				if (InputManager.GetInput(InputType.Rotate).Valid)
				{
					RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
					flag = true;
				}
				if (!flag)
				{
					if (config.GetBool("DraggingMove"))
					{
						if (InputManager.GetInput(InputType.Pan).Valid)
						{
							DragCamera((Vector2)InputManager.GetInput(InputType.Pan).Value);
						}
						else
						{
							UpdateDragMomentum();
							panning = false;
						}
					}
					if (!panning)
					{
						if (config.GetBool("KeyMove") && InputManager.GetInput(InputType.Move).Valid)
						{
							MoveCamera((Vector2)InputManager.GetInput(InputType.Move).Value);
						}
						if (config.GetBool("ScreenBorderMove"))
						{
							MoveCameraByScreenBorder(UnityEngine.Input.mousePosition);
						}
					}
				}
				UpdateDir();
			}
			activateTimeout -= Time.deltaTime;
		}

		private void UpdateCollision()
		{
			if ((bool)collision)
			{
				float _;
				collision.ProcessCollision(cameraTarget, cameraTarget, UnityCamera.transform.forward, targetDistance, out _, out _);
			}
		}

		public override void FixedStepUpdate()
		{
			UpdateCollision();
		}
	}
}
