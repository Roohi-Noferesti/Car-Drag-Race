using System;
using System.Collections.Generic;

public static class DictionaryExtension
{
    public static T GetValue<T>(this Dictionary<string, string> keyValues, string key)
    {
        if (keyValues.ContainsKey(key))
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(Convert.ToBoolean(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(keyValues[key] as object);
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(Convert.ToInt32(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(long))
            {
                return (T)(Convert.ToInt64(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(short))
            {
                return (T)(Convert.ToInt16(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(float))
            {
                return (T)(Convert.ToSingle(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(double))
            {
                return (T)(Convert.ToDouble(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(DateTime))
            {
                return (T)(Convert.ToDateTime(keyValues[key]) as object);
            }
        }
        return default(T);
    }


    public static T GetValue<T>(this Dictionary<string, object> keyValues, string key,T defaultvalue = default (T))
    {
        if (keyValues.ContainsKey(key))
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(Convert.ToBoolean(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(keyValues[key] as object);
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(Convert.ToInt32(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(long))
            {
                return (T)(Convert.ToInt64(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(short))
            {
                return (T)(Convert.ToInt16(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(float))
            {
                return (T)(Convert.ToSingle(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(double))
            {
                return (T)(Convert.ToDouble(keyValues[key]) as object);
            }

            if (typeof(T) == typeof(DateTime))
            {
                return (T)(Convert.ToDateTime(keyValues[key]) as object);
            }
        }
        return defaultvalue;
    }
}
