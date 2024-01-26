using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	public class fsDictionaryConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			return typeof(IDictionary).IsAssignableFrom(type);
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
		}

		public override FsResult TryDeserialize(fsData data, ref object instance_, Type storageType)
		{
			IDictionary dictionary = (IDictionary)instance_;
			FsResult fsResult = FsResult.Success;
			Type keyStorageType;
			Type valueStorageType;
			GetKeyValueTypes(dictionary.GetType(), out keyStorageType, out valueStorageType);
			if (data.IsList)
			{
				List<fsData> asList = data.AsList;
				for (int i = 0; i < asList.Count; i++)
				{
					fsData data2 = asList[i];
					FsResult fsResult2 = fsResult += CheckType(data2, fsDataType.Object);
					if (fsResult2.Failed)
					{
						return fsResult;
					}
					fsData subitem;
					FsResult fsResult3 = fsResult += CheckKey(data2, "Key", out subitem);
					if (fsResult3.Failed)
					{
						return fsResult;
					}
					fsData subitem2;
					FsResult fsResult4 = fsResult += CheckKey(data2, "Value", out subitem2);
					if (fsResult4.Failed)
					{
						return fsResult;
					}
					object result = null;
					object result2 = null;
					FsResult fsResult5 = fsResult += Serializer.TryDeserialize(subitem, keyStorageType, ref result);
					if (fsResult5.Failed)
					{
						return fsResult;
					}
					FsResult fsResult6 = fsResult += Serializer.TryDeserialize(subitem2, valueStorageType, ref result2);
					if (fsResult6.Failed)
					{
						return fsResult;
					}
					AddItemToDictionary(dictionary, result, result2);
				}
				return fsResult;
			}
			if (data.IsDictionary)
			{
				foreach (KeyValuePair<string, fsData> item in data.AsDictionary)
				{
					if (!fsSerializer.IsReservedKeyword(item.Key))
					{
						fsData data3 = new fsData(item.Key);
						fsData value = item.Value;
						object result3 = null;
						object result4 = null;
						FsResult fsResult7 = fsResult += Serializer.TryDeserialize(data3, keyStorageType, ref result3);
						if (fsResult7.Failed)
						{
							return fsResult;
						}
						FsResult fsResult8 = fsResult += Serializer.TryDeserialize(value, valueStorageType, ref result4);
						if (fsResult8.Failed)
						{
							return fsResult;
						}
						AddItemToDictionary(dictionary, result3, result4);
					}
				}
				return fsResult;
			}
			return FailExpectedType(data, fsDataType.Array, fsDataType.Object);
		}

		public override FsResult TrySerialize(object instance_, out fsData serialized, Type storageType)
		{
			serialized = fsData.Null;
			FsResult fsResult = FsResult.Success;
			IDictionary dictionary = (IDictionary)instance_;
			Type keyStorageType;  Type valueStorageType;
			GetKeyValueTypes(dictionary.GetType(), out  keyStorageType, out  valueStorageType);
			IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
			bool flag = true;
			List<fsData> list = new List<fsData>(dictionary.Count);
			List<fsData> list2 = new List<fsData>(dictionary.Count);
			while (enumerator.MoveNext())
			{
				fsData data;
				FsResult fsResult2 = fsResult += Serializer.TrySerialize(keyStorageType, enumerator.Key, out data);
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				fsData data2;
				FsResult fsResult3 = fsResult += Serializer.TrySerialize(valueStorageType, enumerator.Value, out data2);
				if (fsResult3.Failed)
				{
					return fsResult;
				}
				list.Add(data);
				list2.Add(data2);
				flag &= data.IsString;
			}
			if (flag)
			{
				serialized = fsData.CreateDictionary();
				Dictionary<string, fsData> asDictionary = serialized.AsDictionary;
				for (int i = 0; i < list.Count; i++)
				{
					fsData fsData = list[i];
					fsData value = list2[i];
					asDictionary[fsData.AsString] = value;
				}
			}
			else
			{
				serialized = fsData.CreateList(list.Count);
				List<fsData> asList = serialized.AsList;
				for (int j = 0; j < list.Count; j++)
				{
					fsData value2 = list[j];
					fsData value3 = list2[j];
					Dictionary<string, fsData> dictionary2 = new Dictionary<string, fsData>();
					dictionary2["Key"] = value2;
					dictionary2["Value"] = value3;
					asList.Add(new fsData(dictionary2));
				}
			}
			return fsResult;
		}

		private FsResult AddItemToDictionary(IDictionary dictionary, object key, object value)
		{
			if (key == null || value == null)
			{
				Type @interface = fsReflectionUtility.GetInterface(dictionary.GetType(), typeof(ICollection<>));
				if (@interface == null)
				{
					return FsResult.Warn(dictionary.GetType() + " does not extend ICollection");
				}
				Type type = @interface.GetGenericArguments()[0];
				object obj = Activator.CreateInstance(type, key, value);
				MethodInfo flattenedMethod = @interface.GetFlattenedMethod("Add");
				flattenedMethod.Invoke(dictionary, new object[1]
				{
					obj
				});
				return FsResult.Success;
			}
			dictionary[key] = value;
			return FsResult.Success;
		}

		private static void GetKeyValueTypes(Type dictionaryType, out Type keyStorageType, out Type valueStorageType)
		{
			Type @interface = fsReflectionUtility.GetInterface(dictionaryType, typeof(IDictionary<, >));
			if (@interface != null)
			{
				Type[] genericArguments = @interface.GetGenericArguments();
				keyStorageType = genericArguments[0];
				valueStorageType = genericArguments[1];
			}
			else
			{
				keyStorageType = typeof(object);
				valueStorageType = typeof(object);
			}
		}
	}
}
