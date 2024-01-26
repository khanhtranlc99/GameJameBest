using Game.Character.Config;
using Game.Character.Input;
using Game.Character.Modes;
using Game.Character.Utils;
using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(FlyConfig))]
	public class FlyCameraMode : CameraMode
	{
		public bool LerpFromLastPos = true;

		public bool dbgRing;

		private bool rotationInput;

		private float rotationInputTimeout;

		private float rotX;

		private float rotY;

		private Vector3 targetVelocity;

		private float collisionDistance;

		private float collisionZoomVelocity;

		private float rollbackDistance;

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

		private PositionFilter velocityFilter;

		private bool autoCameraVelocityTurn;

		private float autoRotSpeed;

		public override Game.Character.Modes.Type Type => Game.Character.Modes.Type.Fly;

		public override void Init()
		{
			base.Init();
			if (LerpFromLastPos)
			{
				targetPos = Target.position;
			}
			UnityCamera.transform.LookAt(cameraTarget);
			config = GetComponent<FlyConfig>();
			config.Awake();
			lastTargetPos = Target.position;
			targetVelocity = Vector3.zero;
			if (dbgRing)
			{
				debugRing = RingPrimitive.Create(3f, 3f, 0.1f, 50, Color.red);
			}
			targetFilter = new PositionFilter(10, 1f);
			targetFilter.Reset(Target.position);
			velocityFilter = new PositionFilter(10, 1f);
			velocityFilter.Reset(Vector3.zero);
			Game.Character.Utils.DebugDraw.Enabled = true;
			resetTimeout = 0f;
		}

		public override void OnActivate()
		{
			base.OnActivate();
			config.SetCameraMode(DefaultConfiguration);
			targetVelocity = Vector3.zero;
			activateTimeout = 1f;
		}

		private void RotateCamera()
		{
			Vector2 vector = new Vector2(Controls.GetAxis("Horizontal_R"), Controls.GetAxis("Vertical_R"));
			rotationInput = (vector.sqrMagnitude > Mathf.Epsilon);
			rotationInputTimeout += Time.deltaTime;
			if (rotationInput)
			{
				rotationInputTimeout = 0f;
				autoRotSpeed = config.GetFloat("AutoTurnMinSpeed");
			}
			Game.Character.Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
			rotY += config.GetFloat("RotationSpeedY") * vector.y;
			rotX += config.GetFloat("RotationSpeedX") * vector.x;
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
				Vector3 eulerAngles = Quaternion.LookRotation(velocityFilter.GetValue()).eulerAngles;
				float x = eulerAngles.x;
				rotY = (0f - x) * ((float)System.Math.PI / 180f);
				Vector3 forward = Target.forward;
				float x2 = forward.x;
				Vector3 forward2 = Target.forward;
				rotX = Mathf.Atan2(x2, forward2.z);
			}
			UpdateAutoReset();
			Vector3 dir;
			Game.Character.Utils.Math.ToCartesian(rotX, rotY, out dir);
			UnityCamera.transform.forward = dir;
			float @float = config.GetFloat("Distance");
			float float2 = config.GetFloat("SpeedRollback");
			rollbackDistance = @float * Mathf.Clamp01(velocityFilter.GetValue().magnitude * float2);
			UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
			lastTargetPos = targetPos;
		}

		private void UpdateAutoReset()
		{
			if (!config.GetBool("AutoReset"))
			{
				return;
			}
			resetTimeout -= Time.deltaTime;
			if (rotationInputTimeout < 0.1f)
			{
				resetTimeout = config.GetFloat("AutoResetTimeout");
			}
			if (resetTimeout <= 0f)
			{
				resetTimeout = 0f;
				float num = rotX;
				if (targetVelocity.magnitude * 3.6f > config.GetFloat("WhenRotateSpeed") || (autoCameraVelocityTurn && targetVelocity.magnitude * 3.6f > config.GetFloat("WhenRotateSpeed") * 0.1f))
				{
					num = Mathf.Atan2(targetVelocity.x, targetVelocity.z);
					autoCameraVelocityTurn = true;
				}
				else
				{
					autoRotSpeed = config.GetFloat("AutoTurnMinSpeed");
				}
				if (autoCameraVelocityTurn && targetVelocity.magnitude * 3.6f < config.GetFloat("WhenRotateSpeed") * 0.1f)
				{
					autoCameraVelocityTurn = false;
				}
				Vector3 eulerAngles = Quaternion.LookRotation(velocityFilter.GetValue()).eulerAngles;
				float num2 = 0f - eulerAngles.x;
				num2 *= (float)System.Math.PI / 180f;
				Vector3 dir;
				Game.Character.Utils.Math.ToCartesian(num, num2, out dir);
				Vector3 dir2;
				Game.Character.Utils.Math.ToCartesian(rotX, rotY, out dir2);
				Quaternion quaternion = Quaternion.FromToRotation(dir2, dir);
				float value = Quaternion.Angle(Quaternion.identity, quaternion);
				float max = Mathf.Min(autoRotSpeed * config.GetFloat("AutoTurnAcceleration"), config.GetFloat("AutoTurnMaxSpeed"));
				autoRotSpeed = Mathf.Clamp(value, autoRotSpeed, max);
				Quaternion quaternion2 = Quaternion.RotateTowards(Quaternion.identity, quaternion, autoRotSpeed * Time.deltaTime);
				if (Quaternion.Angle(Quaternion.identity, quaternion2) < Mathf.Epsilon)
				{
					autoRotSpeed = config.GetFloat("AutoTurnMinSpeed");
				}
				dir2 = quaternion2 * dir2;
				Game.Character.Utils.Math.ToSpherical(dir2, out rotX, out rotY);
			}
			else
			{
				autoCameraVelocityTurn = false;
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

		public override void Reset()
		{
			activateTimeout = 1f;
		}

		public override void PostUpdate()
		{
			if (InputManager.GetInput(InputType.Reset, defaultValue: false))
			{
				activateTimeout = 0.1f;
			}
			UpdateFOV();
			RotateCamera();
			UpdateDistance();
			UpdateDir();
		}

		private void UpdateCollision()
		{
			Vector3 cameraTarget = targetPos + GetOffsetPos();
			float @float = config.GetFloat("Distance");
			@float += rollbackDistance;
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
				targetPos = Target.position;
			}
			UpdateTargetDummy();
			Vector3 a = targetPos - lastTargetPos;
			if (Time.deltaTime > 0f)
			{
				targetVelocity = a / Time.deltaTime;
				velocityFilter.AddSample(targetVelocity);
			}
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
