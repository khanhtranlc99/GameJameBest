using UnityEngine;

namespace Game.Character.Utils
{
	public class CursorLocking : MonoBehaviour
	{
		public bool LockCursor;

		public KeyCode LockKey;

		public KeyCode UnlockKey;

		public static bool IsLocked;

		private static CursorLocking instance;

		private void Awake()
		{
			instance = this;
		}

		private void Update()
		{
			if (LockCursor)
			{
				Lock();
			}
			else
			{
				Unlock();
			}
			IsLocked = Compatibility.IsCursorLocked();
			if (UnityEngine.Input.GetKeyDown(LockKey))
			{
				Lock();
			}
			if (UnityEngine.Input.GetKeyDown(UnlockKey))
			{
				Unlock();
			}
			if (!Compatibility.IsCursorLocked())
			{
				Compatibility.SetCursorVisible(status: true);
			}
		}

		public static void Lock()
		{
			Compatibility.LockCursor(status: true);
			Compatibility.SetCursorVisible(status: false);
			instance.LockCursor = true;
		}

		public static void Unlock()
		{
			Compatibility.LockCursor(status: false);
			Compatibility.SetCursorVisible(status: true);
			instance.LockCursor = false;
		}
	}
}
