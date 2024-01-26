using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class LookControlsSwitcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public TouchPad TouchPad;

	public JoyPad JoyPad;

	public ButtonHandler ButtonHandler;

	public string[] InputButtons = new string[2]
	{
		"Aim",
		"Fire"
	};

	private void Awake()
	{
		if (!TouchPad)
		{
			TouchPad = base.transform.parent.GetComponentInChildren<TouchPad>();
		}
		if (!TouchPad)
		{
			UnityEngine.Debug.LogError("Can't find `TouchPad`");
			base.enabled = false;
		}
		if (!JoyPad)
		{
			JoyPad = GetComponent<JoyPad>();
		}
		if (!JoyPad)
		{
			UnityEngine.Debug.LogError("Can't find `JoyPad`");
			base.enabled = false;
		}
		if (!ButtonHandler)
		{
			ButtonHandler = GetComponent<ButtonHandler>();
		}
		if (!ButtonHandler)
		{
			UnityEngine.Debug.LogError("Can't find `ButtonHandler`");
			base.enabled = false;
		}
	}

	private void Start()
	{
		JoyPad.Start();
	}

	public void OnPointerDown(PointerEventData data)
	{
		TouchPad.enabled = false;
		JoyPad.enabled = true;
		JoyPad.OnPointerDown(data);
		string[] inputButtons = InputButtons;
		foreach (string downState in inputButtons)
		{
			ButtonHandler.SetDownState(downState);
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		JoyPad.enabled = false;
		TouchPad.enabled = true;
		JoyPad.OnPointerUp(data);
		string[] inputButtons = InputButtons;
		foreach (string upState in inputButtons)
		{
			ButtonHandler.SetUpState(upState);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		ResertLookControlsSwitcher();
	}

	public void ResertLookControlsSwitcher()
	{
		JoyPad.enabled = false;
		TouchPad.enabled = true;
		JoyPad.ResetJoyPad();
		string[] inputButtons = InputButtons;
		foreach (string upState in inputButtons)
		{
			ButtonHandler.SetUpState(upState);
		}
	}

	private void OnEnable()
	{
		JoyPad.ResetJoypadPosition();
	}

	private void OnDisable()
	{
		OnPointerUp(null);
	}
}
