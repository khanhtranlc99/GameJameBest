using System;
using System.Collections;

namespace FullSerializer.Internal
{
	public class fsReflectedConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			if (type.Resolve().IsArray || typeof(ICollection).IsAssignableFrom(type))
			{
				return false;
			}
			return true;
		}

		public override FsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = fsData.CreateDictionary();
			FsResult success = FsResult.Success;
			fsMetaType fsMetaType = fsMetaType.Get(Serializer.Config, instance.GetType());
			fsMetaType.EmitAotData();
			for (int i = 0; i < fsMetaType.Properties.Length; i++)
			{
				fsMetaProperty fsMetaProperty = fsMetaType.Properties[i];
				if (fsMetaProperty.CanRead)
				{
					fsData data;
					FsResult result = Serializer.TrySerialize(fsMetaProperty.StorageType, fsMetaProperty.OverrideConverterType, fsMetaProperty.Read(instance), out data);
					success.AddMessages(result);
					if (!result.Failed)
					{
						serialized.AsDictionary[fsMetaProperty.JsonName] = data;
					}
				}
			}
			return success;
		}

		public override FsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			FsResult success = FsResult.Success;
			FsResult m_fsResult = success += CheckType(data, fsDataType.Object);
			if (m_fsResult.Failed)
			{
				return success;
			}
			fsMetaType fsMetaType = fsMetaType.Get(Serializer.Config, storageType);
			fsMetaType.EmitAotData();
			for (int i = 0; i < fsMetaType.Properties.Length; i++)
			{
				fsMetaProperty fsMetaProperty = fsMetaType.Properties[i];
				fsData value;
				if (fsMetaProperty.CanWrite && data.AsDictionary.TryGetValue(fsMetaProperty.JsonName, out value))
				{
					object result = null;
					if (fsMetaProperty.CanRead)
					{
						result = fsMetaProperty.Read(instance);
					}
					FsResult result2 = Serializer.TryDeserialize(value, fsMetaProperty.StorageType, fsMetaProperty.OverrideConverterType, ref result);
					success.AddMessages(result2);
					if (!result2.Failed)
					{
						fsMetaProperty.Write(instance, result);
					}
				}
			}
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			fsMetaType fsMetaType = fsMetaType.Get(Serializer.Config, storageType);
			return fsMetaType.CreateInstance();
		}
	}
}
