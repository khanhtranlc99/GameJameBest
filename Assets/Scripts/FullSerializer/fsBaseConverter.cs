using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSerializer
{
	public abstract class fsBaseConverter
	{
		public fsSerializer Serializer;

		public virtual object CreateInstance(fsData data, Type storageType)
		{
			if (RequestCycleSupport(storageType))
			{
				throw new InvalidOperationException("Please override CreateInstance for " + GetType().FullName + "; the object graph for " + storageType + " can contain potentially contain cycles, so separated instance creation is needed");
			}
			return storageType;
		}

		public virtual bool RequestCycleSupport(Type storageType)
		{
			if (storageType == typeof(string))
			{
				return false;
			}
			return storageType.Resolve().IsClass || storageType.Resolve().IsInterface;
		}

		public virtual bool RequestInheritanceSupport(Type storageType)
		{
			return !storageType.Resolve().IsSealed;
		}

		public abstract FsResult TrySerialize(object instance, out fsData serialized, Type storageType);

		public abstract FsResult TryDeserialize(fsData data, ref object instance, Type storageType);

		protected FsResult FailExpectedType(fsData data, params fsDataType[] types)
		{
			return FsResult.Fail(GetType().Name + " expected one of " + string.Join(", ", (from t in types
				select t.ToString()).ToArray()) + " but got " + data.Type + " in " + data);
		}

		protected FsResult CheckType(fsData data, fsDataType type)
		{
			if (data.Type != type)
			{
				return FsResult.Fail(GetType().Name + " expected " + type + " but got " + data.Type + " in " + data);
			}
			return FsResult.Success;
		}

		protected FsResult CheckKey(fsData data, string key, out fsData subitem)
		{
			return CheckKey(data.AsDictionary, key, out subitem);
		}

		protected FsResult CheckKey(Dictionary<string, fsData> data, string key, out fsData subitem)
		{
			if (!data.TryGetValue(key, out subitem))
			{
				return FsResult.Fail(GetType().Name + " requires a <" + key + "> key in the data " + data);
			}
			return FsResult.Success;
		}

		protected FsResult SerializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, T value)
		{
			fsData data2;
			FsResult result = Serializer.TrySerialize(typeof(T), overrideConverterType, value, out data2);
			if (result.Succeeded)
			{
				data[name] = data2;
			}
			return result;
		}

		protected FsResult DeserializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, out T value)
		{
			fsData value2;
			if (!data.TryGetValue(name, out value2))
			{
				value = default(T);
				return FsResult.Fail("Unable to find member \"" + name + "\"");
			}
			object result = null;
			FsResult result2 = Serializer.TryDeserialize(value2, typeof(T), overrideConverterType, ref result);
			value = (T)result;
			return result2;
		}
	}
}
