using Game.Enemy;
using Game.Vehicle;
using UnityEngine;

public class CarTransformer : MonoBehaviour
{
	public GameObject NPCRobotPrefab;

	private void Start()
	{
		VehicleStatus componentInChildren = GetComponentInChildren<VehicleStatus>();
		HumanoidStatusNPC component = NPCRobotPrefab.GetComponent<HumanoidStatusNPC>();
		componentInChildren.Health = component.Health;
		componentInChildren.Defence.Set(component.Defence);
		componentInChildren.ExperienceForAKill = component.ExperienceForAKill;
	}
}
