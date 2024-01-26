using System;

namespace FullSerializer.Internal
{
	public struct fsVersionedType
	{
		public fsVersionedType[] Ancestors;

		public string VersionString;

		public Type ModelType;

		public object Migrate(object ancestorInstance)
		{
			return Activator.CreateInstance(ModelType, ancestorInstance);
		}

		public override string ToString()
		{
			return "fsVersionedType [ModelType=" + ModelType + ", VersionString=" + VersionString + ", Ancestors.Length=" + Ancestors.Length + "]";
		}

		public override bool Equals(object obj)
		{
			int result;
			if (obj is fsVersionedType)
			{
				Type modelType = ModelType;
				fsVersionedType fsVersionedType = (fsVersionedType)obj;
				result = ((modelType == fsVersionedType.ModelType) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
		}

		public override int GetHashCode()
		{
			return ModelType.GetHashCode();
		}

		public static bool operator ==(fsVersionedType a, fsVersionedType b)
		{
			return a.ModelType == b.ModelType;
		}

		public static bool operator !=(fsVersionedType a, fsVersionedType b)
		{
			return a.ModelType != b.ModelType;
		}
	}
}
