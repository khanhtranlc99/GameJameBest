using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
	public class fsSerializer
	{
		internal class fsLazyCycleDefinitionWriter
		{
			private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();

			private HashSet<int> _references = new HashSet<int>();

			public void WriteDefinition(int id, fsData data)
			{
				if (_references.Contains(id))
				{
					EnsureDictionary(data);
					data.AsDictionary["$id"] = new fsData(id.ToString());
				}
				else
				{
					_pendingDefinitions[id] = data;
				}
			}

			public void WriteReference(int id, Dictionary<string, fsData> dict)
			{
				if (_pendingDefinitions.ContainsKey(id))
				{
					fsData fsData = _pendingDefinitions[id];
					EnsureDictionary(fsData);
					fsData.AsDictionary["$id"] = new fsData(id.ToString());
					_pendingDefinitions.Remove(id);
				}
				else
				{
					_references.Add(id);
				}
				dict["$ref"] = new fsData(id.ToString());
			}

			public void Clear()
			{
				_pendingDefinitions.Clear();
				_references.Clear();
			}
		}

		private const string Key_ObjectReference = "$ref";

		private const string Key_ObjectDefinition = "$id";

		private const string Key_InstanceType = "$type";

		private const string Key_Version = "$version";

		private const string Key_Content = "$content";

		private static HashSet<string> _reservedKeywords;

		private Dictionary<Type, fsBaseConverter> _cachedConverterTypeInstances;

		private Dictionary<Type, fsBaseConverter> _cachedConverters;

		private Dictionary<Type, List<fsObjectProcessor>> _cachedProcessors;

		private readonly List<fsConverter> _availableConverters;

		private readonly Dictionary<Type, fsDirectConverter> _availableDirectConverters;

		private readonly List<fsObjectProcessor> _processors;

		private readonly fsCyclicReferenceManager _references;

		private readonly fsLazyCycleDefinitionWriter _lazyReferenceWriter;

		public fsContext Context;

		public fsConfig Config;

		public fsSerializer()
		{
			_cachedConverterTypeInstances = new Dictionary<Type, fsBaseConverter>();
			_cachedConverters = new Dictionary<Type, fsBaseConverter>();
			_cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
			_references = new fsCyclicReferenceManager();
			_lazyReferenceWriter = new fsLazyCycleDefinitionWriter();
			_availableConverters = new List<fsConverter>
			{
				new fsNullableConverter
				{
					Serializer = this
				},
				new fsGuidConverter
				{
					Serializer = this
				},
				new fsTypeConverter
				{
					Serializer = this
				},
				new fsDateConverter
				{
					Serializer = this
				},
				new fsEnumConverter
				{
					Serializer = this
				},
				new fsPrimitiveConverter
				{
					Serializer = this
				},
				new fsArrayConverter
				{
					Serializer = this
				},
				new fsDictionaryConverter
				{
					Serializer = this
				},
				new fsIEnumerableConverter
				{
					Serializer = this
				},
				new fsKeyValuePairConverter
				{
					Serializer = this
				},
				new fsWeakReferenceConverter
				{
					Serializer = this
				},
				new fsReflectedConverter
				{
					Serializer = this
				}
			};
			_availableDirectConverters = new Dictionary<Type, fsDirectConverter>();
			_processors = new List<fsObjectProcessor>
			{
				new fsSerializationCallbackProcessor()
			};
			_processors.Add(new fsSerializationCallbackReceiverProcessor());
			Context = new fsContext();
			Config = new fsConfig();
			foreach (Type converter in fsConverterRegistrar.Converters)
			{
				AddConverter((fsBaseConverter)Activator.CreateInstance(converter));
			}
		}

		static fsSerializer()
		{
			_reservedKeywords = new HashSet<string>
			{
				"$ref",
				"$id",
				"$type",
				"$version",
				"$content"
			};
		}

		public static bool IsReservedKeyword(string key)
		{
			return _reservedKeywords.Contains(key);
		}

		private static bool IsObjectReference(fsData data)
		{
			if (!data.IsDictionary)
			{
				return false;
			}
			return data.AsDictionary.ContainsKey("$ref");
		}

		private static bool IsObjectDefinition(fsData data)
		{
			if (!data.IsDictionary)
			{
				return false;
			}
			return data.AsDictionary.ContainsKey("$id");
		}

		private static bool IsVersioned(fsData data)
		{
			if (!data.IsDictionary)
			{
				return false;
			}
			return data.AsDictionary.ContainsKey("$version");
		}

		private static bool IsTypeSpecified(fsData data)
		{
			if (!data.IsDictionary)
			{
				return false;
			}
			return data.AsDictionary.ContainsKey("$type");
		}

		private static bool IsWrappedData(fsData data)
		{
			if (!data.IsDictionary)
			{
				return false;
			}
			return data.AsDictionary.ContainsKey("$content");
		}

		public static void StripDeserializationMetadata(ref fsData data)
		{
			if (data.IsDictionary && data.AsDictionary.ContainsKey("$content"))
			{
				data = data.AsDictionary["$content"];
			}
			if (data.IsDictionary)
			{
				Dictionary<string, fsData> asDictionary = data.AsDictionary;
				asDictionary.Remove("$ref");
				asDictionary.Remove("$id");
				asDictionary.Remove("$type");
				asDictionary.Remove("$version");
			}
		}

		private static void ConvertLegacyData(ref fsData data)
		{
			if (!data.IsDictionary)
			{
				return;
			}
			Dictionary<string, fsData> asDictionary = data.AsDictionary;
			if (asDictionary.Count <= 2)
			{
				string key = "ReferenceId";
				string key2 = "SourceId";
				string key3 = "Data";
				string key4 = "Type";
				string key5 = "Data";
				if (asDictionary.Count == 2 && asDictionary.ContainsKey(key4) && asDictionary.ContainsKey(key5))
				{
					data = asDictionary[key5];
					EnsureDictionary(data);
					ConvertLegacyData(ref data);
					data.AsDictionary["$type"] = asDictionary[key4];
				}
				else if (asDictionary.Count == 2 && asDictionary.ContainsKey(key2) && asDictionary.ContainsKey(key3))
				{
					data = asDictionary[key3];
					EnsureDictionary(data);
					ConvertLegacyData(ref data);
					data.AsDictionary["$id"] = asDictionary[key2];
				}
				else if (asDictionary.Count == 1 && asDictionary.ContainsKey(key))
				{
					data = fsData.CreateDictionary();
					data.AsDictionary["$ref"] = asDictionary[key];
				}
			}
		}

		private static void Invoke_OnBeforeSerialize(List<fsObjectProcessor> processors, Type storageType, object instance)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeSerialize(storageType, instance);
			}
		}

		private static void Invoke_OnAfterSerialize(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data)
		{
			for (int num = processors.Count - 1; num >= 0; num--)
			{
				processors[num].OnAfterSerialize(storageType, instance, ref data);
			}
		}

		private static void Invoke_OnBeforeDeserialize(List<fsObjectProcessor> processors, Type storageType, ref fsData data)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeDeserialize(storageType, ref data);
			}
		}

		private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
			}
		}

		private static void Invoke_OnAfterDeserialize(List<fsObjectProcessor> processors, Type storageType, object instance)
		{
			for (int num = processors.Count - 1; num >= 0; num--)
			{
				processors[num].OnAfterDeserialize(storageType, instance);
			}
		}

		private static void EnsureDictionary(fsData data)
		{
			if (!data.IsDictionary)
			{
				fsData value = data.Clone();
				data.BecomeDictionary();
				data.AsDictionary["$content"] = value;
			}
		}

		public void AddProcessor(fsObjectProcessor processor)
		{
			_processors.Add(processor);
			_cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
		}

		public void RemoveProcessor<TProcessor>()
		{
			int num = 0;
			while (num < _processors.Count)
			{
				if (_processors[num] is TProcessor)
				{
					_processors.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			_cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
		}

		private List<fsObjectProcessor> GetProcessors(Type type)
		{
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
			List<fsObjectProcessor> value;
			if (attribute != null && attribute.Processor != null)
			{
				fsObjectProcessor item = (fsObjectProcessor)Activator.CreateInstance(attribute.Processor);
				value = new List<fsObjectProcessor>();
				value.Add(item);
				_cachedProcessors[type] = value;
			}
			else if (!_cachedProcessors.TryGetValue(type, out value))
			{
				value = new List<fsObjectProcessor>();
				for (int i = 0; i < _processors.Count; i++)
				{
					fsObjectProcessor fsObjectProcessor = _processors[i];
					if (fsObjectProcessor.CanProcess(type))
					{
						value.Add(fsObjectProcessor);
					}
				}
				_cachedProcessors[type] = value;
			}
			return value;
		}

		public void AddConverter(fsBaseConverter converter)
		{
			if (converter.Serializer != null)
			{
				throw new InvalidOperationException("Cannot add a single converter instance to multiple fsConverters -- please construct a new instance for " + converter);
			}
			if (converter is fsDirectConverter)
			{
				fsDirectConverter fsDirectConverter = (fsDirectConverter)converter;
				_availableDirectConverters[fsDirectConverter.ModelType] = fsDirectConverter;
			}
			else
			{
				if (!(converter is fsConverter))
				{
					throw new InvalidOperationException("Unable to add converter " + converter + "; the type association strategy is unknown. Please use either fsDirectConverter or fsConverter as your base type.");
				}
				_availableConverters.Insert(0, (fsConverter)converter);
			}
			converter.Serializer = this;
			_cachedConverters = new Dictionary<Type, fsBaseConverter>();
		}

		private fsBaseConverter GetConverter(Type type, Type overrideConverterType)
		{
			if (overrideConverterType != null)
			{
				fsBaseConverter value;
				if (!_cachedConverterTypeInstances.TryGetValue(overrideConverterType, out value))
				{
					value = (fsBaseConverter)Activator.CreateInstance(overrideConverterType);
					value.Serializer = this;
					_cachedConverterTypeInstances[overrideConverterType] = value;
				}
				return value;
			}
			fsBaseConverter value2;
			if (_cachedConverters.TryGetValue(type, out value2))
			{
				return value2;
			}
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
			if (attribute != null && attribute.Converter != null)
			{
				value2 = (fsBaseConverter)Activator.CreateInstance(attribute.Converter);
				value2.Serializer = this;
				fsBaseConverter fsBaseConverter = value2;
				_cachedConverters[type] = fsBaseConverter;
				return fsBaseConverter;
			}
			fsForwardAttribute attribute2 = fsPortableReflection.GetAttribute<fsForwardAttribute>(type);
			if (attribute2 != null)
			{
				value2 = new fsForwardConverter(attribute2);
				value2.Serializer = this;
				fsBaseConverter fsBaseConverter = value2;
				_cachedConverters[type] = fsBaseConverter;
				return fsBaseConverter;
			}
			if (!_cachedConverters.TryGetValue(type, out value2))
			{
				if (_availableDirectConverters.ContainsKey(type))
				{
					value2 = _availableDirectConverters[type];
					fsBaseConverter fsBaseConverter = value2;
					_cachedConverters[type] = fsBaseConverter;
					return fsBaseConverter;
				}
				for (int i = 0; i < _availableConverters.Count; i++)
				{
					if (_availableConverters[i].CanProcess(type))
					{
						value2 = _availableConverters[i];
						fsBaseConverter fsBaseConverter = value2;
						_cachedConverters[type] = fsBaseConverter;
						return fsBaseConverter;
					}
				}
			}
			throw new InvalidOperationException("Internal error -- could not find a converter for " + type);
		}

		public FsResult TrySerialize<T>(T instance, out fsData data)
		{
			return TrySerialize(typeof(T), instance, out data);
		}

		public void TryDeserialize<T>(fsData data, ref T instance)
		{
			object result = instance;
			FsResult result2 = TryDeserialize(data, typeof(T), ref result);
			if (result2.Succeeded)
			{
				instance = (T)result;
			}
			//return result2;
		}
		public void TryDeserializeTest<T>(fsData data, ref T instance)
		{
			object result = instance;
			ConvertLegacyData(ref data);
			List<fsObjectProcessor> processors2 = new List<fsObjectProcessor>();
				FsResult result2 = InternalDeserialize_1_CycleReference(null, data, typeof(T), ref result, out processors2);
				if (result2.Succeeded)
				{
					Invoke_OnAfterDeserialize(processors2, typeof(T), result);
					instance = (T)result;
				}
				//return result2;
		}

		public FsResult TrySerialize(Type storageType, object instance, out fsData data)
		{
			return TrySerialize(storageType, null, instance, out data);
		}

		public FsResult TrySerialize(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			List<fsObjectProcessor> processors = GetProcessors((instance != null) ? instance.GetType() : storageType);
			Invoke_OnBeforeSerialize(processors, storageType, instance);
			if (object.ReferenceEquals(instance, null))
			{
				data = new fsData();
				Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
				return FsResult.Success;
			}
			FsResult result = InternalSerialize_1_ProcessCycles(storageType, overrideConverterType, instance, out data);
			Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
			return result;
		}

		private FsResult InternalSerialize_1_ProcessCycles(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			try
			{
				_references.Enter();
				fsBaseConverter converter = GetConverter(instance.GetType(), overrideConverterType);
				if (!converter.RequestCycleSupport(instance.GetType()))
				{
					return InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
				}
				if (_references.IsReference(instance))
				{
					data = fsData.CreateDictionary();
					_lazyReferenceWriter.WriteReference(_references.GetReferenceId(instance), data.AsDictionary);
					return FsResult.Success;
				}
				_references.MarkSerialized(instance);
				FsResult result = InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
				if (result.Failed)
				{
					return result;
				}
				_lazyReferenceWriter.WriteDefinition(_references.GetReferenceId(instance), data);
				return result;
				//IL_00ca:
				//FsResult result2;
				//return result2;
			}
			finally
			{
				if (_references.Exit())
				{
					_lazyReferenceWriter.Clear();
				}
			}
		}

		private FsResult InternalSerialize_2_Inheritance(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			FsResult result = InternalSerialize_3_ProcessVersioning(overrideConverterType, instance, out data);
			if (result.Failed)
			{
				return result;
			}
			if (storageType != instance.GetType() && GetConverter(storageType, overrideConverterType).RequestInheritanceSupport(storageType))
			{
				EnsureDictionary(data);
				data.AsDictionary["$type"] = new fsData(instance.GetType().FullName);
			}
			return result;
		}

		private FsResult InternalSerialize_3_ProcessVersioning(Type overrideConverterType, object instance, out fsData data)
		{
			fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(instance.GetType());
			if (versionedType.HasValue)
			{
				fsVersionedType value = versionedType.Value;
				FsResult result = InternalSerialize_4_Converter(overrideConverterType, instance, out data);
				if (result.Failed)
				{
					return result;
				}
				EnsureDictionary(data);
				data.AsDictionary["$version"] = new fsData(value.VersionString);
				return result;
			}
			return InternalSerialize_4_Converter(overrideConverterType, instance, out data);
		}

		private FsResult InternalSerialize_4_Converter(Type overrideConverterType, object instance, out fsData data)
		{
			Type type = instance.GetType();
			return GetConverter(type, overrideConverterType).TrySerialize(instance, out data, type);
		}

		public  FsResult TryDeserialize(fsData data, Type storageType, ref object result)
		{
			return TryDeserialize(data, storageType, null, ref result);
		}
		public FsResult TryDeserialize(fsData data, Type storageType, Type overrideConverterType, ref object result)
		{
			if (data.IsNull)
			{
				result = null;
				List<fsObjectProcessor> processors = GetProcessors(storageType);
				Invoke_OnBeforeDeserialize(processors, storageType, ref data);
				Invoke_OnAfterDeserialize(processors, storageType, null);
				return FsResult.Success;
			}
			ConvertLegacyData(ref data);
			try
			{
				_references.Enter();
				List<fsObjectProcessor> processors2;
				FsResult result2 = InternalDeserialize_1_CycleReference(overrideConverterType, data, storageType, ref result, out processors2);
				if (result2.Succeeded)
				{
					Invoke_OnAfterDeserialize(processors2, storageType, result);
				}
				return result2;
			}
			finally
			{
				_references.Exit();
			}
		}

		private FsResult InternalDeserialize_1_CycleReference(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			if (IsObjectReference(data))
			{
				int id = int.Parse(data.AsDictionary["$ref"].AsString);
				result = _references.GetReferenceObject(id);
				processors = GetProcessors(result.GetType());
				return FsResult.Success;
			}
			return InternalDeserialize_2_Version(overrideConverterType, data, storageType, ref result, out processors);
		}

		private FsResult InternalDeserialize_2_Version(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			if (IsVersioned(data))
			{
				string asString = data.AsDictionary["$version"].AsString;
				fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(storageType);
				if (versionedType.HasValue)
				{
					fsVersionedType value = versionedType.Value;
					if (value.VersionString != asString)
					{
						FsResult success = FsResult.Success;
						List<fsVersionedType> path = new List<fsVersionedType>();
						success += fsVersionManager.GetVersionImportPath(asString, versionedType.Value, out path);
						if (success.Failed)
						{
							processors = GetProcessors(storageType);
							return success;
						}
						FsResult a = success;
						fsVersionedType fsVersionedType = path[0];
						success = a + InternalDeserialize_3_Inheritance(overrideConverterType, data, fsVersionedType.ModelType, ref result, out processors);
						if (success.Failed)
						{
							return success;
						}
						for (int i = 1; i < path.Count; i++)
						{
							result = path[i].Migrate(result);
						}
						if (IsObjectDefinition(data))
						{
							int id = int.Parse(data.AsDictionary["$id"].AsString);
							_references.AddReferenceWithId(id, result);
						}
						processors = GetProcessors(success.GetType());
						return success;
					}
				}
			}
			return InternalDeserialize_3_Inheritance(overrideConverterType, data, storageType, ref result, out processors);
		}

		private FsResult InternalDeserialize_3_Inheritance(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			FsResult fsResult = FsResult.Success;
			Type type = storageType;
			if (IsTypeSpecified(data))
			{
				fsData fsData = data.AsDictionary["$type"];
				if (!fsData.IsString)
				{
					fsResult.AddMessage("$type value must be a string (in " + data + ")");
				}
				else
				{
					string asString = fsData.AsString;
					Type type2 = fsTypeCache.GetType(asString);
					if (type2 == null)
					{
						fsResult += FsResult.Fail("Unable to locate specified type \"" + asString + "\"");
					}
					else if (!storageType.IsAssignableFrom(type2))
					{
						fsResult.AddMessage("Ignoring type specifier; a field/property of type " + storageType + " cannot hold an instance of " + type2);
					}
					else
					{
						type = type2;
					}
				}
			}
			processors = GetProcessors(type);
			if (fsResult.Failed)
			{
				return fsResult;
			}
			Invoke_OnBeforeDeserialize(processors, storageType, ref data);
			if (ReferenceEquals(result, null) || result.GetType() != type)
			{
				result = GetConverter(type, overrideConverterType).CreateInstance(data, type);
			}
			Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);
			
			return InternalDeserialize_4_Cycles(overrideConverterType, data, type, ref result);
		}

		private FsResult InternalDeserialize_4_Cycles(Type overrideConverterType, fsData data, Type resultType, ref object result)
		{
			if (IsObjectDefinition(data))
			{
				int id = int.Parse(data.AsDictionary["$id"].AsString);
				_references.AddReferenceWithId(id, result);
			}
			return InternalDeserialize_5_Converter(overrideConverterType, data, resultType, ref result);
		}

		private FsResult InternalDeserialize_5_Converter(Type overrideConverterType, fsData data, Type resultType, ref object result)
		{
			if (IsWrappedData(data))
			{
				data = data.AsDictionary["$content"];
			}
			return GetConverter(resultType, overrideConverterType).TryDeserialize(data, ref result, resultType);
		}
	}
}
