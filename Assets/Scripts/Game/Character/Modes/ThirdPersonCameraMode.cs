using Game.Character.CharacterController;
using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Utils;
using System;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(ThirdPersonConfig))]
	public class ThirdPersonCameraMode : CameraMode
	{
		public float AutoResetTimeout = 1f;

		public float AutoResetSpeed = 1f;

		public bool AutoReset = true;

		public bool LerpFromLastPos = true;

		public bool dbgRing;

		private bool rotationInput;

		private float rotationInputTimeout;

		private bool hasRotated;

		private float rotX;

		private float rotY;

		private float targetVelocity;

		private float collisionDistance;

		private float collisionZoomVelocity;

		private float currCollisionTargetDist;

		private float collisionTargetDist;

		private float collisionTargetVelocity;

		private Vector3 targetPos;

		private Vector3 lastTargetPos;

		private Vector3 springVelocity;

		private GameObject debugRing;

		private float activateTimeout;

		private float resetTimeout;

		private PositionFilter targetFilter;

		public float AutoAimSensitivity = 1f;

		public float AutoAimDuration = 0.2f;

		private float currentAutoAimDuration;

		private bool currentAutoAimPerformed;

		public override Type Type => Type.ThirdPerson;

		public override void Init()
		{
			base.Init();
			if (LerpFromLastPos)
			{
				targetPos = Target.position;
			}
			UnityCamera.transform.LookAt(cameraTarget);
			config = GetComponent<ThirdPersonConfig>();
			lastTargetPos = Target.position;
			targetVelocity = 0f;
			if (dbgRing)
			{
				debugRing = RingPrimitive.Create(3f, 3f, 0.1f, 50, Color.red);
				Game.Character.Utils.Debug.SetActive(debugRing, dbgRing);
			}
			targetFilter = new PositionFilter(10, 1f);
			targetFilter.Reset(Target.position);
			Game.Character.Utils.DebugDraw.Enabled = true;
			resetTimeout = 0f;
		}

		public override void OnActivate()
		{
			base.OnActivate();
			config.SetCameraMode(DefaultConfiguration);
			targetVelocity = 0f;
			activateTimeout = 1f;
		}

		private void RotateCamera(Vector2 mousePosition)
		{
			rotationInput = (mousePosition.sqrMagnitude > Mathf.Epsilon);
			rotationInputTimeout += Time.deltaTime;
			if (rotationInput)
			{
				rotationInputTimeout = 0f;
				hasRotated = true;
			}
			Game.Character.Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
			rotY += config.GetFloat("RotationSpeedY") * mousePosition.y;
			rotX += config.GetFloat("RotationSpeedX") * mousePosition.x;
			float num = (0f - rotY) * 57.29578f;
			float @float = config.GetFloat("RotationYMax");
			float float2 = config.GetFloat("RotationYMin");
			if (num > @float)
			{
				rotY = (0f - @float) * ((float)System.Math.PI / 180f);
			}
			if (num < float2)
			{
				rotY = (0f - float2) * ((float)System.Math.PI / 180f);
			}
		}

		private void UpdateFollow()
		{
			Vector3 vector = targetPos - lastTargetPos;
			vector.y = 0f;
			float num = Mathf.Clamp(vector.magnitude, 0f, 5f);
			if (Time.deltaTime > Mathf.Epsilon)
			{
				targetVelocity = num / Time.deltaTime;
			}
			else
			{
				targetVelocity = 0f;
			}
			if (InputManager.GetInput(InputType.Move).Valid)
			{
				Vector2 vector2 = (Vector2)InputManager.GetInput(InputType.Move).Value;
				vector2.Normalize();
				float @float = config.GetFloat("FollowCoef");
				float f = Mathf.Atan2(vector2.x, vector2.y);
				float num2 = Mathf.Sin(f);
				float num3 = Mathf.Clamp01(rotationInputTimeout);
				float num4 = num2 * Time.deltaTime * @float * targetVelocity * 0.2f * num3;
				rotX += num4;
			}
		}

		private void UpdateDistance()
		{
			Vector3 a = targetPos + GetOffsetPos();
			cameraTarget = Vector3.Lerp(a, GetTargetHeadPos(), 1f - currCollisionTargetDist);
		}

		private void UpdateFOV()
		{
			UnityCamera.fieldOfView = config.GetFloat("FOV");
		}

		private void UpdateDir()
		{
			activateTimeout -= Time.deltaTime;
			if (activateTimeout > 0f)
			{
				float @float = config.GetFloat("DefaultYRotation");
				rotY = (0f - @float) * ((float)System.Math.PI / 180f);
				Vector3 forward = Target.forward;
				float x = forward.x;
				Vector3 forward2 = Target.forward;
				rotX = Mathf.Atan2(x, forward2.z);
			}

			Vector3 dir;
			Game.Character.Utils.Math.ToCartesian(rotX, rotY, out dir);
			UpdateAutoReset(ref dir);
			UnityCamera.transform.forward = dir;
			UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
			lastTargetPos = targetPos;
		}

		private void UpdateAutoReset(ref Vector3 dir)
		{
			if (!AutoReset)
			{
				return;
			}
			resetTimeout -= Time.deltaTime;
			if (rotationInputTimeout < 0.1f)
			{
				resetTimeout = AutoResetTimeout;
			}
			if (resetTimeout < 0f && hasRotated)
			{
				float @float = config.GetFloat("DefaultYRotation");
				float num = (0f - @float) * ((float)System.Math.PI / 180f);
				Vector3 forward = Target.forward;
				float x = forward.x;
				Vector3 forward2 = Target.forward;
				float num2 = Mathf.Atan2(x, forward2.z);
				if (Mathf.Abs(num2 - rotX) < 0.1f && Mathf.Abs(num - rotY) < 0.1f)
				{
					hasRotated = false;
				}

				Vector3 dir2;
				Game.Character.Utils.Math.ToCartesian(num2, num, out dir2);
				Quaternion b = Quaternion.FromToRotation(dir, dir2);
				Quaternion rotation = Quaternion.Slerp(Quaternion.identity, b, Time.deltaTime * AutoResetSpeed * 10f);
				dir = rotation * dir;
			}
		}

		private Vector3 GetOffsetPos()
		{
			Vector3 vector = Vector3.zero;
			if (config.IsVector3("TargetOffset"))
			{
				vector = config.GetVector3("TargetOffset");
			}
			Vector3 a = Game.Character.Utils.Math.VectorXZ(UnityCamera.transform.forward);
			Vector3 a2 = Game.Character.Utils.Math.VectorXZ(UnityCamera.transform.right);
			Vector3 up = Vector3.up;
			return a * vector.z + a2 * vector.x + up * vector.y;
		}

		private void UpdateYRotation()
		{
			if (!rotationInput && targetVelocity > 0.1f)
			{
				float a = (0f - rotY) * 57.29578f;
				float @float = config.GetFloat("DefaultYRotation");
				float num = Mathf.Clamp01(rotationInputTimeout);
				float t = Mathf.Clamp01(targetVelocity * config.GetFloat("AutoYRotation") * Time.deltaTime) * num;
				a = Mathf.Lerp(a, @float, t);
				rotY = (0f - a) * ((float)System.Math.PI / 180f);
			}
		}

		public override void Reset()
		{
			activateTimeout = 1f;
		}

		public void UpdateAutoAim()
		{
			if (!TargetManager.Instance.UseAutoAim)
			{
				return;
			}
			Transform autoAimTarget = TargetManager.Instance.AutoAimTarget;
			Vector3 targetLocalOffset = TargetManager.Instance.TargetLocalOffset;
			if ((bool)autoAimTarget && !autoAimTarget.gameObject.activeSelf)
			{
				return;
			}
			if (currentAutoAimPerformed)
			{
				if (!InputManager.GetInput(InputType.Aim).Valid || !(bool)InputManager.GetInput(InputType.Aim).Value || autoAimTarget == null)
				{
					currentAutoAimPerformed = false;
					currentAutoAimDuration = 0f;
				}
			}
			else if (InputManager.GetInput(InputType.Aim).Valid && (bool)InputManager.GetInput(InputType.Aim).Value && autoAimTarget != null)
			{
				UnityCamera.transform.forward = Vector3.Lerp(UnityCamera.transform.forward, autoAimTarget.position + targetLocalOffset - UnityCamera.transform.position, AutoAimSensitivity * Time.deltaTime);
				currentAutoAimDuration += Time.deltaTime;
				if (currentAutoAimDuration >= AutoAimDuration)
				{
					currentAutoAimPerformed = true;
				}
			}
		}

		public override void PostUpdate()
		{
			if (!disableInput && (bool)InputManager)
			{
				if (InputManager.GetInput(InputType.Reset, defaultValue: false))
				{
					activateTimeout = 0.1f;
				}
				UpdateFOV();
				if (InputManager.GetInput(InputType.Rotate).Valid)
				{
					RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
				}
				UpdateFollow();
				UpdateDistance();
				UpdateYRotation();
				UpdateDir();
				UpdateAutoAim();
			}
		}

		private void UpdateCollision()
		{
			Vector3 cameraTarget = targetPos + GetOffsetPos();
			float @float = config.GetFloat("Distance");
			collision.ProcessCollision(cameraTarget, GetTargetHeadPos(), UnityCamera.transform.forward, @float, out collisionTargetDist, out collisionDistance);
			float num = collisionDistance / @float;
			if (collisionTargetDist > num)
			{
				collisionTargetDist = num;
			}
			targetDistance = Interpolation.Lerp(targetDistance, collisionDistance, (!(targetDistance > collisionDistance)) ? collision.GetReturnSpeed() : collision.GetClipSpeed());
			currCollisionTargetDist = Mathf.SmoothDamp(currCollisionTargetDist, collisionTargetDist, ref collisionTargetVelocity, (!(currCollisionTargetDist > collisionTargetDist)) ? collision.GetReturnTargetSpeed() : collision.GetTargetClipSpeed());
		}

		public override void GameUpdate()
		{
			base.GameUpdate();
			float @float = config.GetFloat("Spring");
			Vector2 vector = config.GetVector2("DeadZone");
			if (@float <= 0f && vector.sqrMagnitude <= Mathf.Epsilon)
			{
				targetPos = targetFilter.GetValue();
			}
			UpdateTargetDummy();
		}

		public override void FixedStepUpdate()
		{
			targetFilter.AddSample(Target.position);
			UpdateCollision();
			Vector2 vector = config.GetVector2("DeadZone");
			if (vector.sqrMagnitude > Mathf.Epsilon)
			{
				if (dbgRing)
				{
					RingPrimitive.Generate(debugRing, vector.x, vector.y, 0.1f, 50);
					debugRing.transform.position = targetPos + Vector3.up * 2f;
					Vector3 forward = Game.Character.Utils.Math.VectorXZ(UnityCamera.transform.forward);
					if (forward.sqrMagnitude < Mathf.Epsilon)
					{
						forward = Vector3.forward;
					}
					debugRing.transform.forward = forward;
					Game.Character.Utils.Debug.SetActive(debugRing, dbgRing);
				}
				Vector3 a = targetFilter.GetValue() - targetPos;
				float magnitude = a.magnitude;
				a /= magnitude;
				if (magnitude > vector.x || magnitude > vector.y)
				{
					Vector3 vector2 = UnityCamera.transform.InverseTransformDirection(a);
					float f = Mathf.Atan2(vector2.x, vector2.z);
					Vector3 vector3 = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
					float magnitude2 = new Vector3(vector3.x * vector.x, 0f, vector3.z * vector.y).magnitude;
					if (magnitude > magnitude2)
					{
						Vector3 target = targetPos + a * (magnitude - magnitude2);
						targetPos = Vector3.SmoothDamp(targetPos, target, ref springVelocity, config.GetFloat("Spring"));
					}
				}
			}
			else
			{
				targetPos = Vector3.SmoothDamp(targetPos, targetFilter.GetValue(), ref springVelocity, config.GetFloat("Spring"));
			}
		}
	}
}
