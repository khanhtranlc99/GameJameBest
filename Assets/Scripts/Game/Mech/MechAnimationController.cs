using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Mech
{
	public class MechAnimationController : MonoBehaviour
	{
		private const float turnBackInputValue = -0.9f;

		public bool isSpider;

		public LayerMask groundLayerMask = -1;

		public bool OnGround = true;

		private Animator animator;

		private IComparer<RaycastHit> rayHitComparer;

		private SlowUpdateProc slowUpdateProc;

		private Transform target;

		private DrivableMech drivableMech;

		private int TurnHash;

		private int ForwardHash;

		private int UpHash;

		private int DownHash;

		private int Right90Hash;

		private int Left90Hash;

		private int Turn180Hash;

		private int makeShootHash;

		private float forwardValue = 0.5f;

		private float turnLeftValue = -0.5f;

		private float turnRightValue = 0.5f;

		private bool makeShoot;

		private bool buttonsAvailableForUse = true;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			if (!animator)
			{
				animator = GetComponentInChildren<Animator>();
			}
			drivableMech = GetComponent<DrivableMech>();
			rayHitComparer = new RayHitComparer();
			GenerateAnimatorHashes();
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 0.1f);
		}

		private void GenerateAnimatorHashes()
		{
			ForwardHash = Animator.StringToHash("Forward");
			TurnHash = Animator.StringToHash("Turn");
			UpHash = Animator.StringToHash("Up");
			DownHash = Animator.StringToHash("Down");
			Right90Hash = Animator.StringToHash("Right90");
			Left90Hash = Animator.StringToHash("Left90");
			Turn180Hash = Animator.StringToHash("Turn180");
			makeShootHash = Animator.StringToHash("MakeShoot");
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			CheckIsOnGround();
		}

		public void Move(MechInputs inputs)
		{
			float y = inputs.move.y;
			float x = inputs.move.x;
			animator.SetFloat(ForwardHash, y);
			animator.SetFloat(TurnHash, x);
			if (!isSpider)
			{
				makeShoot = false;
				if (inputs.right90 && buttonsAvailableForUse)
				{
					animator.SetTrigger(Right90Hash);
					buttonsAvailableForUse = false;
				}
				else if (inputs.left90 && buttonsAvailableForUse)
				{
					animator.SetTrigger(Left90Hash);
					buttonsAvailableForUse = false;
				}
				else if (y <= -0.9f && buttonsAvailableForUse)
				{
					animator.SetTrigger(Turn180Hash);
					buttonsAvailableForUse = false;
				}
				else if (!inputs.right90 && !inputs.left90 && y >= -0.9f && !buttonsAvailableForUse)
				{
					buttonsAvailableForUse = true;
				}
				if (Mathf.Abs(x) < 0.1f && y < 0.1f && inputs.fire && drivableMech.GunController.DirectionPermittedForShooting(CameraManager.Instance.UnityCamera.transform.forward))
				{
					makeShoot = true;
				}
				animator.SetBool(makeShootHash, makeShoot);
			}
		}

		public void StandUp()
		{
			ResetMechPosition();
			animator.enabled = true;
			animator.SetTrigger(UpHash);
		}

		public void Down()
		{
			animator.SetTrigger(DownHash);
			Invoke("DisableAnimator", 2.8f);
		}

		private void CheckIsOnGround()
		{
			Ray ray = new Ray(base.transform.position + Vector3.up * 0.1f, -Vector3.up);
			RaycastHit[] array = Physics.RaycastAll(ray, 0.5f, groundLayerMask);
			Array.Sort(array, rayHitComparer);
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (!raycastHit.collider.isTrigger)
				{
					OnGround = true;
				}
			}
		}

		private void ResetMechPosition()
		{
			base.transform.rotation = Quaternion.identity;
		}

		private void DisableAnimator()
		{
			animator.enabled = false;
		}
	}
}
