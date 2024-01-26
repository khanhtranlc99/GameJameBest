using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class JoyPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
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

	public int MovementRange = 100;

	public GameObject Thumb;

	public GameObject Base;

	public AxisOption AxesToUse;

	public string HorizontalAxisName = "Horizontal";

	public string VerticalAxisName = "Vertical";

	private bool useX;

	private bool useY;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

	private Transform thumbTransform;

	private Transform baseTransform;

	private Vector3 onDownPosition;

	public ControlStyle PadControlStyle = ControlStyle.Swipe;

	private Vector3 pointerDelta;

	private Vector3 previousTouchPos;

	private RectTransform thumbRectTransform;

	private RectTransform baseRectTransform;

	private Vector3 thumbStartPosition;

	private Vector3 baseStartPosition;

	public bool UseJoy = true;

	[Range(0f, 1f)]
	public float JoySensitivity = 1f;

	[Range(0f, 1f)]
	[Tooltip("`Радиус`, внутри которого джойстик не работает\n0\t- везде работает\n0.5\t- работает от половины MovementRange до полного\n1\t- не работает")]
	public float JoySensitivityLimit;

	public float Multipler = 1f;

	public bool UsePad = true;

	private bool dragging;

	private int id = -1;

	private float maxScreenSide;

	public void ResetJoypadPosition()
	{
		InitializeComponents();
		thumbRectTransform.anchoredPosition = thumbStartPosition;
		baseRectTransform.anchoredPosition = baseStartPosition;
	}

	private void InitializeComponents()
	{
		if (!thumbRectTransform || !baseRectTransform)
		{
			thumbTransform = Thumb.transform;
			baseTransform = Base.transform;
			thumbRectTransform = Thumb.GetComponent<RectTransform>();
			thumbStartPosition = thumbRectTransform.anchoredPosition;
			baseRectTransform = Base.GetComponent<RectTransform>();
			baseStartPosition = baseRectTransform.anchoredPosition;
		}
	}

	private void OnEnable()
	{
		CreateVirtualAxes();
		maxScreenSide = Mathf.Max(Screen.width, Screen.height);
	}

	private void OnDisable()
	{
		OnPointerUp(null);
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
			horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(HorizontalAxisName, matchToInputSettings: false);
		}
		if (useY)
		{
			verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(VerticalAxisName, matchToInputSettings: false);
		}
	}

	public void Awake()
	{
		InitializeComponents();
	}

	public void Start()
	{
		if (onDownPosition.Equals(Vector3.zero))
		{
			onDownPosition = thumbTransform.position;
			previousTouchPos = thumbTransform.position;
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		thumbTransform.position = data.position;
		baseTransform.position = data.position;
		dragging = true;
		id = data.pointerId;
		if (PadControlStyle != 0)
		{
			onDownPosition = data.position;
			previousTouchPos = data.position;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		ResetJoyPad();
	}

	public void ResetJoyPad()
	{
		thumbRectTransform.anchoredPosition = thumbStartPosition;
		baseRectTransform.anchoredPosition = baseStartPosition;
		dragging = false;
		id = -1;
		UpdateVirtualAxes(Vector3.zero);
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		ResetJoyPad();
	}

	private void Update()
	{
		if (!dragging || (!UseJoy && !UsePad))
		{
			return;
		}
		Vector3 inputPosition = GetInputPosition();
		Transform transform = thumbTransform;
		float x;
		if (useX)
		{
			x = inputPosition.x;
		}
		else
		{
			Vector3 position = baseTransform.position;
			x = position.x;
		}
		float y;
		if (useY)
		{
			y = inputPosition.y;
		}
		else
		{
			Vector3 position2 = baseTransform.position;
			y = position2.y;
		}
		transform.position = new Vector3(x, y);
		Vector3 vector = thumbTransform.localPosition - baseTransform.localPosition;
		Vector3 localPosition = Vector3.ClampMagnitude(-vector, MovementRange) + thumbTransform.localPosition;
		localPosition.x = 0f - Mathf.Clamp(0f - localPosition.x, MovementRange, Screen.width - MovementRange);
		localPosition.y = Mathf.Clamp(localPosition.y, MovementRange, Screen.height - MovementRange);
		baseTransform.localPosition = localPosition;
		if (UsePad)
		{
			switch (PadControlStyle)
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
			if (UseJoy && pointerDelta.Equals(Vector3.zero))
			{
				pointerDelta = GetJoyValue(vector) * JoySensitivity;
			}
		}
		else
		{
			pointerDelta = GetJoyValue(vector) * JoySensitivity;
		}
		UpdateVirtualAxes(pointerDelta * Multipler);
	}

	private Vector3 GetJoyValue(Vector3 localThumbPos)
	{
		Vector3 zero = Vector3.zero;
		zero.x = ClampTwoSided(localThumbPos.x, MovementRange, JoySensitivityLimit) / (float)MovementRange;
		zero.y = ClampTwoSided(localThumbPos.y, MovementRange, JoySensitivityLimit) / (float)MovementRange;
		return zero;
	}

	private float ClampTwoSided(float subject, float maxVal, float minLimit)
	{
		if (maxVal <= 0f || minLimit < 0f || minLimit >= 1f)
		{
			return 0f;
		}
		float num = Mathf.Sign(subject);
		float value = num * subject - maxVal * minLimit;
		float num2 = 1f - minLimit;
		return num * Mathf.Clamp(value, 0f, maxVal * num2) / num2;
	}

	private Vector3 GetInputPosition()
	{
		Vector3 result = Vector3.zero;
		if (Input.touchCount >= id && id != -1 && id >=0)
		{
			result = Input.touches[id].position;
		}
		return result;
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
