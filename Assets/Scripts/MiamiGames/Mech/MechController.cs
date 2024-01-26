using Game.Character;
using Game.Character.Input;
using Game.GlobalComponent;
using Game.Mech;
using Game.Vehicle;
using System;
using UnityEngine;

namespace MiamiGames.Mech
{
	public class MechController : VehicleController
	{
		protected const string SteerAxeName = "Horizontal";

		protected const string ThrottleAxeName = "Vertical";

		protected const string LaserShootStateName = "Fire";

		[Separator("Mech variables")]
		public LayerMask GroundMask;

		public AudioClip FootStepsClip;

		public AudioClip IdleClip;

		public AudioClip StadUp;

		public AudioClip SitDown;

		public float IdlePitch = 0.8f;

		public float minMovePitch = 0.4f;

		public float maxMovePitch = 1.2f;

		protected float timerForSpecialSound;

		protected DrivableMech drivableMech;

		protected MechAnimationController animationController;

		protected InputManager inputManager;

		protected MechInputs inputs;

		protected float motorInput;

		protected float steerInput;

		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			drivableMech = (drivableVehicle as DrivableMech);
			inputManager = InputManager.Instance;
			animationController = GetComponentInParent<MechAnimationController>();
			animationController.enabled = true;
			animationController.StandUp();
			EffectForEnableOrDisable(enable: true);
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			MainRigidbody.ResetInertiaTensor();
			inputs.move = Vector2.zero;
			animationController.Move(inputs);
			animationController.Down();
			EffectForEnableOrDisable(enable: false);
			base.DeInit(callbackAfterDeInit);
		}

		protected override void FixedUpdate()
		{
			if (IsInitialized)
			{
				if (drivableMech.DeepInWater)
				{
					PlayerInteractionsManager.Instance.GetOutFromVehicle();
				}
				Footsteps();
				Inputs();
			}
		}

		protected void EffectForEnableOrDisable(bool enable)
		{
			PointSoundManager.Instance.PlayCustomClipAtPoint(base.transform.position, (!enable) ? SitDown : StadUp);
		}

		protected virtual void Footsteps()
		{
			if (timerForSpecialSound > 0f)
			{
				timerForSpecialSound -= Time.deltaTime;
				return;
			}
			float num = Mathf.Abs(inputs.move.y);
			float num2 = Mathf.Abs(inputs.move.x);
			float num3 = 1f;
			float magnitude = inputs.move.magnitude;
			if (num2 >= 0.75f && num <= 0.45f)
			{
				num3 = 1.2f;
			}
			else if (magnitude >= 1.25f)
			{
				num3 = 0.7f;
			}
			magnitude *= num3;
			float num4 = NormalizeValue(magnitude, minMovePitch, maxMovePitch, 0f, 1.2f);
			float num5 = (float)AudioSettings.dspTime + num4;
			if (num > 0.2f || num2 > 0.2f)
			{
				EngineAudioSource.clip = FootStepsClip;
				EngineAudioSource.pitch = num4;
				if (!EngineAudioSource.isPlaying)
				{
					EngineAudioSource.PlayScheduled(num5);
				}
			}
			else if (animationController.isActiveAndEnabled)
			{
				EngineAudioSource.clip = IdleClip;
				EngineAudioSource.pitch = IdlePitch;
				if (!EngineAudioSource.isPlaying)
				{
					num5 = (float)AudioSettings.dspTime + IdlePitch;
					EngineAudioSource.PlayScheduled(num5);
				}
			}
		}

		protected void Inputs()
		{
			inputs.move = inputManager.GetInput(InputType.Move, Vector2.zero);
			inputs.fire = Controls.GetButton("Fire");
			inputs.right90 = Controls.GetButton("TurnRight");
			inputs.left90 = Controls.GetButton("TurnLeft");
			animationController.Move(inputs);
		}

		public float NormalizeValue(float value, float minNewRange, float maxNewRange, float minOldRange, float maxOldRange)
		{
			float num = maxOldRange - minOldRange;
			float num2 = maxNewRange - minNewRange;
			return (value - minOldRange) * num2 / num + minNewRange;
		}
	}
}
