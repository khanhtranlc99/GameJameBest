using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class Wheel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEventSystemHandler
{
	public int MovementRange = 100;

	public GameObject WheelImage;

	public float MaxMinRotationAngle = 540f;

	public float WheelMoveSmoothness = 1f;

	public string horizontalAxisName = "Horizontal";

	private Vector3 _startPos;

	private Vector3 _tempWheelPos;

	private Vector3 _wheelStartPos;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private float _wheelRadius;

	private bool _isSteering;

	private float _currWheelAngle;

	private float _angleOld;

	private float _angleNew;

	private RectTransform wheelTransform;

	private void OnEnable()
	{
		CreateVirtualAxes();
		_isSteering = false;
	}

	public void Start()
	{
		wheelTransform = WheelImage.GetComponent<RectTransform>();
		_startPos = wheelTransform.position;
		_wheelStartPos = wheelTransform.position;
		Rect rect = wheelTransform.rect;
		_wheelRadius = Mathf.Min(rect.height, rect.height) * 0.5f;
		_wheelRadius *= ScaleRatio(wheelTransform);
	}

	private float ScaleRatio(RectTransform rectTransform)
	{
		Vector3 position = rectTransform.position;
		Vector3 localPosition = rectTransform.localPosition;
		rectTransform.position += Vector3.one;
		float result = Vector3.one.magnitude / (rectTransform.localPosition - localPosition).magnitude;
		rectTransform.position = position;
		return result;
	}

	private void UpdateVirtualAxes(float value)
	{
		float value2 = value / MaxMinRotationAngle;
		horizontalVirtualAxis.Update(value2);
	}

	private void CreateVirtualAxes()
	{
		horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
	}

	public void OnDrag(PointerEventData data)
	{
		Vector3 normalized = ((Vector3)(data.position - new Vector2(_tempWheelPos.x, _tempWheelPos.y))).normalized;
		float num = Mathf.Atan(normalized.y / normalized.x) * 180f / (float)Math.PI;
		if (!_isSteering)
		{
			_isSteering = true;
			_angleNew = num;
			_angleOld = num;
		}
		_angleOld = _angleNew;
		_angleNew = num;
		float num2 = _angleNew - _angleOld;
		if (Mathf.Abs(num2) > 90f)
		{
			num2 = 0f;
		}
		_currWheelAngle += num2;
		_currWheelAngle = Mathf.Clamp(_currWheelAngle, 0f - MaxMinRotationAngle, MaxMinRotationAngle);
		UpdateVirtualAxes(_currWheelAngle);
	}

	private void TurnWheel()
	{
		if (!_isSteering && _currWheelAngle != 0f)
		{
			_currWheelAngle = Mathf.Lerp(_currWheelAngle, 0f, Time.deltaTime * WheelMoveSmoothness);
			UpdateVirtualAxes(_currWheelAngle);
		}
		wheelTransform.rotation = Quaternion.Euler(0f, 0f, _currWheelAngle);
	}

	public void OnPointerUp(PointerEventData data)
	{
		_isSteering = false;
		base.transform.position = _startPos;
		wheelTransform.position = _wheelStartPos;
	}

	public void OnPointerDown(PointerEventData data)
	{
		Vector2 b = new Vector2(_startPos.x, _startPos.y);
		Vector2 b2 = -(data.position - b).normalized * _wheelRadius;
		_tempWheelPos = data.position + b2;
		wheelTransform.position = _tempWheelPos;
		OnDrag(data);
	}

	public void FixedUpdate()
	{
		TurnWheel();
	}

	private void OnDisable()
	{
		horizontalVirtualAxis.Remove();
	}
}
