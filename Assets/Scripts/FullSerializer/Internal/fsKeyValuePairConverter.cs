using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	public class fsKeyValuePairConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<, >);
		}

		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		public override FsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			FsResult success = FsResult.Success;
			fsData subitem;
			FsResult m_fsResult = success += CheckKey(data, "Key", out subitem);
			if (m_fsResult.Failed)
			{
				return success;
			}
			fsData subitem2;
			FsResult fsResult2 = success += CheckKey(data, "Value", out subitem2);
			if (fsResult2.Failed)
			{
				return success;
			}
			Type[] genericArguments = storageType.GetGenericArguments();
			Type storageType2 = genericArguments[0];
			Type storageType3 = genericArguments[1];
			object result = null;
			object result2 = null;
			success.AddMessages(Serializer.TryDeserialize(subitem, storageType2, ref result));
			success.AddMessages(Serializer.TryDeserialize(subitem2, storageType3, ref result2));
			instance = Activator.CreateInstance(storageType, result, result2);
			return success;
		}

		public override FsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			PropertyInfo declaredProperty = storageType.GetDeclaredProperty("Key");
			PropertyInfo declaredProperty2 = storageType.GetDeclaredProperty("Value");
			object value = declaredProperty.GetValue(instance, null);
			object value2 = declaredProperty2.GetValue(instance, null);
			Type[] genericArguments = storageType.GetGenericArguments();
			Type storageType2 = genericArguments[0];
			Type storageType3 = genericArguments[1];
			FsResult success = FsResult.Success;
			fsData data;
			success.AddMessages(Serializer.TrySerialize(storageType2, value, out data));
			fsData data2;
			success.AddMessages(Serializer.TrySerialize(storageType3, value2, out data2));
			serialized = fsData.CreateDictionary();
			if (data != null)
			{
				serialized.AsDictionary["Key"] = data;
			}
			if (data2 != null)
			{
				serialized.AsDictionary["Value"] = data2;
			}
			return success;
		}
	}
}
