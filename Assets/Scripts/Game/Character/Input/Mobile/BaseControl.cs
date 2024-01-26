using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public abstract class BaseControl : MonoBehaviour
	{
		public Vector2 Position;

		public Vector2 Size;

		public bool PreserveTextureRatio = true;

		public ControlSide Side;

		public int DisableInputGroup = 3;

		public int TouchIndex;

		public int TouchIndexAux;

		public string InputKey0;

		public string InputKey1;

		public bool HideGUI;

		public int Priority = 1;

		public float OperationTimer;

		public bool Active;

		public bool Operating;

		public int Operations;

		protected TouchProcessor touchProcessor;

		public abstract ControlType Type
		{
			get;
		}

		public virtual void Init(TouchProcessor processor)
		{
			base.hideFlags = HideFlags.HideInInspector;
			touchProcessor = processor;
			TouchIndex = -1;
		}

		public virtual Dictionary<string, object> SerializeJSON()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("PositionX", Position.x);
			dictionary.Add("PositionY", Position.y);
			dictionary.Add("SizeX", Size.x);
			dictionary.Add("SizeY", Size.y);
			dictionary.Add("PreserveTextureRatio", PreserveTextureRatio);
			dictionary.Add("Side", (int)Side);
			dictionary.Add("Type", (int)Type);
			dictionary.Add("InputGroup", DisableInputGroup);
			dictionary.Add("TouchIndex", TouchIndex);
			dictionary.Add("TouchIndexAux", TouchIndexAux);
			dictionary.Add("InputKey0", InputKey0);
			dictionary.Add("InputKey1", InputKey1);
			dictionary.Add("HideGUI", HideGUI);
			dictionary.Add("Priority", Priority);
			return dictionary;
		}

		public virtual void DeserializeJSON(Dictionary<string, object> jsonDic)
		{
			Position.x = Convert.ToSingle(jsonDic["PositionX"]);
			Position.y = Convert.ToSingle(jsonDic["PositionY"]);
			Size.x = Convert.ToSingle(jsonDic["SizeX"]);
			Size.y = Convert.ToSingle(jsonDic["SizeY"]);
			PreserveTextureRatio = Convert.ToBoolean(jsonDic["PreserveTextureRatio"]);
			Side = (ControlSide)Convert.ToInt32(jsonDic["Side"]);
			DisableInputGroup = Convert.ToInt32(jsonDic["InputGroup"]);
			TouchIndex = Convert.ToInt32(jsonDic["TouchIndex"]);
			TouchIndexAux = Convert.ToInt32(jsonDic["TouchIndexAux"]);
			InputKey0 = Convert.ToString(jsonDic["InputKey0"]);
			InputKey1 = Convert.ToString(jsonDic["InputKey1"]);
			HideGUI = Convert.ToBoolean(jsonDic["HideGUI"]);
			Priority = Convert.ToInt32(jsonDic["Priority"]);
		}

		public Texture2D FindTexture(string name)
		{
			Texture2D texture2D = Resources.Load<Texture2D>("MobileResources/" + name);
			if ((bool)texture2D)
			{
				return texture2D;
			}
			return null;
		}

		public virtual void GameUpdate()
		{
		}

		public abstract void Draw();

		public virtual Vector2 GetInputAxis()
		{
			return Vector2.zero;
		}

		public virtual bool AbortUpdateOtherControls()
		{
			return false;
		}

		protected virtual void DetectTouches()
		{
			int activeTouchCount = touchProcessor.GetActiveTouchCount();
			if (activeTouchCount == 0)
			{
				TouchIndex = -1;
			}
			else
			{
				if (TouchIndex != -1)
				{
					return;
				}
				for (int i = 0; i < activeTouchCount; i++)
				{
					SimTouch touch = touchProcessor.GetTouch(i);
					if (touch.Status != 0 && IsSide(touch.StartPosition) && TouchIndex == -1)
					{
						TouchIndex = i;
					}
				}
			}
		}

		protected bool IsSide(Vector2 pos)
		{
			if (Side == ControlSide.Arbitrary)
			{
				return true;
			}
			if (pos.x < (float)Screen.width * 0.5f)
			{
				return Side == ControlSide.Left;
			}
			return Side == ControlSide.Right;
		}

		protected virtual void OnTouchDown()
		{
			if (!Application.isEditor || Application.isPlaying)
			{
				InputManager.Instance.EnableInputGroup((InputGroup)DisableInputGroup, status: false);
			}
		}

		protected virtual void OnTouchUp()
		{
			if (!Application.isEditor || Application.isPlaying)
			{
				InputManager.Instance.EnableInputGroup((InputGroup)DisableInputGroup, status: true);
			}
		}
	}
}
