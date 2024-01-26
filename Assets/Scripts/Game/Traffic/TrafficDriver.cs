using Game.Character;
using Game.GlobalComponent;
using Game.Vehicle;
using System.Collections;
using UnityEngine;

namespace Game.Traffic
{
	public class TrafficDriver : MonoBehaviour
	{
		private const int StabilizationForce = 1000000;

		private const float ReturnToLineSpeed = 3f;

		private const float LineFollowAccuracy = 0.1f;

		private const float SpeedAccuracy = 0.1f;

		private const float MinSpeedValue = 0.2f;

		private const float StayTimeout = 0.3f;

		private const float CollideTriggerSpeedCoef = 1.2f;

		private const float MinTriggerSize = 2f;

		private const float KlaxonTimeoutMax = 15f;

		private const float KlaxonTimeoutMin = 3f;

		private const float SpeedLimitDistance = 30f;

		private const float SpeedLimit = 100f;

		private const float OnBrakeForceMultiplier = 3f;

		private const float OnBrakeForceRate = 3f;

		private const float AccelerationImitationRate = 0.4f;

		private const float MinPitch = 0.35f;

		private const float MaxPitch = 1.1f;

		private const string PseudoObstaclesLayerName = "TrafficPseudoObstacles";

		private const float CalculateStartSpeedTime = 1f;

		private static int PseudoObstacleLayerNumber = -1;

		public Rigidbody RootBody;

		public Transform InitialPoint;

		public AudioSource EngineAudioSource;

		public bool IsControlled = true;

		private int pullForce = 300000;

		private int engineTorque = 10000;

		private int rotateForce = 10000;

		private float brakeTorque = 50f;

		private int steerMaxAngle = 60;

		private RoadPoint fromPoint;

		private RoadPoint toPoint;

		private Vector3 target;

		private Vector3 startLine;

		private int line = 1;

		private float cruiseVel = 20f;

		private DrivableVehicle drivableVehicle;

		private WheelCollider[] wheels;

		private SteeringWheels steeringWheels;

		private float speed;

		private float stayTimeout;

		private SlowUpdateProc slowUpdateProc;

		private BoxCollider trigger;

		private float klaxonTimeout;

		private float initTime;

		private Vector3 initPoint;

		private bool roadBlocked;

		private float accelerationImitation = 1f;

		private float steer;

		private DrivableVehicle lastVehicle;

		private GameObject pseudoObsacleObject;

		private bool calculateStartSpeed;

		private float onBrakeForceMultiplier = 3f;

		public TrafficDriver()
		{
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 0.5f);
		}

		public void DisableControll()
		{
			IsControlled = false;
			if (wheels != null)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					wheelCollider.motorTorque = 0f;
					wheelCollider.brakeTorque = brakeTorque;
				}
			}
		}

		public void Init(Rigidbody rb, Transform initialPoint, RoadPoint fromPoint, RoadPoint toPoint, int line)
		{
			this.fromPoint = fromPoint;
			initPoint = fromPoint.Point;
			this.toPoint = toPoint;
			this.line = line;
			IsControlled = true;
			stayTimeout = 0f;
			initTime = Time.timeSinceLevelLoad;
			RootBody = rb;
			InitialPoint = initialPoint;
			base.transform.position = InitialPoint.position;
			base.transform.rotation = InitialPoint.rotation;
			trigger = GetComponent<BoxCollider>();
			BoxCollider boxCollider = trigger;
			Vector3 forward = Vector3.forward;
			Vector3 size = trigger.size;
			Vector3 a = forward * size.z * 0.51f;
			Vector3 up = Vector3.up;
			Vector3 size2 = trigger.size;
			boxCollider.center = a + up * size2.y * 0.51f;
			TrafficManager.Instance.CalcTargetPoint(fromPoint, toPoint, line, out startLine, out target);
			drivableVehicle = RootBody.GetComponent<DrivableVehicle>();
			steeringWheels = RootBody.GetComponent<SteeringWheels>();
			wheels = RootBody.GetComponentsInChildren<WheelCollider>();
			drivableVehicle.ConstraintsSetup(isin: true);
			pullForce = drivableVehicle.VehicleSpecificPrefab.PullForce;
			engineTorque = drivableVehicle.VehicleSpecificPrefab.EngineTorque;
			rotateForce = drivableVehicle.VehicleSpecificPrefab.RotateForce;
			brakeTorque = drivableVehicle.VehicleSpecificPrefab.BrakeTorque;
			steerMaxAngle = drivableVehicle.VehicleSpecificPrefab.SteerMaxAngle;
			cruiseVel = drivableVehicle.MaxSpeed * (float)UnityEngine.Random.Range(20, 40) * 0.01f;
			cruiseVel = Mathf.Min(cruiseVel, 100f);
			roadBlocked = false;
			StartCoroutine(CalculateSpeedForwardVehicles(1f));
			WheelCollider[] array = wheels;
			foreach (WheelCollider wheelCollider in array)
			{
				wheelCollider.motorTorque = 0f;
				wheelCollider.brakeTorque = 0f;
			}
			if (EngineAudioSource != null && drivableVehicle != null && (bool)drivableVehicle.VehicleSpecificPrefab && ((CarSpecific)drivableVehicle.VehicleSpecificPrefab).EngineSounds != null && ((CarSpecific)drivableVehicle.VehicleSpecificPrefab).EngineSounds.Length > 0)
			{
				EngineAudioSource.clip = ((CarSpecific)drivableVehicle.VehicleSpecificPrefab).EngineSounds[0];
				EngineAudioSource.Play();
			}
		}

		public void DeInit()
		{
			if (EngineAudioSource != null)
			{
				EngineAudioSource.Stop();
				EngineAudioSource.clip = null;
			}
			IsControlled = false;
			RootBody = null;
			InitialPoint = null;
		}

		private void Awake()
		{
			if (PseudoObstacleLayerNumber == -1)
			{
				PseudoObstacleLayerNumber = LayerMask.NameToLayer("TrafficPseudoObstacles");
			}
		}

		private IEnumerator CalculateSpeedForwardVehicles(float calculateTime)
		{
			calculateStartSpeed = true;
			bool calculated = false;
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while (calculateTime > 0f)
			{
				if (lastVehicle != null && RootBody.transform.forward == lastVehicle.transform.forward)
				{
					RootBody.velocity = lastVehicle.MainRigidbody.velocity;
					calculated = true;
					break;
				}
				if (pseudoObsacleObject != null)
				{
					RootBody.velocity = Vector3.zero;
					calculated = true;
					break;
				}
				calculateTime -= Time.deltaTime;
				yield return waitForEndOfFrame;
			}
			if (!calculated)
			{
				RootBody.velocity = cruiseVel * (target - startLine).normalized * (5f / 18f);
			}
			calculateStartSpeed = false;
		}

		private float CalcCurrentCruiseVel()
		{
			if (roadBlocked)
			{
				return 0f;
			}
			float num = cruiseVel;
			if (Vector3.Distance(RootBody.transform.position, toPoint.Point) < 30f)
			{
				num = Mathf.Min(num, toPoint.SpeedLimit);
			}
			return num * (5f / 18f);
		}

		private void SlowUpdate()
		{
			if (trigger != null)
			{
				BoxCollider boxCollider = trigger;
				Vector3 size = trigger.size;
				float x = size.x;
				Vector3 size2 = trigger.size;
				boxCollider.size = new Vector3(x, size2.y, Mathf.Max(Mathf.Abs(speed) * 1.2f, 2f));
				BoxCollider boxCollider2 = trigger;
				Vector3 center = trigger.center;
				float x2 = center.x;
				Vector3 center2 = trigger.center;
				float y = center2.y;
				Vector3 size3 = trigger.size;
				boxCollider2.center = new Vector3(x2, y, size3.z / 2f);
			}
			float num = initTime + 2f;
			if (!SectorManager.Instance.IsInActiveSector(RootBody.transform.position) && (num < Time.timeSinceLevelLoad || (num >= Time.timeSinceLevelLoad && !SectorManager.Instance.IsInActiveSector(initPoint))) && !CameraManager.Instance.IsInCameraFrustrum(RootBody.transform.position))
			{
				DestroyVehicle();
			}
		}

		public void DestroyVehicle()
		{
			TrafficManager.Instance.TrafficVehicleOutOfRange(drivableVehicle, this);
		}

		private void Klaxon()
		{
			if (roadBlocked)
			{
				klaxonTimeout -= Time.deltaTime;
			}
			else
			{
				klaxonTimeout = 3f;
			}
			if (klaxonTimeout < 0f)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.Klaxon);
				klaxonTimeout = UnityEngine.Random.Range(3f, 15f);
			}
		}

		private void ProceedEnginePitchSound()
		{
			if ((bool)EngineAudioSource)
			{
				EngineAudioSource.pitch = Mathf.Lerp(EngineAudioSource.pitch, Mathf.Clamp(accelerationImitation + 0.35f, 0.35f, 1.1f), Time.deltaTime);
			}
		}

		private void Update()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
			if (!IsControlled || !drivableVehicle.OnGround)
			{
				return;
			}
			Klaxon();
			if (stayTimeout < 0f)
			{
				stayTimeout = 0f;
				roadBlocked = false;
			}
			else if (stayTimeout > 0f)
			{
				stayTimeout -= Time.deltaTime;
			}
			if (!(RootBody != null))
			{
				return;
			}
			if ((target - RootBody.transform.position).magnitude < 3f)
			{
				TrafficManager.Instance.GetNextRoute(ref fromPoint, ref toPoint, ref line);
				TrafficManager.Instance.CalcTargetPoint(fromPoint, toPoint, line, out startLine, out target);
			}
			Vector3 rhs = target - startLine;
			Vector3 vector = Vector3.Cross(rhs.normalized, Vector3.up);
			Vector3 lhs = -RootBody.transform.position + target;
			Vector3 normalized = lhs.normalized;
			float num = Vector3.Dot(RootBody.transform.forward, normalized);
			float num2 = Vector3.Cross(lhs, rhs).magnitude / rhs.magnitude;
			if (num2 > TrafficManager.Instance.RoadLineSize * 0.1f)
			{
				float num3 = Vector3.Dot(normalized, vector);
				normalized = (normalized + num3 * 3f * vector).normalized;
				num *= 0.5f;
			}
			Vector3 vector2 = RootBody.transform.InverseTransformDirection(RootBody.velocity);
			speed = vector2.z;
			float num4 = CalcCurrentCruiseVel() * Mathf.Max(Mathf.Abs(num), 0.2f);
			float f = num4 - speed;
			float num5 = (!(Mathf.Abs(f) > 0.1f)) ? 0f : Mathf.Sign(f);
			bool flag = Mathf.Sign(num5) * Mathf.Sign(speed) < 0f;
			accelerationImitation = ((CalcCurrentCruiseVel() != 0f) ? Mathf.Lerp(accelerationImitation, 1f, Time.deltaTime * 0.4f) : CalcCurrentCruiseVel());
			onBrakeForceMultiplier = ((!flag) ? 0f : Mathf.Lerp(onBrakeForceMultiplier, 3f, Time.deltaTime * 3f));
			ProceedEnginePitchSound();
			Vector3 a = Vector3.forward * num5 * pullForce * Time.deltaTime;
			a *= ((!flag) ? accelerationImitation : onBrakeForceMultiplier);
			RootBody.AddRelativeForce(a);
			if (wheels != null)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					wheelCollider.motorTorque = num5 * (float)engineTorque / (float)wheels.Length * Time.deltaTime * accelerationImitation;
				}
			}
			Vector3 normalized2 = RootBody.transform.InverseTransformDirection(normalized).normalized;
			steer = normalized2.x;
			float d = Mathf.Sign(speed);
			RootBody.AddRelativeTorque(d * Vector3.up * steer * rotateForce * Time.deltaTime);
			if (steeringWheels != null)
			{
				float num6 = 1f - Mathf.Abs(speed) / (1.2f * drivableVehicle.MaxSpeed * (5f / 18f));
				drivableVehicle.SteerStabilization(steer * (float)steerMaxAngle * num6);
				WheelCollider[] array2 = steeringWheels.Wheels;
				foreach (WheelCollider wheelCollider2 in array2)
				{
					wheelCollider2.steerAngle = steer * (float)steerMaxAngle * num6;
				}
			}
			drivableVehicle.ApplyStabilization(1000000f * Time.deltaTime);
		}

		private void OnTriggerStay(Collider other)
		{
			if (IsControlled)
			{
				stayTimeout = 0.3f;
				roadBlocked = true;
				if (calculateStartSpeed)
				{
					lastVehicle = other.GetComponentInParent<DrivableVehicle>();
					pseudoObsacleObject = ((other.gameObject.layer != PseudoObstacleLayerNumber) ? null : other.gameObject);
				}
			}
		}
	}
}
