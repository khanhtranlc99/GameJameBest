using System;
using UnityEngine;

namespace RopeNamespace
{
	public class Rope : MonoBehaviour
	{
	 

		public Transform HookPlaceholder;

		private State state;

		public float StartDelay;

		private float increasingTime = 0.5f;

		private float straighteningTime = 0.5f;

		private float dragDecreasingTime = 0.1f;

		private float startTime;

		private bool failShoot;

		private Vector3 staticTarget;

		private Transform movingTarget;

		private Vector3 offset;

		private Material mat;

		private LineRenderer lr;

		private RopePointsMaker maker;

		private float t;
		private Vector3 firstPostTail;

		private Vector3 target
		{
			get
			{
				if ((bool)movingTarget)
				{
					return movingTarget.position + offset;
				}
				return staticTarget;
			}
		}

		public bool RopeEnabled => lr.enabled;

		private void Start()
		{
			state = State.disabled;
			lr = GetComponent<LineRenderer>();
			maker = new RopePointsMaker((float)Math.PI / 5f, 0.1f, 20f, 0.1f);
			mat = lr.sharedMaterial;
			lr.enabled = false;
		//	firstPostTail = tail.transform.position;
		}

		private void SetRopeExpandProgress(float n)
		{
			mat.SetFloat("_Progress", n);
		}

		public void ShootTarget(Vector3 target, float incTime, float strTime, bool useDelay = false)
		{
			base.transform.position = HookPlaceholder.position;
			base.transform.LookAt(target);
			movingTarget = null;
			float magnitude = (base.transform.position - target).magnitude;
			if (magnitude > 1f)
			{
				failShoot = false;
				staticTarget = target;
				increasingTime = incTime;
				straighteningTime = strTime;
				Vector3[] array = maker.CreateCurve(magnitude);
				startTime = Time.time;
				lr.positionCount = (array.Length);
				lr.SetPositions(array);
				state = State.increasing;
				lr.enabled = true;
		
			 
			}
		}
		Vector3 lastPoint;
		public void ShootMovingTarget(Transform target, Vector3 offset, float incTime, float strTime, bool useDelay = false)
		{
			base.transform.position = HookPlaceholder.position;
			base.transform.LookAt(target);
			movingTarget = target;
			this.offset = offset;
			failShoot = false;
			increasingTime = incTime;
			straighteningTime = strTime;
			Vector3[] array = maker.CreateCurve((base.transform.position - (movingTarget.position + offset)).magnitude);
			startTime = Time.time;
			lr.positionCount = (array.Length);
			lr.SetPositions(array);
			state = State.increasing;
			lr.enabled = true;
		//	firstPostTail = tail.transform.position;
		//	tail.transform.position = lr.GetPosition(lr.positionCount - 1);

		}



		public void ShootFail(Vector3 direction, float maxDistance, float incTime, float strTime, bool useDelay = false)
		{
			failShoot = true;
			base.transform.position = HookPlaceholder.position;
			base.transform.rotation = Quaternion.LookRotation(direction);
			movingTarget = base.transform;
			offset = direction * maxDistance;
			increasingTime = incTime;
			straighteningTime = strTime;
			Vector3[] array = maker.CreateCurve((base.transform.position - target).magnitude);
			startTime = Time.time;
			lr.positionCount = (array.Length);
			lr.SetPositions(array);
			state = State.increasing;
			lr.enabled = true;
		//	firstPostTail = tail.transform.position;
		//	tail.transform.position = lr.GetPosition(lr.positionCount - 1);
		}

		public void Disable()
		{
			state = State.disabled;
			lr.enabled = false;
		//	tail.transform.position = firstPostTail;
		 
		}

		public void Decrease()
		{
			startTime = Time.time;
			state = State.dragDecreasing;
		}

		private void Update()
		{
			switch (state)
			{
			case State.disabled:
				break;
			case State.fail:
				break;
			case State.increasing:
				base.transform.position = HookPlaceholder.position;
				base.transform.LookAt(target);
				t = Mathf.Clamp01((Time.time - startTime) / increasingTime);
				SetRopeExpandProgress(t);
				if (t >= 1f)
				{
					state = State.straightening;
				}
				break;
			case State.straightening:
				base.transform.position = HookPlaceholder.position;
				base.transform.LookAt(target);
				t = Mathf.Clamp01((Time.time - startTime - increasingTime) / straighteningTime);
				lr.SetPositions(maker.straighteningPoints(t));
				if (t >= 1f)
				{
					state = ((!failShoot) ? State.decreasing : State.dragDecreasing);
					lr.positionCount = (2);
					lr.SetPosition(0, Vector3.zero);
				}
				break;
			case State.decreasing:
				base.transform.position = HookPlaceholder.position;
				base.transform.LookAt(target);
				lr.SetPosition(1, base.transform.InverseTransformPoint(target));
				break;
			case State.dragDecreasing:
				base.transform.position = HookPlaceholder.position;
				base.transform.LookAt(target);
				t = Mathf.Clamp01((Time.time - startTime) / dragDecreasingTime);
				SetRopeExpandProgress(1f - t);
				if (t >= 1f)
				{
					Disable();
				}
				break;
			}
		}
	}
}
