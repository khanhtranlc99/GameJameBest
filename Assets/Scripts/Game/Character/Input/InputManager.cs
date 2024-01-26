using Game.Character.Input.Mobile;
using Game.Character.Utils;
using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.Character.Input
{
	public class InputManager : MonoBehaviour
	{
		public static float DoubleClickTimeout = 0.25f;

		public bool FilterInput = true;

		private static InputManager instance;

		public InputPreset InputPreset;

		public bool MobileInput;

		[HideInInspector]
		public bool IsValid;

		private Input[] inputs;

		private GameInput[] GameInputs;

		private GameInput currInput;

		public static InputManager Instance
		{
			get
			{
				if (!instance)
				{
					instance = CameraInstance.CreateInstance<InputManager>("CameraInput");
				}
				return instance;
			}
		}

		public Input GetInput(InputType type)
		{
			return inputs[(int)type];
		}

		public T GetInput<T>(InputType type, T defaultValue)
		{
			Input input = inputs[(int)type];
			if (input.Valid)
			{
				return (T)input.Value;
			}
			return defaultValue;
		}

		public void SetInputPreset(InputPreset preset)
		{
			GameInput[] gameInputs = GameInputs;
			int num = 0;
			GameInput gameInput;
			while (true)
			{
				if (num < gameInputs.Length)
				{
					gameInput = gameInputs[num];
					if (gameInput.PresetType == preset)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			currInput = gameInput;
			InputPreset = preset;
		}

		public Input[] GetInputArray()
		{
			return inputs;
		}

		public GameInput GetInputPresetCurrent()
		{
			return currInput;
		}

		public void EnableInputGroup(InputGroup inputGroup, bool status)
		{
			switch (inputGroup)
			{
			case InputGroup.All:
			{
				Input[] array = inputs;
				foreach (Input input in array)
				{
					input.Enabled = status;
				}
				break;
			}
			case InputGroup.CameraMove:
				inputs[0].Enabled = status;
				inputs[1].Enabled = status;
				inputs[2].Enabled = status;
				inputs[3].Enabled = status;
				inputs[4].Enabled = status;
				inputs[5].Enabled = status;
				break;
			case InputGroup.Character:
				inputs[6].Enabled = status;
				inputs[7].Enabled = status;
				inputs[8].Enabled = status;
				inputs[9].Enabled = status;
				inputs[10].Enabled = status;
				inputs[11].Enabled = status;
				inputs[12].Enabled = status;
				inputs[13].Enabled = status;
				inputs[14].Enabled = status;
				inputs[15].Enabled = status;
				inputs[16].Enabled = status;
				inputs[17].Enabled = status;
				inputs[18].Enabled = status;
				break;
			}
		}

		private void Awake()
		{
			instance = this;
			inputs = new Input[20];
			int num = 0;
			InputType[] array = (InputType[])Enum.GetValues(typeof(InputType));
			foreach (InputType type in array)
			{
				inputs[num++] = new Input
				{
					Type = type,
					Valid = false,
					Value = null
				};
			}
			GameInputs = base.gameObject.GetComponents<GameInput>();
			SetInputPreset(InputPreset);
			EnableInputGroup(InputGroup.All, status: true);
		}

		private void Start()
		{
			if (!Application.isEditor)
			{
				MobileInput = true;
				Game.Character.Utils.Debug.SetActive(MobileControls.Instance.gameObject, status: true);
				MobileControls.Instance.enabled = true;
			}
		}

		public void ResetInputArray()
		{
			Input[] array = inputs;
			foreach (Input input in array)
			{
				input.Valid = false;
			}
		}

		public void GameUpdate()
		{
			InputWrapper.Mobile = MobileInput;
			IsValid = true;
			if (currInput.ResetInputArray)
			{
				Input[] array = inputs;
				foreach (Input input in array)
				{
					input.Valid = false;
				}
			}
			if (currInput.PresetType != InputPreset)
			{
				SetInputPreset(InputPreset);
			}
			currInput.UpdateInput(inputs);
		}
	}
}
