using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Zoom : BaseControl
	{
		public float ZoomDelta;

		public float Sensitivity = 1f;

		public bool ReverseZoom;

		private Rect rect;

		private float lastDistance;

		public override ControlType Type => ControlType.Zoom;

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			rect = default(Rect);
			UpdateRect();
			ZoomDelta = 0f;
			Side = ControlSide.Arbitrary;
			Priority = 2;
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("Sensitivity", Sensitivity);
			dictionary.Add("ReverseZoom", ReverseZoom);
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
			ReverseZoom = Convert.ToBoolean(jsonDic["ReverseZoom"]);
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
								SimTouch touch2 = touchProcessor.GetTouch(TouchIndex);
								SimTouch touch3 = touchProcessor.GetTouch(TouchIndexAux);
								lastDistance = (touch2.Position - touch3.Position).magnitude;
							}
						}
					}
					Active = (TouchIndex != -1 && TouchIndexAux != -1);
					OperationTimer = 0f;
				}
				else
				{
					SimTouch touch4 = touchProcessor.GetTouch(TouchIndex);
					SimTouch touch5 = touchProcessor.GetTouch(TouchIndexAux);
					if (touch4.Status != 0 && touch5.Status != 0)
					{
						float num = Mathf.Lerp(lastDistance, (touch4.Position - touch5.Position).magnitude, Time.deltaTime * 10f);
						if (lastDistance > 0f)
						{
							ZoomDelta = (lastDistance - num) * 0.01f * Sensitivity;
							if (ReverseZoom)
							{
								ZoomDelta = 0f - ZoomDelta;
							}
						}
						else
						{
							ZoomDelta = 0f;
						}
						lastDistance = num;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				lastDistance = 0f;
				Active = false;
				TouchIndex = -1;
				TouchIndexAux = -1;
				ZoomDelta = 0f;
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
				GUI.Box(rect, "Zoom area");
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
