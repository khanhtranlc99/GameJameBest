using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Config
{
	public class DeadConfig : Config
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
			dictionary.Add("RotationSpeed", new RangeParam
			{
				value = 0.5f,
				min = -10f,
				max = 10f
			});
			dictionary.Add("Angle", new RangeParam
			{
				value = 50f,
				min = 0f,
				max = 80f
			});
			dictionary.Add("TargetOffset", new Vector3Param
			{
				value = Vector3.zero
			});
			dictionary.Add("Collision", new BoolParam
			{
				value = true
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
