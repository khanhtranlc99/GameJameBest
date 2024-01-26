using Code.Game.Race;
using Game.Character;
using Game.Character.CharacterController;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Traffic
{
	public class TrafficManager : MonoBehaviour
	{
		private delegate bool SpawnAtPoint(RoadPoint point);

		private const float LinePointUpShift = 0.5f;

		public static float NodesMaxDistance = 50f;

		private static TrafficManager instance;

		[Header("Common configuration")]
		public LayerMask CollisionDetectionLayes;

		[Header("Vehicles configuration")]
		public float RoadLineSize = 3f;

		public TrafficDriver TrafficDriverPrefab;

		public Autopilot AutopilotPrefab;

		public Autopilot RacerAutopilotPrefab;

		public TransformerAutopilot TransformerAutopilotPrefab;

		public DrivableVehicle[] VehiclesPrefab;

		public DrivableVehicle[] CopVehiclesPrefab;

		public DrivableVehicle[] TransformerVehiclesPrefab;

		public DrivableVehicle[] OnlyPlayerVehicles;

		public VehicleDriversWeight VehicleDriversWeight;

		public int MaxCountVehicles;

		public int MaxCountPedestrians;

		[Header("Sidewalk configuration")]
		public float SideWalkLineSize = 0.3f;

		[Header("Debug")]
		public bool DebugShowLines;

		public TextAsset SerializedMapForVehicle;

		public TextAsset SerializedMapForPedestrian;

		public GameObject CiviliansWeightPoints;

		public float CopsSpawnCooldown = 1f;

		public int CopsPerStar = 1;

		[Tooltip("If lower than 1 they will spawn only in fast spawn areas")]
		public float TransformerSpawnCooldown;

		[Tooltip("Must be multiply three")]
		public int TransformersMaxCountOnHighLevel = 3;

		private int currentVehicleCount;

		private int currentCopsVehicleCount;

		private int maxCopsVehicle;

		private int currStarsCount;

		private float lastTransformerSpawnTime;

		private float lastCopsSpawnTime;

		private float deffTransformerSpawnTime;

		private int currentTransformersCount;

		private int currentPedestrianCount;

		private SlowUpdateProc slowUpdateProc;

		private IDictionary<int, List<RoadPoint>> sectorToRoadPoints = new Dictionary<int, List<RoadPoint>>();

		private IDictionary<int, List<RoadPoint>> sectorToSidewalkPoints = new Dictionary<int, List<RoadPoint>>();

		private List<RoadPoint> listRoadPoints = new List<RoadPoint>();

		private List<RoadPoint> listPointsSidewalk = new List<RoadPoint>();

#if UNITY_EDITOR
		public Node[] arrayNodeRoadPoint;
		public Node[] arrayNodeSidewalk;
		public RoadMapPreprocessor RoadMapPreprocessor;
#endif

		private IDictionary<int, PrefabDistribution> sectorToPedestrianDestribution = new Dictionary<int, PrefabDistribution>();

		private List<CivilianWeightObject> civilianWeightObjects = new List<CivilianWeightObject>();

		private readonly List<int> currentActiveSectors = new List<int>();

		private readonly List<BaseNPC> spawnedNpcs = new List<BaseNPC>();

		private readonly List<DrivableVehicle> spawnedVehicle = new List<DrivableVehicle>();

		private bool awaked;

		private bool isAllowedSpawnTraffic = true;

		private readonly Collider[] arrayForOverlapSphereNonAlloc = new Collider[1];

		public static TrafficManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("TrafficManager is not initialized");
				}
				return instance;
			}
		}

		private int TransformersMaxCount
		{
			get
			{
				if (PlayerInfoManager.Level <= 8)
				{
					return TransformersMaxCountOnHighLevel / 3;
				}
				if (PlayerInfoManager.Level > 16)
				{
					return TransformersMaxCountOnHighLevel;
				}
				return TransformersMaxCountOnHighLevel * 2 / 3;
			}
			set => TransformersMaxCountOnHighLevel = ((value % 3 != 0) ? (value - value % 3) : value);
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			StartCoroutine(AwakeDeferred());
		}

#if UNITY_EDITOR
		private void EditorTestGetData()
		{
			var roadPointCar = RoadMapPreprocessor.GetRoadPointArray(arrayNodeRoadPoint);
			AddPoints(roadPointCar, sectorToRoadPoints, listRoadPoints);
			var roadSideWalk = RoadMapPreprocessor.GetRoadPointArray(arrayNodeSidewalk);
			AddPoints(roadSideWalk, sectorToSidewalkPoints, listPointsSidewalk);
		}
#endif


		private System.Collections.IEnumerator  AwakeDeferred()
		{
			yield return new WaitForSeconds(.5f);
#if UNITY_EDITOR
			EditorTestGetData();
#else
			if (instance == null)
			{
				instance = this;
			}
			if (SerializedMapForVehicle != null)
			{
				object obj = MiamiSerializier.JSONDeserialize(SerializedMapForVehicle.text);
				yield return null;
				RoadPoint[] points = (RoadPoint[])obj;
				AddPoints(points, sectorToRoadPoints, listRoadPoints);
			}
			else
			{
				Debug.LogError("Missing SerializedMapForVehicle");
			}
			yield return new WaitForSeconds(.1f);
			if (SerializedMapForPedestrian != null)
			{
				object obj = MiamiSerializier.JSONDeserialize(SerializedMapForPedestrian.text);
				yield return null;
				RoadPoint[] points = (RoadPoint[])obj;
				AddPoints(points, sectorToSidewalkPoints, listPointsSidewalk);
			}
			else
			{
				Debug.LogError("Missing SerializedMapForVehicle");
			}
#endif 
			yield return null;
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 0.5f);
			civilianWeightObjects.AddRange(CiviliansWeightPoints.GetComponentsInChildren<CivilianWeightObject>());
			InitDistribution();
			PreInitPrefabs();
			deffTransformerSpawnTime = TransformerSpawnCooldown;
			TransformersMaxCount = TransformersMaxCountOnHighLevel;
			awaked = true;
		}
		
		public void CalcTargetPoint(RoadPoint from, RoadPoint to, int line, out Vector3 startLine, out Vector3 endLine)
		{
			Vector3 normalized = (to.Point - from.Point).normalized;
			Vector3 b = CrossRoadShift(to, normalized, RoadLineSize * 2f);
			Vector3 b2 = CrossRoadShift(from, normalized, RoadLineSize * 2f);
			float turnRadius;
			Vector3 a = LineNormal(to, from, out turnRadius);
			float turnRadius2;
			Vector3 a2 = LineNormal(from, to, out turnRadius2);
			float d = CalculateSpacerLineWidth(from, to);
			Vector3 b3 = a * RoadLineSize * ((float)line - 0.5f) * turnRadius + a * d;
			Vector3 b4 = a2 * RoadLineSize * ((float)line - 0.5f) * turnRadius2 + a2 * d;
			endLine = to.Point - b - b3 + 0.5f * Vector3.up;
			startLine = from.Point + b2 + b4 + 0.5f * Vector3.up;
		}

		public void CalcTargetSidewalkPoint(RoadPoint from, RoadPoint to, int line, out Vector3 startLine, out Vector3 endLine)
		{
			Vector3 normalized = (to.Point - from.Point).normalized;
			Vector3 b = CrossRoadShift(to, normalized, SideWalkLineSize);
			Vector3 b2 = CrossRoadShift(from, normalized, SideWalkLineSize);
			float turnRadius;
			Vector3 a = LineNormal(to, from, out turnRadius);
			float turnRadius2;
			Vector3 a2 = LineNormal(from, to, out turnRadius2);
			float d = CalculateSpacerLineWidth(from, to);
			Vector3 b3 = a * SideWalkLineSize * ((float)line - 0.5f) * turnRadius + a * d;
			Vector3 b4 = a2 * SideWalkLineSize * ((float)line - 0.5f) * turnRadius2 + a2 * d;
			endLine = to.Point - b - b3;
			startLine = from.Point + b2 + b4;
		}

		public void GetNextRoute(ref RoadPoint from, ref RoadPoint to, ref int line)
		{
			int num = UnityEngine.Random.Range(0, to.RoadLinks.Length);
			RoadPoint link = to.RoadLinks[num].Link;
			if (link.Equals(from))
			{
				link = to.RoadLinks[(num + 1) % to.RoadLinks.Length].Link;
			}
			from = to;
			to = link;
			if (to.LineCount > from.LineCount || from.RoadLinks.Length != 2)
			{
				line = UnityEngine.Random.Range(0, to.LineCount) + 1;
			}
			else
			{
				line = Mathf.Min(line, to.LineCount);
			}
		}

		public RoadPoint FindClosestPedestrianPoint(Vector3 pos)
		{
			RoadPoint result = null;
			int sector = SectorManager.Instance.GetSector(pos);
			if (sectorToSidewalkPoints.ContainsKey(sector) && sectorToSidewalkPoints[sector].Count > 0)
			{
				float num = float.PositiveInfinity;
				{
					foreach (RoadPoint item in sectorToSidewalkPoints[sector])
					{
						float num2 = Vector3.Distance(item.Point, pos);
						if (num2 < num)
						{
							result = item;
							num = num2;
						}
					}
					return result;
				}
			}
			return result;
		}

		public void TrafficVehicleOutOfRange(DrivableVehicle vehicle, TrafficDriver trafficDriver)
		{
			PoolManager.Instance.ReturnToPool(vehicle);
		}

		public void TakePedestrianSlot(BaseNPC npc)
		{
			currentPedestrianCount++;
			if ((bool)npc)
			{
				spawnedNpcs.Add(npc);
				PoolManager.Instance.AddBeforeReturnEvent(npc, delegate
				{
					spawnedNpcs.Remove(npc);
				});
			}
		}

		public void FreePedestrianSlot()
		{
			currentPedestrianCount--;
		}

		public void TakeCopVehicleSlot()
		{
			currentCopsVehicleCount++;
		}

		public void FreeCopVehicleSlot()
		{
			currentCopsVehicleCount--;
		}

		public void TakeTransformerVehicleSlot()
		{
			currentTransformersCount++;
		}

		public void FreeTransformerVehicleSlot()
		{
			currentTransformersCount--;
		}

		public void ChangeTrafficDensity(float value)
		{
			MaxCountVehicles = (int)value;
			MaxCountPedestrians = (int)value;
		}

		public void CalmDownCops()
		{
			for (int i = 0; i < spawnedNpcs.Count; i++)
			{
				BaseNPC baseNPC = spawnedNpcs[i];
				if (baseNPC.StatusNpc.Faction == Faction.Police)
				{
					baseNPC.ChangeController(baseNPC.QuietControllerType);
				}
			}
			for (int j = 0; j < spawnedVehicle.Count; j++)
			{
				Autopilot componentInChildren = spawnedVehicle[j].GetComponentInChildren<Autopilot>();
				if ((bool)componentInChildren)
				{
					PoolManager.Instance.ReturnToPool(componentInChildren.transform.parent);
				}
			}
		}

		private Vector3 CrossRoadShift(RoadPoint atPoint, Vector3 lineDirection, float shiftValue)
		{
			return (atPoint.RoadLinks.Length <= 2) ? Vector3.zero : (lineDirection * shiftValue * atPoint.LineCount);
		}

		private Vector3 LineNormal(RoadPoint normalPoint, RoadPoint oppositePoint, out float turnRadius)
		{
			Vector3 normalized = (normalPoint.Point - oppositePoint.Point).normalized;
			Vector3 result;
			if (normalPoint.RoadLinks.Length == 2)
			{
				int num = normalPoint.RoadLinks[0].Link.Equals(oppositePoint) ? 1 : 0;
				Vector3 normalized2 = (normalPoint.Point - oppositePoint.Point).normalized;
				Vector3 normalized3 = (normalPoint.RoadLinks[num].Link.Point - normalPoint.Point).normalized;
				Vector3 vector = normalized2 + normalized3;
				turnRadius = (normalized2 - normalized3).magnitude;
				turnRadius = Mathf.Max(1f, turnRadius);
				result = Vector3.Cross(vector.normalized, Vector3.up);
			}
			else
			{
				result = Vector3.Cross(normalized, Vector3.up);
				turnRadius = 1f;
			}
			return result;
		}

		private void AddPoints(RoadPoint[] points, IDictionary<int, List<RoadPoint>> sectorToPoint, List<RoadPoint> listPoints)
		{
			foreach (RoadPoint roadPoint in points)
			{
				listPoints.Add(roadPoint);
				int sector = SectorManager.Instance.GetSector(roadPoint.Point);
				if (!sectorToPoint.ContainsKey(sector))
				{
					sectorToPoint.Add(sector, new List<RoadPoint>());
				}
				sectorToPoint[sector].Add(roadPoint);
			}
			//Debug.LogError(listPoints.Count);

			//Debug.LogError(listPoints.Count);
		}

		private float CalculateSpacerLineWidth(RoadPoint firsPoint, RoadPoint secondPoint)
		{
			RoadLink roadLink = null;
			RoadLink roadLink2 = null;
			RoadLink[] roadLinks = firsPoint.RoadLinks;
			foreach (RoadLink roadLink3 in roadLinks)
			{
				if (roadLink3.Link == secondPoint)
				{
					roadLink = roadLink3;
					break;
				}
			}
			RoadLink[] roadLinks2 = secondPoint.RoadLinks;
			foreach (RoadLink roadLink4 in roadLinks2)
			{
				if (roadLink4.Link == firsPoint)
				{
					roadLink2 = roadLink4;
					break;
				}
			}
			if (roadLink == null && roadLink2 == null)
			{
				return 0f;
			}
			if (roadLink == null && roadLink2 != null)
			{
				return roadLink2.SpacerLineWidth;
			}
			if (roadLink2 == null && roadLink != null)
			{
				return roadLink.SpacerLineWidth;
			}
			return (roadLink.SpacerLineWidth + roadLink2.SpacerLineWidth) / 2f;
		}

		private void OnDrawGizmos()
		{
			if (DebugShowLines)
			{
				Gizmos.color = new Color(0f, 0f, 1f, 1f);
				foreach (RoadPoint listRoadPoint in listRoadPoints)
				{
					RoadLink[] roadLinks = listRoadPoint.RoadLinks;
					foreach (RoadLink roadLink in roadLinks)
					{
						for (int j = 0; j < listRoadPoint.LineCount; j++)
						{
							Vector3 startLine;
							Vector3 endLine;
							CalcTargetPoint(listRoadPoint, roadLink.Link, j + 1, out startLine, out endLine);
							Gizmos.DrawLine(startLine, endLine);
							Gizmos.DrawSphere(startLine, RoadLineSize * 0.5f);
							Gizmos.DrawSphere(endLine, RoadLineSize * 0.5f);
						}
					}
				}
				foreach (RoadPoint item in listPointsSidewalk)
				{
					RoadLink[] roadLinks2 = item.RoadLinks;
					foreach (RoadLink roadLink2 in roadLinks2)
					{
						for (int l = 0; l < item.LineCount; l++)
						{
							Vector3 startLine2;
							Vector3 endLine2;
							CalcTargetSidewalkPoint(item, roadLink2.Link, l + 1, out startLine2, out endLine2);
							Gizmos.DrawLine(startLine2, endLine2);
							Gizmos.DrawSphere(startLine2, SideWalkLineSize * 0.5f);
							Gizmos.DrawSphere(endLine2, SideWalkLineSize * 0.5f);
						}
					}
				}
			}
		}

		private void FixedUpdate()
		{
			if (awaked)
			{
				slowUpdateProc.ProceedOnFixedUpdate();
			}
		}

		private void SlowUpdate()
		{
			currentPedestrianCount = Mathf.Max(0, currentPedestrianCount);
			int num = MaxCountVehicles - currentVehicleCount;
			if (num > 0)
			{
				currentVehicleCount += SpawnTraffic(1, sectorToRoadPoints, SpawnVehicleAtPoint);
			}
			num = MaxCountPedestrians - currentPedestrianCount;
			if (num > 0)
			{
				currentPedestrianCount += SpawnTraffic(1, sectorToSidewalkPoints, SpawnPedestrtianAtPoint);
			}
			UpdateTransformerSpawnRate();
		}

		private void UpdateTransformerSpawnRate()
		{
			if (FastSpawnArea.PlayerInArea)
			{
				TransformerSpawnCooldown = 1f;
			}
			else
			{
				TransformerSpawnCooldown = deffTransformerSpawnTime;
			}
		}

		public void ResetTransformersSpawnTime()
		{
			lastTransformerSpawnTime = Time.time;
		}

		private int SpawnTraffic(int countToSpawn, IDictionary<int, List<RoadPoint>> sectorToPoints, SpawnAtPoint spawnAtPoint)
		{
			if (!isAllowedSpawnTraffic)
			{
				return 0;
			}
			if (countToSpawn == 0)
			{
				return 0;
			}
			int num = 0;
			currentActiveSectors.Clear();
			SectorManager.Instance.GetAllActiveSectorsNonAlloc(currentActiveSectors);
			Vector3 dynamicWorldCenter = SectorManager.Instance.DynamicWorldCenter;
			int num2 = UnityEngine.Random.Range(0, currentActiveSectors.Count);
			int num3 = 0;
			do
			{
				int key = currentActiveSectors[num2];
				if (sectorToPoints.ContainsKey(key) && sectorToPoints[key].Count > 0)
				{
					int num4 = UnityEngine.Random.Range(0, sectorToPoints[key].Count);
					int num5 = 0;
					do
					{
						RoadPoint roadPoint = sectorToPoints[key][num4];
						num4 = (num4 + 1) % sectorToPoints[key].Count;
						num5++;
						if (Vector3.Distance(roadPoint.Point, dynamicWorldCenter) > SectorManager.Instance.DynamicSectorSize && spawnAtPoint(roadPoint))
						{
							num++;
						}
					}
					while (num5 < sectorToPoints[key].Count && num < countToSpawn);
				}
				num2 = (num2 + 1) % currentActiveSectors.Count;
				num3++;
			}
			while (num3 < currentActiveSectors.Count && num < countToSpawn);
			return num;
		}

		private bool SpawnPedestrtianAtPoint(RoadPoint point)
		{
			if (point.RoadLinks.Length == 0)
			{
				return false;
			}
			RoadPoint roadPoint = BestDestinationPoint(point);
			float radius = 2f;
			int num = UnityEngine.Random.Range(0, point.LineCount);
			for (int i = 0; i < point.LineCount; i++)
			{
				Vector3 startLine;
				Vector3 _;
				CalcTargetSidewalkPoint(point, roadPoint, num + 1, out startLine, out _);
				if (Physics.OverlapSphereNonAlloc(startLine, radius, arrayForOverlapSphereNonAlloc, CollisionDetectionLayes) == 0)
				{
					int sector = SectorManager.Instance.GetSector(point.Point);
					GameObject randomPrefab = sectorToPedestrianDestribution[sector].GetRandomPrefab();
					BaseNPC dummyNpc = PoolManager.Instance.GetFromPool(randomPrefab, startLine, Quaternion.identity).GetComponent<BaseNPC>();
					spawnedNpcs.Add(dummyNpc);
					PoolManager.Instance.AddBeforeReturnEvent(dummyNpc, delegate
					{
						spawnedNpcs.Remove(dummyNpc);
						FreePedestrianSlot();
					});
					dummyNpc.WaterSensor.Reset();
					dummyNpc.transform.parent = base.transform;
					dummyNpc.transform.forward = Vector3.forward;
					if (FactionsManager.Instance.GetPlayerRelations(dummyNpc.StatusNpc.Faction) == Relations.Hostile)
					{
						BaseControllerNPC controller;
						dummyNpc.ChangeController(BaseNPC.NPCControllerType.Smart, out controller);
						SmartHumanoidController smartHumanoidController = controller as SmartHumanoidController;
						if (smartHumanoidController != null)
						{
							smartHumanoidController.AddTarget(PlayerInteractionsManager.Instance.Player);
							smartHumanoidController.InitBackToDummyLogic();
						}
					}
					else
					{
						BaseControllerNPC controller2;
						dummyNpc.ChangeController(BaseNPC.NPCControllerType.Pedestrian, out controller2);
						dummyNpc.QuietControllerType = BaseNPC.NPCControllerType.Pedestrian;
						PedestrianHumanoidController pedestrianHumanoidController = controller2 as PedestrianHumanoidController;
						if (pedestrianHumanoidController != null)
						{
							pedestrianHumanoidController.InitPedestrianPath(point, roadPoint, num + 1);
						}
					}
					return true;
				}
				num++;
				num %= point.LineCount;
			}
			return false;
		}

		private bool CheckCopsSpawn()
		{
			float playerRelationsValue = FactionsManager.Instance.GetPlayerRelationsValue(Faction.Police);
			currStarsCount = (int)Math.Truncate(Mathf.Abs(playerRelationsValue) / 2f);
			maxCopsVehicle = currStarsCount * CopsPerStar;
			if (playerRelationsValue > -2f)
			{
				return false;
			}
			if (Time.time - lastCopsSpawnTime < CopsSpawnCooldown)
			{
				return false;
			}
			if (currentCopsVehicleCount >= maxCopsVehicle)
			{
				return false;
			}
			return true;
		}

		private bool SpawnVehicleAtPoint(RoadPoint point)
		{
			if (point.RoadLinks.Length == 0)
			{
				return false;
			}
			RoadPoint roadPoint = BestDestinationPoint(point);
			bool flag = (FastSpawnArea.PlayerInArea || (deffTransformerSpawnTime >= 1f && Time.time - lastTransformerSpawnTime > TransformerSpawnCooldown)) && currentTransformersCount < TransformersMaxCount;
			bool flag2 = CheckCopsSpawn();
			DrivableVehicle drivableVehicle = flag ? TransformerVehiclesPrefab[UnityEngine.Random.Range(0, TransformerVehiclesPrefab.Length)] : ((!flag2) ? VehiclesPrefab[UnityEngine.Random.Range(0, VehiclesPrefab.Length)] : CopVehiclesPrefab[UnityEngine.Random.Range(0, CopVehiclesPrefab.Length)]);
			float maxLength = drivableVehicle.VehicleSpecificPrefab.MaxLength;
			int num = UnityEngine.Random.Range(0, point.LineCount);
			for (int i = 0; i < point.LineCount; i++)
			{
				Vector3 startLine;
				Vector3 endLine;
				CalcTargetPoint(point, roadPoint, num + 1, out startLine, out endLine);
				Collider[] array = Physics.OverlapSphere(startLine, maxLength, CollisionDetectionLayes);
				if (array == null || array.Length == 0)
				{
					DrivableVehicle vehicle = PoolManager.Instance.GetFromPool(drivableVehicle, startLine, Quaternion.identity);
					VehicleStatus vehicleStatus = vehicle.GetVehicleStatus();
					vehicle.transform.parent = base.transform;
					vehicle.transform.forward = Vector3.forward;
					spawnedVehicle.Add(vehicle);
					Vector3 position = vehicle.VehiclePoints.TrafficDriverPosition.position;
					Quaternion rotation = vehicle.VehiclePoints.TrafficDriverPosition.rotation;
					Component driver;
					if (flag)
					{
						Autopilot autopilot = PoolManager.Instance.GetFromPool(TransformerAutopilotPrefab, position, rotation);
						driver = autopilot;
						CarTransformer component = vehicle.GetComponent<CarTransformer>();
						HumanoidStatusNPC component2 = component.NPCRobotPrefab.GetComponent<HumanoidStatusNPC>();
						vehicleStatus.Health.Current = (vehicleStatus.Health.Max = component2.Health.Max);
						TakeTransformerVehicleSlot();
						PoolManager.Instance.AddBeforeReturnEvent(autopilot, delegate
						{
							FreeTransformerVehicleSlot();
							autopilot.DeInit();
						});
					}
					else if (flag2)
					{
						Autopilot autopilot2 = PoolManager.Instance.GetFromPool(AutopilotPrefab, position, rotation);
						driver = autopilot2;
						TakeCopVehicleSlot();
						PoolManager.Instance.AddBeforeReturnEvent(autopilot2, delegate
						{
							if (!autopilot2.DriverWasKilled && !autopilot2.DriverExit)
							{
								FreeCopVehicleSlot();
							}
							else if (!autopilot2.DriverWasKilled && autopilot2.DriverExit)
							{
								autopilot2.ChangeDropedCopKillEvent();
							}
							autopilot2.DeInit();
						});
					}
					else
					{
						TrafficDriver trafficDriver = PoolManager.Instance.GetFromPool(TrafficDriverPrefab, position, rotation);
						driver = trafficDriver;
						PoolManager.Instance.AddBeforeReturnEvent(trafficDriver, delegate
						{
							trafficDriver.DeInit();
						});
					}
					PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate
					{
						spawnedVehicle.Remove(vehicle);
						currentVehicleCount--;
					});
					driver.transform.parent = vehicle.transform;
					if (!(driver is TransformerAutopilot))
					{
						DummyDriver dummyDriver = PoolManager.Instance.GetFromPool(VehicleDriversWeight.GetVehicleDriver(drivableVehicle), vehicle.transform.position, vehicle.transform.rotation);
						PoolManager.Instance.AddBeforeReturnEvent(dummyDriver, delegate
						{
							dummyDriver.DeInitDriver();
						});
						dummyDriver.transform.parent = vehicle.transform;
						PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate
						{
							if (dummyDriver.transform.parent.Equals(vehicle.transform))
							{
								PoolManager.Instance.ReturnToPool(dummyDriver);
							}
						});
						dummyDriver.InitDriver(vehicle);
						vehicleStatus.Faction = dummyDriver.DriverStatus.Faction;
					}
					PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate
					{
						if (vehicle.CurrentDriver == null)
						{
							vehicle.GetVehicleStatus().Faction = Faction.NoneFaction;
						}
						if (driver.transform.parent.Equals(vehicle.transform))
						{
							PoolManager.Instance.ReturnToPool(driver);
						}
					});
					vehicle.transform.forward = (endLine - startLine).normalized;
					TransformerAutopilot transformerAutopilot = driver as TransformerAutopilot;
					if (transformerAutopilot != null)
					{
						vehicleStatus.Faction = Faction.Transformer;
						transformerAutopilot.InitChase(vehicle.MainRigidbody);
						if (currentTransformersCount >= TransformersMaxCount)
						{
							lastTransformerSpawnTime = Time.time;
						}
					}
					else if (driver is Autopilot)
					{
						((Autopilot)driver).InitChase(vehicle.MainRigidbody);
						if (currentCopsVehicleCount >= maxCopsVehicle)
						{
							lastCopsSpawnTime = Time.time;
						}
					}
					else
					{
						((TrafficDriver)driver).Init(vehicle.MainRigidbody, vehicle.VehiclePoints.TrafficDriverPosition, point, roadPoint, num + 1);
					}
					return true;
				}
				num++;
				num %= point.LineCount;
			}
			return false;
		}

		public DrivableVehicle SpawnConcreteVehicleForRace(DrivableVehicle drivableVehiclePrefab, Vector3 bestPoint, string tag, bool withDriver = true)
		{
			DrivableVehicle vehicle = PoolManager.Instance.GetFromPool(drivableVehiclePrefab, bestPoint, Quaternion.identity);
			vehicle.tag = tag;
			vehicle.transform.parent = base.transform;
			vehicle.transform.forward = Vector3.forward;
			spawnedVehicle.Add(vehicle);
			currentVehicleCount++;
			PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate
			{
				spawnedVehicle.Remove(vehicle);
				currentVehicleCount--;
			});
			if (withDriver)
			{
				DummyDriver dummyDriver = PoolManager.Instance.GetFromPool(VehicleDriversWeight.GetVehicleDriver(drivableVehiclePrefab), vehicle.transform.position, vehicle.transform.rotation);
				PoolManager.Instance.AddBeforeReturnEvent(dummyDriver, delegate
				{
					dummyDriver.DeInitDriver();
				});
				dummyDriver.transform.parent = vehicle.transform;
				PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate
				{
					if (dummyDriver.transform.parent.Equals(vehicle.transform))
					{
						PoolManager.Instance.ReturnToPool(dummyDriver);
					}
				});
				dummyDriver.InitDriver(vehicle);
				VehicleStatus vehicleStatus = vehicle.GetVehicleStatus();
				vehicleStatus.Faction = dummyDriver.DriverStatus.Faction;
			}
			return vehicle;
		}

		public void AddAutopilotForRacer(Racer racer, Transform[] wayPoints, int rounds)
		{
			Vector3 position = racer.GetDrivableVehicle().VehiclePoints.TrafficDriverPosition.position;
			Quaternion rotation = racer.GetDrivableVehicle().VehiclePoints.TrafficDriverPosition.rotation;
			Autopilot autopilot = PoolManager.Instance.GetFromPool(RacerAutopilotPrefab, position, rotation);
			Component driver = autopilot;
			driver.transform.parent = racer.GetDrivableVehicle().transform;
			((Autopilot)driver).InitRace(wayPoints, racer, rounds);
			PoolManager.Instance.AddBeforeReturnEvent(autopilot, delegate
			{
				autopilot.DeInit();
			});
			PoolManager.Instance.AddBeforeReturnEvent(racer.GetDrivableVehicle(), delegate
			{
				if (racer.GetDrivableVehicle().CurrentDriver == null)
				{
					racer.GetDrivableVehicle().GetVehicleStatus().Faction = Faction.NoneFaction;
				}
				if (driver.transform.parent.Equals(racer.GetDrivableVehicle().transform))
				{
					PoolManager.Instance.ReturnToPool(driver);
				}
			});
		}

		public void AddTrafficDriverForVehicle(DrivableVehicle vehicle)
		{
			int sector = SectorManager.Instance.GetSector(vehicle.transform.position);
			if (sectorToRoadPoints.ContainsKey(sector))
			{
				Vector3 position = vehicle.VehiclePoints.TrafficDriverPosition.position;
				Quaternion rotation = vehicle.VehiclePoints.TrafficDriverPosition.rotation;
				TrafficDriver trafficDriver = PoolManager.Instance.GetFromPool(TrafficDriverPrefab, position, rotation);
				PoolManager.Instance.AddBeforeReturnEvent(trafficDriver, delegate
				{
					trafficDriver.DeInit();
				});
				PoolManager.Instance.AddBeforeReturnEvent(vehicle, delegate
				{
					if (vehicle.CurrentDriver == null)
					{
						vehicle.GetVehicleStatus().Faction = Faction.NoneFaction;
					}
					if (trafficDriver.transform.parent.Equals(vehicle.transform))
					{
						PoolManager.Instance.ReturnToPool(trafficDriver);
					}
				});
				trafficDriver.transform.parent = vehicle.transform;
				int index = UnityEngine.Random.Range(0, sectorToRoadPoints[sector].Count);
				RoadPoint roadPoint = sectorToRoadPoints[sector][index];
				RoadPoint toPoint = BestDestinationPoint(roadPoint);
				trafficDriver.Init(vehicle.MainRigidbody, vehicle.VehiclePoints.TrafficDriverPosition, roadPoint, toPoint, 1);
			}
		}

		public void AllowSpawnTraffic(bool value)
		{
			isAllowedSpawnTraffic = value;
			if (!isAllowedSpawnTraffic)
			{
				ClearTraffic();
			}
		}

		private void ClearTraffic()
		{
			List<DrivableVehicle> list = new List<DrivableVehicle>();
			List<BaseNPC> list2 = new List<BaseNPC>();
			list.AddRange(spawnedVehicle);
			list2.AddRange(spawnedNpcs);
			foreach (DrivableVehicle item in list)
			{
				item.DestroyVehicle();
			}
			foreach (BaseNPC item2 in list2)
			{
				PoolManager.Instance.ReturnToPool(item2);
			}
		}

		public static RoadPoint BestDestinationPoint(RoadPoint point)
		{
			RoadPoint link = point.RoadLinks[0].Link;
			if (point.RoadLinks.Length > 1)
			{
				link = point.RoadLinks[UnityEngine.Random.Range(0, point.RoadLinks.Length)].Link;
			}
			return link;
		}

		[ContextMenu("Fix please")]
		private void ProblemFixer()
		{
			Node[] array = GetComponentsInChildren<Node>();
			foreach (Node node in array)
			{
				foreach (Node link in node.Links)
				{
					node.NodeLinks.Add(new NodeLink
					{
						Link = link,
						SpacerLineWidth = 0f
					});
				}
			}
		}

		private void InitDistribution()
		{
			foreach (CivilianWeightObject civilianWeightObject in civilianWeightObjects)
			{
				int sector = SectorManager.Instance.GetSector(civilianWeightObject.transform.position);
				if (!sectorToPedestrianDestribution.ContainsKey(sector))
				{
					sectorToPedestrianDestribution.Add(sector, civilianWeightObject.Distribution);
				}
				foreach (PrefabDistribution.Chance chance in civilianWeightObject.Distribution.Chances)
				{
					if (chance.Prefab.GetComponent<BaseNPC>() == null)
					{
						UnityEngine.Debug.LogErrorFormat("Weight object '{0}' contains not suitable prefab = {1}", civilianWeightObject, chance.Prefab.name);
					}
					else
					{
						PoolManager.Instance.InitPoolingPrefab(chance.Prefab);
					}
				}
				int[] aroundSectors = SectorManager.Instance.GetAroundSectors(sector);
				int[] array = aroundSectors;
				foreach (int key in array)
				{
					if (sectorToSidewalkPoints.ContainsKey(key))
					{
						if (sectorToPedestrianDestribution.ContainsKey(key))
						{
							PrefabDistribution prefabDistribution = sectorToPedestrianDestribution[key];
							sectorToPedestrianDestribution[key] = PrefabDistribution.AverageDistribution(prefabDistribution, civilianWeightObject.Distribution);
						}
						else
						{
							sectorToPedestrianDestribution.Add(key, civilianWeightObject.Distribution);
						}
					}
				}
			}
			foreach (int key2 in sectorToSidewalkPoints.Keys)
			{
				if (!sectorToPedestrianDestribution.ContainsKey(key2))
				{
					List<float> list = new List<float>();
					Dictionary<PrefabDistribution, float> dictionary = new Dictionary<PrefabDistribution, float>();
					foreach (CivilianWeightObject civilianWeightObject2 in civilianWeightObjects)
					{
						int sector2 = SectorManager.Instance.GetSector(civilianWeightObject2.transform.position);
						list.Add(SectorManager.Instance.DistanceInSectors(key2, sector2));
						dictionary.Add(civilianWeightObject2.Distribution, SectorManager.Instance.DistanceInSectors(key2, sector2));
					}
					float maxDistance = float.PositiveInfinity;
					if (list.Count >= 3)
					{
						list.Sort();
						maxDistance = list[2];
					}
					IDictionary<PrefabDistribution, float> distributionToDistance = (from pair in dictionary
						where pair.Value <= maxDistance
						select pair).ToDictionary((KeyValuePair<PrefabDistribution, float> pair) => pair.Key, (KeyValuePair<PrefabDistribution, float> pair) => pair.Value);
					PrefabDistribution value = PrefabDistribution.AverageDistanceDistribution(distributionToDistance);
					sectorToPedestrianDestribution.Add(key2, value);
					if (DebugShowLines)
					{
						UnityEngine.Debug.LogFormat("Sector #{0}\n{1}", key2, sectorToPedestrianDestribution[key2].GetStatusForLog());
					}
				}
			}
		}

		private void PreInitPrefabs()
		{
			Component[] first = new Component[3]
			{
				TrafficDriverPrefab,
				AutopilotPrefab,
				TransformerAutopilotPrefab
			};
			List<GameObject> list = new List<GameObject>();
			CivilianWeightObject[] componentsInChildren = CiviliansWeightPoints.GetComponentsInChildren<CivilianWeightObject>();
			foreach (CivilianWeightObject civilianWeightObject in componentsInChildren)
			{
				foreach (PrefabDistribution.Chance chance in civilianWeightObject.Distribution.Chances)
				{
					BaseNPC component = chance.Prefab.GetComponent<BaseNPC>();
					if (component != null && !list.Contains(component.gameObject))
					{
						list.Add(component.gameObject);
						BaseNPC.NPCControllerLink[] controllers = component.Controllers;
						foreach (BaseNPC.NPCControllerLink nPCControllerLink in controllers)
						{
							if (!list.Contains(nPCControllerLink.Controller.gameObject))
							{
								list.Add(nPCControllerLink.Controller.gameObject);
							}
						}
					}
				}
			}
			VehicleDriversWeight.VehicleDistribution[] vehicleDistributions = VehicleDriversWeight.VehicleDistributions;
			foreach (VehicleDriversWeight.VehicleDistribution vehicleDistribution in vehicleDistributions)
			{
				foreach (PrefabDistribution.Chance chance2 in vehicleDistribution.Distribution.Chances)
				{
					DummyDriver component2 = chance2.Prefab.GetComponent<DummyDriver>();
					if (!(component2 == null))
					{
						if (!list.Contains(component2.gameObject))
						{
							list.Add(component2.gameObject);
							list.Add(component2.DriverModel);
						}
						BaseNPC component3 = component2.DriverNPC.GetComponent<BaseNPC>();
						if (!list.Contains(component3.gameObject))
						{
							list.Add(component3.gameObject);
						}
					}
				}
			}
			foreach (DrivableVehicle item in VehiclesPrefab.Concat(CopVehiclesPrefab).Concat(TransformerVehiclesPrefab).Concat(OnlyPlayerVehicles))
			{
				if (!list.Contains(item.VehicleSpecificPrefab.gameObject))
				{
					list.Add(item.VehicleSpecificPrefab.gameObject);
				}
				if (!list.Contains(item.VehicleControllerPrefab))
				{
					list.Add(item.VehicleControllerPrefab);
				}
				GameObject destroyReplace = item.GetVehicleStatus().DestroyReplace;
				if (!list.Contains(destroyReplace))
				{
					list.Add(destroyReplace);
				}
			}
			foreach (Component item2 in first.Concat(VehiclesPrefab).Concat(CopVehiclesPrefab).Concat(TransformerVehiclesPrefab)
				.Concat(OnlyPlayerVehicles))
			{
				PoolManager.Instance.InitPoolingPrefab(item2, 5);
			}
			foreach (GameObject item3 in list)
			{
				PoolManager.Instance.InitPoolingPrefab(item3, 5);
			}
			GC.Collect();
		}
	}
}
