using Game.Character.Stats;
using Game.GlobalComponent;
using Game.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Vehicle
{
	public class TankController : VehicleController
	{
		private const string SteerAxeName = "Horizontal";

		private const string ThrottleAxeName = "Vertical";

		private const int MaxAngularVelocity = 5;

		private const int RpmMultipler = 6;

		private const int OffsetReducer = 1000;

		private const int MaxWheelRpm = 1000;

		private const int SmoothBrakeTorqueReducer = 5;

		private const int SmoothBrakeSpeed = 25;

		private const int MinSpeedToHardBrake = 50;

		private const int SpeedToReversing = 50;

		private const int AccelerationReducer = 10;

		private const int ToShowSpeedMultipler = 3;

		private const int EngineRpmMultipler = 5;

		private const int SmokeInitMinSpeed = 25;

		private const int NormalExhaustMaxSpeed = 15;

		private const int DeInitBrakeMultipler = 10;

		private const int MoveDownRpm = -10;

		[Separator("Tank Links")]
		public Transform[] WheelTransformLeft;

		public Transform[] WheelTransformsRight;

		public WheelCollider[] WheelCollidersLeft;

		public WheelCollider[] WheelCollidersRight;

		public Transform[] UselessGearTransformsLeft;

		public Transform[] UselessGearTransformsRight;

		public Transform[] TrackBoneTransformsLeft;

		public Transform[] TrackBoneTransformsRight;

		public Renderer TrackObjectLeft;

		public Renderer TrackObjectRight;

		public GameObject ExhaustGasPoint;

		[Separator("Settings")]
		public float TrackOffset;

		public float TrackScrollSpeedMultipler = 1f;

		public AnimationCurve EngineTorqueCurve;

		public float EngineTorque = 250f;

		public float BrakeTorque = 250f;

		public float MinEngineRPM = 1000f;

		public float MaxEngineRPM = 5000f;

		public float MaxSpeed = 80f;

		public float SteerTorque = 3f;

		[Separator("Audios")]
		public AudioSource EngineRunningAudioSource;

		public AudioClip EngineRunningAudioClip;

		[Separator("Effects")]
		public GameObject WheelSlipPrefab;

		public ParticleSystem NormalExhaustGas;

		public ParticleSystem HeavyExhaustGas;

		private bool reversing;

		private List<WheelCollider> allWheelColliders = new List<WheelCollider>();

		private float[] rotationValueLeft;

		private float[] rotationValueRight;

		private float speed;

		private float defSteerAngle;

		private float acceleration;

		private float lastVelocity;

		private float engineRPM;

		private float motorInput;

		private float steerInput;

		private float currentVolume;

		private readonly List<ParticleSystem> wheelParticles = new List<ParticleSystem>();

		private ParticleSystem initedNormalExhaustGas;

		private ParticleSystem initedHeavyExhaustGas;

		public bool EngineEnabled => engineEnabled;

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Tank;
		}

		public override void Init(DrivableVehicle drivableVehicle)
		{
			engineEnabled = true;
			base.Init(drivableVehicle);
			currentVolume = SoundManager.instance.GetSoundValue();
			SoundManager.ValueChanged b = delegate(float value)
			{
				currentVolume = SoundManager.instance.GetSoundValue();
				currentVolume *= value;
			};
			SoundManager instance = SoundManager.instance;
			instance.GameSoundChanged = (SoundManager.ValueChanged)Delegate.Combine(instance.GameSoundChanged, b);
			SoundInit(EngineRunningAudioSource, EngineRunningAudioClip);
			DrivableTank drivableTank = drivableVehicle as DrivableTank;
			if (drivableTank != null)
			{
				WheelTransformLeft = drivableTank.WheelTransformLeft;
				WheelTransformsRight = drivableTank.WheelTransformsRight;
				WheelCollidersLeft = drivableTank.WheelCollidersLeft;
				WheelCollidersRight = drivableTank.WheelCollidersRight;
				UselessGearTransformsLeft = drivableTank.UselessGearTransformsLeft;
				UselessGearTransformsRight = drivableTank.UselessGearTransformsRight;
				TrackBoneTransformsLeft = drivableTank.TrackBoneTransformsLeft;
				TrackBoneTransformsRight = drivableTank.TrackBoneTransformsRight;
				TrackObjectLeft = drivableTank.TrackObjectLeft;
				TrackObjectRight = drivableTank.TrackObjectRight;
				ExhaustGasPoint = drivableTank.ExhaustGasPoint;
			}
			MaxSpeed = drivableVehicle.MaxSpeed * player.stats.GetPlayerStat(StatsList.DrivingMaxSpeed);
			SmokeInit();
			MainRigidbody.maxAngularVelocity = 5f;
			rotationValueLeft = new float[WheelCollidersLeft.Length];
			rotationValueRight = new float[WheelCollidersRight.Length];
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			engineEnabled = false;
			base.DeInit(callbackAfterDeInit);
			EngineRunningAudioSource.Stop();
			SmokeDeinit();
			foreach (WheelCollider allWheelCollider in allWheelColliders)
			{
				allWheelCollider.brakeTorque = BrakeTorque * 10f;
			}
		}

		private void Update()
		{
			if (IsInitialized)
			{
				WheelsAlign();
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (IsInitialized && EngineEnabled)
			{
				AnimateGears(UselessGearTransformsRight, WheelCollidersRight, rotationValueRight);
				AnimateGears(UselessGearTransformsLeft, WheelCollidersLeft, rotationValueLeft);
				Engine();
				Braking();
				Inputs();
				AudioSetup();
				SmokeRate();
			}
		}

		private void SoundInit(AudioSource source, AudioClip clip)
		{
			source.clip = clip;
			source.loop = true;
			source.volume = currentVolume;
			source.Play();
		}

		private void SmokeInit()
		{
			if (ExhaustGasPoint != null)
			{
				initedHeavyExhaustGas = PoolManager.Instance.GetFromPool(HeavyExhaustGas).GetComponent<ParticleSystem>();
				initedHeavyExhaustGas.transform.parent = ExhaustGasPoint.transform;
				initedHeavyExhaustGas.transform.localPosition = Vector3.zero;
				initedHeavyExhaustGas.transform.localEulerAngles = Vector3.zero;
				initedNormalExhaustGas = PoolManager.Instance.GetFromPool(NormalExhaustGas).GetComponent<ParticleSystem>();
				initedNormalExhaustGas.transform.parent = ExhaustGasPoint.transform;
				initedNormalExhaustGas.transform.localPosition = Vector3.zero;
				initedNormalExhaustGas.transform.localEulerAngles = Vector3.zero;
			}
			allWheelColliders = WheelCollidersLeft.ToList();
			WheelCollider[] wheelCollidersRight = WheelCollidersRight;
			foreach (WheelCollider item in wheelCollidersRight)
			{
				allWheelColliders.Add(item);
			}
			if ((bool)WheelSlipPrefab)
			{
				foreach (WheelCollider allWheelCollider in allWheelColliders)
				{
					GameObject fromPool = PoolManager.Instance.GetFromPool(WheelSlipPrefab);
					fromPool.transform.position = allWheelCollider.transform.position;
					fromPool.transform.rotation = MainRigidbody.transform.rotation;
					fromPool.transform.parent = allWheelCollider.transform;
					wheelParticles.Add(fromPool.GetComponent<ParticleSystem>());
				}
			}
		}

		private void SmokeDeinit()
		{
			if ((bool)initedHeavyExhaustGas)
			{
				PoolManager.Instance.ReturnToPool(initedHeavyExhaustGas);
			}
			if ((bool)initedNormalExhaustGas)
			{
				PoolManager.Instance.ReturnToPool(initedNormalExhaustGas);
			}
			foreach (ParticleSystem wheelParticle in wheelParticles)
			{
				PoolManager.Instance.ReturnToPool(wheelParticle.gameObject);
			}
		}

		private void WheelsAlign()
		{
			CalculateWheelsTransform(WheelCollidersRight, WheelTransformsRight, TrackBoneTransformsRight, rotationValueRight);
			CalculateWheelsTransform(WheelCollidersLeft, WheelTransformLeft, TrackBoneTransformsLeft, rotationValueLeft);
			CalculateTextureOffset(TrackObjectLeft, WheelCollidersLeft, rotationValueLeft);
			CalculateTextureOffset(TrackObjectRight, WheelCollidersRight, rotationValueRight);
		}

		private void CalculateWheelsTransform(WheelCollider[] collidersArray, Transform[] transformsArray, Transform[] trackBoneTransforms, float[] rotationsArray)
		{
			RaycastHit hitInfo = default(RaycastHit);
			for (int i = 0; i < collidersArray.Length && i < transformsArray.Length; i++)
			{
				WheelCollider wheelCollider = collidersArray[i];
				Transform transform = transformsArray[i];
				Transform transform2 = trackBoneTransforms[i];
				Vector3 vector = wheelCollider.transform.TransformPoint(wheelCollider.center);
				Vector3 origin = vector;
				Vector3 direction = -wheelCollider.transform.up;
				float num = wheelCollider.suspensionDistance + wheelCollider.radius;
				Vector3 localScale = MainRigidbody.transform.localScale;
				if (Physics.Raycast(origin, direction, out hitInfo, num * localScale.y))
				{
					Transform transform3 = transform;
					Vector3 point = hitInfo.point;
					Vector3 a = wheelCollider.transform.up * wheelCollider.radius;
					Vector3 localScale2 = MainRigidbody.transform.localScale;
					transform3.position = point + a * localScale2.y;
					Transform transform4 = transform2;
					Vector3 point2 = hitInfo.point;
					Vector3 a2 = wheelCollider.transform.up * TrackOffset;
					Vector3 localScale3 = MainRigidbody.transform.localScale;
					transform4.position = point2 + a2 * localScale3.y;
				}
				else
				{
					Transform transform5 = transform;
					Vector3 a3 = vector;
					Vector3 a4 = wheelCollider.transform.up * wheelCollider.suspensionDistance;
					Vector3 localScale4 = MainRigidbody.transform.localScale;
					transform5.position = a3 - a4 * localScale4.y;
					Transform transform6 = transform2;
					Vector3 a5 = vector;
					Vector3 a6 = wheelCollider.transform.up * (wheelCollider.suspensionDistance + wheelCollider.radius - TrackOffset);
					Vector3 localScale5 = MainRigidbody.transform.localScale;
					transform6.position = a5 - a6 * localScale5.y;
				}
				transform.rotation = wheelCollider.transform.rotation * Quaternion.Euler(rotationsArray[Mathf.CeilToInt(collidersArray.Length / 2)], 0f, 0f);
				rotationsArray[i] += wheelCollider.rpm * 6f * Time.deltaTime;
			}
		}

		private void CalculateTextureOffset(Renderer trackRenderer, WheelCollider[] collidersArray, float[] rotationsArray)
		{
			trackRenderer.material.SetTextureOffset("_MainTex", new Vector2(rotationsArray[Mathf.CeilToInt(collidersArray.Length / 2)] / 1000f * TrackScrollSpeedMultipler, 0f));
			//trackRenderer.material.SetTextureOffset("_BumpMap", new Vector2(rotationsArray[Mathf.CeilToInt(collidersArray.Length / 2)] / 1000f * TrackScrollSpeedMultipler, 0f));
			//todo : cần check chỗ này
		}

		private void AnimateGears(Transform[] uselessGearArray, WheelCollider[] collidersArray, float[] rotationsArray)
		{
			for (int i = 0; i < uselessGearArray.Length; i++)
			{
				uselessGearArray[i].rotation = collidersArray[i].transform.rotation * Quaternion.Euler(rotationsArray[Mathf.CeilToInt(collidersArray.Length / 2)], collidersArray[i].steerAngle, 0f);
			}
		}

		private void Engine()
		{
			if (speed > MaxSpeed)
			{
				for (int i = 0; i < allWheelColliders.Count; i++)
				{
					allWheelColliders[i].motorTorque = 0f;
				}
			}
			else
			{
				ApplyingMotorTorque(WheelCollidersLeft, 1);
				ApplyingMotorTorque(WheelCollidersRight, -1);
			}
			if (WheelCollidersLeft[2].isGrounded || WheelCollidersRight[2].isGrounded)
			{
				Vector3 angularVelocity = MainRigidbody.angularVelocity;
				if (Mathf.Abs(angularVelocity.y) < 1f)
				{
					Vector3 a = (!reversing) ? Vector3.up : (-Vector3.up);
					MainRigidbody.AddRelativeTorque(a * steerInput * SteerTorque, ForceMode.Acceleration);
				}
			}
		}

		private void ApplyingMotorTorque(WheelCollider[] wheelsArray, int steerInputCounter)
		{
			foreach (WheelCollider wheelCollider in wheelsArray)
			{
				if (!reversing)
				{
					if (wheelCollider.isGrounded && Mathf.Abs(wheelCollider.rpm) < 1000f)
					{
						wheelCollider.motorTorque = EngineTorque * Mathf.Clamp(Mathf.Clamp(motorInput, 0f, 1f) + Mathf.Clamp(steerInput * (float)steerInputCounter, -1f, 1f), -1f, 1f) * EngineTorqueCurve.Evaluate(speed);
					}
					else
					{
						wheelCollider.motorTorque = 0f;
					}
				}
				else if (speed < 30f)
				{
					wheelCollider.motorTorque = EngineTorque * motorInput;
				}
				else
				{
					wheelCollider.motorTorque = 0f;
				}
			}
		}

		private void Braking()
		{
			for (int i = 0; i < allWheelColliders.Count; i++)
			{
				WheelCollider wheelCollider = allWheelColliders[i];
				if (motorInput == 0f)
				{
					if (speed < 25f && Mathf.Abs(steerInput) < 0.1f)
					{
						wheelCollider.brakeTorque = BrakeTorque / 5f;
					}
					else
					{
						wheelCollider.brakeTorque = 0f;
					}
				}
				else if (motorInput < -0.1f && allWheelColliders[0].rpm > 50f)
				{
					wheelCollider.brakeTorque = BrakeTorque * Mathf.Abs(motorInput);
				}
				else
				{
					wheelCollider.brakeTorque = 0f;
				}
			}
		}

		private void Inputs()
		{
			motorInput = Controls.GetAxis("Vertical");
			steerInput = Controls.GetAxis("Horizontal");
			reversing = (motorInput < 0f && allWheelColliders[0].rpm < 50f);
			if (WheelCollidersLeft[0].rpm < -10f && WheelCollidersRight[0].rpm < -10f && !reversing)
			{
				steerInput = 0f - steerInput;
			}
			speed = MainRigidbody.velocity.magnitude * 3f;
			acceleration = 0f;
			Vector3 vector = MainRigidbody.transform.InverseTransformDirection(MainRigidbody.velocity);
			float z = vector.z;
			acceleration = (z - lastVelocity) / Time.deltaTime;
			lastVelocity = z;
			MainRigidbody.drag = Mathf.Clamp(acceleration / 10f, 0f, 1f);
			engineRPM = Mathf.Clamp(Mathf.Abs(WheelCollidersLeft[0].rpm + WheelCollidersRight[0].rpm) * 5f + MinEngineRPM, MinEngineRPM, MaxEngineRPM);
		}

		private void AudioSetup()
		{
			EngineRunningAudioSource.pitch = Mathf.Lerp(EngineRunningAudioSource.pitch, Mathf.Lerp(0.4f, 1f, (engineRPM - MinEngineRPM / 1.5f) / (MaxEngineRPM + MinEngineRPM) + Mathf.Clamp(Mathf.Clamp(motorInput, 0f, 1f) + Mathf.Clamp(Mathf.Abs(steerInput), 0f, 0.5f), 0.35f, 0.85f)), Time.deltaTime * 2f);
			EngineRunningAudioSource.volume = Mathf.Lerp(EngineRunningAudioSource.volume, Mathf.Clamp(Mathf.Clamp(Mathf.Abs(motorInput), 0f, 1f) + Mathf.Clamp(Mathf.Abs(steerInput), 0f, 1f), 0.35f, 0.85f) * currentVolume, Time.deltaTime * 2f);
		}

		private void SmokeRate()
		{
			if ((bool)WheelSlipPrefab && wheelParticles.Count > 0)
			{
				for (int i = 0; i < allWheelColliders.Count; i++)
				{
					//wheelParticles[i].emit = (speed > 25f && allWheelColliders[i].isGrounded);
				}
			}
			if ((bool)initedNormalExhaustGas)
			{
				if (speed < 15f)
				{
					initedNormalExhaustGas.Play();
				}
			}
			if ((bool)initedHeavyExhaustGas)
			{
				if (Mathf.Abs(motorInput) > 0.1f || Mathf.Abs(steerInput) > 0.1f)
				{
					initedHeavyExhaustGas.Play();
				}
				//Debug.LogError("track speed heavy");
			}
		}

		protected override void Drowning()
		{
			base.Drowning();
			foreach (WheelCollider allWheelCollider in allWheelColliders)
			{
				allWheelCollider.brakeTorque = BrakeTorque;
			}
		}

		public override void DisableEngine()
		{
			base.DisableEngine();
			SmokeDeinit();
			EngineRunningAudioSource.Stop();
			engineEnabled = false;
		}
	}
}
