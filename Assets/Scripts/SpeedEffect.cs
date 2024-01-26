using Game.Character.CharacterController;
using System.Collections.Generic;
using UnityEngine;

public class SpeedEffect : MonoBehaviour
{
	public List<Transform> lines;

	private float minZPos = -8f;

	private float maxZPos = -4f;

	private float lineLocalZPos = 10f;

	public float deltaZ = 0.5f;

	public float deltaRot = 0.5f;

	public float velocityForMaxEffect = 20f;

	public float velocityForEnableEffect = 5f;

	private Rigidbody curRbody => PlayerManager.Instance.Player.rigidbody;

	private void Update()
	{
		if (curRbody.gameObject.activeInHierarchy && curRbody.velocity.magnitude > velocityForEnableEffect)
		{
			float t = (Mathf.Clamp(curRbody.velocity.magnitude, velocityForEnableEffect, velocityForMaxEffect) - velocityForEnableEffect) / (velocityForMaxEffect - velocityForEnableEffect);
			Transform transform = base.transform;
			Vector3 localPosition = base.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = base.transform.localPosition;
			transform.localPosition = new Vector3(x, localPosition2.y, Mathf.Lerp(minZPos, maxZPos, t));
			base.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f - deltaRot, deltaRot));
			RandomizeLinesZPositions();
		}
		else
		{
			Transform transform2 = base.transform;
			Vector3 localPosition3 = base.transform.localPosition;
			float x2 = localPosition3.x;
			Vector3 localPosition4 = base.transform.localPosition;
			transform2.localPosition = new Vector3(x2, localPosition4.y, minZPos);
		}
	}

	private void RandomizeLinesZPositions()
	{
		for (int i = 0; i < lines.Count; i++)
		{
			Transform transform = lines[i];
			Transform transform2 = transform;
			Vector3 localPosition = transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = transform.localPosition;
			transform2.localPosition = new Vector3(x, localPosition2.y, lineLocalZPos + UnityEngine.Random.Range(0f - deltaZ, deltaZ));
		}
	}
}
