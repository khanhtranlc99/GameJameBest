using System.Collections.Generic;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

public class PreviewRotator : MonoBehaviour
{
	public Transform[] RotatableTransforms;

	public float normalRotationSpeed = 0.1f;

	private float drug;

	private float startRotationSpeed;

	private float rotationSpeed = 1f;

	private float x;

	private List<Quaternion> defaultRotations = new List<Quaternion>();

	private void Start()
	{
		Transform[] rotatableTransforms = RotatableTransforms;
		foreach (Transform transform in rotatableTransforms)
		{
			defaultRotations.Add(transform.localRotation);
		}
	}

	private void Update()
	{
		drug = 0f - CrossPlatformInputManager.GetVirtualOnlyAxis("Horizontal", raw: false);
		if (drug != 0f)
		{
			rotationSpeed = drug * 300f;
			x = 0f;
			startRotationSpeed = rotationSpeed;
		}
		else if (rotationSpeed != normalRotationSpeed)
		{
			x += 0.01f;
			rotationSpeed = Mathf.Lerp(startRotationSpeed, normalRotationSpeed, x);
		}
		Transform[] rotatableTransforms = RotatableTransforms;
		foreach (Transform transform in rotatableTransforms)
		{
			transform.Rotate(Vector3.up, rotationSpeed);
		}
	}

	public void ResetRotators()
	{
		rotationSpeed = normalRotationSpeed;
		for (int i = 0; i < RotatableTransforms.Length; i++)
		{
			RotatableTransforms[i].localRotation = defaultRotations[i];
		}
	}
}
