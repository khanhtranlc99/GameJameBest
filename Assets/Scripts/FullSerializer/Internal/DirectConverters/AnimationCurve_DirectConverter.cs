using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class AnimationCurve_DirectConverter : fsDirectConverter<AnimationCurve>
	{
		protected override FsResult DoSerialize(AnimationCurve model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			success += SerializeMember(serialized, null, "keys", model.keys);
			success += SerializeMember(serialized, null, "preWrapMode", model.preWrapMode);
			return success + SerializeMember(serialized, null, "postWrapMode", model.postWrapMode);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref AnimationCurve model)
		{
			FsResult success = FsResult.Success;
			Keyframe[] value = model.keys;
			success += DeserializeMember(data, null, "keys", out value);
			model.keys = value;
			WrapMode value2 = model.preWrapMode;
			success += DeserializeMember(data, null, "preWrapMode", out value2);
			model.preWrapMode = value2;
			WrapMode value3 = model.postWrapMode;
			success += DeserializeMember(data, null, "postWrapMode", out value3);
			model.postWrapMode = value3;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return new AnimationCurve();
		}
	}
}
