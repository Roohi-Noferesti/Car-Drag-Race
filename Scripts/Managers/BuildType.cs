using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildType : MonoBehaviour
{
    private static bool? _isAppTuttiBuild;
    private static bool? _isVasBuild;

    public static bool IsAppTuttiBuild
    {
        get
        {
            if (!_isAppTuttiBuild.HasValue)
            {
                BuildConfiguration buildConfig = Resources.Load<BuildConfiguration>("BuildConfiguration");
                if (buildConfig != null) {
                    _isAppTuttiBuild = (buildConfig.buildTargetStore==BuildConfiguration.BuildTargetStore.AppTutti);
                } else {
                    _isAppTuttiBuild = false;
                }
                Resources.UnloadAsset(buildConfig);
            }

            return _isAppTuttiBuild.Value;
        }
        set
        {
            _isAppTuttiBuild = value;
        }
    }
    
    public static bool IsVasBuild
    {
        get
        {
            if (!_isVasBuild.HasValue)
            {
                BuildConfiguration buildConfig = Resources.Load<BuildConfiguration>("BuildConfiguration");
                if (buildConfig != null) {
                    _isVasBuild = (buildConfig.buildTargetStore==BuildConfiguration.BuildTargetStore.Vas);
                } else {
                    _isVasBuild = false;
                }
                Resources.UnloadAsset(buildConfig);
            }

            return _isVasBuild.Value;
        }
        set
        {
            _isVasBuild = value;
        }
    }
    
    public static bool CanShowRate()
    {
        return !IsAppTuttiBuild && !IsVasBuild;
    }
    
    public static bool CanShowShareButton()
    {
        return !IsAppTuttiBuild && !IsVasBuild;
    }

    public static bool CanShowGooglePlay()
    {
        return !IsAppTuttiBuild && !IsVasBuild;
    }

    public static bool CanCollectData()
    {
        return !IsAppTuttiBuild && !IsVasBuild;
    }
}
