using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class AxisTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public string axisName = "Horizontal";

	public float axisValue = 1f;

	public float responseSpeed = 3f;

	public float returnToCentreSpeed = 3f;

	private AxisTouchButton pairedWith;

	private CrossPlatformInputManager.VirtualAxis axis;

	private void OnEnable()
	{
		axis = (CrossPlatformInputManager.VirtualAxisReference(axisName) ?? new CrossPlatformInputManager.VirtualAxis(axisName));
		FindPairedButton();
	}

	private void FindPairedButton()
	{
		AxisTouchButton[] array = UnityEngine.Object.FindObjectsOfType(typeof(AxisTouchButton)) as AxisTouchButton[];
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].axisName == axisName && array[i] != this)
			{
				pairedWith = array[i];
			}
		}
	}

	private void OnDisable()
	{
		axis.Remove();
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (pairedWith == null)
		{
			FindPairedButton();
		}
		axis.Update(Mathf.MoveTowards(axis.GetValue, axisValue, responseSpeed * Time.deltaTime));
	}

	public void OnPointerUp(PointerEventData data)
	{
		axis.Update(Mathf.MoveTowards(axis.GetValue, 0f, responseSpeed * Time.deltaTime));
	}
}
