using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public enum AxisOption
	{
		Both,
		OnlyHorizontal,
		OnlyVertical
	}

	public enum ControlStyle
	{
		Absolute,
		Relative,
		Swipe,
		Look
	}

	public AxisOption AxesToUse;

	public float Sensitivity = 1f;

	public ControlStyle ControlType = ControlStyle.Look;

	public string HorizontalAxisName = "Horizontal";

	public string VerticalAxisName = "Vertical";

	private bool useX;

	private bool useY;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

	private bool dragging;

	private PointerEventData pointerData;

	private Vector3 onDownPosition;

	private Vector3 previousTouchPos;

	private Vector3 pointerDelta;

	private float maxScreenSide;

	private Vector3 lastInput;

	private void OnEnable()
	{
		CreateVirtualAxes();
		maxScreenSide = Mathf.Max(Screen.width, Screen.height);
	}

	private void OnDisable()
	{
		if (useX)
		{
			horizontalVirtualAxis.Remove();
		}
		if (useY)
		{
			verticalVirtualAxis.Remove();
		}
	}

	private void CreateVirtualAxes()
	{
		useX = (AxesToUse == AxisOption.Both || AxesToUse == AxisOption.OnlyHorizontal);
		useY = (AxesToUse == AxisOption.Both || AxesToUse == AxisOption.OnlyVertical);
		if (useX)
		{
			horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(HorizontalAxisName);
		}
		if (useY)
		{
			verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(VerticalAxisName);
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		dragging = true;
		pointerData = data;
		if (ControlType != 0)
		{
			onDownPosition = data.position;
			previousTouchPos = data.position;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		dragging = false;
		pointerData = null;
		UpdateVirtualAxes(Vector3.zero);
	}

	private void Update()
	{
		if (dragging)
		{
			Vector3 inputPosition = GetInputPosition();
			switch (ControlType)
			{
			case ControlStyle.Swipe:
				onDownPosition = previousTouchPos;
				pointerDelta = (inputPosition - onDownPosition).normalized;
				break;
			case ControlStyle.Look:
				onDownPosition = previousTouchPos;
				pointerDelta = (inputPosition - onDownPosition) / maxScreenSide;
				break;
			default:
				pointerDelta = (inputPosition - onDownPosition).normalized;
				break;
			}
			previousTouchPos = inputPosition;
			UpdateVirtualAxes(pointerDelta * Sensitivity);
		}
	}

	private Vector3 GetInputPosition()
	{
		if (pointerData != null)
		{
			lastInput = pointerData.position;
		}
		return lastInput;
	}

	private void UpdateVirtualAxes(Vector3 value)
	{
		if (useX)
		{
			horizontalVirtualAxis.Update(value.x);
		}
		if (useY)
		{
			verticalVirtualAxis.Update(value.y);
		}
	}
}
