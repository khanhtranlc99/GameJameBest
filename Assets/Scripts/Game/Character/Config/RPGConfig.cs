using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class RPGConfig : Config
	{
		public override void LoadDefault()
		{
			Dictionary<string, Param> dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 10f,
				min = 0f,
				max = 100f
			});
			dictionary.Add("DistanceMin", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 100f
			});
			dictionary.Add("DistanceMax", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 100f
			});
			dictionary.Add("ZoomSpeed", new RangeParam
			{
				value = 0.5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("EnableZoom", new BoolParam
			{
				value = true
			});
			dictionary.Add("DefaultAngleX", new RangeParam
			{
				value = 45f,
				min = -180f,
				max = 180f
			});
			dictionary.Add("EnableRotation", new BoolParam
			{
				value = true
			});
			dictionary.Add("RotationSpeed", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("AngleY", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 85f
			});
			dictionary.Add("AngleZoomMin", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 85f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			dictionary.Add("OrthoMin", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 100f
			});
			dictionary.Add("OrthoMax", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 100f
			});
			Dictionary<string, Param> value = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
			});
			dictionary.Add("Distance", new RangeParam
			{
				value = 10f,
				min = 0f,
				max = 100f
			});
			dictionary.Add("DistanceMin", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 100f
			});
			dictionary.Add("DistanceMax", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 100f
			});
			dictionary.Add("ZoomSpeed", new RangeParam
			{
				value = 0.5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("EnableZoom", new BoolParam
			{
				value = true
			});
			dictionary.Add("DefaultAngleX", new RangeParam
			{
				value = 45f,
				min = -180f,
				max = 180f
			});
			dictionary.Add("EnableRotation", new BoolParam
			{
				value = true
			});
			dictionary.Add("RotationSpeed", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("AngleY", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 85f
			});
			dictionary.Add("AngleZoomMin", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 85f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			dictionary.Add("OrthoMin", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 100f
			});
			dictionary.Add("OrthoMax", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 100f
			});
			Dictionary<string, Param> value2 = dictionary;
			Params = new Dictionary<string, Dictionary<string, Param>>
			{
				{
					"Default",
					value
				},
				{
					"Interior",
					value2
				}
			};
			Transitions = new Dictionary<string, float>();
			foreach (KeyValuePair<string, Dictionary<string, Param>> param in Params)
			{
				Transitions.Add(param.Key, 0.25f);
			}
			Deserialize(base.DefaultConfigPath);
			base.LoadDefault();
		}

		public override void Awake()
		{
			base.Awake();
			LoadDefault();
		}
	}
}
