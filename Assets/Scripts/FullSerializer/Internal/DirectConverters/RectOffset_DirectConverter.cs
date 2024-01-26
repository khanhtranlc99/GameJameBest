using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class RectOffset_DirectConverter : fsDirectConverter<RectOffset>
	{
		protected override FsResult DoSerialize(RectOffset model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			success += SerializeMember(serialized, null, "bottom", model.bottom);
			success += SerializeMember(serialized, null, "left", model.left);
			success += SerializeMember(serialized, null, "right", model.right);
			return success + SerializeMember(serialized, null, "top", model.top);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref RectOffset model)
		{
			FsResult success = FsResult.Success;
			int value = model.bottom;
			success += DeserializeMember(data, null, "bottom", out value);
			model.bottom = value;
			int value2 = model.left;
			success += DeserializeMember(data, null, "left", out value2);
			model.left = value2;
			int value3 = model.right;
			success += DeserializeMember(data, null, "right", out value3);
			model.right = value3;
			int value4 = model.top;
			success += DeserializeMember(data, null, "top", out value4);
			model.top = value4;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return new RectOffset();
		}
	}
}
