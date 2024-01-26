using UnityEngine;

public class TurnOffIn10sec : MonoBehaviour
{
	public float timeDelay = 5f;

	private bool isTimerOn;

	private float timer;

	private void OnEnable()
	{
		isTimerOn = true;
		timer = timeDelay;
	}

	private void Update()
	{
		if (isTimerOn)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				timer = timeDelay;
				isTimerOn = false;
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
