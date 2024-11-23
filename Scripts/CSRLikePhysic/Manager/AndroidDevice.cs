using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
public class AndroidDevice : BaseDevice
{
    private static List<string> lowEndDeviceList = new List<string>
    {
        "GT-P5110",
        "Kindle Fire",
        "IdeaTabA1000-F",
        "ME173X",
        "XT910",
        "HTC EVO 3D X515a",
        "KFOT",
        "B1-A71",
        "SM-G7102"
    };

    private static List<string> lowEndGPUList = new List<string>
    {
        "NVIDIA Tegra 2"
    };

    private bool isAmazonDevice;

    private bool isAndroid_L;

    private static Material FramebufferAlphaFixMat;

    public static bool mIsQuitting = false;

    public static int OriginalScreenWidth;
    public static int OriginalScreenHeight;

    public AndroidDevice()
    {
        OriginalScreenWidth = Screen.width;
        OriginalScreenHeight = Screen.height;
        this.DeviceBaseName = "Android";
        string model = SystemInfo.deviceModel;//AndroidSpecific.GetModel();
        string manufacturer = "";//AndroidSpecific.GetManufacturer();
        string deviceOSCodename = SystemInfo.operatingSystem;//AndroidSpecific.GetDeviceOSCodename();
        string graphicsDeviceName = SystemInfo.graphicsDeviceName;
        string deviceOSVersion = SystemInfo.operatingSystem;//BasePlatform.ActivePlatform.GetDeviceOSVersion();
        if (deviceOSCodename.Equals("REL"))
        {
            string[] array = deviceOSVersion.Split(new char[]
            {
                '.'
            });
            int num = int.Parse(array[0]);
            this.isAndroid_L = (num >= 5);
        }
        else
        {
            this.isAndroid_L = BasePlatform.ActivePlatform.GetDeviceOSVersion().Equals("L");
        }
        bool flag = this.IsLowMemoryDevice();
        QualitySettings.SetQualityLevel(0, true);
        RenderSettings.fog = false;
        if (AndroidDevice.lowEndDeviceList.Contains(model))
        {
            flag = true;
        }
        if (AndroidDevice.lowEndGPUList.Contains(graphicsDeviceName))
        {
            flag = true;
        }
        if (manufacturer.ToLower().Equals("amazon"))
        {
            this.isAmazonDevice = true;
        }
        else
        {
            this.isAmazonDevice = false;
        }
        if (flag)
        {
            this.DeviceQuality = AssetQuality.Low;
            EnvQualitySettings.SetShaderQuality(EnvShaderLOD.Low);
            EnvQualitySettings.HighQualityReflections = false;
            CarQualitySettings.LowQuality();
            CarQualitySettings.RaceLod = CarShaderLod.RaceLow;
            //EnvQualitySettings.EnvSceneQuality = EnvSceneQuality.Low;
            Screen.SetResolution((int)(OriginalScreenWidth * .7F), (int)(OriginalScreenHeight * .7F), true);
        }
        else
        {
            this.DeviceQuality = AssetQuality.High;
            EnvQualitySettings.SetShaderQuality(EnvShaderLOD.High);
            CarQualitySettings.HighQuality();
            //EnvQualitySettings.EnvSceneQuality = EnvSceneQuality.Medium;
            Screen.SetResolution((int)(OriginalScreenWidth * 0.7), (int)(OriginalScreenHeight * 0.7), true);
        }
        Screen.fullScreen = false;
        QualitySettings.vSyncCount = 0;
        

    }

    public override void ApplyInitialQuality()
    {
        // Amin barmaki added this
        if (PlayerPrefs.GetInt("_optimumQuality", 0) == 0)
        {
            SetToOptimumQuality();
        }
        else
        {
            SetToHighQuality();
        }
    }

    public override bool IsLowMemoryDevice()
    {
        return SystemInfo.systemMemorySize < 768;
    }

    public override bool IsAmazonDevice()
    {
        return this.isAmazonDevice;
    }

    public override bool IsAndroid_L()
    {
        return this.isAndroid_L;
    }

    // public override bool HasHardwareBackButton()
    // {
    //     return BasePlatform.ActivePlatform.IsSupportedGooglePlayStore() && !AndroidSpecific.GetFullscreenMode();
    // }

  
    public static IEnumerator FlickerOnQuitFix()
    {
        yield break;
        
    }

    public override int GetScreenWidth()
    {
        return Math.Max(Screen.width, Screen.height);
    }

    public override int GetScreenHeight()
    {
        return Math.Min(Screen.width, Screen.height);
    }

    protected override void SetupShaderKeywords()
    {
        if (SystemInfo.graphicsDeviceName.Contains("Adreno"))
        {
            Shader.EnableKeyword("NOCLIP");
            Shader.DisableKeyword("USECLIP");
        }
    }

    public override sealed void SetToOptimumQuality()
    {
        EnvQualitySettings.SetToOptimumQuality(OriginalScreenWidth, OriginalScreenHeight);
    }

    public override sealed void SetToHighQuality()
    {
        EnvQualitySettings.SetToHighQuality(OriginalScreenWidth, OriginalScreenHeight);
    }

    public override void SetToScreenShotQuality()
    {
        EnvQualitySettings.SetToScreenShotQuality(OriginalScreenWidth, OriginalScreenHeight);
    }
}
#endif
