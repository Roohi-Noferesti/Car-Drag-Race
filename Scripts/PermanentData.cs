using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentData
{
    public static void SaveTimeZone(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static string GetTimeZone(string key)
    {
        return PlayerPrefs.GetString(key);
    }
}
