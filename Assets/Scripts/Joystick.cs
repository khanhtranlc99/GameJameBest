using Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public enum AxisOption
	{
		Both,
		OnlyHorizontal,
		OnlyVertical
	}

	public bool DebugLog;

	public int MovementRange = 100;

	public GameObject Thumb;

	public GameObject Base;

	public bool StaticPosition;

	public AnimationCurve InputModefaer = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AxisOption axesToUse;

	public string horizontalAxisName = "Horizontal";

	public string verticalAxisName = "Vertical";

	private bool useX;

	private bool useY;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

	private Transform _thumbTransform;

	private Transform _baseTransform;

	private RectTransform thumbRectTransform;

	private RectTransform baseRectTransform;

	private Vector3 thumbStartPosition;

	private Vector3 baseStartPosition;

	private void OnEnable()
	{
		CreateVirtualAxes();
		thumbRectTransform.anchoredPosition = thumbStartPosition;
		baseRectTransform.anchoredPosition = baseStartPosition;
	}

	private void Awake()
	{
		_thumbTransform = Thumb.transform;
		_baseTransform = Base.transform;
		thumbRectTransform = Thumb.GetComponent<RectTransform>();
		thumbStartPosition = thumbRectTransform.anchoredPosition;
		baseRectTransform = Base.GetComponent<RectTransform>();
		baseStartPosition = baseRectTransform.anchoredPosition;
	}

	private void CreateVirtualAxes()
	{
		useX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
		useY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);
		if (useX)
		{
			horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName, matchToInputSettings: false);
		}
		if (useY)
		{
			verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName, matchToInputSettings: false);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		UpdateVirtualAxes(CalcInput(data));
	}

	public Vector3 CalcInput(PointerEventData data)
	{
		Vector3 position = _baseTransform.position;
		Transform thumbTransform = _thumbTransform;
		float x;
		if (useX)
		{
			Vector2 position2 = data.position;
			x = position2.x;
		}
		else
		{
			x = position.x;
		}
		float y;
		if (useY)
		{
			Vector2 position3 = data.position;
			y = position3.y;
		}
		else
		{
			y = position.y;
		}
		thumbTransform.position = new Vector3(x, y);
		Vector3 vector = _thumbTransform.localPosition - _baseTransform.localPosition;
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Clamp(vector.x, -MovementRange, MovementRange);
		zero.y = Mathf.Clamp(vector.y, -MovementRange, MovementRange);
		_thumbTransform.localPosition = Vector3.ClampMagnitude(vector, MovementRange) + _baseTransform.localPosition;
		return zero;
	}

	private void UpdateVirtualAxes(Vector3 value)
	{
		Vector3 vector = value;
		if (DebugLog && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log(vector);
		}
		vector.x /= MovementRange;
		vector.y /= MovementRange;
		vector.x = Mathf.Sign(vector.x) * InputModefaer.Evaluate(Mathf.Abs(vector.x));
		vector.y = Mathf.Sign(vector.y) * InputModefaer.Evaluate(Mathf.Abs(vector.y));
		if (useX)
		{
			horizontalVirtualAxis.Update(vector.x);
		}
		if (useY)
		{
			verticalVirtualAxis.Update(vector.y);
		}
		if (DebugLog && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log((value / MovementRange).ToString("0.000") + " -> " + vector.ToString("0.000"));
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		ResetJoyStick();
	}

	public void ResetJoyStick()
	{
		thumbRectTransform.anchoredPosition = thumbStartPosition;
		baseRectTransform.anchoredPosition = baseStartPosition;
		UpdateVirtualAxes(Vector3.zero);
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		ResetJoyStick();
	}

	public void OnPointerUp(PointerEventData data)
	{
		thumbRectTransform.anchoredPosition = thumbStartPosition;
		baseRectTransform.anchoredPosition = baseStartPosition;
		UpdateVirtualAxes(Vector3.zero);
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (!StaticPosition)
		{
			_thumbTransform.position = data.position;
			_baseTransform.position = data.position;
		}
		else
		{
			UpdateVirtualAxes(CalcInput(data));
		}
	}

	private void OnDisable()
	{
		UpdateVirtualAxes(Vector3.zero);
		thumbRectTransform.anchoredPosition = thumbStartPosition;
		baseRectTransform.anchoredPosition = baseStartPosition;
		if (useX)
		{
			horizontalVirtualAxis.Remove();
		}
		if (useY)
		{
			verticalVirtualAxis.Remove();
		}
	}
}
