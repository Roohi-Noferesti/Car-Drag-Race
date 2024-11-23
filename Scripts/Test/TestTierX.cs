using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DataSerialization;
using UnityEngine;

public class TestTierX : MonoBehaviour
{
    public TierXThemeDescriptor tierXThemeDescriptor;

    void Awake()
    {
        var ThemeDescriptor = this.LoadFromBinary<ThemeLayout>(tierXThemeDescriptor.ThemeDescriptor.bytes);
        var boss = ThemeDescriptor.CanSwipe;
    }


    public T LoadFromBinary<T>(byte[] bytes) where T : class
    {
        TierXConfigurationSerializer tierXConfigurationSerializer = new TierXConfigurationSerializer();
        using (MemoryStream memoryStream = new MemoryStream(bytes))
        {
            try
            {
                return tierXConfigurationSerializer.Deserialize(memoryStream, null, typeof(T)) as T;
            }
            catch (Exception var_2_34)
            {
            }
        }
        return (T)((object)null);
    }
}
