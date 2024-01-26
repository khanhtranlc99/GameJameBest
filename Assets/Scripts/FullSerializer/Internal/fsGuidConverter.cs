using System;

namespace FullSerializer.Internal
{
	public class fsGuidConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			return type == typeof(Guid);
		}

		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		public override FsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = new fsData(((Guid)instance).ToString());
			return FsResult.Success;
		}

		public override FsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (data.IsString)
			{
				instance = new Guid(data.AsString);
				return FsResult.Success;
			}
			return FsResult.Fail("fsGuidConverter encountered an unknown JSON data type");
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Guid);
		}
	}
}
