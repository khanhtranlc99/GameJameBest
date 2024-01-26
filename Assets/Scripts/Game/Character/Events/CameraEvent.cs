using Game.Character.CameraEffects;
using Game.Character.Config;
using Game.Character.Modes;
using Game.Character.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Events
{
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(BoxCollider))]
	public class CameraEvent : MonoBehaviour
	{
		private abstract class ITween
		{
			public string mode;

			public string key;

			public float time;

			public float timeout;

			public abstract void Interpolate(float t);
		}

		private class FloatTween : ITween
		{
			public float t0;

			public float t1;

			public override void Interpolate(float t)
			{
				float inputValue = Interpolation.LerpS(t0, t1, t);
				Game.Character.Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
				configuration.SetFloat(mode, key, inputValue);
			}
		}

		private class Vector2Tween : ITween
		{
			public Vector2 t0;

			public Vector2 t1;

			public override void Interpolate(float t)
			{
				Vector2 inputValue = Interpolation.LerpS(t0, t1, t);
				Game.Character.Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
				configuration.SetVector2(mode, key, inputValue);
			}
		}

		private class Vector3Tween : ITween
		{
			public Vector3 t0;

			public Vector3 t1;

			public override void Interpolate(float t)
			{
				Vector3 inputValue = Interpolation.LerpS(t0, t1, t);
				Game.Character.Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
				configuration.SetVector3(mode, key, inputValue);
			}
		}

		private List<ITween> tweens;

		public EventType Type;

		public Game.Character.Modes.Type CameraMode;

		public string StringParam0;

		public string StringParam1;

		public Game.Character.Config.Config.ConfigValue ConfigParamValueType;

		public bool ConfigParamBool;

		public string ConfigParamString;

		public float ConfigParamFloat;

		public Vector2 ConfigParamVector2;

		public Vector3 ConfigParamVector3;

		public Game.Character.CameraEffects.Type CameraEffect;

		public GameObject CustomObject;

		public bool RestoreOnExit;

		public bool SmoothFloatParams;

		public float SmoothTimeout;

		public bool LookAtFrom;

		public bool LookAtTo;

		public Transform LookAtFromObject;

		public Transform LookAtToObject;

		public bool RestoreOnTimeout;

		public float RestoreTimeout;

		public bool RestoreConfiguration;

		public string RestoreConfigurationName;

		private Collider cameraTrigger;

		private object oldParam0;

		private object oldParam1;

		private object oldParam2;

		private float restorationTimeout;

		private bool paramChanged;

		private void Awake()
		{
			tweens = new List<ITween>();
		}

		private void SmoothParam(string mode, string key, float t0, float t1, float time)
		{
			FloatTween floatTween = new FloatTween();
			floatTween.key = key;
			floatTween.mode = mode;
			floatTween.t0 = t0;
			floatTween.t1 = t1;
			floatTween.time = time;
			floatTween.timeout = time;
			FloatTween item = floatTween;
			tweens.Add(item);
		}

		private void SmoothParam(string mode, string key, Vector2 t0, Vector2 t1, float time)
		{
			Vector2Tween vector2Tween = new Vector2Tween();
			vector2Tween.key = key;
			vector2Tween.mode = mode;
			vector2Tween.t0 = t0;
			vector2Tween.t1 = t1;
			vector2Tween.time = time;
			vector2Tween.timeout = time;
			Vector2Tween item = vector2Tween;
			tweens.Add(item);
		}

		private void SmoothParam(string mode, string key, Vector3 t0, Vector3 t1, float time)
		{
			Vector3Tween vector3Tween = new Vector3Tween();
			vector3Tween.key = key;
			vector3Tween.mode = mode;
			vector3Tween.t0 = t0;
			vector3Tween.t1 = t1;
			vector3Tween.time = time;
			vector3Tween.timeout = time;
			Vector3Tween item = vector3Tween;
			tweens.Add(item);
		}

		private void Update()
		{
			foreach (ITween tween in tweens)
			{
				tween.timeout -= Time.deltaTime;
				float t = 1f - Mathf.Clamp01(tween.timeout / tween.time);
				tween.Interpolate(t);
				if (tween.timeout < 0f)
				{
					tweens.Remove(tween);
					break;
				}
			}
			if (cameraTrigger != null && RestoreOnTimeout)
			{
				restorationTimeout -= Time.deltaTime;
				if (restorationTimeout < 0f)
				{
					Exit(true, cameraTrigger);
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other || !other.gameObject)
			{
				return;
			}
			CameraTrigger component = other.gameObject.GetComponent<CameraTrigger>();
			if ((bool)component)
			{
				if ((bool)cameraTrigger)
				{
					return;
				}
				cameraTrigger = other;
				switch (Type)
				{
				case EventType.ConfigMode:
				{
					Game.Character.Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
					if ((bool)configuration && !string.IsNullOrEmpty(StringParam0))
					{
						oldParam0 = configuration.GetCurrentMode();
						if ((string)oldParam0 != StringParam0)
						{
							paramChanged = configuration.SetCameraMode(StringParam0);
						}
					}
					break;
				}
				case EventType.ConfigParam:
				{
					Game.Character.Config.Config configuration2 = CameraManager.Instance.GetCurrentCameraMode().Configuration;
					string mode = (string)(oldParam2 = configuration2.GetCurrentMode());
					if (!configuration2 || string.IsNullOrEmpty(StringParam0))
					{
						break;
					}
					oldParam0 = StringParam0;
					switch (ConfigParamValueType)
					{
					case Game.Character.Config.Config.ConfigValue.Bool:
						oldParam1 = configuration2.GetBool(mode, StringParam0);
						configuration2.SetBool(mode, StringParam0, ConfigParamBool);
						break;
					case Game.Character.Config.Config.ConfigValue.Range:
						oldParam1 = configuration2.GetFloat(mode, StringParam0);
						if (SmoothFloatParams)
						{
							SmoothParam(mode, StringParam0, (float)oldParam1, ConfigParamFloat, SmoothTimeout);
						}
						else
						{
							configuration2.SetFloat(mode, StringParam0, ConfigParamFloat);
						}
						break;
					case Game.Character.Config.Config.ConfigValue.Selection:
						oldParam1 = configuration2.GetSelection(mode, StringParam0);
						configuration2.SetSelection(mode, StringParam0, StringParam1);
						break;
					case Game.Character.Config.Config.ConfigValue.String:
						oldParam1 = configuration2.GetString(mode, StringParam0);
						configuration2.SetString(mode, StringParam0, StringParam1);
						break;
					case Game.Character.Config.Config.ConfigValue.Vector2:
						oldParam1 = configuration2.GetVector2(mode, StringParam0);
						if (SmoothFloatParams)
						{
							SmoothParam(mode, StringParam0, (Vector2)oldParam1, ConfigParamVector2, SmoothTimeout);
						}
						else
						{
							configuration2.SetVector2(mode, StringParam0, ConfigParamVector2);
						}
						break;
					case Game.Character.Config.Config.ConfigValue.Vector3:
						oldParam1 = configuration2.GetVector3(mode, StringParam0);
						if (SmoothFloatParams)
						{
							SmoothParam(mode, StringParam0, (Vector3)oldParam1, ConfigParamVector3, SmoothTimeout);
						}
						else
						{
							configuration2.SetVector2(mode, StringParam0, ConfigParamVector2);
						}
						break;
					}
					break;
				}
				case EventType.Effect:
				{
					CameraEffect cameraEffect = EffectManager.Instance.Create(CameraEffect);
					cameraEffect.Play();
					break;
				}
				case EventType.CustomMessage:
					if ((bool)CustomObject && !string.IsNullOrEmpty(StringParam0))
					{
						CustomObject.SendMessage(StringParam0);
					}
					break;
				case EventType.LookAt:
					if ((!LookAtFrom || (bool)LookAtFromObject) && (!LookAtTo || (bool)LookAtToObject) && (LookAtTo || LookAtFrom))
					{
						oldParam0 = CameraManager.Instance.GetCurrentCameraMode().Type;
					}
					break;
				}
			}
			if (RestoreOnTimeout)
			{
				restorationTimeout = RestoreTimeout;
			}
		}

		private void Exit(bool onTimeout, Collider other)
		{
			bool flag = false;
			flag = ((!onTimeout) ? (RestoreOnExit && cameraTrigger == other) : RestoreOnTimeout);
			if (!RestoreOnExit && !RestoreOnTimeout)
			{
				cameraTrigger = null;
			}
			if (!flag)
			{
				return;
			}
			cameraTrigger = null;
			switch (Type)
			{
			case EventType.ConfigMode:
				if (paramChanged)
				{
					Game.Character.Config.Config configuration2 = CameraManager.Instance.GetCurrentCameraMode().Configuration;
					if ((bool)configuration2 && !string.IsNullOrEmpty((string)oldParam0) && (string)oldParam0 != configuration2.GetCurrentMode())
					{
						configuration2.SetCameraMode((string)oldParam0);
					}
				}
				break;
			case EventType.ConfigParam:
			{
				Game.Character.Config.Config configuration = CameraManager.Instance.GetCurrentCameraMode().Configuration;
				if (!configuration || string.IsNullOrEmpty((string)oldParam0) || oldParam1 == null || string.IsNullOrEmpty((string)oldParam2))
				{
					break;
				}
				switch (ConfigParamValueType)
				{
				case Game.Character.Config.Config.ConfigValue.Bool:
					configuration.SetBool((string)oldParam2, (string)oldParam0, (bool)oldParam1);
					break;
				case Game.Character.Config.Config.ConfigValue.Range:
				{
					float @float = configuration.GetFloat((string)oldParam2, (string)oldParam0);
					if (SmoothFloatParams)
					{
						SmoothParam((string)oldParam2, (string)oldParam0, @float, (float)oldParam1, SmoothTimeout);
					}
					else
					{
						configuration.SetFloat((string)oldParam2, (string)oldParam0, (float)oldParam1);
					}
					break;
				}
				case Game.Character.Config.Config.ConfigValue.Selection:
					configuration.SetSelection((string)oldParam2, (string)oldParam0, (string)oldParam1);
					break;
				case Game.Character.Config.Config.ConfigValue.String:
					configuration.SetString((string)oldParam2, (string)oldParam0, (string)oldParam1);
					break;
				case Game.Character.Config.Config.ConfigValue.Vector2:
				{
					Vector2 vector2 = configuration.GetVector2((string)oldParam2, (string)oldParam0);
					if (SmoothFloatParams)
					{
						SmoothParam((string)oldParam2, (string)oldParam0, vector2, (Vector2)oldParam1, SmoothTimeout);
					}
					else
					{
						configuration.SetVector2((string)oldParam2, (string)oldParam0, (Vector2)oldParam1);
					}
					break;
				}
				case Game.Character.Config.Config.ConfigValue.Vector3:
				{
					Vector3 vector = configuration.GetVector3((string)oldParam2, (string)oldParam0);
					if (SmoothFloatParams)
					{
						SmoothParam((string)oldParam2, (string)oldParam0, vector, (Vector3)oldParam1, SmoothTimeout);
					}
					else
					{
						configuration.SetVector3((string)oldParam2, (string)oldParam0, (Vector3)oldParam1);
					}
					break;
				}
				}
				break;
			}
			case EventType.CustomMessage:
				if ((bool)CustomObject && !string.IsNullOrEmpty(StringParam1))
				{
					CustomObject.SendMessage(StringParam1);
				}
				break;
			case EventType.LookAt:
				if (oldParam0 is Game.Character.Modes.Type && RestoreConfiguration && !string.IsNullOrEmpty(RestoreConfigurationName))
				{
					CameraManager.Instance.SetDefaultConfiguration((Game.Character.Modes.Type)(int)oldParam0, RestoreConfigurationName);
				}
				break;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			Exit(false, other);
		}
	}
}
