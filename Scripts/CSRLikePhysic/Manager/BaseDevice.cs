using System;
using UnityEngine;

public abstract class BaseDevice
{
	public const float DefaultWidth = 1280f;

	public const float DefaultHeight = 720f;

	public static readonly BaseDevice ActiveDevice;

	public float WidthMultiplier;

	public float HeightMultiplier;

	public float Aspect;

	public AssetQuality DeviceQuality = AssetQuality.High;

	public string DeviceBaseName;

	public bool RenderScaleEnabled;

	static BaseDevice()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
		ActiveDevice = new AndroidDevice();
#elif UNITY_EDITOR
        ActiveDevice = new DefaultDevice();
#elif UNITY_IPHONE
        ActiveDevice = new DefaultDevice();
#endif
		ActiveDevice.WidthMultiplier = (float)ActiveDevice.GetScreenWidth() / 1280f;
		ActiveDevice.HeightMultiplier = (float)ActiveDevice.GetScreenHeight() / 720f;
		ActiveDevice.Aspect = (float)ActiveDevice.GetScreenWidth() / (float)ActiveDevice.GetScreenHeight();
		ActiveDevice.SetupShaderKeywords();
		Application.targetFrameRate = 60;
	}

	public virtual bool IsLowMemoryDevice()
	{
		return false;
	}

	public virtual bool IsAmazonDevice()
	{
		return false;
	}

	public virtual bool IsAndroid_L()
	{
		return false;
	}

	public virtual bool IsWindowsDevice()
	{
		return false;
	}

	public virtual bool HasLowGPUPerformanceAndHighResScreen()
	{
		return false;
	}

	public virtual bool HasHardwareBackButton()
	{
		return false;
	}

	public virtual int GetScreenWidth()
	{
	    return Screen.width;
	}

	public virtual int GetScreenHeight()
	{
	    return Screen.height;
	}

	public virtual bool IsDesktopDevice()
	{
		return false;
	}

	public virtual bool HasTouchScreen()
	{
		return true;
	}

	public virtual bool KeyboardActive()
	{
	    return false;//TouchKeyboardHelper.Instance.IsKeyboardActive;
	}

	public virtual bool KeyboardAttached()
	{
		return false;
	}

	protected virtual void SetupShaderKeywords()
	{
		Shader.EnableKeyword("USECLIP");
	}

	public virtual bool SupportsVideoCapture()
	{
		return false;
	}

    public virtual bool IsNewVersion
    {
        get { return CompareVersion("0.1.1.23") > 0; }
    }

    public virtual string GameVersion
    {
        get { return Application.version; }
    }

    public int CompareVersion(string version)
    {
        var currentCodes = GameVersion.Split(".".ToCharArray());
        var codes = version.Split(".".ToCharArray());

        if (currentCodes.Length != codes.Length)
        {
            throw new Exception(string.Format("invalid version code to caompare. Current : {0}, Target : {1}",
                GameVersion, version));
        }

        for (int i = 0; i < currentCodes.Length; i++)
        {
            var current = int.Parse(currentCodes[i]);
            var target = int.Parse(codes[i]);
            if (current > target)
            {
                return 1;
            }
            else if(current<target)
            {
                return -1;
            }
        }
        return 0;
    }

    public virtual void SetToHighQuality()
    {
        EnvQualitySettings.SetToHighQuality(Screen.width, Screen.height);
    }

    public virtual void SetToOptimumQuality()
    {
        
    }

    public virtual void SetToScreenShotQuality()
    {

    }

    public virtual void ApplyInitialQuality()
    {
        SetToHighQuality();
    }
}
