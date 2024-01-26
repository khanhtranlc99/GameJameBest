using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	public class fsIEnumerableConverter : fsConverter
	{
		public override bool CanProcess(Type type)
		{
			if (!typeof(IEnumerable).IsAssignableFrom(type))
			{
				return false;
			}
			return GetAddMethod(type) != null;
		}

		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(Serializer.Config, storageType).CreateInstance();
		}

		public override FsResult TrySerialize(object instance_, out fsData serialized, Type storageType)
		{
			IEnumerable enumerable = (IEnumerable)instance_;
			FsResult success = FsResult.Success;
			Type elementType = GetElementType(storageType);
			serialized = fsData.CreateList(HintSize(enumerable));
			List<fsData> asList = serialized.AsList;
			foreach (object item in enumerable)
			{
				fsData data;
				FsResult result = Serializer.TrySerialize(elementType, item, out data);
				success.AddMessages(result);
				if (!result.Failed)
				{
					asList.Add(data);
				}
			}
			if (IsStack(enumerable.GetType()))
			{
				asList.Reverse();
			}
			return success;
		}

		private bool IsStack(Type type)
		{
			return type.Resolve().IsGenericType && type.Resolve().GetGenericTypeDefinition() == typeof(Stack<>);
		}

		public override FsResult TryDeserialize(fsData data, ref object instance_, Type storageType)
		{
			IEnumerable enumerable = (IEnumerable)instance_;
			FsResult success = FsResult.Success;
			FsResult fsResult = success += CheckType(data, fsDataType.Array);
			if (fsResult.Failed)
			{
				return success;
			}
			Type elementType = GetElementType(storageType);
			MethodInfo addMethod = GetAddMethod(storageType);
			MethodInfo flattenedMethod = storageType.GetFlattenedMethod("get_Item");
			MethodInfo flattenedMethod2 = storageType.GetFlattenedMethod("set_Item");
			if (flattenedMethod2 == null)
			{
				TryClear(storageType, enumerable);
			}
			int num = TryGetExistingSize(storageType, enumerable);
			List<fsData> asList = data.AsList;
			for (int i = 0; i < asList.Count; i++)
			{
				fsData data2 = asList[i];
				object result = null;
				if (flattenedMethod != null && i < num)
				{
					result = flattenedMethod.Invoke(enumerable, new object[1]
					{
						i
					});
				}
				FsResult result2 = Serializer.TryDeserialize(data2, elementType, ref result);
				success.AddMessages(result2);
				if (!result2.Failed)
				{
					if (flattenedMethod2 != null && i < num)
					{
						flattenedMethod2.Invoke(enumerable, new object[2]
						{
							i,
							result
						});
					}
					else
					{
						addMethod.Invoke(enumerable, new object[1]
						{
							result
						});
					}
				}
			}
			return success;
		}

		private static int HintSize(IEnumerable collection)
		{
			if (collection is ICollection)
			{
				return ((ICollection)collection).Count;
			}
			return 0;
		}

		private static Type GetElementType(Type objectType)
		{
			if (objectType.HasElementType)
			{
				return objectType.GetElementType();
			}
			Type @interface = fsReflectionUtility.GetInterface(objectType, typeof(IEnumerable<>));
			if (@interface != null)
			{
				return @interface.GetGenericArguments()[0];
			}
			return typeof(object);
		}

		private static void TryClear(Type type, object instance)
		{
			type.GetFlattenedMethod("Clear")?.Invoke(instance, null);
		}

		private static int TryGetExistingSize(Type type, object instance)
		{
			PropertyInfo flattenedProperty = type.GetFlattenedProperty("Count");
			if (flattenedProperty != null)
			{
				return (int)flattenedProperty.GetGetMethod().Invoke(instance, null);
			}
			return 0;
		}

		private static MethodInfo GetAddMethod(Type type)
		{
			Type @interface = fsReflectionUtility.GetInterface(type, typeof(ICollection<>));
			if (@interface != null)
			{
				MethodInfo declaredMethod = @interface.GetDeclaredMethod("Add");
				if (declaredMethod != null)
				{
					return declaredMethod;
				}
			}
			return type.GetFlattenedMethod("Add") ?? type.GetFlattenedMethod("Push") ?? type.GetFlattenedMethod("Enqueue");
		}
	}
}
