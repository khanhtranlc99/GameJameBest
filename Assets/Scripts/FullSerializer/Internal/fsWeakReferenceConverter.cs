using System;

namespace FullSerializer.Internal
{
	public class fsWeakReferenceConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			return type == typeof(WeakReference);
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
			WeakReference weakReference = (WeakReference)instance;
			FsResult fsResult = FsResult.Success;
			serialized = fsData.CreateDictionary();
			if (weakReference.IsAlive)
			{
				fsData data;
				FsResult fsResult2 = fsResult += Serializer.TrySerialize(weakReference.Target, out data);
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				serialized.AsDictionary["Target"] = data;
				serialized.AsDictionary["TrackResurrection"] = new fsData(weakReference.TrackResurrection);
			}
			return fsResult;
		}

		public override FsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			FsResult success = FsResult.Success;
			FsResult fsResult = success += CheckType(data, fsDataType.Object);
			if (fsResult.Failed)
			{
				return success;
			}
			if (data.AsDictionary.ContainsKey("Target"))
			{
				fsData data2 = data.AsDictionary["Target"];
				object result = null;
				FsResult fsResult2 = success += Serializer.TryDeserialize(data2, typeof(object), ref result);
				if (fsResult2.Failed)
				{
					return success;
				}
				bool trackResurrection = false;
				if (data.AsDictionary.ContainsKey("TrackResurrection") && data.AsDictionary["TrackResurrection"].IsBool)
				{
					trackResurrection = data.AsDictionary["TrackResurrection"].AsBool;
				}
				instance = new WeakReference(result, trackResurrection);
			}
			return success;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return new WeakReference(null);
		}
	}
}
