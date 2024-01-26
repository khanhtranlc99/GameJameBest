using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	public static class fsVersionManager
	{
		private static readonly Dictionary<Type, fsOption<fsVersionedType>> _cache = new Dictionary<Type, fsOption<fsVersionedType>>();

		public static FsResult GetVersionImportPath(string currentVersion, fsVersionedType targetVersion, out List<fsVersionedType> path)
		{
			path = new List<fsVersionedType>();
			if (!GetVersionImportPathRecursive(path, currentVersion, targetVersion))
			{
				return FsResult.Fail("There is no migration path from \"" + currentVersion + "\" to \"" + targetVersion.VersionString + "\"");
			}
			path.Add(targetVersion);
			return FsResult.Success;
		}

		private static bool GetVersionImportPathRecursive(List<fsVersionedType> path, string currentVersion, fsVersionedType current)
		{
			for (int i = 0; i < current.Ancestors.Length; i++)
			{
				fsVersionedType fsVersionedType = current.Ancestors[i];
				if (fsVersionedType.VersionString == currentVersion || GetVersionImportPathRecursive(path, currentVersion, fsVersionedType))
				{
					path.Add(fsVersionedType);
					return true;
				}
			}
			return false;
		}

		public static fsOption<fsVersionedType> GetVersionedType(Type type)
		{
			fsOption<fsVersionedType> value;
			if (!_cache.TryGetValue(type, out value))
			{
				fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
				if (attribute != null && (!string.IsNullOrEmpty(attribute.VersionString) || attribute.PreviousModels != null))
				{
					if (attribute.PreviousModels != null && string.IsNullOrEmpty(attribute.VersionString))
					{
						throw new Exception("fsObject attribute on " + type + " contains a PreviousModels specifier - it must also include a VersionString modifier");
					}
					fsVersionedType[] array = new fsVersionedType[(attribute.PreviousModels != null) ? attribute.PreviousModels.Length : 0];
					for (int i = 0; i < array.Length; i++)
					{
						fsOption<fsVersionedType> versionedType = GetVersionedType(attribute.PreviousModels[i]);
						if (versionedType.IsEmpty)
						{
							throw new Exception("Unable to create versioned type for ancestor " + versionedType + "; please add an [fsObject(VersionString=\"...\")] attribute");
						}
						array[i] = versionedType.Value;
					}
					fsVersionedType fsVersionedType = default(fsVersionedType);
					fsVersionedType.Ancestors = array;
					fsVersionedType.VersionString = attribute.VersionString;
					fsVersionedType.ModelType = type;
					fsVersionedType fsVersionedType2 = fsVersionedType;
					VerifyUniqueVersionStrings(fsVersionedType2);
					VerifyConstructors(fsVersionedType2);
					value = fsOption.Just(fsVersionedType2);
				}
				_cache[type] = value;
			}
			return value;
		}

		private static void VerifyConstructors(fsVersionedType type)
		{
			ConstructorInfo[] declaredConstructors = type.ModelType.GetDeclaredConstructors();
			int num = 0;
			Type modelType;
			while (true)
			{
				if (num >= type.Ancestors.Length)
				{
					return;
				}
				modelType = type.Ancestors[num].ModelType;
				bool flag = false;
				for (int i = 0; i < declaredConstructors.Length; i++)
				{
					ParameterInfo[] parameters = declaredConstructors[i].GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType == modelType)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
				num++;
			}
			throw new fsMissingVersionConstructorException(type.ModelType, modelType);
		}

		private static void VerifyUniqueVersionStrings(fsVersionedType type)
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			Queue<fsVersionedType> queue = new Queue<fsVersionedType>();
			queue.Enqueue(type);
			fsVersionedType fsVersionedType;
			while (true)
			{
				if (queue.Count > 0)
				{
					fsVersionedType = queue.Dequeue();
					if (dictionary.ContainsKey(fsVersionedType.VersionString) && dictionary[fsVersionedType.VersionString] != fsVersionedType.ModelType)
					{
						break;
					}
					dictionary[fsVersionedType.VersionString] = fsVersionedType.ModelType;
					fsVersionedType[] ancestors = fsVersionedType.Ancestors;
					foreach (fsVersionedType item in ancestors)
					{
						queue.Enqueue(item);
					}
					continue;
				}
				return;
			}
			throw new fsDuplicateVersionNameException(dictionary[fsVersionedType.VersionString], fsVersionedType.ModelType, fsVersionedType.VersionString);
		}
	}
}
