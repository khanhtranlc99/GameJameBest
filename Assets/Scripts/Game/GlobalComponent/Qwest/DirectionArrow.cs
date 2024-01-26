using Game.Character;
using Game.Vehicle;
using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class DirectionArrow : MonoBehaviour
	{
		private const float AmpValue = 0.3f;

		public Vector3 Shift;

		public float AnimSpeed = 0.4f;

		private float animValue;

		private Vector3 currShift;

		private void Start()
		{
			currShift = Shift + PlayerInteractionsManager.Instance.Player.NPCShootVectorOffset;
		}

		public void RefreshShift(Vector3 offset)
		{
			currShift = Shift + offset;
		}

		private void FixedUpdate()
		{
			if (GameEventManager.Instance.MarkedQwest != null && GameEventManager.Instance.MarkedQwest.GetQwestTarget() != null)
			{
				Vector3 position;
				if (PlayerInteractionsManager.Instance.IsDrivingAVehicle())
				{
					DrivableVehicle lastDrivableVehicle = PlayerInteractionsManager.Instance.LastDrivableVehicle;
					Vector3 a = lastDrivableVehicle.transform.TransformPoint(lastDrivableVehicle.VehicleSpecificPrefab.ArrowPosOffset);
					position = a + base.transform.forward * Mathf.Cos(animValue) * 0.3f;
				}
				else
				{
					Transform ragdollHips = PlayerInteractionsManager.Instance.Player.GetRagdollHips();
					if (ragdollHips != null)
					{
						Transform transform = ragdollHips;
						position = Vector3.Lerp(base.transform.position, transform.position + Shift + base.transform.forward * Mathf.Cos(animValue) * 0.3f, Time.deltaTime * 5f);
					}
					else
					{
						Transform transform = PlayerInteractionsManager.Instance.Player.transform;
						position = transform.position + currShift + base.transform.forward * Mathf.Cos(animValue) * 0.3f;
					}
				}
				base.transform.position = position;
				base.transform.LookAt(GameEventManager.Instance.MarkedQwest.GetQwestTarget());
				animValue += AnimSpeed;
				if (animValue > (float)Math.PI)
				{
					animValue = -(float)Math.PI;
				}
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
