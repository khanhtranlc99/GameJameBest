using System;

namespace FullSerializer.Internal
{
	public class fsForwardConverter : fsConverter
	{
		private string _memberName;

		public fsForwardConverter(fsForwardAttribute attribute)
		{
			_memberName = attribute.MemberName;
		}

		public override bool CanProcess(Type type)
		{
			throw new NotSupportedException("Please use the [fsForward(...)] attribute.");
		}

		private FsResult GetProperty(object instance, out fsMetaProperty property)
		{
			fsMetaProperty[] properties = fsMetaType.Get(Serializer.Config, instance.GetType()).Properties;
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].MemberName == _memberName)
				{
					property = properties[i];
					return FsResult.Success;
				}
			}
			property = null;
			return FsResult.Fail("No property named \"" + _memberName + "\" on " + instance.GetType().CSharpName());
		}

		public override FsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = fsData.Null;
			FsResult success = FsResult.Success;
			fsMetaProperty property;
			FsResult fsResult = success += GetProperty(instance, out property);
			if (fsResult.Failed)
			{
				return success;
			}
			object instance2 = property.Read(instance);
			return Serializer.TrySerialize(property.StorageType, instance2, out serialized);
		}

		public override FsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			FsResult success = FsResult.Success;
			fsMetaProperty property;
			FsResult fsResult = success += GetProperty(instance, out property);
			if (fsResult.Failed)
			{
				return success;
			}
			object result = null;
			FsResult fsResult2 = success += Serializer.TryDeserialize(data, property.StorageType, ref result);
			if (fsResult2.Failed)
			{
				return success;
			}
			property.Write(instance, result);
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
		}
	}
}
