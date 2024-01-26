using Game.Character.CharacterController;
using Game.Managers;
using System;
using UnityEngine;

namespace Game.Enemy
{
	public class SimpleHumanoidController : BaseControllerNPC
	{
		[Separator("Simple Controller Paremetrs")]
		public bool DebugLog_SimpleHumanoidController;

		public float StoppingDistance = 1f;

		public float RotateSpeed = 1f;

		[Separator("Simple Controller Links")]
		public DummyNPCSensor ObstacleSensor;

		protected Vector3 MovePoint;

		private bool isMoving;

		public bool IsMoving => isMoving;

		public override void Init(BaseNPC controlledNPC)
		{
			base.Init(controlledNPC);
			MovePoint = CurrentControlledNpc.transform.position;
			BaseStatusNPC statusNpc = CurrentControlledNpc.StatusNpc;
			statusNpc.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Combine(statusNpc.OnHitEvent, new HitEntity.HealthChagedEvent(BecomeSmartOnHit));
		}

		public override void DeInit()
		{
			BaseStatusNPC statusNpc = CurrentControlledNpc.StatusNpc;
			statusNpc.OnHitEvent = (HitEntity.HealthChagedEvent)Delegate.Remove(statusNpc.OnHitEvent, new HitEntity.HealthChagedEvent(BecomeSmartOnHit));
			base.DeInit();
		}

		public void SetMovePoint(Vector3 newPoint)
		{
			MovePoint = newPoint;
		}

		protected override void Update()
		{
			if (base.IsInited)
			{
				base.Update();
				float num = Vector3.Distance(CurrentControlledNpc.transform.position, MovePoint);
				isMoving = (num > StoppingDistance && ObstacleSensor.CanMove);
				LerpLookToPoint(MovePoint);
				if (isMoving)
				{
					MoveToPoint(MovePoint);
				}
				else
				{
					StayOnPosition();
				}
			}
		}

		private void LerpLookToPoint(Vector3 point)
		{
			float x = point.x;
			Vector3 position = CurrentControlledNpc.transform.position;
			Vector3 a = new Vector3(x, position.y, point.z);
			Vector3 normalized = (a - CurrentControlledNpc.transform.position).normalized;
			float num = Vector3.Dot(normalized, CurrentControlledNpc.transform.forward);
			float num2 = 0.5f - num * 0.5f;
			if (num2 > float.Epsilon)
			{
				float t = Mathf.Min(1f, RotateSpeed * Time.deltaTime / num2);
				CurrentControlledNpc.transform.forward = Vector3.Slerp(CurrentControlledNpc.transform.forward, normalized, t);
			}
		}

		private void MoveToPoint(Vector3 point)
		{
			MovePoint = point;
			CurrentControlledNpc.NPCAnimator.SetBool("Walk", value: true);
		}

		private void StayOnPosition()
		{
			CurrentControlledNpc.NPCAnimator.SetBool("Walk", value: false);
		}

		private void BecomeSmartOnHit(HitEntity disturber)
		{
			BaseControllerNPC controller;
			CurrentControlledNpc.ChangeController(BaseNPC.NPCControllerType.Smart, out controller);
			SmartHumanoidController smartHumanoidController = controller as SmartHumanoidController;
			if (smartHumanoidController != null)
			{
				smartHumanoidController.InitBackToDummyLogic();
				if (DebugLog_SimpleHumanoidController && GameManager.ShowDebugs)
				{
					UnityEngine.Debug.Log(disturber);
				}
				smartHumanoidController.AddPersonalTarget(disturber);
			}
		}
	}
}
