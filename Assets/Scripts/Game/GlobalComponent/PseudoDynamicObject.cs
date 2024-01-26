using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class PseudoDynamicObject : MonoBehaviour
	{
		private const float RigidbodyClampSpeedPeriod = 5f;

		public int BodyMass = 100;

		public float StayImpulse = 1000f;

		public float MaxVelocity = 75f;

		public bool isAnimated;

		public Animation animation;

		public bool IsDebug;

		private Vector3 initialPosition;

		private Quaternion initialRotation;

		private Rigidbody ownRigidbody;

		private bool alreadyDynamic;

		private void Awake()
		{
			initialPosition = transform.position;
			initialRotation = transform.rotation;
			MeshCollider component = GetComponent<MeshCollider>();
			if (component != null)
			{
				component.convex = true;
			}
		}

		private void OnDisable()
		{
			if (!gameObject.activeInHierarchy && ownRigidbody != null)
			{
				Destroy(ownRigidbody);
				ownRigidbody = null;
				alreadyDynamic = false;
				transform.position = initialPosition;
				transform.rotation = initialRotation;
				if (isAnimated)
				{
					GetComponent<Animation>().enabled = true;
				}
				StopAllCoroutines();
			}
		}

		public void ReplaceOnDynamic([Optional] Vector3 force, [Optional] Vector3 direction)
		{
			if (isAnimated)
			{
				GetComponent<Animation>().enabled = false;
			}
			if (!alreadyDynamic)
			{
				alreadyDynamic = true;
				ownRigidbody = base.gameObject.AddComponent<Rigidbody>();
				if (!ownRigidbody)
				{
					return;
				}
				ownRigidbody.mass = BodyMass;
				StartCoroutine(ClampRigidbodySpeed(5f));
			}
			ownRigidbody.AddForce(force, ForceMode.Impulse);
			ownRigidbody.AddTorque(direction, ForceMode.Impulse);
		}

		private void OnCollisionEnter(Collision col)
		{
			if (alreadyDynamic)
			{
				return;
			}
			Rigidbody rigidbody = col.rigidbody;
			if (rigidbody == null)
			{
				return;
			}
			if (IsDebug)
			{
				UnityEngine.Debug.LogFormat("PDO collision impule = {0}", col.impulse.magnitude);
			}
			if (!(col.impulse.magnitude < StayImpulse))
			{
				if (isAnimated)
				{
					GetComponent<Animation>().enabled = false;
				}
				rigidbody.velocity = col.relativeVelocity;
				ReplaceOnDynamic();
			}
		}

		private IEnumerator ClampRigidbodySpeed(float time)
		{
			WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
			while (time > 0f)
			{
				yield return waitForEndOfFrame;
				time -= Time.deltaTime;
				if (alreadyDynamic && ownRigidbody.velocity.magnitude > MaxVelocity)
				{
					ownRigidbody.velocity = ownRigidbody.velocity.normalized * MaxVelocity;
				}
			}
		}
	}
}
