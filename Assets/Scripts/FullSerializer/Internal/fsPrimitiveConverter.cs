using System;

namespace FullSerializer.Internal
{
	public class fsPrimitiveConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsPrimitive || type == typeof(string) || type == typeof(decimal);
		}

		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		private static bool UseBool(Type type)
		{
			return type == typeof(bool);
		}

		private static bool UseInt64(Type type)
		{
			return type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong);
		}

		private static bool UseDouble(Type type)
		{
			return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
		}

		private static bool UseString(Type type)
		{
			return type == typeof(string) || type == typeof(char);
		}

		public override FsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Type type = instance.GetType();
			if (Serializer.Config.Serialize64BitIntegerAsString && (type == typeof(long) || type == typeof(ulong)))
			{
				serialized = new fsData((string)Convert.ChangeType(instance, typeof(string)));
				return FsResult.Success;
			}
			if (UseBool(type))
			{
				serialized = new fsData((bool)instance);
				return FsResult.Success;
			}
			if (UseInt64(type))
			{
				serialized = new fsData((long)Convert.ChangeType(instance, typeof(long)));
				return FsResult.Success;
			}
			if (UseDouble(type))
			{
				if (instance.GetType() == typeof(float) && (float)instance != float.MinValue && (float)instance != float.MaxValue && !float.IsInfinity((float)instance) && !float.IsNaN((float)instance))
				{
					serialized = new fsData((double)(decimal)(float)instance);
					return FsResult.Success;
				}
				serialized = new fsData((double)Convert.ChangeType(instance, typeof(double)));
				return FsResult.Success;
			}
			if (UseString(type))
			{
				serialized = new fsData((string)Convert.ChangeType(instance, typeof(string)));
				return FsResult.Success;
			}
			serialized = null;
			return FsResult.Fail("Unhandled primitive type " + instance.GetType());
		}

		public override FsResult TryDeserialize(fsData storage, ref object instance, Type storageType)
		{
			FsResult success = FsResult.Success;
			if (UseBool(storageType))
			{
				FsResult fsResult = success += CheckType(storage, fsDataType.Boolean);
				if (fsResult.Succeeded)
				{
					instance = storage.AsBool;
				}
				return success;
			}
			if (UseDouble(storageType) || UseInt64(storageType))
			{
				if (storage.IsDouble)
				{
					instance = Convert.ChangeType(storage.AsDouble, storageType);
				}
				else if (storage.IsInt64)
				{
					instance = Convert.ChangeType(storage.AsInt64, storageType);
				}
				else
				{
					if (!Serializer.Config.Serialize64BitIntegerAsString || !storage.IsString || (storageType != typeof(long) && storageType != typeof(ulong)))
					{
						return FsResult.Fail(GetType().Name + " expected number but got " + storage.Type + " in " + storage);
					}
					instance = Convert.ChangeType(storage.AsString, storageType);
				}
				return FsResult.Success;
			}
			if (UseString(storageType))
			{
				FsResult fsResult2 = success += CheckType(storage, fsDataType.String);
				if (fsResult2.Succeeded)
				{
					instance = storage.AsString;
				}
				return success;
			}
			return FsResult.Fail(GetType().Name + ": Bad data; expected bool, number, string, but got " + storage);
		}
	}
}
