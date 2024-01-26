using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class Pan : BaseControl
	{
		public Vector2 PanPosition;

		public float Sensitivity = 1f;

		public bool DoublePan = true;

		private Rect rect;

		private Vector2 offset;

		private Vector2 start0;

		private Vector2 start1;

		private float negTimeout;

		public override ControlType Type => ControlType.Pan;

		public override void Init(TouchProcessor processor)
		{
			base.Init(processor);
			rect = default(Rect);
			UpdateRect();
			Side = ControlSide.Arbitrary;
			Priority = 2;
		}

		public override Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = base.SerializeJSON();
			dictionary.Add("Sensitivity", Sensitivity);
			dictionary.Add("DoublePan", DoublePan);
			return dictionary;
		}

		public override void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			base.DeserializeJSON(jsonDic);
			Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
			DoublePan = Convert.ToBoolean(jsonDic["DoublePan"]);
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
			if (activeTouchCount > 0)
			{
				SimTouch touch = touchProcessor.GetTouch(0);
				SimTouch touch2 = touchProcessor.GetTouch(1);
				if (touch.Status == TouchStatus.Start)
				{
					if (ContainPoint(touch.StartPosition))
					{
						TouchIndex = 0;
						offset = Vector2.zero;
						start0 = touch.Position;
						start1 = touch2.Position;
					}
				}
				else if (touch.Status == TouchStatus.End)
				{
					TouchIndex = -1;
					if (TouchIndexAux != -1)
					{
						offset = -touch2.Position + touch.Position;
					}
				}
				if (touch2.Status == TouchStatus.Start)
				{
					if (ContainPoint(touch.StartPosition))
					{
						TouchIndexAux = 1;
						start0 = touch.Position;
						start1 = touch2.Position;
					}
				}
				else if (touch2.Status == TouchStatus.End)
				{
					TouchIndexAux = -1;
				}
				Active = (TouchIndex != -1 || TouchIndexAux != -1);
				if (Active)
				{
					SimTouch touch3 = touchProcessor.GetTouch((TouchIndex == -1) ? TouchIndexAux : TouchIndex);
					Vector2 vector = touch3.Position + offset;
					Operating = ((vector - PanPosition).sqrMagnitude > Mathf.Epsilon);
					if (TouchIndex != -1 && TouchIndexAux != -1)
					{
						if (DoublePan)
						{
							if (negTimeout > 0f)
							{
								start0 = touch.Position;
								start1 = touch2.Position;
								offset = Vector2.zero;
							}
							else
							{
								start0 = Vector2.Lerp(start0, touch.Position, Time.deltaTime * 10f);
								start1 = Vector2.Lerp(start1, touch2.Position, Time.deltaTime * 10f);
								if (offset.sqrMagnitude > Mathf.Epsilon)
								{
									offset = -start1 + start0;
								}
							}
							Vector2 normalized = (touch.Position - start0).normalized;
							Vector2 normalized2 = (touch2.Position - start1).normalized;
							float num = Vector2.Dot(normalized, normalized2);
							if (num < 0.8f)
							{
								Operating = false;
							}
							if (num < 0f)
							{
								negTimeout = 1f;
							}
							if (Operations == 0)
							{
								start0 = touch.Position;
								start1 = touch2.Position;
								offset = Vector2.zero;
							}
						}
						else
						{
							Operations = 0;
						}
					}
					else
					{
						Operations = 4;
					}
					PanPosition = vector;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				Active = false;
				Operating = false;
				TouchIndex = -1;
				TouchIndexAux = -1;
				PanPosition = Vector2.zero;
				offset = Vector2.zero;
			}
		}

		public override void GameUpdate()
		{
			if (Active)
			{
				OperationTimer += Time.deltaTime;
				if (Operating)
				{
					Operations++;
					if (Operations > 20)
					{
						Operations = 20;
					}
				}
				if (OperationTimer > 0.1f)
				{
					OperationTimer = 0f;
					Operations--;
					if (Operations < 0)
					{
						Operations = 0;
					}
				}
				negTimeout -= Time.deltaTime;
				if (negTimeout > 0f)
				{
					Operations = 0;
				}
			}
			else
			{
				OperationTimer = 0f;
				Operations = 0;
			}
			DetectTouches();
		}

		public override void Draw()
		{
			UpdateRect();
			if (!HideGUI)
			{
				GUI.Box(rect, "Pan area");
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
