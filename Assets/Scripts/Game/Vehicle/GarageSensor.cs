using Game.Character.CharacterController;
using Game.GlobalComponent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Vehicle
{
	public class GarageSensor : MonoBehaviour
	{
		public static GarageSensor Instance;

		public ControlableObjectRespawner GarageRespawner;

		private readonly List<VehicleSource> enteredVehicles = new List<VehicleSource>();

		private readonly List<VehicleSource> toRemove = new List<VehicleSource>();

		private Predicate<VehicleSource> toRemovePredicate;

		private SlowUpdateProc slowUpdateProc;

		private bool garageOccupied;

		private bool spawnRequest;

		private GameObject newVehicle;

		public void SpawnNewVehicle(GameObject newVehiclePrefab)
		{
			newVehicle = newVehiclePrefab;
			if (CheckSpawnAvaible())
			{
				GarageRespawner.SetNewObject(newVehicle, RespawnedObjectType.None, !spawnRequest);
				newVehicle = null;
				spawnRequest = false;
				return;
			}
			GarageRespawner.ObjectPrefab = newVehicle;
			if (base.isActiveAndEnabled)
			{
				spawnRequest = true;
			}
		}

		public bool CheckSpawnAvaible()
		{
			GameObject controlledObject = GarageRespawner.GetControlledObject();
			if (controlledObject != null && PlayerManager.Instance.Player.transform.IsChildOf(controlledObject.transform))
			{
				return false;
			}
			if (controlledObject != null && !enteredVehicles.Contains(controlledObject.GetComponent<DrivableVehicle>().GetVehicleSource()))
			{
				return false;
			}
			if (garageOccupied)
			{
				return false;
			}
			for (int i = 0; i < enteredVehicles.Count; i++)
			{
				VehicleSource vehicleSource = enteredVehicles[i];
				if (!PlayerManager.Instance.Player.transform.IsChildOf(vehicleSource.RootVehicle.transform) && PoolManager.Instance.ReturnToPool(vehicleSource.RootVehicle))
				{
					toRemove.Add(vehicleSource);
				}
			}
			enteredVehicles.RemoveAll(toRemovePredicate);
			toRemove.Clear();
			return enteredVehicles.Count == 0;
		}

		private void Awake()
		{
			Instance = this;
			slowUpdateProc = new SlowUpdateProc(SlowUpdate, 1f);
			toRemovePredicate = ((VehicleSource source) => toRemove.Contains(source));
		}

		private void OnEnable()
		{
			if (newVehicle != null)
			{
				SpawnNewVehicle(newVehicle);
			}
		}

		private void OnDisable()
		{
			spawnRequest = false;
		}

		private void FixedUpdate()
		{
			slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (spawnRequest)
			{
				SpawnNewVehicle(newVehicle);
			}
			garageOccupied = false;
		}

		private void OnTriggerEnter(Collider other)
		{
			VehicleSource component = other.GetComponent<VehicleSource>();
			if (!(component == null) && !enteredVehicles.Contains(component))
			{
				enteredVehicles.Add(component);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			VehicleSource component = other.GetComponent<VehicleSource>();
			if (!(component == null) && enteredVehicles.Contains(component))
			{
				enteredVehicles.Remove(component);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			VehicleSource component = other.GetComponent<VehicleSource>();
			if (!(component != null))
			{
				garageOccupied = true;
			}
		}
	}
}
