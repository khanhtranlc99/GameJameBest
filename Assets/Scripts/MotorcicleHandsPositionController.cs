using System;
using UnityEngine;

public class MotorcicleHandsPositionController : MonoBehaviour
{
	[Serializable]
	public class BoneHandler
	{
		private BoneState curState;

		public Transform bone;

		public BoneState TurnLeftState;

		public BoneState MiddleState;

		public BoneState TurnRightState;

		public void Lerp(float t)
		{
			curState = ((!(t < 0f)) ? TurnRightState : TurnLeftState);
			bone.localRotation = Quaternion.Lerp(MiddleState.QRotation, curState.QRotation, Mathf.Abs(t));
		}
	}

	[Serializable]
	public class BoneState
	{
		public Vector3 rotation;

		private Quaternion _QRotation;

		private bool rotInit;

		public Quaternion QRotation
		{
			get
			{
				if (!rotInit)
				{
					_QRotation = Quaternion.Euler(rotation);
					rotInit = true;
				}
				return _QRotation;
			}
		}
	}

	[Serializable]
	public class Arm
	{
		public BoneHandler Upperarm;

		public BoneHandler Forearm;

		public void Lerp(float t)
		{
			Upperarm.Lerp(t);
			Forearm.Lerp(t);
		}
	}

	[Serializable]
	public class Spine
	{
		public Transform spineTransform;

		public Vector3 maxSpinePosition;

		public Vector3 zeroSpinePosition;

		public Transform chestTransform;

		public Vector3 maxChestPosition;

		public Vector3 zeroChestPosition;
	}

	public Spine spine;

	public Arm LeftArm;

	public Arm RightArm;

	public Transform SteeringWheel;

	public float MaxSteeringAngle;

	public float ZeroSteeringAngle;

	private void Update()
	{
		Vector3 eulerAngles = SteeringWheel.localRotation.eulerAngles;
		float num = Mathf.Clamp((eulerAngles.z - ZeroSteeringAngle) / MaxSteeringAngle, -1f, 1f);
		LeftArm.Lerp(num);
		RightArm.Lerp(num);
		if (spine.zeroSpinePosition != Vector3.zero)
		{
			spine.spineTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(spine.zeroSpinePosition), Quaternion.Euler(spine.maxSpinePosition), Mathf.Abs(num));
		}
		if (spine.zeroChestPosition != Vector3.zero)
		{
			spine.chestTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(spine.zeroChestPosition), Quaternion.Euler(spine.maxChestPosition), Mathf.Abs(num));
		}
	}

	public void SetArms(Transform lUpperArm, Transform lForeArm, Transform rUpperArm, Transform rForeArm)
	{
		LeftArm.Forearm.bone = lForeArm;
		LeftArm.Upperarm.bone = lUpperArm;
		RightArm.Forearm.bone = rForeArm;
		RightArm.Upperarm.bone = rUpperArm;
	}

	public void SetMiddlePosition()
	{
		LeftArm.Forearm.MiddleState.rotation = LeftArm.Forearm.bone.localRotation.eulerAngles;
		LeftArm.Upperarm.MiddleState.rotation = LeftArm.Upperarm.bone.localRotation.eulerAngles;
		RightArm.Forearm.MiddleState.rotation = RightArm.Forearm.bone.localRotation.eulerAngles;
		RightArm.Upperarm.MiddleState.rotation = RightArm.Upperarm.bone.localRotation.eulerAngles;
		if ((bool)spine.spineTransform)
		{
			spine.zeroSpinePosition = spine.spineTransform.localEulerAngles;
		}
		if ((bool)spine.chestTransform)
		{
			spine.zeroChestPosition = spine.chestTransform.localEulerAngles;
		}
	}

	public void SetRightPosition()
	{
		LeftArm.Forearm.TurnRightState.rotation = LeftArm.Forearm.bone.localRotation.eulerAngles;
		LeftArm.Upperarm.TurnRightState.rotation = LeftArm.Upperarm.bone.localRotation.eulerAngles;
		RightArm.Forearm.TurnRightState.rotation = RightArm.Forearm.bone.localRotation.eulerAngles;
		RightArm.Upperarm.TurnRightState.rotation = RightArm.Upperarm.bone.localRotation.eulerAngles;
		if ((bool)spine.spineTransform)
		{
			spine.maxSpinePosition = spine.spineTransform.localEulerAngles;
		}
		if ((bool)spine.chestTransform)
		{
			spine.maxChestPosition = spine.chestTransform.localEulerAngles;
		}
	}

	public void SetLeftPosition()
	{
		LeftArm.Forearm.TurnLeftState.rotation = LeftArm.Forearm.bone.localRotation.eulerAngles;
		LeftArm.Upperarm.TurnLeftState.rotation = LeftArm.Upperarm.bone.localRotation.eulerAngles;
		RightArm.Forearm.TurnLeftState.rotation = RightArm.Forearm.bone.localRotation.eulerAngles;
		RightArm.Upperarm.TurnLeftState.rotation = RightArm.Upperarm.bone.localRotation.eulerAngles;
		if ((bool)spine.spineTransform)
		{
			spine.maxSpinePosition = spine.spineTransform.localEulerAngles;
		}
		if ((bool)spine.chestTransform)
		{
			spine.maxChestPosition = spine.chestTransform.localEulerAngles;
		}
	}

	public void SetCharacterMiddlePosition()
	{
		LeftArm.Forearm.bone.localRotation = Quaternion.Euler(LeftArm.Forearm.MiddleState.rotation.x, LeftArm.Forearm.MiddleState.rotation.y, LeftArm.Forearm.MiddleState.rotation.z);
		LeftArm.Upperarm.bone.localRotation = Quaternion.Euler(LeftArm.Upperarm.MiddleState.rotation.x, LeftArm.Upperarm.MiddleState.rotation.y, LeftArm.Upperarm.MiddleState.rotation.z);
		RightArm.Forearm.bone.localRotation = Quaternion.Euler(RightArm.Forearm.MiddleState.rotation.x, RightArm.Forearm.MiddleState.rotation.y, RightArm.Forearm.MiddleState.rotation.z);
		RightArm.Upperarm.bone.localRotation = Quaternion.Euler(RightArm.Upperarm.MiddleState.rotation.x, RightArm.Upperarm.MiddleState.rotation.y, RightArm.Upperarm.MiddleState.rotation.z);
		SteeringWheel.localRotation = Quaternion.Euler(0f, 0f, ZeroSteeringAngle);
		if ((bool)spine.spineTransform)
		{
			spine.spineTransform.localRotation = Quaternion.Euler(spine.zeroSpinePosition.x, spine.zeroSpinePosition.y, spine.zeroSpinePosition.z);
		}
		if ((bool)spine.chestTransform)
		{
			spine.chestTransform.localRotation = Quaternion.Euler(spine.zeroChestPosition.x, spine.zeroChestPosition.y, spine.zeroChestPosition.z);
		}
	}

	public void SetCharacterRightPosition()
	{
		LeftArm.Forearm.bone.localRotation = Quaternion.Euler(LeftArm.Forearm.TurnRightState.rotation.x, LeftArm.Forearm.TurnRightState.rotation.y, LeftArm.Forearm.TurnRightState.rotation.z);
		LeftArm.Upperarm.bone.localRotation = Quaternion.Euler(LeftArm.Upperarm.TurnRightState.rotation.x, LeftArm.Upperarm.TurnRightState.rotation.y, LeftArm.Upperarm.TurnRightState.rotation.z);
		RightArm.Forearm.bone.localRotation = Quaternion.Euler(RightArm.Forearm.TurnRightState.rotation.x, RightArm.Forearm.TurnRightState.rotation.y, RightArm.Forearm.TurnRightState.rotation.z);
		RightArm.Upperarm.bone.localRotation = Quaternion.Euler(RightArm.Upperarm.TurnRightState.rotation.x, RightArm.Upperarm.TurnRightState.rotation.y, RightArm.Upperarm.TurnRightState.rotation.z);
		SteeringWheel.localRotation = Quaternion.Euler(0f, 0f, ZeroSteeringAngle + MaxSteeringAngle);
		if ((bool)spine.spineTransform)
		{
			spine.spineTransform.localRotation = Quaternion.Euler(spine.maxSpinePosition.x, spine.maxSpinePosition.y, spine.maxSpinePosition.z);
		}
		if ((bool)spine.chestTransform)
		{
			spine.chestTransform.localRotation = Quaternion.Euler(spine.maxChestPosition.x, spine.maxChestPosition.y, spine.maxChestPosition.z);
		}
	}

	public void SetCharacterLeftPosition()
	{
		LeftArm.Forearm.bone.localRotation = Quaternion.Euler(LeftArm.Forearm.TurnLeftState.rotation.x, LeftArm.Forearm.TurnLeftState.rotation.y, LeftArm.Forearm.TurnLeftState.rotation.z);
		LeftArm.Upperarm.bone.localRotation = Quaternion.Euler(LeftArm.Upperarm.TurnLeftState.rotation.x, LeftArm.Upperarm.TurnLeftState.rotation.y, LeftArm.Upperarm.TurnLeftState.rotation.z);
		RightArm.Forearm.bone.localRotation = Quaternion.Euler(RightArm.Forearm.TurnLeftState.rotation.x, RightArm.Forearm.TurnLeftState.rotation.y, RightArm.Forearm.TurnLeftState.rotation.z);
		RightArm.Upperarm.bone.localRotation = Quaternion.Euler(RightArm.Upperarm.TurnLeftState.rotation.x, RightArm.Upperarm.TurnLeftState.rotation.y, RightArm.Upperarm.TurnLeftState.rotation.z);
		SteeringWheel.localRotation = Quaternion.Euler(0f, 0f, ZeroSteeringAngle - MaxSteeringAngle);
		if ((bool)spine.spineTransform)
		{
			spine.spineTransform.localRotation = Quaternion.Euler(spine.maxSpinePosition.x, spine.maxSpinePosition.y, spine.maxSpinePosition.z);
		}
		if ((bool)spine.chestTransform)
		{
			spine.chestTransform.localRotation = Quaternion.Euler(spine.maxChestPosition.x, spine.maxChestPosition.y, spine.maxChestPosition.z);
		}
	}
}
