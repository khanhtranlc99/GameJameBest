using System;
using System.Globalization;

namespace FullSerializer.Internal
{
	public class fsDateConverter : fsConverter
	{
		private const string DefaultDateTimeFormatString = "o";

		private const string DateTimeOffsetFormatString = "o";

		private string DateTimeFormatString => Serializer.Config.CustomDateTimeFormatString ?? "o";

		public override bool CanProcess(Type type)
		{
			return type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);
		}

		public override FsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			if (instance is DateTime)
			{
				serialized = new fsData(((DateTime)instance).ToString(DateTimeFormatString));
				return FsResult.Success;
			}
			if (instance is DateTimeOffset)
			{
				serialized = new fsData(((DateTimeOffset)instance).ToString("o"));
				return FsResult.Success;
			}
			if (instance is TimeSpan)
			{
				serialized = new fsData(((TimeSpan)instance).ToString());
				return FsResult.Success;
			}
			throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected serialization type");
		}

		public override FsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (!data.IsString)
			{
				return FsResult.Fail("Date deserialization requires a string, not " + data.Type);
			}
			if (storageType == typeof(DateTime))
			{
				DateTime result;
				if (DateTime.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out result))
				{
					instance = result;
					return FsResult.Success;
				}
				if (fsGlobalConfig.AllowInternalExceptions)
				{
					try
					{
						instance = Convert.ToDateTime(data.AsString);
						return FsResult.Success;
					}
					catch (Exception ex)
					{
						return FsResult.Fail("Unable to parse " + data.AsString + " into a DateTime; got exception " + ex);
					}
				}
				return FsResult.Fail("Unable to parse " + data.AsString + " into a DateTime");
			}
			if (storageType == typeof(DateTimeOffset))
			{
				DateTimeOffset result2;
				if (DateTimeOffset.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out result2))
				{
					instance = result2;
					return FsResult.Success;
				}
				return FsResult.Fail("Unable to parse " + data.AsString + " into a DateTimeOffset");
			}
			if (storageType == typeof(TimeSpan))
			{
				TimeSpan result3;
				if (TimeSpan.TryParse(data.AsString, out result3))
				{
					instance = result3;
					return FsResult.Success;
				}
				return FsResult.Fail("Unable to parse " + data.AsString + " into a TimeSpan");
			}
			throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected deserialization type");
		}
	}
}
