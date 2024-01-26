using Game.GlobalComponent;
using Game.Items;
using Game.Shop;
using Game.Vehicle;
using UnityEngine;

public class GarageManager : MonoBehaviour
{
	private const string VehicleKey = "MainVehicle";

	private static GarageManager instance;

	public ControlableObjectRespawner MainRespawner;

	public GarageSensor GarageSensor;

	public static GarageManager Instance => (instance != null) ? instance : (instance = UnityEngine.Object.FindObjectOfType<GarageManager>());

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		ResetVehicle();
	}

	public void SetVehicle(GameItemVehicle vehicle)
	{
		if (!(vehicle.VehiclePrefab == MainRespawner.ObjectPrefab))
		{
			BaseProfile.StoreValue(vehicle.ID, "MainVehicle");
			GarageSensor.SpawnNewVehicle(vehicle.VehiclePrefab);
		}
	}

	private void ResetVehicle()
	{
		int num = BaseProfile.ResolveValue("MainVehicle", 0);
		if (num != 0 && ShopManager.Instance.BoughtAlredy(num))
		{
			GameItemVehicle gameItemVehicle = ItemsManager.Instance.GetItem(num) as GameItemVehicle;
			if (gameItemVehicle != null)
			{
				MainRespawner.SetNewObject(gameItemVehicle.VehiclePrefab);
			}
		}
	}
}
