using System;
using System.Globalization;
using UnityEngine;

public class LocalStorageExtension 
{
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static void SetDatetime(string key, DateTime value)
    {
        SetString(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return GetInt(key, defaultValue ? 1 : 0) == 1;
    }

    public static string GetString(string key, string defaultValue = null)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static float GetFloat(string key, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static DateTime GetDateTime(string key)
    {
        var value = GetString(key);
        return string.IsNullOrEmpty(value) ? DateTime.Now : Convert.ToDateTime(value);
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
}
