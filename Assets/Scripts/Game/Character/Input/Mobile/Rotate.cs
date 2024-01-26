using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Rotate : BaseControl
	{
		public float RotateAngle;

		public float Sensitivity = 1f;

		private Rect rect;

		private Vector3 lastVector;

		public override ControlType Type => ControlType.Rotate;

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			rect = default(Rect);
			UpdateRect();
			RotateAngle = 0f;
			Side = ControlSide.Arbitrary;
			Priority = 2;
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("Sensitivity", Sensitivity);
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
		}

		public bool ContainPoint(Vector2 point)
		{
			point.y = (float)Screen.height - point.y;
			return rect.Contains(point);
		}

		public override bool AbortUpdateOtherControls()
		{
			return false;
		}

		protected override void DetectTouches()
		{
			int activeTouchCount = touchProcessor.GetActiveTouchCount();
			bool flag = false;
			if (activeTouchCount > 1)
			{
				if (!Active)
				{
					for (int i = 0; i < activeTouchCount; i++)
					{
						SimTouch touch = touchProcessor.GetTouch(i);
						if (ContainPoint(touch.StartPosition) && touch.Status != 0)
						{
							if (TouchIndex == -1)
							{
								TouchIndex = i;
							}
							else if (TouchIndexAux == -1)
							{
								TouchIndexAux = i;
							}
						}
					}
					Active = (TouchIndex != -1 && TouchIndexAux != -1);
				}
				else
				{
					SimTouch touch2 = touchProcessor.GetTouch(TouchIndex);
					SimTouch touch3 = touchProcessor.GetTouch(TouchIndexAux);
					if (touch2.Status != 0 && touch3.Status != 0)
					{
						Vector2 normalized = (touch3.Position - touch2.Position).normalized;
						Vector3 vector = lastVector;
						float num = 5f;
						if (lastVector.x == float.MaxValue)
						{
							vector = normalized;
							num = float.MaxValue;
						}
						float num2 = (Mathf.Atan2(normalized.y, normalized.x) - Mathf.Atan2(vector.y, vector.x)) * 20f * Sensitivity;
						RotateAngle = Mathf.Lerp(RotateAngle, num2, Time.deltaTime * 2f);
						if (num == float.MaxValue)
						{
							RotateAngle = num2;
						}
						RotateAngle = num2;
						lastVector = normalized;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				lastVector.x = float.MaxValue;
				Active = false;
				TouchIndex = -1;
				TouchIndexAux = -1;
				RotateAngle = 0f;
			}
		}

		public override void GameUpdate()
		{
			base.GameUpdate();
			DetectTouches();
		}

		public override void Draw()
		{
			UpdateRect();
			if (!HideGUI)
			{
				GUI.Box(rect, "Rotate area");
			}
		}

		public void UpdateRect()
		{
			rect.x = Position.x * (float)Screen.width;
			rect.y = Position.y * (float)Screen.height;
			rect.width = Size.x * (float)Screen.width;
			rect.height = Size.y * (float)Screen.height;
		}
	}
}
