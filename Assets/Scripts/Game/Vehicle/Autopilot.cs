using Code.Game.Race;
using Code.Game.Race.UI;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Managers;
using Game.Traffic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Vehicle
{
	public class Autopilot : MonoBehaviour
	{
		public enum Tactic
		{
			Chase,
			WaypointTour,
			Race
		}

		private const string WaterAreaName = "Water";

		private const int SteerMaxAngle = 60;

		private const int RotateForce = 10000;

		private const int PullForce = 300000;

		private const int EngineTorque = 10000;

		protected const float BrakeTorque = 10f;

		private const float OnBrakeForceMultiplier = 3f;

		private const int StabilizationForce = 1000000;

		private const float CheckDistance = 30f;

		private const float SpeedLimit = 100f;

		private const float SpeedAccuracy = 5f;

		private const float CheckChaseDistance = 2f;

		private const float NavigatorToUseDistance = 30f;

		private const float TargetForwardShift = 5f;

		private const float SensorSizeVelocityFactor = 0.6f;

		private const float MaxSensorVelocity = 50f;

		private const float MinSensorSize = 0f;

		private const float StackVelocity = 3f;

		private const float StackTimeToAction = 3f;

		private const float OneWayTime = 1f;

		private const float MaxTargetVelocityToDrop = 5f;

		private const float ExitBlockedCheckFreq = 2f;

		private float lastTimeSetDestination;

		private static readonly IDictionary<CollisionTrigger.Sensor, float> SensorsExpanders = new Dictionary<CollisionTrigger.Sensor, float>
		{
			{
				CollisionTrigger.Sensor.Front,
				2f
			},
			{
				CollisionTrigger.Sensor.Left,
				1f
			},
			{
				CollisionTrigger.Sensor.LeftBlocking,
				1f
			},
			{
				CollisionTrigger.Sensor.Right,
				1f
			},
			{
				CollisionTrigger.Sensor.RightBlocking,
				1f
			}
		};

		private static readonly IDictionary<CollisionTrigger.Sensor, float> SensorsShift = new Dictionary<CollisionTrigger.Sensor, float>
		{
			{
				CollisionTrigger.Sensor.Front,
				0f
			},
			{
				CollisionTrigger.Sensor.Left,
				0f
			},
			{
				CollisionTrigger.Sensor.LeftBlocking,
				3f
			},
			{
				CollisionTrigger.Sensor.Right,
				0f
			},
			{
				CollisionTrigger.Sensor.RightBlocking,
				3f
			}
		};

		public GameObject Triggers;

		public UnityEngine.AI.NavMeshAgent NavMeshAgent;

		public LayerMask DropPassengersBlockingLayer;

		public bool DriverExit;

		public bool DriverWasKilled;

		public float MaxDistanceToDrop = 20f;

		private Racer racer;

		private Transform[] waypoints;

		private int waypointIndex;

		private float distanceToPoint;

		private int laps;

		private int lapIndex;

		private Tactic tactic;

		private Rigidbody target;

		private int waterMask;

		private DrivableVehicle drivableVehicle;

		protected WheelCollider[] wheels;

		private SteeringWheels steeringWheels;

		protected Rigidbody rootBody;

		private AudioSource engineAudioSource;

		private HitEntity instanceDriver;

		private float speed;

		private SlowUpdateProc slowUpdateProc;

		private float cruiseVel = 20f;

		private bool frontS;

		private bool leftS;

		private bool leftBlockS;

		private bool rightS;

		private bool rightBlockS;

		private float stackTimer;

		private bool isStacked;

		protected bool isWorking;

		private float exitBlockTime;

		private int stackSteerSign;

		private readonly IDictionary<CollisionTrigger.Sensor, BoxCollider> sensors = new Dictionary<CollisionTrigger.Sensor, BoxCollider>();

		public void OnSensorStay(Collider otherCollider, CollisionTrigger.Sensor sensor)
		{
			if (otherCollider.attachedRigidbody != null)
			{
				Vector3 rhs = otherCollider.attachedRigidbody.velocity - rootBody.velocity;
				if (Vector3.Dot(rootBody.transform.forward, rhs) > 0f - (Vector3.Distance(rootBody.transform.position, otherCollider.attachedRigidbody.position) - 5f))
				{
					return;
				}
			}
			switch (sensor)
			{
			case CollisionTrigger.Sensor.Front:
				frontS = true;
				break;
			case CollisionTrigger.Sensor.Left:
				leftS = true;
				break;
			case CollisionTrigger.Sensor.LeftBlocking:
				leftBlockS = true;
				break;
			case CollisionTrigger.Sensor.Right:
				rightS = true;
				break;
			case CollisionTrigger.Sensor.RightBlocking:
				rightBlockS = true;
				break;
			}
		}

		public void DeInit()
		{
			isWorking = false;
			NavMeshAgent.gameObject.SetActive(value: false);
			speed = 0f;
			stackTimer = 0f;
			exitBlockTime = 0f;
			stackSteerSign = 0;
			KeepNavigatorPlace();
			if (engineAudioSource != null)
			{
				engineAudioSource.Stop();
				engineAudioSource.clip = null;
			}
			if (wheels != null)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					wheelCollider.brakeTorque = 0f;
					wheelCollider.motorTorque = 0f;
					wheelCollider.steerAngle = 0f;
				}
			}
			drivableVehicle = null;
			wheels = null;
			steeringWheels = null;
			rootBody = null;
			target = null;
			waypoints = null;
			instanceDriver = null;
			DriverWasKilled = false;
			DriverExit = false;
		}

		public void InitWaypointTour(Transform[] waypoints, Rigidbody vehicleBody)
		{
			Init(Tactic.WaypointTour, vehicleBody);
			this.waypoints = waypoints;
			if (waypoints != null && waypoints.Length > 1)
			{
				waypointIndex = 0;
				for (int i = 1; i < waypoints.Length; i++)
				{
					Transform transform = waypoints[i];
					if (Vector3.Distance(waypoints[waypointIndex].position, vehicleBody.position) > Vector3.Distance(transform.position, vehicleBody.position))
					{
						waypointIndex = i;
					}
				}
				NavMeshAgent.SetDestination(waypoints[waypointIndex].position);
			}
			cruiseVel = drivableVehicle.MaxSpeed * (float)UnityEngine.Random.Range(20, 70) * 0.01f;
			cruiseVel = Mathf.Min(cruiseVel, 100f);
		}

		public void InitRace(Transform[] waypoints, Racer racer, int rounds)
		{
			Rigidbody mainRigidbody = racer.GetDrivableVehicle().MainRigidbody;
			Init(Tactic.Race, mainRigidbody);
			this.waypoints = waypoints;
			laps = rounds;
			lapIndex = 0;
			if (waypoints != null && waypoints.Length > 1)
			{
				waypointIndex = 0;
				NavMeshAgent.SetDestination(waypoints[waypointIndex].position);
			}
			cruiseVel = drivableVehicle.MaxSpeed;
			this.racer = racer;
			RacePositionController.Instance.AddItem(this.racer);
		}

		public void InitChase(Rigidbody vehicleBody)
		{
			Init(Tactic.Chase, vehicleBody);
			cruiseVel = drivableVehicle.MaxSpeed;
			vehicleBody.velocity = vehicleBody.transform.forward * (5f / 18f) * Mathf.Min(cruiseVel, 100f);
		}

		private void Init(Tactic tactic, Rigidbody vehicleBody)
		{
			this.tactic = tactic;
			rootBody = vehicleBody;
			drivableVehicle = rootBody.GetComponent<DrivableVehicle>();
			steeringWheels = rootBody.GetComponent<SteeringWheels>();
			wheels = rootBody.GetComponentsInChildren<WheelCollider>();
			if (engineAudioSource != null && drivableVehicle != null && drivableVehicle.VehicleSpecificPrefab is CarSpecific)
			{
				AudioClip[] engineSounds = ((CarSpecific)drivableVehicle.VehicleSpecificPrefab).EngineSounds;
				if (engineSounds != null && engineSounds.Length > 0)
				{
					engineAudioSource.clip = engineSounds[0];
					engineAudioSource.Play();
				}
			}
			WheelCollider[] array = wheels;
			foreach (WheelCollider wheelCollider in array)
			{
				wheelCollider.brakeTorque = 0f;
			}
			InitSensors();
			NavMeshAgent.gameObject.SetActive(value: true);
			KeepNavigatorPlace();
			isWorking = true;
		}

		private void InitSensors()
		{
			float maxWidth = drivableVehicle.VehicleSpecificPrefab.MaxWidth;
			float maxHeight = drivableVehicle.VehicleSpecificPrefab.MaxHeight;
			SetSizeX(sensors[CollisionTrigger.Sensor.Front], maxWidth * 0.5f);
			SetSizeX(sensors[CollisionTrigger.Sensor.Left], maxWidth * 0.25f);
			SetSizeX(sensors[CollisionTrigger.Sensor.Right], maxWidth * 0.25f);
			Vector3 size = sensors[CollisionTrigger.Sensor.Front].size;
			float x = size.x;
			Vector3 size2 = sensors[CollisionTrigger.Sensor.Right].size;
			float num = (x + size2.x) * 0.5f;
			SetLocalX(sensors[CollisionTrigger.Sensor.Right], num);
			SetLocalX(sensors[CollisionTrigger.Sensor.Left], 0f - num);
			float num2 = maxWidth;
			Vector3 size3 = sensors[CollisionTrigger.Sensor.LeftBlocking].size;
			float num3 = (num2 + size3.x) * 0.5f;
			SetLocalX(sensors[CollisionTrigger.Sensor.RightBlocking], num3);
			SetLocalX(sensors[CollisionTrigger.Sensor.LeftBlocking], 0f - num3);
			foreach (BoxCollider value in sensors.Values)
			{
				Vector3 size4 = value.size;
				size4.y = maxHeight;
				value.size = size4;
				Vector3 localPosition = value.transform.localPosition;
				localPosition.y = maxHeight * 0.5f;
				value.transform.localPosition = localPosition;
			}
			UpdateSensors();
		}

		private void SetSizeX(BoxCollider box, float x)
		{
			Vector3 size = box.size;
			float y = size.y;
			Vector3 size2 = box.size;
			box.size = new Vector3(x, y, size2.z);
		}

		private void SetLocalX(Component component, float x)
		{
			Transform transform = component.transform;
			Vector3 localPosition = component.transform.localPosition;
			float y = localPosition.y;
			Vector3 localPosition2 = component.transform.localPosition;
			transform.localPosition = new Vector3(x, y, localPosition2.z);
		}

		private void Awake()
		{
			waterMask = 1 << UnityEngine.AI.NavMesh.GetAreaFromName("Water");
			if (NavMeshAgent == null && GameManager.ShowDebugs)
			{
				UnityEngine.Debug.LogError("NavMeshAgent not found");
			}
			engineAudioSource = GetComponent<AudioSource>();
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 0.5f);
			CollisionTrigger[] componentsInChildren = Triggers.GetComponentsInChildren<CollisionTrigger>();
			foreach (CollisionTrigger collisionTrigger in componentsInChildren)
			{
				sensors.Add(collisionTrigger.SensorType, collisionTrigger.GetComponent<BoxCollider>());
			}
		}

		private void SlowUpdate()
		{
			UpdateSensors();
		}

		private void UpdateSensors()
		{
			float num = Mathf.Min(Mathf.Abs(speed), 50f) * 0.6f;
			foreach (KeyValuePair<CollisionTrigger.Sensor, BoxCollider> sensor in sensors)
			{
				BoxCollider value = sensor.Value;
				Vector3 size = value.size;
				size.z = num * SensorsExpanders[sensor.Key] + SensorsShift[sensor.Key];
				value.size = size;
				BoxCollider boxCollider = value;
				Vector3 center = value.center;
				float x = center.x;
				Vector3 center2 = value.center;
				float y = center2.y;
				Vector3 size2 = value.size;
				boxCollider.center = new Vector3(x, y, size2.z * 0.5f);
			}
		}

		private void Drowning()
		{
			isWorking = false;
			rootBody.velocity = Vector3.zero;
			if (wheels != null)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					wheelCollider.brakeTorque = 10f;
				}
			}

			NavMeshAgent.isStopped = true;
		}

		public virtual void DropPassangers()
		{
			DriverExit = true;
			isWorking = false;
			rootBody.velocity = Vector3.zero;
			DummyDriver componentInChildren = rootBody.GetComponentInChildren<DummyDriver>();
			if (componentInChildren != null && !drivableVehicle.CompareTag("Racer"))
			{
				HitEntity player = PlayerInteractionsManager.Instance.Player;
				DriverWasKilled = componentInChildren.DriverDead;
				if (DriverWasKilled)
				{
					TrafficManager.Instance.FreeCopVehicleSlot();
				}
				componentInChildren.InitOutOfVehicle(false, player, false);
				drivableVehicle.OpenVehicleDoor(VehicleDoor.LeftDoor, false);
				instanceDriver = componentInChildren.InitedStatusNPC;
				HitEntity hitEntity = instanceDriver;
				hitEntity.DiedEvent = (HitEntity.AliveStateChagedEvent)Delegate.Combine(hitEntity.DiedEvent, new HitEntity.AliveStateChagedEvent(DropedCopKillEvent));
				if (!DriverWasKilled)
				{
					PoolManager.Instance.AddBeforeReturnEvent(instanceDriver, delegate
					{
						TrafficManager.Instance.FreeCopVehicleSlot();
					});
				}
				PoolManager.Instance.AddBeforeReturnEvent(drivableVehicle, delegate
				{
					if (instanceDriver.transform.IsChildOf(drivableVehicle.transform))
					{
						PoolManager.Instance.ReturnToPool(instanceDriver);
					}
				});
			}
			if (wheels != null)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					wheelCollider.brakeTorque = 10f;
				}
			}

			NavMeshAgent.isStopped = true;
		}

		public virtual void DropedCopKillEvent()
		{
			DriverWasKilled = true;
		}

		public virtual void ChangeDropedCopKillEvent()
		{
			HitEntity hitEntity = instanceDriver;
			hitEntity.DiedEvent = (HitEntity.AliveStateChagedEvent)Delegate.Remove(hitEntity.DiedEvent, new HitEntity.AliveStateChagedEvent(DropedCopKillEvent));
		}

		private void FixedUpdate()
		{
			if (!isWorking)
			{
				return;
			}
			if (drivableVehicle.DeepInWater)
			{
				Drowning();
				return;
			}
			slowUpdateProc.ProceedOnFixedUpdate();
			Vector3 vector = rootBody.transform.InverseTransformDirection(rootBody.velocity);
			speed = vector.z;
			if (rootBody.velocity.magnitude * 3.6f < 3f && !isStacked)
			{
				stackTimer += Time.deltaTime;
				if (stackTimer > 3f)
				{
					isStacked = true;
				}
			}
			switch (tactic)
			{
			case Tactic.WaypointTour:
				WaypointTourFixedUpdate();
				break;
			case Tactic.Race:
				RaceFixedUpdate();
				break;
			case Tactic.Chase:
				ChaseFixedUpdate();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (drivableVehicle != null)
			{
				drivableVehicle.ApplyStabilization(1000000f * Time.deltaTime);
			}
			DropSensorsStatus();
		}

		private void ChaseFixedUpdate()
		{
			target = ((!PlayerInteractionsManager.Instance.IsDrivingAVehicle()) ? PlayerInteractionsManager.Instance.Player.rigidbody : PlayerInteractionsManager.Instance.LastDrivableVehicle.MainRigidbody);
			Vector3 position = rootBody.transform.position;
			position.y = 0f;
			Vector3 position2 = target.position;
			position2.y = 0f;
			float num = Vector3.Distance(position, position2);
			float resultSteer;
			float resultThrottle;
			if (num < MaxDistanceToDrop && target.velocity.magnitude < 1.388889f)
			{
				Vector3 vector = rootBody.transform.InverseTransformDirection(target.transform.forward);
				resultSteer = vector.x;
				ProceedSensors(out resultThrottle, ref resultSteer, 0f);
				if (speed < 5f && exitBlockTime <= 0f)
				{
					exitBlockTime = 2f;
					UnityEngine.AI.NavMeshHit hit;
					Vector3 _;
					if (!drivableVehicle.IsDoorBlockedOffset(DropPassengersBlockingLayer, rootBody.transform, out _) && !drivableVehicle.DeepInWater && !NavMeshAgent.SamplePathPosition(-1, 50f, out hit) && (hit.mask & waterMask) == 0)
					{
						isWorking = false;
						DropPassangers();
					}
				}
			}
			else
			{
				Vector3 vector2 = target.position + target.transform.forward * 5f;
				Vector3 normalized;
				if (Vector3.Distance(position, position2) > 30f)
				{
					SetDestination(vector2, 1f);
					normalized = NavMeshAgent.desiredVelocity.normalized;
				}
				else
				{
					normalized = (vector2 - rootBody.transform.position).normalized;
				}
				Vector3 normalized2 = rootBody.transform.InverseTransformDirection(normalized).normalized;
				resultSteer = normalized2.x;
				float num2 = cruiseVel;
				if (Vector3.Dot(rootBody.transform.forward, normalized) < 0f)
				{
					resultSteer = Mathf.Sign(resultSteer);
					num2 *= 0.1f;
				}
				if (Vector3.Distance(position, vector2) < 2f)
				{
					num2 = 0f;
				}
				ProceedSensors(out resultThrottle, ref resultSteer, num2);
				ProceedStack(ref resultThrottle, ref resultSteer);
			}
			ThrottleFixedUpdate(resultThrottle);
			SteeringFixedUpdate(resultSteer);
			if (exitBlockTime > 0f)
			{
				exitBlockTime -= Time.deltaTime;
			}
		}

		private void Update()
		{
			if (isWorking)
			{
				KeepNavigatorPlace();
			}
		}

		private void SetDestination(Vector3 point, float timeOut)
		{
			if (!(Time.time - lastTimeSetDestination < timeOut))
			{
				NavMeshAgent.SetDestination(point);
				lastTimeSetDestination = Time.time;
			}
		}

		private void DropSensorsStatus()
		{
			frontS = false;
			leftS = false;
			leftBlockS = false;
			rightS = false;
			rightBlockS = false;
		}

		private void KeepNavigatorPlace()
		{
			NavMeshAgent.transform.localPosition = Vector3.zero;
		}

		private void WaypointTourFixedUpdate()
		{
			if (waypoints != null && waypoints.Length >= 1)
			{
				Transform transform = waypoints[waypointIndex];
				if (Vector3.Distance(rootBody.transform.position, transform.position) < 30f)
				{
					waypointIndex = (waypointIndex + 1) % waypoints.Length;
					SetDestination(waypoints[waypointIndex].position, 1f);
				}
				Vector3 normalized = rootBody.transform.InverseTransformDirection(NavMeshAgent.desiredVelocity).normalized;
				float num = normalized.x;
				if (Vector3.Dot(rootBody.transform.forward, NavMeshAgent.desiredVelocity.normalized) < 0f)
				{
					num = Mathf.Sign(num);
				}
				float resultSteer = num;
				float resultThrottle;
				ProceedSensors(out resultThrottle, ref resultSteer, cruiseVel);
				ProceedStack(ref resultThrottle, ref resultSteer);
				ThrottleFixedUpdate(resultThrottle);
				SteeringFixedUpdate(resultSteer);
			}
		}

		private void Finish()
		{
			if (drivableVehicle.CompareTag("Racer"))
			{
				UiRaceManager.Instance.AddItemToResultTable(racer);
			}
			isWorking = false;
			TrafficManager.Instance.AddTrafficDriverForVehicle(drivableVehicle);
			drivableVehicle.tag = "Untagged";
		}

		private void RaceFixedUpdate()
		{
			if (drivableVehicle.CurrentDriver.IsPlayer)
			{
				isWorking = false;
			}
			Transform transform = waypoints[waypointIndex];
			distanceToPoint = Vector3.Distance(rootBody.transform.position, transform.position);
			racer.SetLap(lapIndex);
			racer.SetWaypointIndex(waypointIndex);
			racer.SetDistanceToPoint(distanceToPoint);
			if (distanceToPoint < 30f)
			{
				waypointIndex = (waypointIndex + 1) % waypoints.Length;
				if (waypointIndex == 0)
				{
					lapIndex++;
					if (laps == lapIndex)
					{
						Finish();
					}
				}
				SetDestination(waypoints[waypointIndex].position, 1f);
			}
			Vector3 normalized = rootBody.transform.InverseTransformDirection(NavMeshAgent.desiredVelocity).normalized;
			float num = normalized.x;
			if (Vector3.Dot(rootBody.transform.forward, NavMeshAgent.desiredVelocity.normalized) < 0f)
			{
				num = Mathf.Sign(num);
			}
			float resultSteer = num;
			float resultThrottle;
			ProceedSensors(out resultThrottle, ref resultSteer, cruiseVel);
			ProceedStack(ref resultThrottle, ref resultSteer);
			ThrottleFixedUpdate(resultThrottle);
			SteeringFixedUpdate(resultSteer);
		}

		private void ProceedStack(ref float resultThrottle, ref float resultSteer)
		{
			if (isStacked)
			{
				if (stackTimer > 0f)
				{
					stackTimer -= Time.deltaTime;
				}
				else
				{
					isStacked = false;
					stackSteerSign = 0;
				}
				float num = stackTimer / 2f;
				float num2 = stackTimer - (float)(int)num * 1f;
				resultThrottle = -1f;
				resultSteer = Mathf.Sign(resultSteer);
				if (stackSteerSign == 0)
				{
					stackSteerSign = (int)(0f - resultSteer);
				}
				if (stackSteerSign == 0)
				{
					stackSteerSign = -1;
				}
				resultSteer = stackSteerSign;
				if (num2 <= 1f)
				{
					resultThrottle *= -1f;
					resultSteer = -stackSteerSign;
				}
			}
		}

		private void ProceedSensors(out float resultThrottle, ref float resultSteer, float shouldBeSpeed)
		{
			resultThrottle = 0f;
			if (!isStacked)
			{
				if (frontS)
				{
					shouldBeSpeed = speed * 3.6f * 0.95f;
				}
				if (rightBlockS)
				{
					resultSteer = Mathf.Clamp(resultSteer, -1f, 0f);
				}
				if (leftBlockS)
				{
					resultSteer = Mathf.Clamp(resultSteer, 0f, 1f);
				}
				if (leftS && !rightBlockS)
				{
					resultSteer += 0.5f;
				}
				else if (rightS && !leftBlockS)
				{
					resultSteer -= 0.5f;
				}
				float num = shouldBeSpeed - speed * 3.6f;
				resultThrottle = Mathf.Clamp(num / 5f, -1f, 1f);
				resultSteer = Mathf.Clamp(resultSteer, -1f, 1f);
			}
		}

		private void SteeringFixedUpdate(float steer)
		{
			float d = Mathf.Sign(speed);
			if (rootBody != null)
			{
				rootBody.AddRelativeTorque(d * Vector3.up * steer * 10000f * Time.deltaTime);
			}
			if (steeringWheels != null)
			{
				WheelCollider[] array = steeringWheels.Wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					float num = 1f - Mathf.Abs(speed) / (1.2f * drivableVehicle.MaxSpeed * (5f / 18f));
					wheelCollider.steerAngle = steer * 60f * num;
				}
			}
		}

		private void ThrottleFixedUpdate(float throttle)
		{
			bool flag = Mathf.Sign(throttle) * Mathf.Sign(speed) < 0f;
			Vector3 vector = Vector3.forward * throttle * 300000f * Time.deltaTime;
			if (flag)
			{
				vector *= 3f;
			}
			if (rootBody != null)
			{
				rootBody.AddRelativeForce(vector);
			}
			if (wheels != null)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider wheelCollider in array)
				{
					wheelCollider.motorTorque = throttle * 10000f / (float)wheels.Length * Time.deltaTime;
				}
			}
		}
	}
}
