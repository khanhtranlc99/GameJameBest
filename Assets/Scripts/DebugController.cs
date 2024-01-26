using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

public class DebugController : MonoBehaviour
{
	public bool DebugMode;

	public bool AdvancedMode;

	private float _timeFps;

	private float _framesCount;

	private float _fps;

	private void Start()
	{
		DebugMode = UnityEngine.Debug.isDebugBuild;
	}

	private void Update()
	{
		if (DebugMode)
		{
			_timeFps += Time.unscaledDeltaTime;
			_framesCount += 1f;
			if (_timeFps > 1f)
			{
				_fps = _framesCount / _timeFps;
				_framesCount = 0f;
				_timeFps = 0f;
			}
		}
	}

	private void OnGUI()
	{
		if (DebugMode)
		{
			GUIStyle gUIStyle = new GUIStyle();
			int num = Screen.height * 2 / 100;
			Rect position = new Rect(0f, 0f, Screen.width, num);
			gUIStyle.alignment = TextAnchor.UpperLeft;
			gUIStyle.fontSize = 45;
			gUIStyle.normal.textColor = new Color(0f, 1f, 0f, 1f);
			if (AdvancedMode)
			{
				float axis = CrossPlatformInputManager.GetAxis("Horizontal");
				float axis2 = CrossPlatformInputManager.GetAxis("Vertical");
				//string text = $"{_fps:0.} fps \n XInput: {axis:0.00} \n YInput: {axis2:0.00} \nScreen: {Screen.width:0}x{Screen.height:0} DPI: {Screen.dpi:0}";
				GUI.Label(position, "", gUIStyle);
			}
			else
			{
				string text2 = Mathf.Round(_fps).ToString();
				GUI.Label(position, text2, gUIStyle);
			}
		}
	}
}
