using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class Bounds_DirectConverter : fsDirectConverter<Bounds>
	{
		protected override FsResult DoSerialize(Bounds model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			success += SerializeMember(serialized, null, "center", model.center);
			return success + SerializeMember(serialized, null, "size", model.size);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref Bounds model)
		{
			FsResult success = FsResult.Success;
			Vector3 value = model.center;
			success += DeserializeMember(data, null, "center", out value);
			model.center = value;
			Vector3 value2 = model.size;
			success += DeserializeMember(data, null, "size", out value2);
			model.size = value2;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Bounds);
		}
	}
}
