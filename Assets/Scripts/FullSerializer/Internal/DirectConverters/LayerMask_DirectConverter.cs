using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class LayerMask_DirectConverter : fsDirectConverter<LayerMask>
	{
		protected override FsResult DoSerialize(LayerMask model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			return success + SerializeMember(serialized, null, "value", model.value);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref LayerMask model)
		{
			FsResult success = FsResult.Success;
			int value = model.value;
			success += DeserializeMember(data, null, "value", out value);
			model.value = value;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(LayerMask);
		}
	}
}
