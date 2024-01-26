using Game.Character.Utils;
using Game.GlobalComponent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	[ExecuteInEditMode]
	public class MobileControls : MonoBehaviour
	{
		public enum Layout
		{
			Empty,
			FPS,
			Orbit,
			RPG,
			RTS,
			ThirdPerson
		}

		private enum ControlPriority
		{
			Pan,
			ZoomRotate
		}

		private static MobileControls instance;

		private ControlPriority controlPriority;

		public int LeftPanelIndex;

		public int RightPanelIndex;

		private TouchProcessor touchProcessor;

		private bool isPanning;

		public static MobileControls Instance
		{
			get
			{
				if (!instance)
				{
					CameraInstance.CreateInstance<MobileControls>("MobileControls");
				}
				return instance;
			}
		}

		private void Awake()
		{
			Init();
			isPanning = false;
		}

		public Dictionary<string, object> Serialize()
		{
			BaseControl[] components = base.gameObject.GetComponents<BaseControl>();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			int num = 0;
			BaseControl[] array = components;
			foreach (BaseControl baseControl in array)
			{
				dictionary.Add(num++.ToString(), baseControl.SerializeJSON());
			}
			return dictionary;
		}

		public void Deserialize(Dictionary<string, object> dic)
		{
			LeftPanelIndex = 0;
			RightPanelIndex = 0;
			BaseControl[] components = base.gameObject.GetComponents<BaseControl>();
			BaseControl[] array = components;
			foreach (BaseControl button in array)
			{
				RemoveControl(button);
			}
			foreach (Dictionary<string, object> value in dic.Values)
			{
				ControlType controlType = (ControlType)Convert.ToInt32(value["Type"]);
				switch (controlType)
				{
				case ControlType.Button:
				{
					Button button2 = CreateButton(string.Empty);
					button2.DeserializeJSON(value);
					break;
				}
				case ControlType.Stick:
				case ControlType.CameraPanel:
				{
					BaseControl baseControl = DeserializeMasterControl(controlType);
					baseControl.DeserializeJSON(value);
					break;
				}
				case ControlType.Zoom:
				{
					Zoom zoom = CreateZoom(string.Empty);
					zoom.DeserializeJSON(value);
					break;
				}
				case ControlType.Rotate:
				{
					Rotate rotate = CreateRotation(string.Empty);
					rotate.DeserializeJSON(value);
					break;
				}
				case ControlType.Pan:
				{
					Pan pan = CreatePan(string.Empty);
					pan.DeserializeJSON(value);
					break;
				}
				}
			}
		}

		public void LoadLayout(Layout layout)
		{
			string str = layout.ToString() + "Layout";
			TextAsset textAsset = Resources.Load<TextAsset>("Config/MobileLayouts/" + str);
			if ((bool)textAsset && !string.IsNullOrEmpty(textAsset.text))
			{
				Dictionary<string, object> dic = MiamiSerializier.JSONDeserialize(textAsset.text) as Dictionary<string, object>;
				Deserialize(dic);
			}
		}

		private void Init()
		{
			if (!(instance == null))
			{
				return;
			}
			instance = this;
			touchProcessor = new TouchProcessor(2);
			BaseControl[] controls = GetControls();
			if (controls != null)
			{
				BaseControl[] array = controls;
				foreach (BaseControl baseControl in array)
				{
					baseControl.Init(touchProcessor);
				}
			}
		}

		public BaseControl[] GetControls()
		{
			BaseControl[] components = base.gameObject.GetComponents<BaseControl>();
			Array.Sort(components, (BaseControl a, BaseControl b) => a.OperationTimer.CompareTo(b.Operations));
			return components;
		}

		public Button CreateButton(string btnName)
		{
			Button button = base.gameObject.AddComponent<Button>();
			button.Init(touchProcessor);
			button.InputKey0 = btnName;
			return button;
		}

		public Zoom CreateZoom(string btnName)
		{
			Zoom zoom = base.gameObject.AddComponent<Zoom>();
			zoom.Init(touchProcessor);
			zoom.InputKey0 = btnName;
			return zoom;
		}

		public Rotate CreateRotation(string btnName)
		{
			Rotate rotate = base.gameObject.AddComponent<Rotate>();
			rotate.Init(touchProcessor);
			rotate.InputKey0 = btnName;
			return rotate;
		}

		public Pan CreatePan(string btnName)
		{
			Pan pan = base.gameObject.AddComponent<Pan>();
			pan.Init(touchProcessor);
			pan.InputKey0 = btnName;
			return pan;
		}

		public void DuplicateButtonValues(Button target, Button source)
		{
			target.Position = source.Position;
			target.Size = source.Size;
			target.PreserveTextureRatio = source.PreserveTextureRatio;
			target.Toggle = source.Toggle;
			target.TextureDefault = source.TextureDefault;
			target.TexturePressed = source.TexturePressed;
		}

		public void DuplicateBasicValues(BaseControl target, BaseControl source)
		{
			target.Position = source.Position;
			target.Size = source.Size;
		}

		public void RemoveControl(BaseControl button)
		{
			Game.Character.Utils.Debug.Destroy(button, allowDestroyingAssets: true);
		}

		private BaseControl DeserializeMasterControl(ControlType type)
		{
			BaseControl baseControl = null;
			switch (type)
			{
			case ControlType.CameraPanel:
				baseControl = base.gameObject.AddComponent<CameraPanel>();
				break;
			case ControlType.Stick:
				baseControl = base.gameObject.AddComponent<Stick>();
				break;
			}
			if (baseControl != null)
			{
				baseControl.Init(touchProcessor);
			}
			return baseControl;
		}

		public BaseControl CreateMasterControl(string axis0, string axis1, ControlType type, ControlSide side)
		{
			RemoveMasterControl(side);
			BaseControl baseControl = null;
			switch (type)
			{
			case ControlType.CameraPanel:
				baseControl = base.gameObject.AddComponent<CameraPanel>();
				break;
			case ControlType.Stick:
				baseControl = base.gameObject.AddComponent<Stick>();
				break;
			}
			if (baseControl != null)
			{
				baseControl.Init(touchProcessor);
				baseControl.Side = side;
				baseControl.InputKey0 = axis0;
				baseControl.InputKey1 = axis1;
			}
			return baseControl;
		}

		public void RemoveMasterControl(ControlSide side)
		{
			BaseControl[] controls = GetControls();
			if (controls == null)
			{
				return;
			}
			BaseControl[] array = controls;
			foreach (BaseControl baseControl in array)
			{
				if (baseControl.Side == side)
				{
					RemoveControl(baseControl);
				}
			}
		}

		public bool GetButton(string key)
		{
			BaseControl ctrl;
			if (TryGetControl(key, out ctrl))
			{
				return ctrl.Type == ControlType.Button && ((Button)ctrl).IsPressed();
			}
			return false;
		}

		public float GetZoom(string key)
		{
			BaseControl ctrl;
			if (!isPanning && TryGetControl(key, out ctrl) && ctrl.Type == ControlType.Zoom && ctrl.Active)
			{
				return ((Zoom)ctrl).ZoomDelta;
			}
			return 0f;
		}

		public float GetRotation(string key)
		{
			BaseControl ctrl;
			if (!isPanning && TryGetControl(key, out ctrl) && ctrl.Type == ControlType.Rotate && ctrl.Active)
			{
				return ((Rotate)ctrl).RotateAngle;
			}
			return 0f;
		}

		public Vector2 GetPan(string key)
		{
			BaseControl ctrl;
			if (TryGetControl(key, out ctrl) && ctrl.Type == ControlType.Pan && ctrl.Active && ctrl.Operations > 3)
			{
				isPanning = true;
				return ((Pan)ctrl).PanPosition;
			}
			isPanning = false;
			return Vector2.zero;
		}

		public float GetAxis(string key)
		{
			BaseControl ctrl;
			if (TryGetControl(key, out ctrl) && (ctrl.Type == ControlType.Stick || ctrl.Type == ControlType.CameraPanel))
			{
				Vector2 inputAxis = ctrl.GetInputAxis();
				if (key == ctrl.InputKey0)
				{
					return inputAxis.x;
				}
				if (key == ctrl.InputKey1)
				{
					return inputAxis.y;
				}
				return 0f;
			}
			return 0f;
		}

		public bool GetButtonDown(string buttonName)
		{
			BaseControl ctrl;
			if (TryGetControl(buttonName, out ctrl))
			{
				return ctrl.Type == ControlType.Button && ((Button)ctrl).State == Button.ButtonState.Begin;
			}
			return false;
		}

		public bool GetButtonUp(string buttonName)
		{
			BaseControl ctrl;
			if (TryGetControl(buttonName, out ctrl))
			{
				return ctrl.Type == ControlType.Button && ((Button)ctrl).State == Button.ButtonState.End;
			}
			return false;
		}

		private bool TryGetControl(string key, out BaseControl ctrl)
		{
			BaseControl[] controls = GetControls();
			if (controls != null)
			{
				BaseControl[] array = controls;
				foreach (BaseControl baseControl in array)
				{
					if (baseControl.InputKey0 == key || baseControl.InputKey1 == key)
					{
						ctrl = baseControl;
						return true;
					}
				}
			}
			ctrl = null;
			return false;
		}

		private void Update()
		{
			Init();
			touchProcessor.ScanInput();
			BaseControl[] controls = GetControls();
			if (controls != null)
			{
				BaseControl[] array = controls;
				foreach (BaseControl baseControl in array)
				{
					baseControl.GameUpdate();
				}
			}
		}

		private void OnGUI()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			BaseControl[] controls = GetControls();
			if (controls != null)
			{
				BaseControl[] array = controls;
				foreach (BaseControl baseControl in array)
				{
					baseControl.Draw();
				}
			}
		}
	}
}
