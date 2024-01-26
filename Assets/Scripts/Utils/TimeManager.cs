using System;
using System.Collections;
using System.Collections.Generic;
using UI.DailyMission;
using UnityEngine;

namespace Tuhai993Utils
{
    

public class TimeManager : SingletonMonoBehavior<TimeManager>
{
    private const string KEY_TIME_MANAGER_SAVE_DAY = "KEY_TIME_MANAGER_SAVE_DAY";
    
    #if DEBUG_BUILD
    public static bool isEnableFakeTime = false;
    #endif

    private void Start()
    {
        DateTime nowTime = GetNow;
        DateTime savedTime = PlayerPrefs.HasKey(KEY_TIME_MANAGER_SAVE_DAY)
            ? PlayerPrefsX.GetDateTime(KEY_TIME_MANAGER_SAVE_DAY)
            : nowTime;
        int nowDayOfYear = nowTime.DayOfYear;
        if (nowTime.Year != savedTime.Year)
            nowDayOfYear += 365;
        int dayAfterLastTimePlay = nowDayOfYear - savedTime.DayOfYear;
        DayAfterLastTimePlay(dayAfterLastTimePlay);
        PlayerPrefsX.SetDateTime(KEY_TIME_MANAGER_SAVE_DAY,nowTime);
    }

    private void DayAfterLastTimePlay(int day)
    {
        DailyMissionManager.OnDayChange(day);
    }

    public static DateTime GetNow
    {
        get
        {
#if DEBUG_BUILD
            return isEnableFakeTime ? DateTime.Now : UnbiasedTime.Instance.Now();
#endif
            //return UnbiasedTime.Instance.Now();
            return DateTime.Now;
        }
    }
}
}
