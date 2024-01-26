using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput.PlatformSpecific;

namespace UnitySampleAssets.CrossPlatformInput
{
	public static class CrossPlatformInputManager
	{
		public class VirtualAxis
		{
			private float m_Value;

			public string name
			{
				get;
				private set;
			}

			public bool matchWithInputManager
			{
				get;
				private set;
			}

			public float GetValue => m_Value;

			public float GetValueRaw => m_Value;

			public VirtualAxis(string name)
				: this(name, matchToInputSettings: true)
			{
			}

			public VirtualAxis(string name, bool matchToInputSettings)
			{
				this.name = name;
				matchWithInputManager = matchToInputSettings;
				RegisterVirtualAxis(this);
			}

			public void Remove()
			{
				UnRegisterVirtualAxis(name);
			}

			public void Update(float value)
			{
				m_Value = value;
			}
		}

		public class VirtualButton
		{
			private int lastPressedFrame = -5;

			private int releasedFrame = -5;

			private bool pressed;

			public string name
			{
				get;
				private set;
			}

			public bool matchWithInputManager
			{
				get;
				private set;
			}

			public bool GetButton => pressed;

			public bool GetButtonDown => lastPressedFrame - Time.frameCount == 0;

			public bool GetButtonUp => releasedFrame == Time.frameCount;

			public VirtualButton(string name)
				: this(name, matchToInputSettings: true)
			{
			}

			public VirtualButton(string name, bool matchToInputSettings)
			{
				this.name = name;
				matchWithInputManager = matchToInputSettings;
			}

			public void Pressed()
			{
				if (!pressed)
				{
					pressed = true;
					lastPressedFrame = Time.frameCount;
				}
			}

			public void Released()
			{
				pressed = false;
				releasedFrame = Time.frameCount;
			}

			public void Remove()
			{
				UnRegisterVirtualButton(name);
			}
		}

		private static VirtualInput virtualInput;

		public static Vector3 mousePosition => virtualInput.MousePosition();

		static CrossPlatformInputManager()
		{
#if UNITY_EDITOR
			virtualInput = new MobileInput();
			return;
#endif

			virtualInput = new MobileInput();
		}

		public static void RegisterVirtualAxis(VirtualAxis axis)
		{
			virtualInput.RegisterVirtualAxis(axis);
		}

		public static void RegisterVirtualButton(VirtualButton button)
		{
			virtualInput.RegisterVirtualButton(button);
		}

		public static void UnRegisterVirtualAxis(string _name)
		{
			if (_name == null)
			{
				throw new ArgumentNullException("_name");
			}
			virtualInput.UnRegisterVirtualAxis(_name);
		}

		public static void UnRegisterVirtualButton(string name)
		{
			virtualInput.UnRegisterVirtualButton(name);
		}

		public static VirtualAxis VirtualAxisReference(string name)
		{
			return virtualInput.VirtualAxisReference(name);
		}

		public static float GetAxis(string name)
		{
			return GetAxis(name, raw: false);
		}

		public static float GetVirtualOnlyAxis(string name, bool raw)
		{
			return virtualInput.GetVirtualOnlyAxis(name, raw);
		}

		public static float GetAxisRaw(string name)
		{
			return GetAxis(name, raw: true);
		}

		private static float GetAxis(string name, bool raw)
		{
			return virtualInput.GetAxis(name, raw);
		}

		public static bool GetButton(string name)
		{
			return virtualInput.GetButton(name);
		}

		public static bool GetButtonDown(string name)
		{
			return virtualInput.GetButtonDown(name);
		}

		public static bool GetButtonUp(string name)
		{
			return virtualInput.GetButtonUp(name);
		}

		public static void SetButtonDown(string name)
		{
			virtualInput.SetButtonDown(name);
		}

		public static void SetButtonUp(string name)
		{
			virtualInput.SetButtonUp(name);
		}

		public static void SetAxisPositive(string name)
		{
			virtualInput.SetAxisPositive(name);
		}

		public static void SetAxisNegative(string name)
		{
			virtualInput.SetAxisNegative(name);
		}

		public static void SetAxisZero(string name)
		{
			virtualInput.SetAxisZero(name);
		}

		public static void SetAxis(string name, float value)
		{
			virtualInput.SetAxis(name, value);
		}

		public static void SetVirtualMousePositionX(float f)
		{
			virtualInput.SetVirtualMousePositionX(f);
		}

		public static void SetVirtualMousePositionY(float f)
		{
			virtualInput.SetVirtualMousePositionY(f);
		}

		public static void SetVirtualMousePositionZ(float f)
		{
			virtualInput.SetVirtualMousePositionZ(f);
		}
	}
}
