using System;

namespace FullSerializer.Internal
{
	public class fsNullableConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public override FsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			return Serializer.TrySerialize(Nullable.GetUnderlyingType(storageType), instance, out serialized);
		}

		public override FsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			return Serializer.TryDeserialize(data, Nullable.GetUnderlyingType(storageType), ref instance);
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return storageType;
		}
	}
}
