using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class RTSConfig : Config
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
			dictionary.Add("EnableRotation", new BoolParam
			{
				value = true
			});
			dictionary.Add("DefaultAngleX", new RangeParam
			{
				value = 45f,
				min = -180f,
				max = 180f
			});
			dictionary.Add("RotationSpeed", new RangeParam
			{
				value = 8f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("GroundOffset", new RangeParam
			{
				value = 0f,
				min = -100f,
				max = 100f
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
			dictionary.Add("FollowTargetY", new BoolParam
			{
				value = true
			});
			dictionary.Add("DraggingMove", new BoolParam
			{
				value = true
			});
			dictionary.Add("ScreenBorderMove", new BoolParam
			{
				value = true
			});
			dictionary.Add("ScreenBorderOffset", new RangeParam
			{
				value = 10f,
				min = 1f,
				max = 500f
			});
			dictionary.Add("ScreenBorderSpeed", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("KeyMove", new BoolParam
			{
				value = true
			});
			dictionary.Add("MoveSpeed", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("MapCenter", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("MapSize", new Vector2Param
			{
				value = new Vector2(100f, 100f)
			});
			dictionary.Add("SoftBorder", new RangeParam
			{
				value = 5f,
				min = 0f,
				max = 20f
			});
			dictionary.Add("DisableHorizontal", new BoolParam
			{
				value = false
			});
			dictionary.Add("DisableVertical", new BoolParam
			{
				value = false
			});
			dictionary.Add("DragMomentum", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 3f
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
