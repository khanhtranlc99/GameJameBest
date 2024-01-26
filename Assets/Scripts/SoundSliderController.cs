using Game.Managers;
using UnityEngine;
using UnityEngine.UI;

public class SoundSliderController : MonoBehaviour
{
	public SoundManager.SoundType SoundType;

	private void Awake()
	{
		Slider component = GetComponent<Slider>();
		if (component != null)
		{
			component.value = SoundManager.instance.GetValueByType(SoundType);
		}
	}
}
