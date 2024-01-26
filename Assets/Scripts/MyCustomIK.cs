using System;
using UnityEngine;

public class MyCustomIK : MonoBehaviour
{
	[Serializable]
	public class Limb
	{
		public string LimbName;

		public Transform upperArm;

		public Transform forearm;

		public Transform hand;

		public Transform target;

		public Transform elbowTarget;

		private Vector3 LastFrameTargetPosition;

		private Vector3 LastFrameElbowTargetPosition;

		private Quaternion LastFrameTargetRotation;

		private Quaternion LastFrameElbowTargetRotation;

		public bool IsEnabled;

		public float Weight = 1f;

		private Quaternion upperArmStartRotation;

		private Quaternion forearmStartRotation;

		private Quaternion handStartRotation;

		private Vector3 targetRelativeStartPosition;

		private Vector3 elbowTargetRelativeStartPosition;

		private GameObject upperArmAxisCorrection;

		private GameObject forearmAxisCorrection;

		private GameObject handAxisCorrection;

		private GameObject parentForAxis;

		public void Initialialization(MyCustomIK IKObject)
		{
			upperArmStartRotation = upperArm.rotation;
			forearmStartRotation = forearm.rotation;
			handStartRotation = hand.rotation;
			elbowTargetRelativeStartPosition = elbowTarget.position - upperArm.position;
			parentForAxis = new GameObject("IK_" + LimbName);
			upperArmAxisCorrection = new GameObject("upperArmAxisCorrection_" + LimbName);
			forearmAxisCorrection = new GameObject("forearmAxisCorrection_" + LimbName);
			handAxisCorrection = new GameObject("handAxisCorrection_" + LimbName);
			parentForAxis.transform.parent = IKObject.transform;
			upperArmAxisCorrection.transform.parent = parentForAxis.transform;
			forearmAxisCorrection.transform.parent = upperArmAxisCorrection.transform;
			handAxisCorrection.transform.parent = forearmAxisCorrection.transform;
			LastFrameTargetPosition = default(Vector3);
			LastFrameElbowTargetPosition = default(Vector3);
		}

		public void CalculateIK()
		{
			if (target == null)
			{
				targetRelativeStartPosition = Vector3.zero;
				return;
			}
			if (targetRelativeStartPosition == Vector3.zero && target != null)
			{
				targetRelativeStartPosition = target.position - upperArm.position;
			}
			if (upperArm != null && forearm != null && hand != null)
			{
				float num = Vector3.Distance(upperArm.position, forearm.position);
				float num2 = Vector3.Distance(forearm.position, hand.position);
				float num3 = num + num2;
				float num4 = num;
				float a = Vector3.Distance(upperArm.position, target.position);
				a = Mathf.Min(a, num3 - 0.0001f);
				float num5 = (num4 * num4 - num2 * num2 + a * a) / (2f * a);
				float x = Mathf.Acos(num5 / num4) * 57.29578f;
				Vector3 position = target.position;
				Vector3 position2 = elbowTarget.position;
				Transform parent = upperArm.parent;
				Transform parent2 = forearm.parent;
				Transform parent3 = hand.parent;
				Vector3 localScale = upperArm.localScale;
				Vector3 localScale2 = forearm.localScale;
				Vector3 localScale3 = hand.localScale;
				Vector3 localPosition = upperArm.localPosition;
				Vector3 localPosition2 = forearm.localPosition;
				Vector3 localPosition3 = hand.localPosition;
				Quaternion rotation = upperArm.rotation;
				Quaternion rotation2 = forearm.rotation;
				Quaternion rotation3 = hand.rotation;
				Quaternion localRotation = hand.localRotation;
				target.position = targetRelativeStartPosition + upperArm.position;
				elbowTarget.position = elbowTargetRelativeStartPosition + upperArm.position;
				upperArm.rotation = upperArmStartRotation;
				forearm.rotation = forearmStartRotation;
				hand.rotation = handStartRotation;
				parentForAxis.transform.position = upperArm.position;
				parentForAxis.transform.LookAt(position, position2 - parentForAxis.transform.position);
				upperArmAxisCorrection.transform.position = upperArm.position;
				upperArmAxisCorrection.transform.LookAt(forearm.position, Vector3.up);
				upperArm.parent = upperArmAxisCorrection.transform;
				forearmAxisCorrection.transform.position = forearm.position;
				forearmAxisCorrection.transform.LookAt(hand.position, Vector3.up);
				forearm.parent = forearmAxisCorrection.transform;
				handAxisCorrection.transform.position = hand.position;
				hand.parent = handAxisCorrection.transform;
				target.position = position;
				elbowTarget.position = position2;
				upperArmAxisCorrection.transform.LookAt(target, Vector3.up);
				upperArmAxisCorrection.transform.localRotation = Quaternion.Euler(upperArmAxisCorrection.transform.localRotation.eulerAngles - new Vector3(x, 0f, 0f));
				upperArmAxisCorrection.transform.LookAt(forearmAxisCorrection.transform, Vector3.up);
				forearmAxisCorrection.transform.LookAt(target, Vector3.up);
				handAxisCorrection.transform.rotation = target.rotation;
				upperArmAxisCorrection.transform.LookAt(forearmAxisCorrection.transform, Vector3.up);
				Weight = Mathf.Clamp01(Weight);
				upperArm.parent = parent;
				upperArm.localScale = localScale;
				upperArm.localPosition = localPosition;
				forearm.parent = parent2;
				forearm.localScale = localScale2;
				forearm.localPosition = localPosition2;
				hand.parent = parent3;
				hand.localScale = localScale3;
				hand.localPosition = localPosition3;
				hand.rotation = target.rotation;
				LastFrameTargetPosition = target.position;
				LastFrameElbowTargetPosition = elbowTarget.position;
				LastFrameTargetRotation = target.rotation;
				LastFrameElbowTargetRotation = elbowTarget.rotation;
			}
		}

		public bool NeedToRecalculate()
		{
			return LastFrameTargetPosition != target.position || LastFrameElbowTargetPosition != elbowTarget.position || LastFrameTargetRotation != target.rotation || LastFrameElbowTargetRotation != elbowTarget.rotation;
		}
	}

	public Limb[] Limbs;

	private void Start()
	{
		for (int i = 0; i < Limbs.Length; i++)
		{
			Limb limb = Limbs[i];
			limb.Initialialization(this);
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < Limbs.Length; i++)
		{
			Limb limb = Limbs[i];
			if (limb.IsEnabled && limb.NeedToRecalculate())
			{
				limb.CalculateIK();
			}
		}
	}
}
