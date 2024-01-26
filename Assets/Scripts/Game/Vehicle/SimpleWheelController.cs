using Game.GlobalComponent;
using UnityEngine;

namespace Game.Vehicle
{
	[RequireComponent(typeof(WheelCollider))]
	public class SimpleWheelController : MonoBehaviour
	{
		public WheelCollider Wheel;

		public Transform WheelModel;

		public Transform WheelPoint;

		public bool IsBikeWheel;

		private float maxWheelRadius;

		private float startWheelRadius;

		private SlowUpdateProc slowUpdate;

		private void Awake()
		{
			if (WheelModel == null && base.transform.childCount > 0)
			{
				WheelModel = base.transform.GetChild(0);
			}
			if (Wheel == null)
			{
				Wheel = GetComponent<WheelCollider>();
			}
			startWheelRadius = Wheel.radius;
			maxWheelRadius = startWheelRadius * 1.5f;
			slowUpdate = new SlowUpdateProc(SlowUpdate, 0.1f);
		}

		private void Update()
		{
			if (!(WheelModel == null) && !(Wheel == null))
			{
				Vector3 pos = Vector3.zero;
				Quaternion quat = default(Quaternion);
				Wheel.GetWorldPose(out pos, out quat);
				if (IsBikeWheel && (bool)WheelPoint)
				{
					Transform wheelModel = WheelModel;
					Vector3 position = WheelPoint.position;
					float x = position.x;
					float y = pos.y;
					Vector3 position2 = WheelPoint.position;
					wheelModel.position = new Vector3(x, y, position2.z);
				}
				else
				{
					WheelModel.position = pos;
				}
				WheelModel.rotation = quat;
			}
		}

		private void FixedUpdate()
		{
			slowUpdate.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (!Wheel)
			{
				return;
			}
			if (!Wheel.isGrounded)
			{
				if (Wheel.radius < maxWheelRadius)
				{
					Wheel.radius += Time.deltaTime;
				}
			}
			else if (Wheel.radius > startWheelRadius)
			{
				float num = Wheel.radius - Time.deltaTime * 2f;
				Wheel.radius = ((!(num >= startWheelRadius)) ? startWheelRadius : num);
			}
		}

		public void ResetWheelCollider()
		{
			Wheel.radius = startWheelRadius;
		}
	}
}
