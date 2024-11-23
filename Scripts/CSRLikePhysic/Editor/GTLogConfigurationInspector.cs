using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GTLogConfiguration))]
public class GTLogConfigurationInspector : Editor 
{

    private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
            {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS
            };

    private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[]
            {
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS,
            };


    [MenuItem("Tools/Log/GTLogsConfiguration")]
    private static void SelectConfiguration()
    {
        var config = (GTLogConfiguration)AssetDatabase.LoadAssetAtPath<GTLogConfiguration>("Assets/DefaultResources/Resources/GTLogConfiguration.asset");
        Selection.activeObject = config;
    }


    [MenuItem("Tools/Log/Enable Log")]
    private static void Enable()
    {
        SetEnabled("GT_DEBUG_LOGGING", true, true);
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
            case BuildTarget.iOS:
            case BuildTarget.StandaloneWindows:
            case BuildTarget.PSM:
            case BuildTarget.Tizen:
            case BuildTarget.WSAPlayer:
                EditorUtility.DisplayDialog("GTLog",
                                            "You have enabled Log.",
                                            "OK");
                break;

            default:
                EditorUtility.DisplayDialog("GTLog", "You have enabled Log.","OK");
                break;
        }
    }

    [MenuItem("Tools/Log/Enable Log", true)]
    private static bool EnableValidate()
    {
        var defines = GetDefinesList(mobileBuildTargetGroups[0]);
        return !defines.Contains("GT_DEBUG_LOGGING");
    }


    [MenuItem("Tools/Log/Disable Log")]
    private static void Disable()
    {
        SetEnabled("GT_DEBUG_LOGGING", false, true);
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
            case BuildTarget.iOS:
            case BuildTarget.StandaloneWindows:
                EditorUtility.DisplayDialog("GTLog",
                                            "You have disabled Log.",
                                            "OK");
                break;
        }
    }


    [MenuItem("Tools/Log/Disable Log", true)]
    private static bool DisableValidate()
    {
        var defines = GetDefinesList(mobileBuildTargetGroups[0]);
        return defines.Contains("GT_DEBUG_LOGGING");
    }

    private static List<string> GetDefinesList(BuildTargetGroup group)
    {
        return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
    }


    private static void SetEnabled(string defineName, bool enable, bool mobile)
    {
        //Debug.Log("setting "+defineName+" to "+enable);
        foreach (var group in mobile ? mobileBuildTargetGroups : buildTargetGroups)
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

    void OnEnable()
    {
        var config = target as GTLogConfiguration;

        var values = Enum.GetValues(typeof(GTLogChannel)).Cast<GTLogChannel>();

        foreach (var log in values)
        {
            if (!config.Logs.Any(l=>l.Channel==log))
            {
                config.Logs.Add(new GTLogConfiguration.LogStatus()
                {
                    Channel = log,
                    Active = false
                });
            }
        }
    }


    public override void OnInspectorGUI()
    {
        var config = target as GTLogConfiguration;

        foreach (var log in config.Logs.OrderBy(l=>l.Channel.ToString()))
        {
            EditorGUILayout.BeginHorizontal();
            log.Active = EditorGUILayout.Toggle(log.Channel.ToString(), log.Active);
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(config);
        }
    }
}
