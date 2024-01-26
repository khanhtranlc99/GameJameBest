using UnityEngine;

namespace Game.Character.Utils
{
	public static class Compatibility
	{
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

		public static void SetCursorVisible(bool status)
		{
			Cursor.visible = status;
		}

		public static void LockCursor(bool status)
		{
			Cursor.lockState = (status ? CursorLockMode.Locked : CursorLockMode.Confined);
		}

		public static bool IsCursorLocked()
		{
			return Cursor.lockState == CursorLockMode.Locked;
		}
	}
}
