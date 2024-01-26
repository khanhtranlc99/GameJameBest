using UnityEngine;

public class CloakResetter : MonoBehaviour
{
	public bool DebugLog;

	[Space(10f)]
	public Cloth cloak;

	public int Frames = 1;

	private int framesToWait;

	private bool reseted;

	private void Start()
	{
		if (cloak == null)
		{
			cloak = GetComponent<Cloth>();
		}
	}

	private void OnEnable()
	{
		if (!(cloak == null))
		{
			cloak.enabled = false;
			framesToWait = Frames;
			reseted = false;
		}
	}

	private void Update()
	{
		if (reseted)
		{
			return;
		}
		if (framesToWait <= 0)
		{
			cloak.enabled = true;
			reseted = true;
			if (DebugLog)
			{
				MonoBehaviour.print("Cloak reseted");
			}
		}
		else
		{
			framesToWait--;
		}
	}
}
