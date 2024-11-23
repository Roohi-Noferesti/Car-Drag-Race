using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ServerEndpointTools
{
    // Custom compiler defines:
    //
    // CROSS_PLATFORM_INPUT : denotes that cross platform input package exists, so that other packages can use their CrossPlatformInput functions.
    // EDITOR_MOBILE_INPUT : denotes that mobile input should be used in editor, if a mobile build target is selected. (i.e. using Unity Remote app).
    // MOBILE_INPUT : denotes that mobile input should be used right now!

    static ServerEndpointTools()
    {
        //var defines = GetDefinesList(buildTargetGroups[0]);
        //if (!defines.Contains("LOCAL_HOST"))
        //{
        //    SetEnabled("LOCAL_HOST", true);
        //}
    }


    [MenuItem("Tools/Endpoint/Local")]
    private static void Enable()
    {
        SetEnabled("LOCAL_HOST", true);
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            default:
                EditorUtility.DisplayDialog("LOCAL HOST",
                    "You have enabled LOCAL HOST.",
                    "OK");
                break;
        }
    }


    [MenuItem("Tools/Endpoint/Local", true)]
    private static bool LocalValidate()
    {
        var defines = GetDefinesList(buildTargetGroups[0]);
        return !defines.Contains("LOCAL_HOST");
    }



    [MenuItem("Tools/Endpoint/Remote")]
    private static void Disable()
    {
        SetEnabled("LOCAL_HOST", false);
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            default:
                EditorUtility.DisplayDialog("LOCAL HOST",
                    "You have disabled LOCAL HOST.",
                    "OK");
                break;
        }
    }


    [MenuItem("Tools/Endpoint/Remote", true)]
    private static bool RemoteValidate()
    {
        var defines = GetDefinesList(buildTargetGroups[0]);
        return defines.Contains("LOCAL_HOST");
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
