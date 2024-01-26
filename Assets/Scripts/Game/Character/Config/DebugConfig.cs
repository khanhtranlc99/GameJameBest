using System.Collections.Generic;

namespace Game.Character.Config
{
	public class DebugConfig : Config
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
			dictionary.Add("Orthographic", new BoolParam
			{
				value = false
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
			dictionary.Add("MoveSpeed", new RangeParam
			{
				value = 0.5f,
				min = 0f,
				max = 1f
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
