using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class Gradient_DirectConverter : fsDirectConverter<Gradient>
	{
		protected override FsResult DoSerialize(Gradient model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			success += SerializeMember(serialized, null, "alphaKeys", model.alphaKeys);
			return success + SerializeMember(serialized, null, "colorKeys", model.colorKeys);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref Gradient model)
		{
			FsResult success = FsResult.Success;
			GradientAlphaKey[] value = model.alphaKeys;
			success += DeserializeMember(data, null, "alphaKeys", out value);
			model.alphaKeys = value;
			GradientColorKey[] value2 = model.colorKeys;
			success += DeserializeMember(data, null, "colorKeys", out value2);
			model.colorKeys = value2;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return new Gradient();
		}
	}
}
