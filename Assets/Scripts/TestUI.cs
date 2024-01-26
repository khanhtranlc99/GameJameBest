using Game.GlobalComponent;
using UnityEngine;

public class TestUI : MonoBehaviour
{
	public RectTransform UIObject;

	public RectTransform UIObjectOnCanvas;

	private bool work;

	private bool rotate;

	private bool rotateCanvas;

	private float timer;

	private void Update()
	{
		if (timer <= 0f && work)
		{
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Gems, "geeeeems");
			timer = 1f;
		}
		else
		{
			timer -= Time.deltaTime;
		}
		if (rotate)
		{
			UIObject.Rotate(new Vector3(0f, 0f, 1f), 15f);
		}
		if (rotateCanvas)
		{
			UIObjectOnCanvas.Rotate(new Vector3(0f, 0f, 1f), 10f);
		}
	}

	public void Work()
	{
		work = !work;
	}

	public void Rotate()
	{
		rotate = !rotate;
	}

	public void RotateCanvas()
	{
		rotateCanvas = !rotateCanvas;
	}
}
