using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AssetBundlePreProcessorTools
{
    static AssetBundlePreProcessorTools()
    {
        //var defines = GetDefinesList(buildTargetGroups[0]);
        //if (!defines.Contains("LOCAL_HOST"))
        //{
        //    SetEnabled("LOCAL_HOST", true);
        //}
    }


    [MenuItem("Tools/Asset Bundle/Use Project")]
    private static void Enable()
    {
        SetEnabled("USE_ASSET_BUNDLE", false);
    }


    [MenuItem("Tools/Asset Bundle/Use Project", true)]
    private static bool UseProjectValidate()
    {
        var defines = GetDefinesList(buildTargetGroups[0]);
        return defines.Contains("USE_ASSET_BUNDLE");
    }



    [MenuItem("Tools/Asset Bundle/Use AssetBundle")]
    private static void Disable()
    {
        SetEnabled("USE_ASSET_BUNDLE", true);
    }


    [MenuItem("Tools/Asset Bundle/Use AssetBundle", true)]
    private static bool UseAssetBundleValidate()
    {
        var defines = GetDefinesList(buildTargetGroups[0]);
        return !defines.Contains("USE_ASSET_BUNDLE");
    }


    private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
    {
        BuildTargetGroup.Standalone,
        BuildTargetGroup.Android,
        BuildTargetGroup.iOS,
    };


    private static void SetEnabled(string defineName, bool enable)
    {
        //Debug.Log("setting "+defineName+" to "+enable);
        foreach (var group in buildTargetGroups)
        {
            var defines = GetDefinesList(group);
            if (enable)
            {
                if (defines.Contains(defineName))
                {
                    return;
                }
                defines.Add(defineName);
            }
            else
            {
                if (!defines.Contains(defineName))
                {
                    return;
                }
                while (defines.Contains(defineName))
                {
                    defines.Remove(defineName);
                }
            }
            string definesString = string.Join(";", defines.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
        }
    }


    private static List<string> GetDefinesList(BuildTargetGroup group)
    {
        return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
    }
}
