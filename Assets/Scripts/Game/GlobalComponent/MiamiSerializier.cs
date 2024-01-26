using FullSerializer;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class MiamiSerializier
	{
		public static string JSONSerialize(object obj)
		{
			fsSerializer fsSerializer = new fsSerializer();
			fsData data;
			fsSerializer.TrySerialize(obj, out data);
			return data.ToString();
		}

		public static object JSONDeserialize(string str)
		{
			return JSONDeserialize<object>(str);
		}

		public static T JSONDeserialize<T>(string str)
		{
			fsData data = fsJsonParser.Parse(str);
			fsSerializer fsSerializer = new fsSerializer();
			T instance = default(T);
			fsSerializer.TryDeserialize(data, ref instance);
			return instance;
		}
	}
}
