using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class Keyframe_DirectConverter : fsDirectConverter<Keyframe>
	{
		protected override FsResult DoSerialize(Keyframe model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			success += SerializeMember(serialized, null, "time", model.time);
			success += SerializeMember(serialized, null, "value", model.value);
			//success += SerializeMember(serialized, null, "tangentMode", model.tangentMode);
			success += SerializeMember(serialized, null, "inTangent", model.inTangent);
			return success + SerializeMember(serialized, null, "outTangent", model.outTangent);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref Keyframe model)
		{
			FsResult success = FsResult.Success;
			float value = model.time;
			success += DeserializeMember(data, null, "time", out value);
			model.time = value;
			float value2 = model.value;
			success += DeserializeMember(data, null, "value", out value2);
			model.value = value2;
			//int value3 = model.tangentMode;
			//success += DeserializeMember(data, null, "tangentMode", out value3);
			//model.tangentMode = value3;
			float value4 = model.inTangent;
			success += DeserializeMember(data, null, "inTangent", out value4);
			model.inTangent = value4;
			float value5 = model.outTangent;
			success += DeserializeMember(data, null, "outTangent", out value5);
			model.outTangent = value5;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Keyframe);
		}
	}
}
