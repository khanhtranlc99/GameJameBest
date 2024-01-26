using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Button : BaseControl
	{
		public enum ButtonState
		{
			Pressed,
			Begin,
			End,
			None
		}

		public bool Toggle;

		public bool HoldDrag;

		public bool InvalidateOnDrag;

		public float HoldTimeout = 0.3f;

		public Texture2D TextureDefault;

		public Texture2D TexturePressed;

		public ButtonState State;

		private Rect rect;

		private bool pressed;

		private Vector2 startTouch;

		public override ControlType Type => ControlType.Button;

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			rect = default(Rect);
			UpdateRect();
			State = ButtonState.None;
			Side = ControlSide.Arbitrary;
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("Toggle", Toggle);
			dictionary.Add("HoldDrag", HoldDrag);
			dictionary.Add("HoldTimeout", HoldTimeout);
			dictionary.Add("InvalidateOnDrag", InvalidateOnDrag);
			if ((bool)TextureDefault)
			{
				dictionary.Add("TextureDefault", TextureDefault.name);
			}
			if ((bool)TextureDefault)
			{
				dictionary.Add("TexturePressed", TexturePressed.name);
			}
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			Toggle = Convert.ToBoolean(jsonDic["Toggle"]);
			HoldDrag = Convert.ToBoolean(jsonDic["HoldDrag"]);
			HoldTimeout = Convert.ToSingle(jsonDic["HoldTimeout"]);
			if (jsonDic.ContainsKey("InvalidateOnDrag"))
			{
				InvalidateOnDrag = Convert.ToBoolean(jsonDic["InvalidateOnDrag"]);
			}
			if (jsonDic.ContainsKey("TextureDefault"))
			{
				TextureDefault = FindTexture(Convert.ToString(jsonDic["TextureDefault"]));
			}
			if (jsonDic.ContainsKey("TexturePressed"))
			{
				TexturePressed = FindTexture(Convert.ToString(jsonDic["TexturePressed"]));
			}
		}

		public bool ContainPoint(Vector2 point)
		{
			point.y = (float)Screen.height - point.y;
			return rect.Contains(point);
		}

		public void Press()
		{
			if (Toggle)
			{
				pressed = !pressed;
			}
			else
			{
				pressed = true;
			}
			OnTouchDown();
		}

		public bool IsPressed()
		{
			return pressed;
		}

		public void Reset()
		{
			pressed = false;
			OnTouchUp();
		}

		private void CheckForMove(Vector2 touch)
		{
			if (InvalidateOnDrag && (touch - startTouch).sqrMagnitude > 10f)
			{
				State = ButtonState.None;
				pressed = false;
			}
		}

		protected override void DetectTouches()
		{
			int activeTouchCount = touchProcessor.GetActiveTouchCount();
			bool flag = false;
			if (activeTouchCount > 0)
			{
				for (int i = 0; i < activeTouchCount; i++)
				{
					SimTouch touch = touchProcessor.GetTouch(i);
					if (ContainPoint(touch.StartPosition))
					{
						TouchStatus status = touch.Status;
						if (status == TouchStatus.Start)
						{
							Press();
							State = ButtonState.Begin;
							startTouch = touch.StartPosition;
							TouchIndex = i;
						}
					}
					if (TouchIndex == i)
					{
						switch (touch.Status)
						{
						case TouchStatus.Stationary:
						case TouchStatus.Moving:
							State = ButtonState.Pressed;
							CheckForMove(touch.Position);
							break;
						case TouchStatus.End:
							State = ButtonState.End;
							CheckForMove(touch.Position);
							flag = true;
							break;
						case TouchStatus.Invalid:
							flag = true;
							break;
						}
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				if (TouchIndex == -1)
				{
					State = ButtonState.None;
				}
				else if (!HoldDrag && IsHoldDrag())
				{
					State = ButtonState.None;
				}
				TouchIndex = -1;
				if (!Toggle)
				{
					Reset();
				}
			}
		}

		public override void GameUpdate()
		{
			DetectTouches();
		}

		public override void Draw()
		{
			UpdateRect();
			if (!HideGUI)
			{
				Texture2D texture2D = (!pressed) ? TextureDefault : TexturePressed;
				if ((bool)texture2D)
				{
					GUI.DrawTexture(rect, texture2D);
				}
			}
		}

		public void UpdateRect()
		{
			rect.x = Position.x * (float)Screen.width;
			rect.y = Position.y * (float)Screen.height;
			rect.width = Size.x * (float)Screen.width;
			rect.height = Size.y * (float)Screen.height;
		}

		private bool IsHoldDrag()
		{
			if (TouchIndex != -1)
			{
				SimTouch touch = touchProcessor.GetTouch(TouchIndex);
				return touch.PressTime > HoldTimeout;
			}
			return false;
		}
	}
}
