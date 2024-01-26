using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Game.PickUps
{
	public class PickUp : MonoBehaviour
	{
		public GameObject RotateCenter;

		private SlowUpdateProc slowUpdateProc;

		public void Awake()
		{
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 2f);
		}

		private void Update()
		{
			RotateCenter.transform.Rotate(Vector3.up);
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (!SectorManager.Instance.IsInActiveSector(base.transform.position))
			{
				ReturnPickup();
			}
		}

		private void ReturnPickup()
		{
			if (!PoolManager.Instance.ReturnToPool(base.gameObject))
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		protected virtual void TakePickUp()
		{
			PickUpManager.Instance.OnTakedPickup(this);
			ReturnPickup();
		}

		private void OnTriggerEnter(Collider col)
		{
			if (col.gameObject.GetComponent<CharacterSensor>() !=null)
			{
				//Debug.LogError("Meet Character");
				TakePickUp();
				return;
			}
			VehicleStatus componentInParent = col.gameObject.GetComponentInParent<VehicleStatus>();
			if (componentInParent != null && componentInParent.Faction == Faction.Player)
			{
				//Debug.LogError("Meet Character bike");
				TakePickUp();
			}
		}
	}
}
