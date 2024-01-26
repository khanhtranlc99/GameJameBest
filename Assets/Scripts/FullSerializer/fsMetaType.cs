using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;

namespace FullSerializer
{
	public class fsMetaType
	{
		private static Dictionary<fsConfig, Dictionary<Type, fsMetaType>> _configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();

		public Type ReflectedType;

		private bool _hasEmittedAotData;

		private bool? _hasDefaultConstructorCache;

		private bool _isDefaultConstructorPublic;

		public fsMetaProperty[] Properties
		{
			get;
			private set;
		}

		public bool HasDefaultConstructor
		{
			get
			{
				if (!_hasDefaultConstructorCache.HasValue)
				{
					if (ReflectedType.Resolve().IsArray)
					{
						_hasDefaultConstructorCache = true;
						_isDefaultConstructorPublic = true;
					}
					else if (ReflectedType.Resolve().IsValueType)
					{
						_hasDefaultConstructorCache = true;
						_isDefaultConstructorPublic = true;
					}
					else
					{
						ConstructorInfo declaredConstructor = ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes);
						_hasDefaultConstructorCache = (declaredConstructor != null);
						if (declaredConstructor != null)
						{
							_isDefaultConstructorPublic = declaredConstructor.IsPublic;
						}
					}
				}
				return _hasDefaultConstructorCache.Value;
			}
		}

		private fsMetaType(fsConfig config, Type reflectedType)
		{
			ReflectedType = reflectedType;
			List<fsMetaProperty> list = new List<fsMetaProperty>();
			CollectProperties(config, list, reflectedType);
			Properties = list.ToArray();
		}

		public static fsMetaType Get(fsConfig config, Type type)
		{
			Dictionary<Type, fsMetaType> value = new Dictionary<Type, fsMetaType>();
			if (!_configMetaTypes.TryGetValue(config, out value))
			{
				Dictionary<Type, fsMetaType> dictionary = new Dictionary<Type, fsMetaType>();
				_configMetaTypes[config] = dictionary;
				value = dictionary;
			}
			fsMetaType value2;
			if (!value.TryGetValue(type, out value2))
			{
				value2 = (value[type] = new fsMetaType(config, type));
			}
			return value2;
		}

		public static void ClearCache()
		{
			_configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();
		}

		private static void CollectProperties(fsConfig config, List<fsMetaProperty> properties, Type reflectedType)
		{
			bool flag = config.DefaultMemberSerialization == fsMemberSerialization.OptIn;
			bool flag2 = config.DefaultMemberSerialization == fsMemberSerialization.OptOut;
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(reflectedType);
			if (attribute != null)
			{
				flag = (attribute.MemberSerialization == fsMemberSerialization.OptIn);
				flag2 = (attribute.MemberSerialization == fsMemberSerialization.OptOut);
			}
			MemberInfo[] declaredMembers = reflectedType.GetDeclaredMembers();
			MemberInfo[] array = declaredMembers;
			MemberInfo member;
			for (int i = 0; i < array.Length; i++)
			{
				member = array[i];
				if (config.IgnoreSerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t)))
				{
					continue;
				}
				PropertyInfo propertyInfo = member as PropertyInfo;
				FieldInfo fieldInfo = member as FieldInfo;
				if ((propertyInfo == null && fieldInfo == null) || (propertyInfo != null && !config.EnablePropertySerialization) || (flag && !config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t))) || (flag2 && config.IgnoreSerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t))))
				{
					continue;
				}
				if (propertyInfo != null)
				{
					if (CanSerializeProperty(config, propertyInfo, declaredMembers, flag2))
					{
						properties.Add(new fsMetaProperty(config, propertyInfo));
					}
				}
				else if (fieldInfo != null && CanSerializeField(config, fieldInfo, flag2))
				{
					properties.Add(new fsMetaProperty(config, fieldInfo));
				}
			}
			if (reflectedType.Resolve().BaseType != null)
			{
				CollectProperties(config, properties, reflectedType.Resolve().BaseType);
			}
		}

		private static bool IsAutoProperty(PropertyInfo property, MemberInfo[] members)
		{
			return property.CanWrite && property.CanRead && fsPortableReflection.HasAttribute(property.GetGetMethod(), typeof(CompilerGeneratedAttribute), shouldCache: false);
		}

		private static bool CanSerializeProperty(fsConfig config, PropertyInfo property, MemberInfo[] members, bool annotationFreeValue)
		{
			if (typeof(Delegate).IsAssignableFrom(property.PropertyType))
			{
				return false;
			}
			MethodInfo getMethod = property.GetGetMethod(nonPublic: false);
			MethodInfo setMethod = property.GetSetMethod(nonPublic: false);
			if ((getMethod != null && getMethod.IsStatic) || (setMethod != null && setMethod.IsStatic))
			{
				return false;
			}
			if (property.GetIndexParameters().Length > 0)
			{
				return false;
			}
			if (config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(property, t)))
			{
				return true;
			}
			if (!property.CanRead || !property.CanWrite)
			{
				return false;
			}
			if ((config.SerializeNonAutoProperties || IsAutoProperty(property, members)) && getMethod != null && (config.SerializeNonPublicSetProperties || setMethod != null))
			{
				return true;
			}
			return annotationFreeValue;
		}

		private static bool CanSerializeField(fsConfig config, FieldInfo field, bool annotationFreeValue)
		{
			if (typeof(Delegate).IsAssignableFrom(field.FieldType))
			{
				return false;
			}
			if (field.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
			{
				return false;
			}
			if (field.IsStatic)
			{
				return false;
			}
			if (config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(field, t)))
			{
				return true;
			}
			if (!annotationFreeValue && !field.IsPublic)
			{
				return false;
			}
			return true;
		}

		public bool EmitAotData()
		{
			if (!_hasEmittedAotData)
			{
				_hasEmittedAotData = true;
				for (int i = 0; i < Properties.Length; i++)
				{
					if (!Properties[i].IsPublic)
					{
						return false;
					}
					if (Properties[i].IsReadOnly)
					{
						return false;
					}
				}
				if (!HasDefaultConstructor)
				{
					return false;
				}
				fsAotCompilationManager.AddAotCompilation(ReflectedType, Properties, _isDefaultConstructorPublic);
				return true;
			}
			return false;
		}

		public object CreateInstance()
		{
			if (ReflectedType.Resolve().IsInterface || ReflectedType.Resolve().IsAbstract)
			{
				throw new Exception("Cannot create an instance of an interface or abstract type for " + ReflectedType);
			}
			if (typeof(ScriptableObject).IsAssignableFrom(ReflectedType))
			{
				return ScriptableObject.CreateInstance(ReflectedType);
			}
			if (typeof(string) == ReflectedType)
			{
				return string.Empty;
			}
			if (!HasDefaultConstructor)
			{
				return FormatterServices.GetSafeUninitializedObject(ReflectedType);
			}
			if (ReflectedType.Resolve().IsArray)
			{
				return Array.CreateInstance(ReflectedType.GetElementType() ?? throw new InvalidOperationException(), 0);
			}
			try
			{
				return Activator.CreateInstance(ReflectedType, nonPublic: true);
			}
			catch (MissingMethodException innerException)
			{
				throw new InvalidOperationException("Unable to create instance of " + ReflectedType + "; there is no default constructor", innerException);
				IL_00f3:
				object result;
				return result;
			}
			catch (TargetInvocationException innerException2)
			{
				throw new InvalidOperationException("Constructor of " + ReflectedType + " threw an exception when creating an instance", innerException2);
				IL_0115:
				object result;
				return result;
			}
			catch (MemberAccessException innerException3)
			{
				throw new InvalidOperationException("Unable to access constructor of " + ReflectedType, innerException3);
				IL_0132:
				object result;
				return result;
			}
		}
	}
}
