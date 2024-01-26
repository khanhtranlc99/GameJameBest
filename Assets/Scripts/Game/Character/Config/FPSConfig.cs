using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class FPSConfig : Config
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
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			dictionary.Add("HideTarget", new BoolParam
			{
				value = true
			});
			Dictionary<string, Param> value = dictionary;
			dictionary = new Dictionary<string, Param>();
			dictionary.Add("FOV", new RangeParam
			{
				value = 60f,
				min = 20f,
				max = 100f
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
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
			});
			dictionary.Add("HideTarget", new BoolParam
			{
				value = true
			});
			Dictionary<string, Param> value2 = dictionary;
			Params = new Dictionary<string, Dictionary<string, Param>>
			{
				{
					"Default",
					value
				},
				{
					"Crouch",
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
