using UnityEngine;

public class RigidBodySlowDown : MonoBehaviour
{
	private float MaxSpeed = 3f;

	public Rigidbody MainRigidbody;

	private Rigidbody rigidbody;

	private Transform parrantTransform;

	private float StartDistanceToParant;

	private float distanceToParrant;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		parrantTransform = base.transform.parent;
		StartDistanceToParant = Vector3.Distance(parrantTransform.position, base.transform.position);
	}

	private void Update()
	{
		if ((bool)rigidbody && (MainRigidbody.velocity - rigidbody.velocity).magnitude > MaxSpeed)
		{
			rigidbody.velocity = MainRigidbody.velocity;
		}
		distanceToParrant = Vector3.Distance(parrantTransform.position, base.transform.position);
		if (distanceToParrant > StartDistanceToParant * 1.5f)
		{
			if ((bool)rigidbody)
			{
				rigidbody.Sleep();
			}
			base.transform.position = parrantTransform.position + (base.transform.position - parrantTransform.position).normalized * StartDistanceToParant;
			if ((bool)rigidbody)
			{
				rigidbody.WakeUp();
			}
		}
	}
}
