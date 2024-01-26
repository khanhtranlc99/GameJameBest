using System.Diagnostics;
using UnityEngine;

namespace Game.Character.Utils
{
	public static class Debug
	{
		[Conditional("UNITY_EDITOR")]
		public static void Assert(bool condition, string message = "")
		{
			if (!condition)
			{
				UnityEngine.Debug.LogError("Assert! " + message);
				UnityEngine.Debug.Break();
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void Log(string format, params object[] args)
		{
			UnityEngine.Debug.Log(string.Format(format, args));
		}

		[Conditional("UNITY_EDITOR")]
		public static void Log(object arg)
		{
			UnityEngine.Debug.Log(arg.ToString());
		}

		public static Vector3 GetCentroid(GameObject obj)
		{
			MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
			Vector3 a = Vector3.zero;
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				SkinnedMeshRenderer componentInChildren = obj.GetComponentInChildren<SkinnedMeshRenderer>();
				if ((bool)componentInChildren)
				{
					return componentInChildren.bounds.center;
				}
				return obj.transform.position;
			}
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer meshRenderer in array)
			{
				a += meshRenderer.bounds.center;
			}
			return a / componentsInChildren.Length;
		}

		public static void SetVisible(GameObject obj, bool status, bool includeInactive)
		{
			if ((bool)obj)
			{
				MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>(includeInactive);
				MeshRenderer[] array = componentsInChildren;
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = status;
				}
				SkinnedMeshRenderer[] componentsInChildren2 = obj.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
				SkinnedMeshRenderer[] array2 = componentsInChildren2;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
				{
					skinnedMeshRenderer.enabled = status;
				}
			}
		}

		public static void ClearLog()
		{
		}

		public static bool IsActive(GameObject obj)
		{
			return (bool)obj && obj.activeSelf;
		}

		public static void SetActive(GameObject obj, bool status)
		{
			if ((bool)obj)
			{
				obj.SetActive(status);
			}
		}

		public static void SetActiveRecursively(GameObject obj, bool status)
		{
			if ((bool)obj)
			{
				int childCount = obj.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					SetActiveRecursively(obj.transform.GetChild(i).gameObject, status);
				}
				obj.SetActive(status);
			}
		}

		public static void EnableCollider(GameObject obj, bool status)
		{
			if ((bool)obj)
			{
				Collider[] componentsInChildren = obj.GetComponentsInChildren<Collider>();
				Collider[] array = componentsInChildren;
				foreach (Collider collider in array)
				{
					collider.enabled = status;
				}
			}
		}

		public static void Destroy(Object obj, bool allowDestroyingAssets)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(obj);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(obj, allowDestroyingAssets);
			}
		}
	}
}
