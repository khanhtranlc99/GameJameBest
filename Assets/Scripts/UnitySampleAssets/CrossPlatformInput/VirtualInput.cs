using System.Collections.Generic;
using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput
{
	public abstract class VirtualInput
	{
		protected Dictionary<string, CrossPlatformInputManager.VirtualAxis> virtualAxes = new Dictionary<string, CrossPlatformInputManager.VirtualAxis>();

		protected Dictionary<string, CrossPlatformInputManager.VirtualButton> virtualButtons = new Dictionary<string, CrossPlatformInputManager.VirtualButton>();

		protected List<string> alwaysUseVirtual = new List<string>();

		public Vector3 virtualMousePosition
		{
			get;
			private set;
		}

		public void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			if (virtualAxes.ContainsKey(axis.name))
			{
				UnityEngine.Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
				return;
			}
			virtualAxes.Add(axis.name, axis);
			if (!axis.matchWithInputManager)
			{
				alwaysUseVirtual.Add(axis.name);
			}
		}

		public void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			if (virtualButtons.ContainsKey(button.name))
			{
				UnityEngine.Debug.LogError("There is already a virtual button named " + button.name + " registered.");
				return;
			}
			virtualButtons.Add(button.name, button);
			if (!button.matchWithInputManager)
			{
				alwaysUseVirtual.Add(button.name);
			}
		}

		public void UnRegisterVirtualAxis(string name)
		{
			if (virtualAxes.ContainsKey(name))
			{
				virtualAxes.Remove(name);
			}
		}

		public void UnRegisterVirtualButton(string name)
		{
			if (virtualButtons.ContainsKey(name))
			{
				virtualButtons.Remove(name);
			}
		}

		public CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			return (!virtualAxes.ContainsKey(name)) ? null : virtualAxes[name];
		}

		public void SetVirtualMousePositionX(float f)
		{
			Vector3 virtualMousePosition = this.virtualMousePosition;
			float y = virtualMousePosition.y;
			Vector3 virtualMousePosition2 = this.virtualMousePosition;
			this.virtualMousePosition = new Vector3(f, y, virtualMousePosition2.z);
		}

		public void SetVirtualMousePositionY(float f)
		{
			Vector3 virtualMousePosition = this.virtualMousePosition;
			float x = virtualMousePosition.x;
			Vector3 virtualMousePosition2 = this.virtualMousePosition;
			this.virtualMousePosition = new Vector3(x, f, virtualMousePosition2.z);
		}

		public void SetVirtualMousePositionZ(float f)
		{
			Vector3 virtualMousePosition = this.virtualMousePosition;
			float x = virtualMousePosition.x;
			Vector3 virtualMousePosition2 = this.virtualMousePosition;
			this.virtualMousePosition = new Vector3(x, virtualMousePosition2.y, f);
		}

		public abstract float GetAxis(string name, bool raw);

		public abstract float GetVirtualOnlyAxis(string name, bool raw);

		public abstract bool GetButton(string name);

		public abstract bool GetButtonDown(string name);

		public abstract bool GetButtonUp(string name);

		public abstract void SetButtonDown(string name);

		public abstract void SetButtonUp(string name);

		public abstract void SetAxisPositive(string name);

		public abstract void SetAxisNegative(string name);

		public abstract void SetAxisZero(string name);

		public abstract void SetAxis(string name, float value);

		public abstract Vector3 MousePosition();
	}
}
