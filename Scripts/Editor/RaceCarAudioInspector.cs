using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RaceCarAudio))]
public class RaceCarAudioInspector : Editor
{
    private static string m_defaultPath;

    void OnEnable()
    {
        if (string.IsNullOrEmpty(m_defaultPath))
        {
            m_defaultPath = EditorPrefs.GetString("OpenFilePath");
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Load..."))
        {
            if (string.IsNullOrEmpty(m_defaultPath))
            {
                m_defaultPath = EditorPrefs.GetString("OpenFilePath");
                if (string.IsNullOrEmpty(m_defaultPath))
                    m_defaultPath = Application.persistentDataPath;
            }
            var path = EditorUtility.OpenFilePanel(
                "load animation curve from json", m_defaultPath, "txt");


            if (path.Length != 0)
            {
                //Save Directory
                var pathDir = Path.GetDirectoryName(path);
                EditorPrefs.SetString("OpenFilePath", pathDir);
                m_defaultPath = pathDir;

                var so = new SerializedObject(target);
                var json = File.ReadAllText(path);
                JsonReader jsonReader = JsonConverter.Reader(json);
                jsonReader.Read();
                JsonList jsonList = new JsonList();
                if (!jsonList.Read(jsonReader))
                {
                    return;
                }
                var jsonDict = jsonList.GetJsonDict(0);
                so.FindProperty("TransmissionWobbleMultiplier").floatValue =
                    jsonDict.GetInt("TransmissionWobbleMultiplier");
                so.FindProperty("EngineWobbleMultiplier").floatValue =
                    jsonDict.GetInt("EngineWobbleMultiplier");
                so.FindProperty("RevsLimitFade").intValue =
                    jsonDict.GetInt("RevsLimitFade");
                so.FindProperty("RevsLimitAbsolute").intValue =
                    jsonDict.GetInt("RevsLimitAbsolute");
                so.FindProperty("RevsBoostIdleGear0").intValue =
                    jsonDict.GetInt("RevsBoostIdleGear0");
                so.FindProperty("RevsBoostIdleGear1").intValue =
                    jsonDict.GetInt("RevsBoostIdleGear1");
                so.FindProperty("RevsBoostIdleGear2").intValue =
                    jsonDict.GetInt("RevsBoostIdleGear2");
                so.FindProperty("RevsBoostIdleGear3").intValue =
                    jsonDict.GetInt("RevsBoostIdleGear3");
                so.FindProperty("RevsBoostIdleGear4").intValue =
                    jsonDict.GetInt("RevsBoostIdleGear4");
                so.FindProperty("RevsBoostIdleGear5").intValue =
                    jsonDict.GetInt("RevsBoostIdleGear5");
                so.FindProperty("RevsBoostIdleGear6").intValue =
                    jsonDict.GetInt("RevsBoostIdleGear6");

                var durations = jsonDict.GetFloatArray("EngineWobbleDurationGearMultiplier");

                var durationArrayProp = so.FindProperty("EngineWobbleDurationGearMultiplier");
                durationArrayProp.arraySize = durations.Length;
                for (int i = 0; i < durations.Length; i++)
                {
                    durationArrayProp.GetArrayElementAtIndex(i).floatValue = durations[i];
                }

                so.ApplyModifiedProperties();
            }
        }
    }
}
