using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyMission", menuName = "DailyMission/NewMission", order = 100)]
public class DailyMissionData : ScriptableObject
{
    public DailyMission[] AllMissions;
    public int money;
    public int gems;
}

[System.Serializable]
public struct DailyMission
{
    public DailyMissionType dailyMissionType;
    public string description;
    public int goalAmount;
}

public enum DailyMissionType
{
    KILL_HUMANS = 0,
    DESTROY_CARS = 1,
    DESTROY_MOTORS =2,
}
