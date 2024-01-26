using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class Rect_DirectConverter : fsDirectConverter<Rect>
	{
		protected override FsResult DoSerialize(Rect model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			success += SerializeMember(serialized, null, "xMin", model.xMin);
			success += SerializeMember(serialized, null, "yMin", model.yMin);
			success += SerializeMember(serialized, null, "xMax", model.xMax);
			return success + SerializeMember(serialized, null, "yMax", model.yMax);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref Rect model)
		{
			FsResult success = FsResult.Success;
			float value = model.xMin;
			success += DeserializeMember(data, null, "xMin", out value);
			model.xMin = value;
			float value2 = model.yMin;
			success += DeserializeMember(data, null, "yMin", out value2);
			model.yMin = value2;
			float value3 = model.xMax;
			success += DeserializeMember(data, null, "xMax", out value3);
			model.xMax = value3;
			float value4 = model.yMax;
			success += DeserializeMember(data, null, "yMax", out value4);
			model.yMax = value4;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Rect);
		}
	}
}
