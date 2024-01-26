using UnityEngine;

public class ClimbingSensors : MonoBehaviour
{
	public float RaycastLength = 1f;

	public bool DrawDebug;

	public Transform TopPoint;

	public Transform BottomPoint;

	public Transform playerTransform;

	public LayerMask HookLayerMask;

	private bool prevHasWall;

	private int timer;

	private int maxNoWallTick = 10;

	private bool disabled;

	public float disableTime = 1f;

	private float lastCheckTime;

	public void DisableSensorsForJumpOffTheWall()
	{
		disabled = true;
		Invoke("EnableSensors", disableTime);
	}

	private void EnableSensors()
	{
		disabled = false;
	}

	public void CheckWall(out bool hasWall, out bool shouldClimbToTop)
	{
		if (Time.time - lastCheckTime > 0.5f)
		{
			prevHasWall = false;
		}
		if (disabled)
		{
			hasWall = false;
			shouldClimbToTop = false;
			prevHasWall = false;
			return;
		}
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(TopPoint.position, playerTransform.forward, out hitInfo, RaycastLength, HookLayerMask);
		bool flag2 = Physics.Raycast(BottomPoint.position, playerTransform.forward, out hitInfo, RaycastLength, HookLayerMask);
		shouldClimbToTop = (!flag && flag2);
		hasWall = (flag && flag2);
		if (!hasWall && prevHasWall)
		{
			if (timer < maxNoWallTick)
			{
				timer++;
				hasWall = true;
				prevHasWall = true;
			}
			else
			{
				hasWall = false;
				timer = 0;
				prevHasWall = false;
			}
		}
		prevHasWall = hasWall;
		lastCheckTime = Time.time;
	}

	private void OnDrawGizmos()
	{
		if (DrawDebug)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(TopPoint.position, 0.1f);
			Gizmos.DrawSphere(BottomPoint.position, 0.1f);
		}
	}
}
