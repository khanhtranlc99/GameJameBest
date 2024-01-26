using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

public class ControlButtonManager : MonoBehaviour
{
	public string AxisName = "Vertical";

	private CrossPlatformInputManager.VirtualAxis VirtualAxis;

	private void OnEnable()
	{
		VirtualAxis = new CrossPlatformInputManager.VirtualAxis(AxisName);
	}

	public void SetAxis(float value)
	{
		VirtualAxis.Update(value);
	}

	private void OnDisable()
	{
		VirtualAxis.Remove();
	}
}
