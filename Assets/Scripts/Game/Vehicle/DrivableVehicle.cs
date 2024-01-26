using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using Game.Managers;
using Game.Traffic;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Vehicle
{
	[RequireComponent(typeof(Rigidbody))]
	public class DrivableVehicle : MonoBehaviour, IInitable
	{
		[Serializable]
		public class VehiclePointsContainer
		{
			public Transform CenterOfMass;

			public Transform DriverPosition;

			public Transform TrafficDriverPosition;

			public Transform[] EnterFromPositions;

			public Transform JumpOutPosition;
		}

		private const float MaxSpeedToEnter = 20f;

		private const float MagicConstant = 0.083f;

		private const int BrakeTorqueOnStart = 10;

		private const float relativeSpeedForStrongHit = 30f;

		private const float relativeSpeedForHitSmallDynamic = 7f;

		private const float relativeSpeedForHitBigDynamic = 2f;

		private static int WaterLayerNumber = -1;

		public bool DebugLog;

		public GameObject VehicleSubstrateColider;

		public GameObject Wheels;

		public GameObject VehicleControllerPrefab;

		public VehicleSpecific VehicleSpecificPrefab;

		public VehicleSound SoundsPrefab;

		public GameObject SimpleModel;

		[Header("Tuning")]
		public Renderer[] BodyRenderers;

		public Material[] BodyMaterials;

		public bool Tuning;

		public bool OnGround;

		[Header("Vehicle Acceleration Multiplier")]
		[Range(1f, 10f)]
		public float Acceleration = 1f;

		[Header("MaxSpeed KMH")]
		public float MaxSpeed = 100f;

		public float DamagePerSpeed = 10f;

		public float SpeedForImpact = 7f;

		public bool DamagingFromCollision = true;

		[Space(10f)]
		public VehiclePointsContainer VehiclePoints = new VehiclePointsContainer();

		public WaterSensor WaterSensor;

		public float MaxAcceptableWaterHigh = 0.8f;

		public float ToSitPositionLerpTic = 0.15f;

		[Space(10f)]
		public DriverStatus CurrentDriver;

		[HideInInspector]
		public bool AnimateGetInOut;

		[HideInInspector]
		public bool DeepInWater;

		[HideInInspector]
		public DummyDriver DummyDriver;

		private int bodyMaterialIndex = -1;

		private UnityEngine.AI.NavMeshObstacle obstacle;

		protected BoxCollider boxSource;

		private SlowUpdateProc verySlowUpdateProc;

		private float waterDepth = -1000f;

		private float hitTimer = 1f;

		private bool isTransformer;

		private VehicleSource vehicleSource;

		[HideInInspector]
		public VehicleController controller;

		public float ExitRightMinDistance;

		public float ExitLeftMinDistance;

		public float ExitUpMinDistance = 3f;

		protected VehicleController oldController;

		protected VehicleStatus vehStatus;

		private Rigidbody mainRigidbody;

		public static int terrainLayerNumber
		{
			get;
			private set;
		}

		public static int chatacterLayerNumber
		{
			get;
			private set;
		}

		public static int staticObjectLayerNumber
		{
			get;
			private set;
		}

		public static int smallDynamicLayerNumber
		{
			get;
			private set;
		}

		public static int complexStaticObjectLayerNumber
		{
			get;
			private set;
		}

		public static int bigDynamicLayerNumber
		{
			get;
			private set;
		}

		public static int defaultLayerNumber
		{
			get;
			private set;
		}

		public bool DriverIsVulnerable => !vehStatus.IsArmored;

		public Rigidbody MainRigidbody => mainRigidbody ? mainRigidbody : (mainRigidbody = GetComponent<Rigidbody>());

		public float Speed()
		{
			return Vector3.Dot(MainRigidbody.velocity, MainRigidbody.transform.forward) * 3.6f;
		}

		public virtual VehicleType GetVehicleType()
		{
			return VehicleType.Car;
		}

		public virtual bool HasExitAnimation()
		{
			return true;
		}

		public virtual bool HasEnterAnimation()
		{
			return true;
		}

		public virtual bool IsControlsPlayerAnimations()
		{
			return false;
		}

		public virtual void ApplyStabilization(float force)
		{
		}

		public virtual void OnDriverStatusDamageEvent(float damage, HitEntity owner)
		{
		}

		public virtual void CheckLocationOnGround()
		{
			OnGround = true;
		}

		public virtual void SetDummyDriver(DummyDriver driver)
		{
			if ((bool)driver)
			{
				DummyDriver = driver;
				CurrentDriver = DummyDriver.DriverStatus;
				PoolManager.Instance.AddBeforeReturnEvent(driver, delegate
				{
					DummyDriver = null;
					if (PlayerInteractionsManager.Instance.LastDrivableVehicle != this)
					{
						CurrentDriver = null;
						GetVehicleStatus().Faction = Faction.NoneFaction;
					}
				});
			}
		}

		public VehicleStatus GetVehicleStatus()
		{
			if (vehStatus == null)
			{
				vehStatus = GetComponentInChildren<VehicleStatus>();
			}
			return vehStatus;
		}

		protected virtual void Awake()
		{
			if (chatacterLayerNumber == 0)
			{
				chatacterLayerNumber = LayerMask.NameToLayer("Character");
			}
			if (smallDynamicLayerNumber == 0)
			{
				smallDynamicLayerNumber = LayerMask.NameToLayer("SmallDynamic");
			}
			if (terrainLayerNumber == 0)
			{
				terrainLayerNumber = LayerMask.NameToLayer("Terrain");
			}
			if (complexStaticObjectLayerNumber == 0)
			{
				complexStaticObjectLayerNumber = LayerMask.NameToLayer("ComplexStaticObject");
			}
			if (staticObjectLayerNumber == 0)
			{
				staticObjectLayerNumber = LayerMask.NameToLayer("SimpleStaticObject");
			}
			if (bigDynamicLayerNumber == 0)
			{
				bigDynamicLayerNumber = LayerMask.NameToLayer("BigDynamic");
			}
			if (defaultLayerNumber == 0)
			{
				defaultLayerNumber = LayerMask.NameToLayer("Default");
			}
			if (WaterLayerNumber == 0)
			{
				WaterLayerNumber = LayerMask.NameToLayer("Water");
			}
			AnimateGetInOut = true;
			vehicleSource = GetComponentInChildren<VehicleSource>();
			boxSource = vehicleSource.SourceCollider;
			vehicleSource.RootVehicle = this;
			verySlowUpdateProc = new SlowUpdateProc(VerySlowUpdate, 10f);
			if (GetComponent<CarTransformer>() != null)
			{
				isTransformer = true;
			}
		}

		public virtual void Init()
		{
			if (VehicleControllerPrefab == null)
			{
				throw new Exception($"{base.gameObject.name} DrivableVehicle is missing VehicleControllerPrefab");
			}
			if (vehStatus == null)
			{
				vehStatus = GetComponentInChildren<VehicleStatus>();
			}
			vehStatus.Initialization();
			ApplyCenterOfMass(VehiclePoints.CenterOfMass);
			ChangeBodyColor(BodyRenderers);
			if (!WaterSensor)
			{
				WaterSensor = GetComponentInChildren<WaterSensor>();
			}
			if ((bool)WaterSensor)
			{
				WaterSensor.Init();
			}
		}
		public Material GetBodyMaterial()
		{
			if (bodyMaterialIndex == -1)
			{
				int num = bodyMaterialIndex = UnityEngine.Random.Range(0, BodyMaterials.Length);
			}
			return BodyMaterials[bodyMaterialIndex];
		}

		public virtual void DeInit()
		{
			AnimateGetInOut = true;
			if (controller != null)
			{
				PoolManager.Instance.ReturnToPool(controller);
				controller = null;
			}
			RemoveObstacle();
			if ((bool)WaterSensor)
			{
				WaterSensor.Reset();
			}
		}

		public virtual bool IsAbleToEnter()
		{
			return MainRigidbody.velocity.magnitude < 20f && !isTransformer && !base.gameObject.CompareTag("Racer");
		}

		public VehicleSource GetVehicleSource()
		{
			return vehicleSource;
		}

		public virtual bool PointOnTheLeft(Vector3 pointPosition)
		{
			float num = Vector3.Distance(MainRigidbody.transform.position + MainRigidbody.transform.right, pointPosition);
			float num2 = Vector3.Distance(MainRigidbody.transform.position - MainRigidbody.transform.right, pointPosition);
			return num < num2;
		}

		public void ChangeSubstrate(bool isSubstrate)
		{
			if ((bool)Wheels && (bool)VehicleSubstrateColider)
			{
				Wheels.SetActive(!isSubstrate);
				VehicleSubstrateColider.SetActive(isSubstrate);
			}
		}

		public virtual void ConstraintsSetup(bool isin)
		{
		}

		public virtual void Drive(Player driver)
		{
			if (VehicleControllerPrefab != null && controller == null)
			{
				ChangeSubstrate(isSubstrate: false);
				if (vehStatus == null)
				{
					vehStatus = GetComponentInChildren<VehicleStatus>();
					vehStatus.Initialization();
				}
				vehStatus.SetCameraIgnoreCollision();
				vehStatus.Faction = driver.Faction;
				MainRigidbody.velocity = Vector3.zero;
				controller = PoolManager.Instance.GetFromPool<VehicleController>(VehicleControllerPrefab);
				PoolManager.Instance.AddBeforeReturnEvent(controller, delegate
				{
					if (SimpleModel != null)
					{
						SimpleModel.SetActive(value: true);
					}
				});
				GameObject gameObject = controller.gameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				controller.Init(this);
				if (SimpleModel != null)
				{
					SimpleModel.SetActive(value: false);
				}
				TrafficDriver componentInChildren = GetComponentInChildren<TrafficDriver>();
				if (componentInChildren != null)
				{
					PoolManager.Instance.ReturnToPool(componentInChildren);
				}
				GameEventManager.Instance.Event.GetIntoVehicleEvent(this);
				AnimateGetInOut = true;
			}
		}

		public virtual void SteerStabilization(float steer)
		{
		}

		public virtual void GetOut()
		{
			CurrentDriver = null;
			if (controller != null)
			{
				oldController = controller;
				controller.DeInit(delegate
				{
					ChangeSubstrate(isSubstrate: true);
					PoolManager.Instance.ReturnToPool(oldController);
					GameEventManager.Instance.Event.GetOutVehicleEvent(this);
				});
				controller = null;
			}
			vehStatus.RemoveCameraIgnoreCollision();
			vehStatus.Faction = Faction.NoneFaction;
		}

		public void ChangeBodyColor(Renderer[] renderers)
		{
			if (!Tuning)
			{
				return;
			}
			Material bodyMaterial = GetBodyMaterial();
			foreach (Renderer renderer in renderers)
			{
				if (renderer.materials.Length == 1)
				{
					renderer.material = bodyMaterial;
				}
				else
				{
					if (renderer.materials.Length < 2)
					{
						continue;
					}
					Material[] materials = renderer.materials;
					int num = materials.Length;
					int num2 = -1;
					for (int j = 0; j < num; j++)
					{
						string a = materials[j].name.Substring(0, 4);
						if (a == "Body")
						{
							num2 = j;
						}
					}
					materials[num2] = bodyMaterial;
					renderer.materials = materials;
				}
			}
		}

		public virtual bool AddObstacle()
		{
			if (obstacle != null)
			{
				return false;
			}
			obstacle = base.transform.gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();
			obstacle.size = boxSource.size;
			obstacle.center = boxSource.center;
			obstacle.carving = true;
			obstacle.carveOnlyStationary = false;
			return true;
		}

		public virtual void RemoveObstacle()
		{
			if ((bool)obstacle)
			{
				UnityEngine.Object.Destroy(obstacle);
			}
		}

		public virtual void StopVehicle()
		{
			WheelCollider[] componentsInChildren = GetComponentsInChildren<WheelCollider>();
			WheelCollider[] array = componentsInChildren;
			foreach (WheelCollider wheelCollider in array)
			{
				wheelCollider.brakeTorque = 10f;
			}
		}

		private void OnEnable()
		{
			StopVehicle();
			StartCoroutine(CheckingIsOnWater());
		}

		private IEnumerator CheckingIsOnWater()
		{
			WaitForSeconds checkingIsOnWater = new WaitForSeconds(0.5f);
			while (true)
			{
				float num = waterDepth;
				Vector3 vector = base.transform.position + base.transform.up * MaxAcceptableWaterHigh;
				DeepInWater = (num > vector.y);
				yield return checkingIsOnWater;
			}
		}

		public virtual void OnCollisionEnter(Collision c)
		{
			Effects(c);
			float num = Vector3.Dot(c.relativeVelocity, c.contacts[0].normal);
			if (!(Mathf.Abs(num) >= SpeedForImpact))
			{
				return;
			}
			if (c.collider.gameObject.layer == bigDynamicLayerNumber)
			{
				VehicleStatus vehicleStatus = c.collider.transform.GetComponentInChildren<VehicleStatus>();
				if (!vehicleStatus)
				{
					vehicleStatus = c.collider.transform.GetComponentInParent<VehicleStatus>();
				}
				if ((bool)vehicleStatus)
				{
					vehicleStatus.OnHit(DamageType.Collision, CurrentDriver, num * DamagePerSpeed, c.contacts[0].point, (c.contacts[0].point - base.transform.position).normalized, 0f);
				}
			}
			if (DamagingFromCollision && (c.collider.gameObject.layer == complexStaticObjectLayerNumber || c.collider.gameObject.layer == staticObjectLayerNumber || c.collider.gameObject.layer == terrainLayerNumber))
			{
				vehStatus.OnHit(DamageType.Collision, CurrentDriver, num * DamagePerSpeed, c.contacts[0].point, c.contacts[0].normal.normalized, 0f);
			}
			OnCollisionSpecific(c);
		}

		protected virtual void OnCollisionSpecific(Collision col)
		{
		}

		public virtual void OpenVehicleDoor(VehicleDoor door, bool isGettingIn)
		{
		}

		public virtual Vector3 GetExitPosition(bool toLeft)
		{
			return (!toLeft) ? VehiclePoints.EnterFromPositions[0].position : VehiclePoints.EnterFromPositions[1].position;
		}

		public virtual bool IsDoorBlockedOffset(LayerMask blockedLayerMask, Transform driver, out Vector3 offset, bool horizontalCheckOnly = true)
		{
			Vector3 vector = base.transform.position + base.transform.up * VehicleSpecificPrefab.MaxHeight * 0.5f;
			CapsuleCollider capsuleCollider = PlayerManager.Instance.Player.collider as CapsuleCollider;
			Vector3 vector2 = capsuleCollider.transform.position + capsuleCollider.center + capsuleCollider.transform.up * capsuleCollider.height / 2f;
			UnityEngine.Debug.DrawRay(vector, -base.transform.right * ExitLeftMinDistance, Color.cyan, 5f);
			UnityEngine.Debug.DrawRay(vector2, -capsuleCollider.transform.up * capsuleCollider.height, Color.yellow, 5f);
			RaycastHit hitInfo;
			bool flag = !Physics.Raycast(vector, -base.transform.right, out hitInfo, ExitLeftMinDistance, blockedLayerMask);
			bool flag2 = horizontalCheckOnly || !Physics.Raycast(vector2, -capsuleCollider.transform.up, out hitInfo, capsuleCollider.height, blockedLayerMask);
			if (flag && flag2)
			{
				offset = -base.transform.right * ExitLeftMinDistance;
				return false;
			}
			offset = ((!Physics.Raycast(vector, base.transform.right, out hitInfo, ExitRightMinDistance, blockedLayerMask)) ? (base.transform.right * ExitRightMinDistance) : (Vector3.up * ExitUpMinDistance));
			return true;
		}

		protected virtual void FixedUpdate()
		{
			verySlowUpdateProc.ProceedOnFixedUpdate();
			if (hitTimer > -1f)
			{
				hitTimer -= 2f * Time.deltaTime;
			}
			waterDepth = WaterSensor.CurrWaterSurfaceHeight;
			if (WaterSensor.InWater && DeepInWater && CurrentDriver != null)
			{
				CurrentDriver.Drowning(waterDepth, 1f);
				GameEventManager.Instance.Event.VehicleDrawingEvent(this);
			}
			DrowningDummyDriver();
			CheckLocationOnGround();
		}

		public void ResetDriver()
		{
			CurrentDriver = null;
			vehStatus.Faction = Faction.NoneFaction;
		}

		protected void DrowningDummyDriver()
		{
			if (DeepInWater && (bool)DummyDriver)
			{
				DummyDriver.DropRagdoll(null, Vector3.zero, false, true, waterDepth);
				ResetDriver();
			}
		}

		private void VerySlowUpdate()
		{
			if (!SectorManager.Instance.IsInActiveSector(base.transform.position) && !base.gameObject.CompareTag("Racer") && !base.gameObject.CompareTag("PlayerRacer"))
			{
				DestroyVehicle();
			}
		}

		public void DestroyVehicle()
		{
			TrafficDriver componentInChildren = GetComponentInChildren<TrafficDriver>();
			if (componentInChildren != null)
			{
				componentInChildren.DestroyVehicle();
			}
			else if (!PoolManager.Instance.ReturnToPool(base.gameObject))
			{
				if (GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log("Shouldn't happen: Vehicle Destroy on DrivableVehicle");
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void ApplyCenterOfMass(Transform newCenterOfMass)
		{
			if (MainRigidbody != null && newCenterOfMass != null)
			{
				Rigidbody rigidbody = MainRigidbody;
				Vector3 localPosition = newCenterOfMass.localPosition;
				float x = localPosition.x;
				Vector3 localScale = base.transform.localScale;
				float x2 = x * localScale.x;
				Vector3 localPosition2 = newCenterOfMass.localPosition;
				float y = localPosition2.y;
				Vector3 localScale2 = base.transform.localScale;
				float y2 = y * localScale2.y;
				Vector3 localPosition3 = newCenterOfMass.localPosition;
				float z = localPosition3.z;
				Vector3 localScale3 = base.transform.localScale;
				rigidbody.centerOfMass = new Vector3(x2, y2, z * localScale3.z);
			}
			CalculateInertiaTensor();
		}

		private void CalculateInertiaTensor()
		{
			Quaternion rotation = base.transform.rotation;
			base.transform.rotation = Quaternion.identity;
			Collider component = vehStatus.GetComponent<Collider>();
			var bounds = component.bounds;
			Vector3 size = bounds.size;
			float x = size.x;
			Vector3 size2 = bounds.size;
			float y = size2.y;
			Vector3 size3 = bounds.size;
			float z = size3.z;
			float mass = MainRigidbody.mass;
			float x2 = 0.083f * mass * (z * z + y * y);
			x2 = x2 <= 0 ? .1f : x2;
			float y2 = 0.083f * mass * (x * x + z * z);
			y2 = y2 <= 0 ? .1f : y2;
			float z2 = 0.083f * mass * (x * x + y * y);
			z2 = z2 <= 0 ? .1f : z2;
			MainRigidbody.inertiaTensorRotation = Quaternion.identity;
			MainRigidbody.inertiaTensor = new Vector3(x2, y2, z2);
			MainRigidbody.angularVelocity = Vector3.zero;
			transform.rotation = rotation;
		}

		public void Effects(Collision c)
		{
			if (c.contacts.Length < 1 || c.gameObject.layer == terrainLayerNumber)
			{
				return;
			}
			float num = Mathf.Abs(Vector3.Dot(c.relativeVelocity, c.contacts[0].normal));
			if (c.gameObject.layer == chatacterLayerNumber && num > 7f)
			{
				if (c.gameObject.tag == "Player" && PlayerManager.Instance.Player.IsTransformer)
				{
					PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitCar);
				}
				else
				{
					PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitHuman);
				}
			}
			else if (c.gameObject.layer == staticObjectLayerNumber || c.gameObject.layer == complexStaticObjectLayerNumber)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, (!(num < 30f)) ? TypeOfSound.StrongCarHit : TypeOfSound.CarHit);
				if ((bool)controller)
				{
					controller.Particles(c);
				}
			}
			else if ((c.gameObject.layer == bigDynamicLayerNumber && num > 7f) || c.gameObject.layer == defaultLayerNumber)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, (!(num < 30f)) ? TypeOfSound.StrongCarHit : TypeOfSound.CarHitCar);
				if ((bool)controller)
				{
					controller.Particles(c);
				}
			}
			else
			{
				if (c.gameObject.layer != smallDynamicLayerNumber || !(num > 7f))
				{
					return;
				}
				Rigidbody component = c.gameObject.GetComponent<Rigidbody>();
				if ((bool)component)
				{
					float mass = component.mass;
					if (mass < 5f && hitTimer <= 0f)
					{
						hitTimer = 1f;
						PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitHuman);
					}
				}
				else if (hitTimer <= 0f)
				{
					hitTimer = 1f;
					PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, (!(num < 30f)) ? TypeOfSound.StrongCarHit : TypeOfSound.CarHit);
					if ((bool)controller)
					{
						controller.Particles(c);
					}
				}
			}
		}
	}
}
