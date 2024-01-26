using Game.Managers;
using UnityEngine;
using UnityEngine.UI;

public class SoundButtonToggle : MonoBehaviour
{
	public Slider ValueSlider;

	public SoundManager.SoundType Type;

	private void Awake()
	{
		SoundManager.instance.AddValueChangeByType(Type, delegate(float value)
		{
			if (this != null)
			{
				ValueChanged(value);
			}
		});
	}

	private void Start()
	{
		ValueChanged(SoundManager.instance.GetValueByType(Type));
	}

	private void ValueChanged(float value)
	{
		Toggle component = GetComponent<Toggle>();
		if (component != null)
		{
			component.isOn = (value > 0f);
		}
	}

	public void ToggleAction(bool isChecked)
	{
		float value;
		if (isChecked)
		{
			float valueByType = SoundManager.instance.GetValueByType(Type);
			value = ((!(valueByType > 0f)) ? 0.5f : valueByType);
		}
		else
		{
			value = 0f;
		}
		SoundManager.instance.SetValueByType(value, Type);
		if (ValueSlider != null)
		{
			if (!isChecked && ValueSlider.value > 0f)
			{
				ValueSlider.value = 0f;
			}
			if (isChecked && ValueSlider.value == 0f)
			{
				ValueSlider.value = value;
			}
		}
	}
}
