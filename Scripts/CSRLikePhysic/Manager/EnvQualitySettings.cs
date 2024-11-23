using UnityEngine;

public static class EnvQualitySettings
{
    private static bool _useMotionBlur = true;
    //private static bool _useVignette = true;
    public static bool HighQualityReflections = true;
    private static float multiplier;
    public static float ScreenScale => multiplier;
    public static bool UseMotionBlur
    {
        get { return _useMotionBlur; }
        private set { _useMotionBlur = value; }
    }


    //public static bool UseVignette
    //{
    //    get { return _useVignette; }
    //    private set { _useVignette = value; }
    //}

    public static EnvSceneQuality EnvSceneQuality = EnvSceneQuality.High;

    public static void SetShaderQuality(EnvShaderLOD lod)
    {
        Shader.globalMaximumLOD = (int)lod;
    }


    public static void SetToOptimumQuality(int screenWidth, int screenHeight)
    {
        CalculateMultiplier(320);
     //   Debug.LogError(QualitySettings.GetQualityLevel());
        var heightResolution = Screen.currentResolution.height;
        if (heightResolution >=1440)
        {
            QualitySettings.antiAliasing = 2;
            UseMotionBlur = true;
            //UseVignette = true;
        }
        if (heightResolution >=1600)
        {
            QualitySettings.antiAliasing = 2;
            UseMotionBlur = true;
            //UseVignette = true;
        }
        else if (screenHeight >=1080)
        {
            QualitySettings.antiAliasing = 0;
            UseMotionBlur = true;
            //UseVignette = true ;

        }
        else if (screenHeight >= 720)
        {
            QualitySettings.antiAliasing = 0;
            UseMotionBlur = false;
            //UseVignette = true;

        }
        else if (screenHeight >= 540)
        {
            QualitySettings.antiAliasing = 2;
            UseMotionBlur = false;
            //UseVignette = false;

        }
        else
        {
            QualitySettings.antiAliasing = 0;
            UseMotionBlur = false;
            //UseVignette = false;


        }
        SetResolution(screenWidth, screenHeight);
  //      Debug.LogError("Height Resolution: " + Screen.currentResolution.height);
  //      Debug.LogError("Width Resolution: " + Screen.currentResolution.width);
    }


    public static void SetToHighQuality(int screenWidth, int screenHeight)
    {
   //     Debug.LogError(Screen.dpi);
        CalculateMultiplier(320);
        var heightResolution = Screen.currentResolution.height;
        if (heightResolution >= 1600)
        {
            QualitySettings.antiAliasing = 4;
            UseMotionBlur = true;
            //UseVignette = true;
        }
        else if (heightResolution >= 1080)
        {
            QualitySettings.antiAliasing = 0;
            UseMotionBlur = false;
            //UseVignette = true ;

        }
        else if (heightResolution >= 720)
        {
            QualitySettings.antiAliasing = 0;
            UseMotionBlur = false;
            //UseVignette = true;

        }
        else if (heightResolution >= 540)
        {
            QualitySettings.antiAliasing = 2;
            UseMotionBlur = false;
            //UseVignette = false;

        }
        else
        {
            QualitySettings.antiAliasing = 0;
            UseMotionBlur = false;
            //UseVignette = false;

        }
        SetResolution(screenWidth, screenHeight);
        //Debug.LogError("Height Resolution: " + Screen.currentResolution.height);
        //Debug.LogError("Width Resolution: " + Screen.currentResolution.width);
    }

    public static void SetToScreenShotQuality(int screenWidth, int screenHeight)
    {

        QualitySettings.antiAliasing = 8;
        QualitySettings.masterTextureLimit = 0;
        //Screen.SetResolution((int)(screenWidth * 1F), (int)(screenHeight *1F), true);
        UseMotionBlur = true;

    }
    
    private static void CalculateMultiplier(float ppi)
    {
        var screenPpi = Screen.dpi;
        var memorySize = SystemInfo.graphicsMemorySize;
        float ppiMultiplier;
        if (screenPpi <= 320f) ppiMultiplier = 1f;
        else ppiMultiplier = ppi / screenPpi;
        
        if (memorySize >= 2048)
        {
            QualitySettings.SetQualityLevel(5);
            QualitySettings.masterTextureLimit = 0;
            multiplier = ppiMultiplier * 0.9F;


        }
        else if(memorySize >= 1024)
        {
            QualitySettings.SetQualityLevel(4);
            QualitySettings.masterTextureLimit = 0;
            multiplier = ppiMultiplier * 0.8F;
        }
        else if (memorySize >= 512)
        {
            QualitySettings.SetQualityLevel(3);
            QualitySettings.masterTextureLimit = 0;
            multiplier = ppiMultiplier * 0.8F;
        }
        else if (memorySize >= 256)
        {
            QualitySettings.SetQualityLevel(2);
            QualitySettings.masterTextureLimit = 1;
            multiplier = ppiMultiplier * 0.7F;
        }
        else if (memorySize >= 128)
        {
            QualitySettings.SetQualityLevel(1);
            QualitySettings.masterTextureLimit = 1;
            multiplier = ppiMultiplier * 0.6F;
        }
        else
        {
            QualitySettings.SetQualityLevel(0);
            QualitySettings.masterTextureLimit = 1;
            multiplier = ppiMultiplier * 0.6F;
        }
        Debug.Log(QualitySettings.GetQualityLevel());
    }

    private static void SetResolution(int screenWidth, int screenHeight)
    {
        Screen.SetResolution((int)(screenWidth * multiplier), (int)(screenHeight * multiplier), true);
    }
}


public enum EnvShaderLOD
{
    High = 600,
    Med = 500,
    Low = 400
}