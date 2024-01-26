using System;
using System.Collections.Generic;

namespace FullSerializer
{
	public abstract class fsDirectConverter : fsBaseConverter
	{
		public abstract Type ModelType
		{
			get;
		}
	}
	public abstract class fsDirectConverter<TModel> : fsDirectConverter
	{
		public override Type ModelType => typeof(TModel);

		public sealed override FsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Dictionary<string, fsData> dictionary = new Dictionary<string, fsData>();
			FsResult result = DoSerialize((TModel)instance, dictionary);
			serialized = new fsData(dictionary);
			return result;
		}

		public sealed override FsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			FsResult success = FsResult.Success;
			var fsResult = success += CheckType(data, fsDataType.Object);
			if (fsResult.Failed)
			{
				return success;
			}
			TModel model = (TModel)instance;
			success += DoDeserialize(data.AsDictionary, ref model);
			instance = model;
			return success;
		}

		protected abstract FsResult DoSerialize(TModel model, Dictionary<string, fsData> serialized);

		protected abstract FsResult DoDeserialize(Dictionary<string, fsData> data, ref TModel model);
	}
}
