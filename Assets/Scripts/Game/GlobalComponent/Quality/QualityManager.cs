using Game.Character;
using Game.Traffic;
using System;
using RopeHeroViceCity.UI_Features.UI_PleaseWaitPanel.Scripts;
using UnityEngine;

namespace Game.GlobalComponent.Quality
{
	public class QualityManager : MonoBehaviour
	{
		public delegate void UpdateQuality();

		public const int LowQualityLevelIndex = 6;

		public const int MidQualityLevelIndex = 7;

		public const int HighQualityLevelIndex = 8;

		public const int UltraQualityLevelIndex = 9;

		public const int LowQualityClipPlane = 50;

		public const int MidQualityClipPlane = 250;

		public const int HighQualityClipPlane = 450;

		public const int UltraQualityClipPlane = 650;

		public const int MinFarClipPlane = 100;

		public const int MaxFarClipPlane = 650;

		public const int MinCountVehicles = 3;

		public const int MaxCountVehicles = 10;

		public const int MinCountPedestrians = 3;

		public const int MaxCountPedestrians = 10;

		private static QualityManager instance;

		public static UpdateQuality updateQuality;

		public static int CountVehicles
		{
			get
			{
				return BaseProfile.ResolveValue("MaxCountVehicles", 5);
			}
			set
			{
				BaseProfile.StoreValue(Mathf.Clamp(value, 3, 10), "MaxCountVehicles");
			}
		}

		public static int CountPedestrians
		{
			get
			{
				return BaseProfile.ResolveValue("MaxCountPedestrians", 5);
			}
			set
			{
				BaseProfile.StoreValue(Mathf.Clamp(value, 3, 10), "MaxCountPedestrians");
			}
		}

		public static QualityManager Instance
		{
			get
			{
				if (instance == null)
				{
					throw new Exception("QualityManager is not initialized");
				}
				return instance;
			}
		}

		public static int FarClipPlane
		{
			get
			{
				return BaseProfile.ResolveValue("FarClipPlane", 50);
			}
			set
			{
				BaseProfile.StoreValue(value, "FarClipPlane");
			}
		}

		private static int QualityLevel
		{
			get
			{
				return BaseProfile.ResolveValue("QualityLevel", 6);
			}
			set
			{
				BaseProfile.StoreValue(value, "QualityLevel");
			}
		}

		public static QualityLvls QualityLvl
		{
			get
			{
				switch (QualityLevel)
				{
				case 6:
					return QualityLvls.Low;
				case 7:
					return QualityLvls.Mid;
				case 8:
					return QualityLvls.High;
				case 9:
					return QualityLvls.Ultra;
				default:
					return QualityLvls.Low;
				}
			}
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			SetQuality(QualityLevel, FarClipPlane, change: true);
		}

		public static void ChangeFog(bool disable = false)
		{
			if (QualityLevel == 6 || disable)
			{
				RenderSettings.fog = false;
				return;
			}
			RenderSettings.fog = true;
			RenderSettings.fogMode = FogMode.Linear;
			RenderSettings.fogEndDistance = FarClipPlane;
			RenderSettings.fogStartDistance = FarClipPlane / 4;
		}

		public static void SetCountPedestrians(int value)
		{
			CountPedestrians = value;
			TrafficSlider.UpdteValue();
			try
			{
				if ((bool)TrafficManager.Instance)
				{
					TrafficManager.Instance.MaxCountPedestrians = value;
				}
			}
			catch (Exception)
			{
			}
		}

		public static void SetCountVehicles(int value)
		{
			CountVehicles = value;
			TrafficSlider.UpdteValue();
			try
			{
				if ((bool)TrafficManager.Instance)
				{
					TrafficManager.Instance.MaxCountVehicles = value;
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		public static void ChangeQuality(QualityLvls lvl, bool setNow = false)
		{
			switch (lvl)
			{
			case QualityLvls.Low:
				ChangeQuality(6, setNow);
				break;
			case QualityLvls.Mid:
				ChangeQuality(7, setNow);
				break;
			case QualityLvls.High:
				ChangeQuality(8, setNow);
				break;
			case QualityLvls.Ultra:
				ChangeQuality(9, setNow);
				break;
			}
		}

		public static void ChangeQuality(int level, bool setNow = false)
		{
			int level2;
			int farClip;
			switch (level)
			{
			case 7:
				level2 = 7;
				farClip = 250;
				break;
			case 8:
				level2 = 8;
				farClip = 450;
				break;
			case 9:
				level2 = 9;
				farClip = 650;
				break;
			default:
				level2 = 6;
				farClip = 50;
				break;
			}
			SetQuality(level2, farClip, setNow);
		}

		public void ChangeCameraCliping(float farClip)
		{
			CameraManager.Instance.UnityCamera.farClipPlane = (int)farClip;
			FarClipPlane = (int)farClip;
			ChangeFog();
		}

		private static void ChangeTrafficDensity(int qualityLevel)
		{
			SetCountPedestrians((qualityLevel != 6) ? CountPedestrians : 3);
			SetCountVehicles((qualityLevel != 6) ? CountVehicles : 3);
		}

		private static void SetQuality(int level, int farClip, bool change)
		{
			if (change)
			{
				UI_PleaseWait.OpenWaiting(delegate
				{
					QualitySettings.SetQualityLevel(level);
					ChangeFog();
					CameraManager.Instance.UnityCamera.farClipPlane = farClip;
				});
			}
			ChangeTrafficDensity(level);
			QualityLevel = level;
			FarClipPlane = farClip;
			if (updateQuality != null)
			{
				updateQuality();
			}
		}
	}
}
