using System;
using UnityEngine;

public class DontGoTrowBuilding : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (!collider.gameObject.CompareTag("Player"))
		{
			return;
		}
		Transform transform = collider.transform;
		Vector3 a = transform.position;
		int num = 0;
		int num2 = 0;
		Vector3 a2 = default(Vector3);
		while (num2 != 8)
		{
			float num3 = UnityEngine.Random.Range(5, 10);
			float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
			float x = num3 * Mathf.Cos(f);
			float z = num3 * Mathf.Sin(f);
			a += new Vector3(x, 0f, z);
			RaycastHit hitInfo;
			if (Physics.Raycast(new Ray(a + 10f * Vector3.up, new Vector3(0f, -1000f, 0f)), out hitInfo))
			{
				a2 = hitInfo.point;
				num2 = hitInfo.collider.gameObject.layer;
			}
			num++;
			if (num > 130)
			{
				break;
			}
		}
		transform.position = a2 + Vector3.up;
	}
}
