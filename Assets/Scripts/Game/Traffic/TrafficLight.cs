using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Traffic
{
	public class TrafficLight : MonoBehaviour
	{
		[Serializable]
		public class LightObjects
		{
			public TrafficLightColor TrafficLight;

			public float ActiveTime;

			public List<GameObject> Objects;
		}

		public enum TrafficLightColor
		{
			None = -1,
			Red,
			Yellow,
			Green
		}

		private const string TrafficObstaclesLayerName = "TrafficPseudoObstacles";

		public static bool IsDebug;

		public bool RedFirst;

		public LightObjects[] LightAndObjects;

		private float lastSignalChangeTime;

		private TrafficLightColor currentTrafficLight;

		private float currentSignalLength;

		private bool redPhase;

		private float cycleTime;

		private readonly IDictionary<TrafficLightColor, Color> colorsByType = new Dictionary<TrafficLightColor, Color>
		{
			{
				TrafficLightColor.Red,
				Color.red
			},
			{
				TrafficLightColor.Yellow,
				Color.yellow
			},
			{
				TrafficLightColor.Green,
				Color.green
			}
		};

		public void ActivateLight(TrafficLightColor trafficLightType)
		{
			if (currentTrafficLight != TrafficLightColor.None)
			{
				SetLightObjectsStatus(currentTrafficLight, activate: false);
			}
			SetLightObjectsStatus(trafficLightType, true, out currentSignalLength);
			currentTrafficLight = trafficLightType;
			lastSignalChangeTime = Time.time;
		}

		private void Awake()
		{
			LightAndObjects[0].ActiveTime /= 2f;
			LightAndObjects[LightAndObjects.Length - 1].ActiveTime /= 2f;
			LightObjects[] lightAndObjects = LightAndObjects;
			foreach (LightObjects lightObjects in lightAndObjects)
			{
				cycleTime += lightObjects.ActiveTime;
				foreach (GameObject @object in lightObjects.Objects)
				{
					@object.SetActive(value: false);
				}
			}
		}

		private void OnEnable()
		{
			int num = (int)Mathf.Floor(Time.time / cycleTime);
			float phaseTime = Time.time % cycleTime;
			float signalLength;
			TrafficLightColor trafficLightType = CalculateCurrentColor(phaseTime, num % 2 == 0, out signalLength, out redPhase);
			ActivateLight(trafficLightType);
			currentSignalLength = signalLength;
		}

		private void FixedUpdate()
		{
			if (Time.time >= lastSignalChangeTime + currentSignalLength)
			{
				NextSignal();
			}
		}

		private void NextSignal()
		{
			int num = CalculateNextLightIndex();
			if (num < 0 || num > LightAndObjects.Length - 1)
			{
				redPhase = !redPhase;
				lastSignalChangeTime = Time.time;
			}
			else
			{
				ActivateLight((TrafficLightColor)num);
			}
		}

		private int CalculateNextLightIndex()
		{
			int num = redPhase ? 1 : (-1);
			return (int)(currentTrafficLight + num);
		}

		private void SetLightObjectsStatus(TrafficLightColor trafficLightType, bool activate)
		{
			float _;
			SetLightObjectsStatus(trafficLightType, activate, out _);
		}

		private void SetLightObjectsStatus(TrafficLightColor trafficLightType, bool activate, out float signalLength)
		{
			List<GameObject> list = null;
			signalLength = 0f;
			LightObjects[] lightAndObjects = LightAndObjects;
			foreach (LightObjects lightObjects in lightAndObjects)
			{
				if (lightObjects.TrafficLight == trafficLightType)
				{
					list = lightObjects.Objects;
					signalLength = lightObjects.ActiveTime;
					break;
				}
			}
			if (list != null && list.Count > 0)
			{
				foreach (GameObject item in list)
				{
					item.SetActive(activate);
				}
			}
		}

		private TrafficLightColor CalculateCurrentColor(float phaseTime, bool evenCycle, out float signalLength, out bool localRedPhase)
		{
			localRedPhase = ((!RedFirst) ? (!evenCycle) : evenCycle);
			int num = localRedPhase ? 1 : (-1);
			int num2 = (!localRedPhase) ? (LightAndObjects.Length - 1) : 0;
			float num3 = 0f;
			while (true)
			{
				if (phaseTime < num3 + LightAndObjects[num2].ActiveTime)
				{
					int num4 = num2 + num;
					if (num4 < 0 || num4 > LightAndObjects.Length - 1 || phaseTime > num3 - LightAndObjects[num4].ActiveTime)
					{
						break;
					}
				}
				num3 += LightAndObjects[num2].ActiveTime;
				num2 += num;
			}
			TrafficLightColor trafficLight = LightAndObjects[num2].TrafficLight;
			signalLength = LightAndObjects[num2].ActiveTime - (phaseTime - num3);
			return trafficLight;
		}

		private void OnDrawGizmos()
		{
			if (!IsDebug)
			{
				return;
			}
			int num = LayerMask.NameToLayer("TrafficPseudoObstacles");
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.gameObject.layer == num)
				{
					Gizmos.matrix = transform.localToWorldMatrix;
					if (Application.isEditor)
					{
						Gizmos.color = ((!RedFirst) ? Color.green : Color.red);
					}
					if (Application.isPlaying)
					{
						Gizmos.color = colorsByType[currentTrafficLight];
					}
					BoxCollider component = transform.GetComponent<BoxCollider>();
					Gizmos.DrawCube(component.center, component.size);
				}
			}
		}
	}
}
