using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class FlyConfig : Config
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
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("WhenRotateSpeed", new RangeParam
			{
				value = 20f,
				min = 0f,
				max = 300f
			});
			dictionary.Add("SpeedRollback", new RangeParam
			{
				value = 0.03f,
				min = 0.001f,
				max = 1f
			});
			dictionary.Add("AutoResetTimeout", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 360f
			});
			dictionary.Add("AutoTurnMinSpeed", new RangeParam
			{
				value = 5f,
				min = 0.1f,
				max = 720f
			});
			dictionary.Add("AutoTurnMaxSpeed", new RangeParam
			{
				value = 180f,
				min = 1f,
				max = 720f
			});
			dictionary.Add("AutoTurnAcceleration", new RangeParam
			{
				value = 1.1f,
				min = 1f,
				max = 10f
			});
			dictionary.Add("AutoReset", new BoolParam
			{
				value = true
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
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
			dictionary.Add("RotationYMax", new RangeParam
			{
				value = 80f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("RotationYMin", new RangeParam
			{
				value = -80f,
				min = -80f,
				max = 0f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("FollowCoef", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 10f
			});
			dictionary.Add("Spring", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("AutoYRotation", new RangeParam
			{
				value = 0f,
				min = 0f,
				max = 1f
			});
			dictionary.Add("DeadZone", new Vector2Param
			{
				value = Vector2.zero
			});
			dictionary.Add("WhenRotateSpeed", new RangeParam
			{
				value = 20f,
				min = 0f,
				max = 300f
			});
			dictionary.Add("SpeedRollback", new RangeParam
			{
				value = 0.03f,
				min = 0.001f,
				max = 1f
			});
			dictionary.Add("AutoResetTimeout", new RangeParam
			{
				value = 1f,
				min = 0f,
				max = 360f
			});
			dictionary.Add("AutoTurnMinSpeed", new RangeParam
			{
				value = 5f,
				min = 0.1f,
				max = 720f
			});
			dictionary.Add("AutoTurnMaxSpeed", new RangeParam
			{
				value = 180f,
				min = 1f,
				max = 720f
			});
			dictionary.Add("AutoTurnAcceleration", new RangeParam
			{
				value = 1.1f,
				min = 1f,
				max = 10f
			});
			dictionary.Add("AutoReset", new BoolParam
			{
				value = true
			});
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			Dictionary<string, Param> value2 = dictionary;
			Params = new Dictionary<string, Dictionary<string, Param>>
			{
				{
					"Default",
					value
				},
				{
					"Glide",
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
