using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class OrbitConfig : Config
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
			dictionary.Add("ZoomSpeed", new RangeParam
			{
				value = 2f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedX", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationSpeedY", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("PanSpeed", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 90f,
				min = 0f,
				max = 90f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -90f,
				min = -90f,
				max = 0f
			});
			dictionary.Add("DragLimits", new BoolParam
			{
				value = false
			});
			dictionary.Add("DragLimitX", new Vector2Param
			{
				value = new Vector2(-10f, 10f)
			});
			dictionary.Add("DragLimitY", new Vector2Param
			{
				value = new Vector2(-10f, 10f)
			});
			dictionary.Add("DragLimitZ", new Vector2Param
			{
				value = new Vector2(-10f, 10f)
			});
			dictionary.Add("DisablePan", new BoolParam
			{
				value = false
			});
			dictionary.Add("DisableZoom", new BoolParam
			{
				value = false
			});
			dictionary.Add("DisableRotation", new BoolParam
			{
				value = false
			});
			dictionary.Add("TargetInterpolation", new RangeParam
			{
				value = 0.5f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value = dictionary;
			Params = new Dictionary<string, Dictionary<string, Param>>
			{
				{
					"Default",
					value
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
