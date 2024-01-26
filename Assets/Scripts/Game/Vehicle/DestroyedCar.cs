using Game.GlobalComponent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Vehicle
{
	public class DestroyedCar : MonoBehaviour
	{
		private class StartedTranform
		{
			public GameObject Object;

			public Vector3 LocalPosition;

			public Quaternion LocalRotation;
		}

		public Rigidbody[] LeftWheels;

		public Rigidbody[] RightWheels;

		public float WheelPushStrength = 10f;

		private readonly List<StartedTranform> startedTranforms = new List<StartedTranform>();

		public void Init()
		{
			if (startedTranforms.Count == 0)
			{
				AddTranformsInList(LeftWheels);
				AddTranformsInList(RightWheels);
			}
			ApplyForceOnToWheels(LeftWheels, -base.transform.right);
			ApplyForceOnToWheels(RightWheels, base.transform.right);
		}

		public void DeInitWithDelay(float delay)
		{
			StartCoroutine(DeInit(delay));
		}

		private IEnumerator DeInit(float delay)
		{
			yield return new WaitForSeconds(delay);
			foreach (StartedTranform st in startedTranforms)
			{
				st.Object.transform.parent = base.transform;
				st.Object.transform.localPosition = st.LocalPosition;
				st.Object.transform.localRotation = st.LocalRotation;
			}
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		private void AddTranformsInList(Rigidbody[] objects)
		{
			foreach (Rigidbody rigidbody in objects)
			{
				startedTranforms.Add(new StartedTranform
				{
					Object = rigidbody.gameObject,
					LocalPosition = rigidbody.transform.localPosition,
					LocalRotation = rigidbody.transform.localRotation
				});
			}
		}

		private void ApplyForceOnToWheels(Rigidbody[] wheels, Vector3 direction)
		{
			foreach (Rigidbody rigidbody in wheels)
			{
				rigidbody.transform.parent = null;
				rigidbody.AddForce(direction * WheelPushStrength);
			}
		}
	}
}
