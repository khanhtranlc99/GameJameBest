using UnityEngine;

namespace Game.GlobalComponent
{
	public class RealFixedJoint : MonoBehaviour
	{
		public bool Debug;

		public Transform Forearm;

		public Transform Hand;

		public Transform Target;

		public Transform RooTransform;

		public Vector3 OfsetRotation = new Vector3(90f, 0f, 0f);

		public Vector3 ForearmOfsetRotation = new Vector3(90f, 0f, 0f);

		public Vector3 HandOfsetRotation = new Vector3(90f, 0f, 0f);

		public float Transition = 0.1f;

		public float elbowAngle;

		private Transform armIK;

		private Transform armRotation;

		private float upperArmLength;

		private float forearmLength;

		private float armLength;

		private void Start()
		{
			GameObject gameObject = new GameObject("JointStart");
			armIK = gameObject.transform;
			GameObject gameObject2 = new GameObject("JointEnd");
			armRotation = gameObject2.transform;
			armRotation.parent = armIK;
			upperArmLength = Vector3.Distance(base.transform.position, Forearm.position);
			forearmLength = Vector3.Distance(Forearm.position, Hand.position);
			armLength = upperArmLength + forearmLength;
		}

		private void LateUpdate()
		{
			Quaternion rotation = base.transform.rotation;
			Quaternion rotation2 = Forearm.rotation;
			Quaternion rotation3 = RooTransform.rotation;
			RooTransform.rotation = Quaternion.identity;
			RooTransform.eulerAngles = OfsetRotation;
			armIK.position = base.transform.position;
			armIK.LookAt(Forearm);
			armRotation.position = base.transform.position;
			armRotation.rotation = base.transform.rotation;
			armIK.LookAt(Target);
			base.transform.rotation = armRotation.rotation;
			float a = Vector3.Distance(base.transform.position, Target.position);
			a = Mathf.Min(a, (float)((double)armLength - 0.1));
			float num = (upperArmLength * upperArmLength - forearmLength * forearmLength + a * a) / (2f * a);
			float num2 = Mathf.Acos(num / upperArmLength) * 57.29578f;
			if (!float.IsNaN(num2))
			{
				base.transform.RotateAround(base.transform.position, base.transform.forward, 0f - num2);
				armIK.position = Forearm.position;
				armIK.LookAt(Hand);
				armRotation.position = Forearm.position;
				armRotation.rotation = Forearm.rotation;
				armIK.LookAt(Target);
				Forearm.rotation = armRotation.rotation;
				base.transform.RotateAround(base.transform.position, Target.position - base.transform.position, elbowAngle);
				Transition = Mathf.Clamp01(Transition);
				base.transform.rotation = Quaternion.Slerp(rotation, base.transform.rotation, Transition);
				Forearm.rotation = Quaternion.Slerp(rotation2, Forearm.rotation, Transition);
				Forearm.eulerAngles += ForearmOfsetRotation;
				base.transform.eulerAngles += HandOfsetRotation;
				RooTransform.rotation = rotation3;
			}
		}
	}
}
