using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class SectorManager : MonoBehaviour
	{
		public delegate void SectorStatusChange(int[] sectors);

		private const float VelocityEpsilon = 0.1f;

		private const float ExtraSectorsAtVelocity = 15f;

		private static SectorManager instance;

		[Separator("Update Configuration")]
		public float SectorUpdateTime = 2f;

		[Header("0 - for speed independent SectorUpdateTime")]
		public float SectorUpdateTimeSpeedFactor = 0.2f;

		[Separator("Sector Configuration")]
		public float SectorSize = 100f;

		public int SectorLineCount = 10;

		[Separator("Debug")]
		public bool IsDebug;

		private SectorStatusChange activateSectors;

		private SectorStatusChange deactivateSectors;

		private SlowUpdateProc sectorUpdateProc;

		private Transform dynamicWorldCenter;

		private int viewerSector;

		private int[] activeSectors;

		private int[] debugActiveSectors;

		private int[] debugDeactiveSectors;

		private Vector3 dynamicCenterVelocity = Vector3.zero;

		private Vector3 dynamicWorldCenterOld = Vector3.zero;

		public static SectorManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("SectorManager is not initialized");
				}
				return instance;
			}
		}

		public Vector3 DynamicWorldCenter
		{
			get
			{
				if (dynamicWorldCenter == null)
				{
					dynamicWorldCenter = Camera.main.transform;
					dynamicWorldCenterOld = dynamicWorldCenter.position;
				}
				return dynamicWorldCenter.position;
			}
		}

		public float DynamicSectorSize => SectorSize * ((!IsExtraSectors()) ? 1f : 2f);

		public float SectorCount => SectorLineCount * SectorLineCount;

		public bool IsInActiveSector(Vector3 pos)
		{
			int sector = GetSector(pos);
			if (sector == viewerSector)
			{
				return true;
			}
			for (int i = 0; i < activeSectors.Length; i++)
			{
				if (sector == activeSectors[i])
				{
					return true;
				}
			}
			return false;
		}

		public int[] GetAllActiveSectors()
		{
			int[] array = new int[activeSectors.Length + 1];
			for (int i = 0; i < activeSectors.Length; i++)
			{
				array[i] = activeSectors[i];
			}
			array[activeSectors.Length] = viewerSector;
			return array;
		}

		public void GetAllActiveSectorsNonAlloc(List<int> listToFil)
		{
			for (int i = 0; i < activeSectors.Length; i++)
			{
				listToFil.Add(activeSectors[i]);
			}
			listToFil.Add(viewerSector);
		}

		public void AddOnActivateListener(SectorStatusChange onChange)
		{
			activateSectors = (SectorStatusChange)Delegate.Combine(activateSectors, onChange);
		}

		public void AddOnDeactivateListener(SectorStatusChange onChange)
		{
			deactivateSectors = (SectorStatusChange)Delegate.Combine(deactivateSectors, onChange);
		}

		public int GetSector(Vector3 pos)
		{
			Vector3 b = StartPoint();
			Vector3 vector = EndPoint();
			if (pos.x < b.x || pos.z < b.z || pos.x > vector.x || pos.z > vector.z)
			{
				UnityEngine.Debug.LogError("Out of sector net");
				return -1;
			}
			Vector3 vector2 = pos - b;
			int x = (int)(vector2.x / SectorSize);
			int z = (int)(vector2.z / SectorSize);
			return SectorByCoords(x, z);
		}

		public int[] GetAroundSectors(int centerSector)
		{
			if (centerSector < 0 || centerSector > SectorLineCount * SectorLineCount - 1)
			{
				return new int[0];
			}

			int x;
			int z;
			GetSectorCoords(centerSector, out x, out z);
			int num = 9;
			if (centerSector == 0 || centerSector == SectorLineCount - 1 || centerSector == SectorLineCount * (SectorLineCount - 1) || centerSector == SectorLineCount * SectorLineCount - 1)
			{
				num = 4;
			}
			else if (x == SectorLineCount - 1 || z == SectorLineCount - 1 || x == 0 || z == 0)
			{
				num = 6;
			}
			int[] array = new int[num];
			int num2 = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						int num3 = SectorByCoords(x + i, z + j);
						if (num3 != -1)
						{
							array[num2] = num3;
							num2++;
						}
					}
				}
			}
			array[array.Length - 1] = centerSector;
			return array;
		}

		public float DistanceInSectors(int sector1, int sector2)
		{
			int x;
			int z;
			int x2;
			int z2;
			GetSectorCoords(sector1, out  x, out  z);
			GetSectorCoords(sector2, out  x2, out  z2);
			return Mathf.Sqrt(Mathf.Pow(x2 - x, 2f) + Mathf.Pow(z2 - z, 2f));
		}

		private void SectorStatusUpdate()
		{
			dynamicCenterVelocity = (dynamicWorldCenterOld - DynamicWorldCenter) / sectorUpdateProc.DeltaTime;
			dynamicCenterVelocity.y = 0f;
			if (dynamicCenterVelocity.magnitude < 0.1f)
			{
				dynamicCenterVelocity = Vector3.zero;
			}
			float updateTime = SectorUpdateTime / (1f + dynamicCenterVelocity.magnitude * SectorUpdateTimeSpeedFactor);
			sectorUpdateProc.UpdateTime = updateTime;
			int sector = GetSector(DynamicWorldCenter);
			if (sector != viewerSector)
			{
				int[] collection = GetAroundSectors(sector);
				HashSet<int> hashSet = new HashSet<int>(activeSectors);
				HashSet<int> hashSet2 = new HashSet<int>(collection);
				if (IsExtraSectors())
				{
					int x;
					int z;
					Vector3 normalized = dynamicCenterVelocity.normalized;
					GetSectorCoords(sector, out  x, out z);
					if (Mathf.Abs(normalized.x) > 0.3f)
					{
						x -= (int)Mathf.Sign(normalized.x);
					}
					if (Mathf.Abs(normalized.z) > 0.3f)
					{
						z -= (int)Mathf.Sign(normalized.z);
					}
					int centerSector = SectorByCoords(x, z);
					hashSet2.UnionWith(GetAroundSectors(centerSector));
					collection = hashSet2.ToArray();
				}
				hashSet.ExceptWith(hashSet2);
				hashSet2.ExceptWith(activeSectors);
				int[] array = new int[hashSet2.Count];
				hashSet2.CopyTo(array);
				int[] array2 = new int[hashSet.Count];
				hashSet.CopyTo(array2);
				if (activateSectors != null)
				{
					activateSectors(array);
				}
				if (deactivateSectors != null)
				{
					deactivateSectors(array2);
				}
				activeSectors = collection;
				viewerSector = sector;
			}
			dynamicWorldCenterOld = DynamicWorldCenter;
		}

		private bool IsExtraSectors()
		{
			return dynamicCenterVelocity.magnitude > 15f;
		}

		private void GetSectorCoords(int sector, out int x, out int z)
		{
			x = sector % SectorLineCount;
			z = sector / SectorLineCount;
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				sectorUpdateProc = new SlowUpdateProc(SectorStatusUpdate, 1f);
				viewerSector = GetSector(DynamicWorldCenter);
				activeSectors = GetAroundSectors(viewerSector);
				if (IsDebug)
				{
					activateSectors = (SectorStatusChange)Delegate.Combine(activateSectors, (SectorStatusChange)delegate(int[] sectors)
					{
						debugActiveSectors = sectors;
					});
					deactivateSectors = delegate(int[] sectors)
					{
						debugDeactiveSectors = sectors;
					};
				}
			}
		}

		private void FixedUpdate()
		{
			sectorUpdateProc.ProceedOnFixedUpdate();
		}

		private void OnDrawGizmos()
		{
			if (!IsDebug)
			{
				return;
			}
			Gizmos.color = new Color(0f, 1f, 0f, 1f);
			Vector3 a = StartPoint();
			for (int i = 0; i < SectorLineCount + 1; i++)
			{
				float d = SectorSize * (float)i;
				Gizmos.DrawLine(a + Vector3.right * d, a + Vector3.right * d + (float)SectorLineCount * SectorSize * Vector3.forward);
				Gizmos.DrawLine(a + Vector3.forward * d, a + Vector3.forward * d + (float)SectorLineCount * SectorSize * Vector3.right);
			}
			Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
			if (debugActiveSectors != null)
			{
				int[] array = debugActiveSectors;
				foreach (int sector in array)
				{
					Gizmos.DrawCube(GetSectorCenter(sector), Vector3.one * SectorSize * 0.95f);
				}
			}
			Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
			if (debugDeactiveSectors != null)
			{
				int[] array2 = debugDeactiveSectors;
				foreach (int sector2 in array2)
				{
					Gizmos.DrawCube(GetSectorCenter(sector2), Vector3.one * SectorSize * 0.95f);
				}
			}
			Gizmos.color = new Color(1f, 0f, 1f, 0.4f);
			if (activeSectors != null)
			{
				int[] array3 = activeSectors;
				foreach (int sector3 in array3)
				{
					Gizmos.DrawCube(size: new Vector3(1f, 0.05f, 1f) * SectorSize * 0.95f, center: GetSectorCenter(sector3));
				}
			}
		}

		private Vector3 StartPoint()
		{
			return base.transform.position + (float)SectorLineCount * SectorSize * 0.5f * Vector3.back + (float)SectorLineCount * SectorSize * 0.5f * Vector3.left;
		}

		private Vector3 EndPoint()
		{
			return base.transform.position + (float)SectorLineCount * SectorSize * 0.5f * Vector3.forward + (float)SectorLineCount * SectorSize * 0.5f * Vector3.right;
		}

		private int SectorByCoords(int x, int z)
		{
			if (x >= SectorLineCount || x < 0 || z >= SectorLineCount || z < 0)
			{
				return -1;
			}
			return x + z * SectorLineCount;
		}

		public Vector3 GetSectorCenter(int sector)
		{
			int num = sector % SectorLineCount;
			int num2 = sector / SectorLineCount;
			Vector3 a = StartPoint();
			return a + Vector3.right * ((float)num + 0.5f) * SectorSize + Vector3.forward * ((float)num2 + 0.5f) * SectorSize;
		}
	}
}
