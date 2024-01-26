using UnityEngine;
using System;
public class PlayerPrefsX  {

    public static void SetBool(string name, bool booleanValue)
    {
        PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
    }

    public static bool GetBool(string name)
    {
        return PlayerPrefs.GetInt(name) == 1 ? true : false;
    }

    public static bool GetBool(string name, bool defaultValue)
    {
        if (PlayerPrefs.HasKey(name))
        {
            return GetBool(name);
        }

        return defaultValue;
    }
    public static DateTime GetDateTime(string key)
    {
        if (!PlayerPrefs.HasKey(key))
            return Tuhai993Utils.TimeManager.GetNow;
        return DateTime.FromBinary(long.Parse(PlayerPrefs.GetString(key)));
    }
    public static void SetDateTime(string key,DateTime value)
    {
        var stringValue = value.ToBinary().ToString();
        PlayerPrefs.SetString(key, stringValue);
    }

    public static long GetLong(string key, long defaultValue)
    {
        int lowBits, highBits;
        SplitLong(defaultValue, out lowBits, out highBits);
        lowBits = PlayerPrefs.GetInt(key + "_lowBits", lowBits);
        highBits = PlayerPrefs.GetInt(key + "_highBits", highBits);

        // unsigned, to prevent loss of sign bit.
        ulong ret = (uint)highBits;
        ret = (ret << 32);
        return (long)(ret | (ulong)(uint)lowBits);
    }

    public static long GetLong(string key)
    {
        int lowBits = PlayerPrefs.GetInt(key + "_lowBits");
        int highBits = PlayerPrefs.GetInt(key + "_highBits");

        // unsigned, to prevent loss of sign bit.
        ulong ret = (uint)highBits;
        ret = (ret << 32);
        return (long)(ret | (ulong)(uint)lowBits);
    }

    private static void SplitLong(long input, out int lowBits, out int highBits)
    {
        // unsigned everything, to prevent loss of sign bit.
        lowBits = (int)(uint)(ulong)input;
        highBits = (int)(uint)(input >> 32);
    }

    public static void SetLong(string key, long value)
    {
        int lowBits, highBits;
        SplitLong(value, out lowBits, out highBits);
        PlayerPrefs.SetInt(key + "_lowBits", lowBits);
        PlayerPrefs.SetInt(key + "_highBits", highBits);
    }

}
