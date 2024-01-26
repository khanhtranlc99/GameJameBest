using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Stick : BaseControl
	{
		public float CircleSize = 160f;

		public float HitSize = 32f;

		public Texture2D MoveControlsCircle;

		public Texture2D MoveControlsHit;

		public float Sensitivity = 1f;

		private Rect rect;

		private bool pressed;

		private Vector2 input;

		public override ControlType Type => ControlType.Stick;

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("CircleSize", CircleSize);
			dictionary.Add("HitSize", HitSize);
			dictionary.Add("Sensitivity", Sensitivity);
			if ((bool)MoveControlsCircle)
			{
				dictionary.Add("MoveControlsCircle", MoveControlsCircle.name);
			}
			if ((bool)MoveControlsHit)
			{
				dictionary.Add("MoveControlsHit", MoveControlsHit.name);
			}
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			CircleSize = Convert.ToSingle(jsonDic["CircleSize"]);
			HitSize = Convert.ToSingle(jsonDic["HitSize"]);
			Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
			if (jsonDic.ContainsKey("MoveControlsCircle"))
			{
				MoveControlsCircle = FindTexture(Convert.ToString(jsonDic["MoveControlsCircle"]));
			}
			if (jsonDic.ContainsKey("MoveControlsHit"))
			{
				MoveControlsHit = FindTexture(Convert.ToString(jsonDic["MoveControlsHit"]));
			}
		}

		public override void GameUpdate()
		{
			DetectTouches();
			input = Vector2.zero;
			if (TouchIndex == -1)
			{
				return;
			}
			SimTouch touch = touchProcessor.GetTouch(TouchIndex);
			if (touch.Status != 0)
			{
				Vector2 vector = touch.Position - touch.StartPosition;
				float magnitude = vector.magnitude;
				if (Sensitivity < 1f)
				{
					Quaternion b = Quaternion.FromToRotation(vector, Vector2.up);
					Quaternion rotation = Quaternion.Slerp(Quaternion.identity, b, 1f - Sensitivity);
					vector = rotation * vector;
				}
				if (magnitude > Mathf.Epsilon)
				{
					float num = CircleSize / 2f - HitSize / 2f;
					float d = magnitude / num;
					Vector2 a = vector * d;
					a.x = Mathf.Clamp(a.x, 0f - num, num);
					a.y = Mathf.Clamp(a.y, 0f - num, num);
					input = a / num;
				}
			}
			else
			{
				TouchIndex = -1;
			}
		}

		public override Vector2 GetInputAxis()
		{
			return input;
		}

		public override void Draw()
		{
			if (!HideGUI && TouchIndex != -1)
			{
				SimTouch touch = touchProcessor.GetTouch(TouchIndex);
				float num = (0f - CircleSize) * 0.5f;
				if ((bool)MoveControlsCircle)
				{
					Rect position = new Rect(num + touch.StartPosition.x, num + ((float)Screen.height - touch.StartPosition.y), CircleSize, CircleSize);
					GUI.DrawTexture(position, MoveControlsCircle, ScaleMode.StretchToFill);
				}
				if ((bool)MoveControlsHit)
				{
					num = (0f - HitSize) * 0.5f;
					Rect position2 = new Rect(num + touch.Position.x, num + ((float)Screen.height - touch.Position.y), HitSize, HitSize);
					GUI.DrawTexture(position2, MoveControlsHit, ScaleMode.StretchToFill);
				}
			}
		}
	}
}
