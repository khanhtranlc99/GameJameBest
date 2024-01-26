using System.Collections.Generic;

namespace Game.Character.Config
{
	public class LookAtConfig : Config
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
			dictionary.Add("InterpolateTarget", new BoolParam
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
