#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    //[MenuItem("Assets/Asset Bundle/Build Windows")]
    static void BuildAllAssetBundlesWindows()
    {
        CreateBundle(BuildTarget.StandaloneWindows);
    }

    //[MenuItem("Assets/Asset Bundle/Build Android")]
    static void BuildAllAssetBundlesAndroid()
    {
        CreateBundle(BuildTarget.Android);
    }

    public static void CreateBundle(BuildTarget buildTarget)
    {
        string assetBundleDirectory = Application.dataPath + "/../BuiltBundles/" + buildTarget + "/AppDataRoot/AssetBundles/";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        //BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ForceRebuildAssetBundle, buildTarget);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.StrictMode, buildTarget);
    }
}
#endif