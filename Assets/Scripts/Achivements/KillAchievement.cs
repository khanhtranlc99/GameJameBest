using Game.Character.CharacterController;
using Game.Vehicle;
using UnityEngine;

public class KillAchievement : Achievement
{
	private const int ACHIEVTARGERT = 50;

	public override void Init()
	{
		achiveParams = new SaveLoadAchievmentStruct(false, 0, 50);
	}

	public override void NpcKilledEvent(Vector3 position, Faction npcFaction, HitEntity victim, HitEntity killer)
	{
		if (!(victim is VehicleStatus) && killer is Player)
		{
			achiveParams.achiveCounter++;
			if (achiveParams.achiveCounter >= achiveParams.achiveTarget)
			{
				AchievComplite();
			}
		}
	}
}
