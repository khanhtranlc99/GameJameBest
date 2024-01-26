using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UseProfile : MonoBehaviour
{
    public static int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.CURRENT_LEVEL, 1);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.CURRENT_LEVEL, value);
            PlayerPrefs.Save();
        }
    }

    public int CurrentLevelPlay
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.CURRENT_LEVEL_PLAY, 1);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.CURRENT_LEVEL_PLAY, value);
            PlayerPrefs.Save();
        }
    }

    public bool IsRemoveAds
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.REMOVE_ADS, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.REMOVE_ADS, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool IsFirstTimeInstall
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.FIRST_TIME_INSTALL, 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.FIRST_TIME_INSTALL, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int RetentionD
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.RETENTION_D, 0);
        }
        set
        {
            if (value < 0)
                value = 0;

            PlayerPrefs.SetInt(StringHelper.RETENTION_D, value);
            PlayerPrefs.Save();
        }
    }
    public static bool LoggedRevenueCents
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.LOGGED_REVENUE_CENTS, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.LOGGED_REVENUE_CENTS, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static int PreviousRevenueD0_D1
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.PREVIOUS_REVENUE_D0_D1, 0);
        }
        set
        {
            if (value < 0)
                value = 0;

            PlayerPrefs.SetInt(StringHelper.PREVIOUS_REVENUE_D0_D1, value);
            PlayerPrefs.Save();
        }
    }

    public static float RevenueD0_D1
    {
        get
        {
            return PlayerPrefs.GetFloat(StringHelper.REVENUE_D0_D1, 0);
        }
        set
        {
            if (value < 0f)
                value = 0f;

            PlayerPrefs.SetFloat(StringHelper.REVENUE_D0_D1, value);
            PlayerPrefs.Save();
        }
    }
    public static int NumberOfDisplayedInterstitialD0_D1
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.NUMBER_OF_DISPLAYED_INTERSTITIAL_D0_D1, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.NUMBER_OF_DISPLAYED_INTERSTITIAL_D0_D1, value);
            PlayerPrefs.Save();
        }
    }


    public static int PreviousRevenueD1
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.PREVIOUS_REVENUE_D1, 0);
        }
        set
        {
            if (value < 0)
                value = 0;

            PlayerPrefs.SetInt(StringHelper.PREVIOUS_REVENUE_D1, value);
            PlayerPrefs.Save();
        }
    }

    public static float RevenueD1
    {
        get
        {
            return PlayerPrefs.GetFloat(StringHelper.REVENUE_D1, 0);
        }
        set
        {
            if (value < 0f)
                value = 0f;

            PlayerPrefs.SetFloat(StringHelper.REVENUE_D1, value);
            PlayerPrefs.Save();
        }
    }
    public static int NumberOfDisplayedInterstitialD1
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.NUMBER_OF_DISPLAYED_INTERSTITIAL_D1, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.NUMBER_OF_DISPLAYED_INTERSTITIAL_D1, value);
            PlayerPrefs.Save();
        }
    }


    public static int DaysPlayed
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.DAYS_PLAYED, 1);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.DAYS_PLAYED, value);
            PlayerPrefs.Save();
        }
    }

    public static int PayingType
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.PAYING_TYPE, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.PAYING_TYPE, value);
            PlayerPrefs.Save();
        }
    }


    public static DateTime FirstTimeOpenGame
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.FIRST_TIME_OPEN_GAME))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.FIRST_TIME_OPEN_GAME));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = UnbiasedTime.Instance.Now().AddDays(-1);
                PlayerPrefs.SetString(StringHelper.FIRST_TIME_OPEN_GAME, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.FIRST_TIME_OPEN_GAME, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }

    public static DateTime LastTimeOpenGame
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_OPEN_GAME))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_OPEN_GAME));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = UnbiasedTime.Instance.Now().AddDays(-1);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_OPEN_GAME, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_OPEN_GAME, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
    public static DateTime LastTimeResetSalePackShop
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_RESET_SALE_PACK_SHOP))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_RESET_SALE_PACK_SHOP));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = UnbiasedTime.Instance.Now().AddDays(-1);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_RESET_SALE_PACK_SHOP, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_RESET_SALE_PACK_SHOP, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }


    public static bool CanShowRate
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.CAN_SHOW_RATE, 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.CAN_SHOW_RATE, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }


    public int NumShowedAccumulationRewardRandom//Khi có chim mới => bản mới sẽ NumShowedAccumulationRewardRandom = 0
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.NUM_SHOWED_ACCUMULATION_REWARD_RANDOM, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.NUM_SHOWED_ACCUMULATION_REWARD_RANDOM, value);
            PlayerPrefs.Save();
        }
    }

    public static bool StarterPackIsCompleted
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.STARTER_PACK_IS_COMPLETED, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.STARTER_PACK_IS_COMPLETED, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool HasPackInWeekToday
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.HAS_PACK_IN_WEEK_TODAY, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.HAS_PACK_IN_WEEK_TODAY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static bool HasPackWeekendToday
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.HAS_PACK_WEEKEND_TODAY, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.HAS_PACK_WEEKEND_TODAY, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public static string CurrentPackInWeek
    {
        get
        {
            return PlayerPrefs.GetString(StringHelper.CURRENT_PACK_IN_WEEK, "");
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.CURRENT_PACK_IN_WEEK, value);
            PlayerPrefs.Save();
        }
    }
    public static string CurrentPackWeekend
    {
        get
        {
            return PlayerPrefs.GetString(StringHelper.CURRENT_PACK_WEEKEND, "");
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.CURRENT_PACK_WEEKEND, value);
            PlayerPrefs.Save();
        }
    }
    public static string CurrentRandomSetSkin
    {
        get
        {
            return PlayerPrefs.GetString(StringHelper.CURRENT_RANDOM_SET_SKIN, "");
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.CURRENT_RANDOM_SET_SKIN, value);
            PlayerPrefs.Save();
        }
    }
    public static int NumberOfAdsInPlay;
    public static int NumberOfAdsInDay
    {
        get
        {
            return PlayerPrefs.GetInt(StringHelper.NUMBER_OF_ADS_IN_DAY, 0);
        }
        set
        {
            PlayerPrefs.SetInt(StringHelper.NUMBER_OF_ADS_IN_DAY, value);
            PlayerPrefs.Save();
        }
    }

    public static DateTime LastTimeOnline
    {
        get
        {
            if (PlayerPrefs.HasKey(StringHelper.LAST_TIME_ONLINE))
            {
                var temp = Convert.ToInt64(PlayerPrefs.GetString(StringHelper.LAST_TIME_ONLINE));
                return DateTime.FromBinary(temp);
            }
            else
            {
                var newDateTime = UnbiasedTime.Instance.Now().AddDays(-1);
                PlayerPrefs.SetString(StringHelper.LAST_TIME_ONLINE, newDateTime.ToBinary().ToString());
                PlayerPrefs.Save();
                return newDateTime;
            }
        }
        set
        {
            PlayerPrefs.SetString(StringHelper.LAST_TIME_ONLINE, value.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
}

