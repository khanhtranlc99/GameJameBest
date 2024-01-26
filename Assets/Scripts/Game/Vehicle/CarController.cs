using Game.Character;
using Game.Character.Extras;
using Game.Character.Stats;
using Game.GlobalComponent;
using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class CarController : VehicleController
	{
		private const string SteerAxeName = "Horizontal";

		private const string ThrottleAxeName = "Vertical";

		private const int SteerMaxAngle = 30;

		private const int EngineTorque = 30000;

		private const int PullForce = 800000;

		private const int RotateForce = 1000;

		private const int StabilizationForce = 3000000;

		private const float IdleEnginePitch = 0.3f;

		private const int GearsCount = 6;

		private const float BrakeTorque = 100f;

		private const float normalizedImpactAngle = 0.7f;

		private const float minScratchVelocity = 3f;

		private const float lastGearOverallPercentage = 0.3f;

		private const float StartPitch = 0.7f;

		private const float relativeSpeedForStrongHit = 30f;

		public GameObject ScratchParticles;

		public float speed;

		public PhysicMaterial minFrictionMaterial;

		public PhysicMaterial standartFrictionMaterial;

		private static int chatacterLayerNumber = -1;

		private static int smallDynamicLayerNumber = -1;

		private static int terrainLayerNumber = -1;

		private static int staticObjectLayerNumber = -1;

		private static int complexStaticObjectLayerNumber = -1;

		private static int bigDynamicLayerNumber = -1;

		private static int defaultLayerNumber = -1;

		private CarSpecific carSpecific;

		private VehicleStatus vehicleStatus;

		private WheelCollider[] wheels;

		private WheelCollider[] steerWheels;

		private float hitTimer = 1f;

		private Action deInitAction;

		private VehicleSound vehicleSound;

		private int currGear;

		protected float speedToNextGear;

		protected float gearEffect;

		private float shiftGearTimer;

		private float shiftGearTimeOut = 0.9f;

		private float averageSpeed;

		private bool reducePitch;

		private float timeForTrack = 1f;

		private float speedDiffSum;

		private float inputSum;

		private float lastSpeed;

		private float speedThrowDifference = 5f;

		private float inputThrowDifference = 3f;

		private bool stuck;

		protected bool isBraking;

		protected bool inited;

		protected override VehicleType GetVehicleType()
		{
			return VehicleType.Car;
		}

		private void Awake()
		{
			if (chatacterLayerNumber == -1)
			{
				chatacterLayerNumber = LayerMask.NameToLayer("Character");
			}
			if (smallDynamicLayerNumber == -1)
			{
				smallDynamicLayerNumber = LayerMask.NameToLayer("SmallDynamic");
			}
			if (terrainLayerNumber == -1)
			{
				terrainLayerNumber = LayerMask.NameToLayer("Terrain");
			}
			if (complexStaticObjectLayerNumber == -1)
			{
				complexStaticObjectLayerNumber = LayerMask.NameToLayer("ComplexStaticObject");
			}
			if (staticObjectLayerNumber == -1)
			{
				staticObjectLayerNumber = LayerMask.NameToLayer("SimpleStaticObject");
			}
			if (bigDynamicLayerNumber == -1)
			{
				bigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
			}
			if (defaultLayerNumber == -1)
			{
				defaultLayerNumber = LayerMask.NameToLayer("Default");
			}
		}

		public override void Init(DrivableVehicle drivableVehicle)
		{
			base.Init(drivableVehicle);
			wheels = MainRigidbody.GetComponentsInChildren<WheelCollider>();
			SteeringWheels componentInChildren = MainRigidbody.GetComponentInChildren<SteeringWheels>();
			if (componentInChildren != null)
			{
				steerWheels = componentInChildren.Wheels;
			}
			WheelCollider[] array = wheels;
			foreach (WheelCollider wheelCollider in array)
			{
				wheelCollider.brakeTorque = 0f;
			}
			carSpecific = (VehicleSpecific as CarSpecific);
			vehicleStatus = drivableVehicle.GetVehicleStatus();
			if (carSpecific != null)
			{
				vehicleSound = drivableVehicle.SoundsPrefab;
				if (carSpecific.EngineSounds != null && carSpecific.EngineSounds.Length > 0)
				{
					vehicleSound.EngineSounds = carSpecific.EngineSounds;
				}
				if (carSpecific.GearShiftSound != null)
				{
					vehicleSound.GearShiftSound = carSpecific.GearShiftSound;
				}
				if ((bool)carSpecific.BrakeSound && (bool)BrakeAudioSource)
				{
					BrakeAudioSource.loop = true;
					BrakeAudioSource.clip = carSpecific.BrakeSound;
				}
				EngineAudioSource.pitch = 0.7f;
			}
			speedToNextGear = (CalcMaxSpeed(isReverse: false) - CalcMaxSpeed(isReverse: false) * 0.3f) / 5f;
			shiftGearTimeOut = CalcMaxSpeed(isReverse: false) / (speedToNextGear * 6f);
			player = PlayerInteractionsManager.Instance.Player;
			inited = true;
		}

		public override void Animate(DrivableVehicle drivableVehicle)
		{
			base.Animate(drivableVehicle);
			carSpecific = (VehicleSpecific as CarSpecific);
			if (carSpecific != null && carSpecific.CarAnimator != null && drivableVehicle.AnimateGetInOut)
			{
				carSpecific.CarAnimator.SetBool("EnterInCar", value: true);
				carSpecific.CarAnimator.SetTrigger("LeftOpen");
			}
		}

		public override void DeInit(Action callbackAfterDeInit)
		{
			StopVehicle();
			if ((bool)EngineAudioSource)
			{
				EngineAudioSource.Stop();
			}
			IsInitialized = false;
			wheels = null;
			steerWheels = null;
			engineEnabled = true;
			if (carSpecific != null && carSpecific.CarAnimator != null && MainRigidbody.GetComponent<DrivableVehicle>().AnimateGetInOut)
			{
				carSpecific.CarAnimator.SetBool("EnterInCar", value: false);
				carSpecific.CarAnimator.SetTrigger("LeftOpen");
				deInitAction = callbackAfterDeInit;
				Invoke("DeferredDeInit", carSpecific.GetOutAnimationTime);
			}
			else
			{
				base.DeInit(callbackAfterDeInit);
			}
			inited = false;
		}

		private void DeferredDeInit()
		{
			base.DeInit(deInitAction);
		}

		private void Update()
		{
			if (!IsInitialized)
			{
				return;
			}
			float axis = Controls.GetAxis("Horizontal");
			if (steerWheels != null)
			{
				WheelCollider[] array = steerWheels;
				foreach (WheelCollider wheelCollider in array)
				{
					wheelCollider.steerAngle = axis * 30f;
				}
			}
			if (hitTimer > -1f)
			{
				hitTimer -= 2f * Time.deltaTime;
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!IsInitialized)
			{
				return;
			}
			speed = CalcSpeedKmh();
			float num = Controls.GetAxis("Vertical");
			float axis = Controls.GetAxis("Horizontal");
			isBraking = ((float)(int)speed * num < 0f);
			BrakeEffect();
			if (!engineEnabled)
			{
				num = 0f;
			}
			ShiftGears(num);
			ProceedEnginePitchSound(num);
			if (!(MainRigidbody != null))
			{
				return;
			}
			bool isReverse = speed < 0f;
			if (Mathf.Abs(speed) < CalcMaxSpeed(isReverse) || (speed > 0f && num < 0f) || (speed < 0f && num > 0f))
			{
				if (OnGround())
				{
					MainRigidbody.AddRelativeForce(Vector3.forward * num * 800000f * Time.deltaTime * gearEffect * DrivableVehicle.Acceleration * player.stats.GetPlayerStat(StatsList.CarAcceleration));
				}
				if (wheels != null)
				{
					WheelCollider[] array = wheels;
					foreach (WheelCollider wheelCollider in array)
					{
						if (!isBraking)
						{
							wheelCollider.motorTorque = num * 30000f / (float)wheels.Length * Time.deltaTime;
							wheelCollider.brakeTorque = 0f;
						}
						else
						{
							wheelCollider.motorTorque = 0f;
							wheelCollider.brakeTorque = 800000f;
						}
					}
				}
			}
			float d = Mathf.Sign(speed);
			MainRigidbody.AddRelativeTorque(d * Vector3.up * axis * 1000f * Time.deltaTime);
			DrivableVehicle.ApplyStabilization(3000000f * Time.deltaTime);
			FixStuckState(speed, num);
		}

		protected virtual void ProceedEnginePitchSound(float throttle)
		{
			if (!(MainRigidbody != null) || !(EngineAudioSource != null))
			{
				return;
			}
			float num = 0f;
			float num2 = 0.4f;
			float num3 = 2f;
			averageSpeed = Mathf.Lerp(averageSpeed, speed, Time.deltaTime);
			if (currGear <= 1)
			{
				num = Mathf.Abs(averageSpeed) / speedToNextGear + carSpecific.PitchOffset;
				num2 = 0.4f + carSpecific.PitchOffset;
				num3 = 1.5f + carSpecific.PitchOffset;
				num = num * num2 + num2;
			}
			else
			{
				num = averageSpeed / (speedToNextGear * (float)currGear) + carSpecific.PitchOffset;
				num2 = 0.45f + carSpecific.PitchOffset;
				num3 = 2f + carSpecific.PitchOffset;
			}
			bool flag = false;
			if (Mathf.Abs(throttle) < 0.1f)
			{
				reducePitch = true;
			}
			else
			{
				flag = reducePitch;
				ProceedEngineSound(currGear);
			}
			if (isBraking)
			{
				flag = false;
				reducePitch = true;
			}
			if (flag)
			{
				EngineAudioSource.pitch = Mathf.Clamp(Mathf.Lerp(EngineAudioSource.pitch, num, Time.deltaTime * 5f), num2, num3);
				if (Mathf.Abs(EngineAudioSource.pitch - num) <= 0.01f)
				{
					reducePitch = false;
				}
			}
			else if (reducePitch)
			{
				EngineAudioSource.pitch = Mathf.Lerp(EngineAudioSource.pitch, num2, Time.deltaTime * 5f);
			}
			else if (!OnGround())
			{
				EngineAudioSource.pitch = Mathf.Lerp(EngineAudioSource.pitch, num3 / 2f, Time.deltaTime * num3);
			}
			else
			{
				EngineAudioSource.pitch = Mathf.Clamp(num, num2, num3);
			}
		}

		public bool OnGround()
		{
			bool result = true;
			for (int i = 0; i < wheels.Length; i++)
			{
				if (!wheels[i].isGrounded)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		private void ProceedEngineSound(int id)
		{
			if (!EngineAudioSource.clip.Equals(vehicleSound.EngineSounds[Mathf.Clamp(id, 0, vehicleSound.EngineSounds.Length - 1)]))
			{
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, vehicleSound.GearShiftSound, 1f);
				EngineAudioSource.clip = vehicleSound.EngineSounds[Mathf.Clamp(id, 0, vehicleSound.EngineSounds.Length - 1)];
			}
			if (!EngineAudioSource.isPlaying)
			{
				EngineAudioSource.Play();
			}
		}

		protected virtual void ShiftGears(float throttle)
		{
			shiftGearTimer -= Time.deltaTime;
			if (!(shiftGearTimer > 0f) && !(Mathf.Abs(speed) < 0.1f) && !(Mathf.Abs(throttle) < 0.2f))
			{
				float f = 1f - ((float)currGear / 6f + (speed - (float)(currGear - 1) * speedToNextGear) / speedToNextGear) / 2f;
				if (speed <= 0f && currGear != 0)
				{
					currGear = 0;
					shiftGearTimer = shiftGearTimeOut;
					f = 1f;
				}
				if (speed > speedToNextGear && currGear < 6 && speed > (float)currGear * speedToNextGear)
				{
					currGear++;
					gearEffect = 0.5f;
					shiftGearTimer = shiftGearTimeOut;
				}
				else if (currGear > 1 && speed + speedToNextGear / 2f < (float)(currGear - 1) * speedToNextGear)
				{
					currGear--;
					shiftGearTimer = shiftGearTimeOut;
				}
				gearEffect = Mathf.Lerp(gearEffect, 1f, Mathf.Abs(f) * Time.deltaTime);
			}
		}

		private float CalcMaxSpeed(bool isReverse)
		{
			float num = (!isReverse) ? DrivableVehicle.MaxSpeed : (DrivableVehicle.MaxSpeed / 2f);
			return num * player.stats.GetPlayerStat(StatsList.DrivingMaxSpeed);
		}

		private float CalcSpeedKmh()
		{
			return Vector3.Dot(MainRigidbody.velocity, MainRigidbody.transform.forward) * 3.6f;
		}

		public void OnHit(Vector3 point)
		{
			PointSoundManager.Instance.PlaySoundAtPoint(point, "Hit");
			SparksHitEffect.Instance.Emit(point);
		}

		public override void Particles(Collision collision)
		{
			Vector3 normal = collision.contacts[0].normal;
			Vector3 normalized = Vector3.Cross(Vector3.Cross(normal, collision.relativeVelocity), normal).normalized;
			float num = Mathf.Abs(Vector3.Dot(normalized, collision.relativeVelocity));
			float num2 = Vector3.Dot(normalized, collision.relativeVelocity.normalized);
			if (num > 3f && num2 > 0.7f && ScratchParticles != null)
			{
				Quaternion rotation = Quaternion.LookRotation(normalized + collision.relativeVelocity);
				GameObject fromPool = PoolManager.Instance.GetFromPool(ScratchParticles);
				fromPool.transform.position = collision.contacts[0].point;
				fromPool.transform.rotation = rotation;
				PoolManager.Instance.ReturnToPoolWithDelay(fromPool, 1f);
			}
		}

		protected virtual void BrakeEffect()
		{
			if (carSpecific == null || carSpecific.Taillights == null || carSpecific.Taillights.Length <= 0)
			{
				return;
			}
			if ((bool)BrakeAudioSource && isBraking != BrakeAudioSource.isPlaying)
			{
				if (Mathf.Abs(speed) > speedToNextGear && isBraking)
				{
					BrakeAudioSource.Play();
				}
				else
				{
					BrakeAudioSource.Stop();
				}
			}
			if (!OnGround())
			{
				BrakeAudioSource.Stop();
			}
			if (isBraking != carSpecific.Taillights[0].activeSelf)
			{
				GameObject[] taillights = carSpecific.Taillights;
				foreach (GameObject gameObject in taillights)
				{
					gameObject.SetActive(isBraking);
				}
			}
		}

		public override bool EnabledToExit()
		{
			int num = 0;
			WheelCollider[] array = wheels;
			foreach (WheelCollider wheelCollider in array)
			{
				if (wheelCollider.isGrounded)
				{
					num++;
				}
			}
			if (num != 0)
			{
				return true;
			}
			return false;
		}

		public override void StopVehicle(bool inMoment = false)
		{
			if (inMoment)
			{
				MainRigidbody.velocity = Vector3.zero;
			}
			WheelCollider[] array = wheels;
			foreach (WheelCollider wheelCollider in array)
			{
				wheelCollider.brakeTorque = 100f;
			}
			vehicleStatus.MainCollider.material = standartFrictionMaterial;
		}

		protected override void Drowning()
		{
			if (!inited)
			{
				return;
			}
			base.Drowning();
			EngineAudioSource.Stop();
			if (wheels != null)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					wheelCollider.brakeTorque = 100f;
				}
			}
		}

		private void FixStuckState(float speed, float throttel)
		{
		}
	}
}
