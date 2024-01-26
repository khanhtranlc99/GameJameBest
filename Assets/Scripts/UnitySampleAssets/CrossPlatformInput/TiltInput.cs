using System;
using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput
{
	public class TiltInput : MonoBehaviour
	{
		public enum AxisOptions
		{
			ForwardAxis,
			SidewaysAxis
		}

		[Serializable]
		public class AxisMapping
		{
			public enum MappingType
			{
				NamedAxis,
				MousePositionX,
				MousePositionY,
				MousePositionZ
			}

			public MappingType type;

			public string axisName;
		}

		public AxisMapping mapping;

		public AxisOptions tiltAroundAxis;

		public float fullTiltAngle = 25f;

		public float centreAngleOffset;

		private CrossPlatformInputManager.VirtualAxis steerAxis;

		private void OnEnable()
		{
			if (mapping.type == AxisMapping.MappingType.NamedAxis)
			{
				steerAxis = new CrossPlatformInputManager.VirtualAxis(mapping.axisName);
			}
		}

		private void Update()
		{
			float value = 0f;
			if (Input.acceleration != Vector3.zero)
			{
				switch (tiltAroundAxis)
				{
				case AxisOptions.ForwardAxis:
				{
					Vector3 acceleration3 = Input.acceleration;
					float x = acceleration3.x;
					Vector3 acceleration4 = Input.acceleration;
					value = Mathf.Atan2(x, 0f - acceleration4.y) * 57.29578f + centreAngleOffset;
					break;
				}
				case AxisOptions.SidewaysAxis:
				{
					Vector3 acceleration = Input.acceleration;
					float z = acceleration.z;
					Vector3 acceleration2 = Input.acceleration;
					value = Mathf.Atan2(z, 0f - acceleration2.y) * 57.29578f + centreAngleOffset;
					break;
				}
				}
			}
			float num = Mathf.InverseLerp(0f - fullTiltAngle, fullTiltAngle, value) * 2f - 1f;
			switch (mapping.type)
			{
			case AxisMapping.MappingType.NamedAxis:
				steerAxis.Update(num);
				break;
			case AxisMapping.MappingType.MousePositionX:
				CrossPlatformInputManager.SetVirtualMousePositionX(num * (float)Screen.width);
				break;
			case AxisMapping.MappingType.MousePositionY:
				CrossPlatformInputManager.SetVirtualMousePositionY(num * (float)Screen.width);
				break;
			case AxisMapping.MappingType.MousePositionZ:
				CrossPlatformInputManager.SetVirtualMousePositionZ(num * (float)Screen.width);
				break;
			}
		}

		private void OnDisable()
		{
			steerAxis.Remove();
		}
	}
}
