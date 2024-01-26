using System;
using UnityEngine;

namespace Game.Character.Input
{
	[Serializable]
	public class OrbitInput : GameInput
	{
		public override InputPreset PresetType => InputPreset.Orbit;

		public override void UpdateInput(Input[] inputs)
		{
			mouseFilter.AddSample(UnityEngine.Input.mousePosition);
			if (InputManager.Instance.MobileInput)
			{
				Vector2 pan = InputWrapper.GetPan("Pan");
				if (pan.sqrMagnitude > Mathf.Epsilon)
				{
					SetInput(inputs, InputType.Pan, pan);
					return;
				}
				float zoom = InputWrapper.GetZoom("Zoom");
				if (Mathf.Abs(zoom) > Mathf.Epsilon)
				{
					SetInput(inputs, InputType.Zoom, zoom);
				}
				float rotation = InputWrapper.GetRotation("Rotate");
				if (Mathf.Abs(rotation) > Mathf.Epsilon)
				{
					SetInput(inputs, InputType.Rotate, new Vector2(rotation, 0f));
					return;
				}
				Vector2 vector = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
				if (vector.sqrMagnitude > Mathf.Epsilon)
				{
					SetInput(inputs, InputType.Rotate, new Vector2(vector.x, vector.y));
				}
				return;
			}
			Vector2 vector2;
			if (InputManager.Instance.FilterInput)
			{
				vector2 = mouseFilter.GetValue();
			}
			else
			{
				Vector3 mousePosition = UnityEngine.Input.mousePosition;
				float x = mousePosition.x;
				Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
				vector2 = new Vector2(x, mousePosition2.y);
			}
			Vector2 vector3 = vector2;
			if (InputWrapper.GetButton("Pan"))
			{
				SetInput(inputs, InputType.Pan, vector3);
			}
			float axis = InputWrapper.GetAxis("Mouse ScrollWheel");
			if (Mathf.Abs(axis) > Mathf.Epsilon)
			{
				SetInput(inputs, InputType.Zoom, axis);
			}
			Vector2 vector4 = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
			if (vector4.sqrMagnitude > Mathf.Epsilon)
			{
				SetInput(inputs, InputType.Rotate, new Vector2(vector4.x, vector4.y));
			}
			if (UnityEngine.Input.GetMouseButton(1))
			{
				SetInput(inputs, InputType.Rotate, new Vector2(InputWrapper.GetAxis("Mouse X"), InputWrapper.GetAxis("Mouse Y")));
			}
			SetInput(inputs, InputType.Reset, UnityEngine.Input.GetKey(KeyCode.R));
			doubleClickTimeout += Time.deltaTime;
			if (UnityEngine.Input.GetMouseButtonDown(2))
			{
				if (doubleClickTimeout < InputManager.DoubleClickTimeout)
				{
					SetInput(inputs, InputType.Reset, true);
				}
				doubleClickTimeout = 0f;
			}
			float axis2 = InputWrapper.GetAxis("Horizontal");
			float axis3 = InputWrapper.GetAxis("Vertical");
			Vector2 sample = new Vector2(axis2, axis3);
			padFilter.AddSample(sample);
			SetInput(inputs, InputType.Move, padFilter.GetValue());
		}
	}
}
