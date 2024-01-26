using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour
{
	private float fpsMeasurePeriod = 0.5f;

	private int fpsAccumulator;

	private float fpsNextPeriod;

	private int currentFps;

	private string display = "{0} FPS";

	private void Start()
	{
		fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
	}

	private void Update()
	{
		fpsAccumulator++;
		if (Time.realtimeSinceStartup > fpsNextPeriod)
		{
			currentFps = (int)((float)fpsAccumulator / fpsMeasurePeriod);
			fpsAccumulator = 0;
			fpsNextPeriod += fpsMeasurePeriod;
			GetComponent<TextMeshProUGUI>().text = string.Format(display, currentFps);
		}
	}
}
