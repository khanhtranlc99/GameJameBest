using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer.Internal.DirectConverters
{
	public class GUIStyleState_DirectConverter : fsDirectConverter<GUIStyleState>
	{
		protected override FsResult DoSerialize(GUIStyleState model, Dictionary<string, fsData> serialized)
		{
			FsResult success = FsResult.Success;
			success += SerializeMember(serialized, null, "background", model.background);
			return success + SerializeMember(serialized, null, "textColor", model.textColor);
		}

		protected override FsResult DoDeserialize(Dictionary<string, fsData> data, ref GUIStyleState model)
		{
			FsResult success = FsResult.Success;
			Texture2D value = model.background;
			success += DeserializeMember(data, null, "background", out value);
			model.background = value;
			Color value2 = model.textColor;
			success += DeserializeMember(data, null, "textColor", out value2);
			model.textColor = value2;
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return new GUIStyleState();
		}
	}
}
