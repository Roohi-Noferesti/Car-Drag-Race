using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataSerialization;
using I2.Loc;
using ProtoBuf;
using ProtoBuf.Meta;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class TierxConfigurationEditor : EditorWindow
{
    public TierXConfiguration TierXConfiguration;
    public TextAsset ThemeLayoutAsset;
    public TextAsset PinScheduleConfigurationAsset;
    public TextAsset PopUpConfigurationAsset;
    public TextAsset ThemeAnimationsConfigurationAsset;
    public TextAsset NarrativeAsset;
    public TextAsset[] NarrativeScenesAssets;
    public LanguageSource LanguageSource;
    private bool m_foldout;
    private int m_size;
    [MenuItem("Tools/TierxConfiguration...")]
    private static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<TierxConfigurationEditor>();
        window.Show();
    }

    void OnEnable()
    {
        TierXConfiguration = LoadAsset<TierXConfiguration>("TierXConfigurationPath");
        ThemeLayoutAsset = LoadAsset<TextAsset>("ThemeLayoutAssetPath");
        PinScheduleConfigurationAsset = LoadAsset<TextAsset>("PinScheduleConfigurationAssetPath");
        PopUpConfigurationAsset = LoadAsset<TextAsset>("PopUpConfigurationAssetPath");
        ThemeAnimationsConfigurationAsset = LoadAsset<TextAsset>("ThemeAnimationsConfigurationAssetPath");
        NarrativeAsset = LoadAsset<TextAsset>("NarrativeAssetPath");

        NarrativeScenesAssets = new TextAsset[10];
        for (int i = 0; i < 10; i++)
        {
            NarrativeScenesAssets[i] = LoadAsset<TextAsset>("NarrativeScenesAssetsPath_" + i);

            if (NarrativeScenesAssets[i] == null)
            {
                Array.Resize(ref NarrativeScenesAssets, i);
                break;
            }
        }

        m_size = NarrativeScenesAssets.Length;
    }

    private T LoadAsset<T>(string path) where T : Object
    {
        var value = EditorPrefs.GetString(path);
        if (!string.IsNullOrEmpty(value))
        {
            return AssetDatabase.LoadAssetAtPath<T>(value);
        }
        return null;
    }

    void OnDisable()
    {
        EditorPrefs.SetString("TierXConfigurationPath", AssetDatabase.GetAssetPath(TierXConfiguration));
        EditorPrefs.SetString("ThemeLayoutAssetPath", AssetDatabase.GetAssetPath(ThemeLayoutAsset));
        EditorPrefs.SetString("PinScheduleConfigurationAssetPath", AssetDatabase.GetAssetPath(PinScheduleConfigurationAsset));
        EditorPrefs.SetString("PopUpConfigurationAssetPath", AssetDatabase.GetAssetPath(PopUpConfigurationAsset));
        EditorPrefs.SetString("ThemeAnimationsConfigurationAssetPath", AssetDatabase.GetAssetPath(ThemeAnimationsConfigurationAsset));
        EditorPrefs.SetString("NarrativeAssetPath", AssetDatabase.GetAssetPath(NarrativeAsset));

        for (int i = 0; i < NarrativeScenesAssets.Length; i++)
        {
            EditorPrefs.SetString("NarrativeScenesAssetsPath_"+i, AssetDatabase.GetAssetPath(NarrativeScenesAssets[i]));
        }

    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Serialization Tools to Serialize and Deserialize TierX Configuration to TextAsset and vice versa");
        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical();
        TierXConfiguration = (TierXConfiguration)EditorGUILayout.ObjectField("TierXConfiguration", TierXConfiguration, typeof(TierXConfiguration), false);
        if (GUILayout.Button("Save ThemeLayout To File"))
        {
            TierXConfiguration.ThemeLayout.DoDeSerializationStaff();
            SaveDialog(TierXConfiguration.ThemeLayout, "TierX_Overview");
        }
        if (GUILayout.Button("Save PinSchedule To File"))
        {
            SaveDialog(TierXConfiguration.PinScheduleConfiguration, "TierX_Overview_PinSchedule");
        }
        if (GUILayout.Button("Save PopupData To File"))
        {
            TierXConfiguration.PopUpConfiguration.DoDeSerializationStaff();
            SaveDialog(TierXConfiguration.PopUpConfiguration, "TierX_Overview_PopupData");
        }
        if (GUILayout.Button("Save ThemeAnimation To File"))
        {
            SaveDialog(TierXConfiguration.ThemeAnimationsConfiguration, "TierX_Overview_ThemeAnimation");
        }
        if (GUILayout.Button("Save Narrative To File"))
        {
            SaveDialog(TierXConfiguration.NarrativeSceneConfiguration, "TierX_Overview_Narrative");
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        ThemeLayoutAsset = (TextAsset) EditorGUILayout.ObjectField("ThemeLayoutText", ThemeLayoutAsset, typeof(TextAsset), false);
        if (GUILayout.Button("Copy To Config"))
        {
            CopyThemeLayoutToConfiguration();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        PinScheduleConfigurationAsset = (TextAsset)EditorGUILayout.ObjectField("PinScheduleText", PinScheduleConfigurationAsset, typeof(TextAsset), false);
        if (GUILayout.Button("Copy To Config"))
        {
            CopyPinScheduleToConfiguration();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        PopUpConfigurationAsset = (TextAsset)EditorGUILayout.ObjectField("PopupsText", PopUpConfigurationAsset, typeof(TextAsset), false);
        if (GUILayout.Button("Copy To Config"))
        {
            CopyPopupToConfiguration();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        ThemeAnimationsConfigurationAsset = (TextAsset)EditorGUILayout.ObjectField("ThemeAnimationText", ThemeAnimationsConfigurationAsset, typeof(TextAsset), false);
        if (GUILayout.Button("Copy To Config"))
        {
            CopyAnimationThemeToConfiguration();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        NarrativeAsset = (TextAsset)EditorGUILayout.ObjectField("NarrativeText", NarrativeAsset, typeof(TextAsset), false);
        if (GUILayout.Button("Copy To Config"))
        {
            CopyNarrativeToConfiguration();
        }
        EditorGUILayout.EndHorizontal();

        m_foldout = EditorGUILayout.Foldout(m_foldout, "NarrativeScenesText");
        if (m_foldout)
        {
            EditorGUI.indentLevel = 1;
            var size = EditorGUILayout.IntField("Size", m_size);
            if (size != m_size)
            {
                m_size = size;
                Array.Resize(ref NarrativeScenesAssets, m_size);
            }

            for (int i = 0; i < m_size; i++)
            {
                NarrativeScenesAssets[i] =
                    (TextAsset) EditorGUILayout.ObjectField("Element " + i, NarrativeScenesAssets[i], typeof(TextAsset),
                        false);
            }
        }

        LanguageSource = (LanguageSource)EditorGUILayout.ObjectField("Language Source", LanguageSource, typeof(LanguageSource), false);
        if (GUILayout.Button("Print Narrative Text"))
        {
            //Register Scenes
            for (int i = 0; i < TierXConfiguration.NarrativeSceneConfiguration.NarrativeScenes.Count; i++)
            {
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                    "Assets/TierX/TierX_Italia/Texts/" +
                    TierXConfiguration.NarrativeSceneConfiguration.NarrativeScenes[i] + ".txt");
                if (textAsset != null)
                {
                    TierXConfiguration.NarrativeSceneConfiguration.RegisterScene(textAsset);
                }
            }

            //Printing
            foreach (var pinSequence in TierXConfiguration.PinScheduleConfiguration.Sequences)
            {
                foreach (var pinSequencePin in pinSequence.Pins)
                {
                    if (!string.IsNullOrEmpty(pinSequencePin.Narrative.PreRaceSceneID))
                    {
                        Debug.Log(pinSequencePin.ID +
                                  "_IntroSceneID................................................");
                        PrintSceneMessages(pinSequencePin.Narrative.IntroSceneID);

                        Debug.Log(pinSequencePin.ID + "_PreRaceScene................................................");
                        PrintSceneMessages(pinSequencePin.Narrative.PreRaceSceneID);

                        Debug.Log(pinSequencePin.ID +
                                  "_PostRaceWinSceneID................................................");
                        PrintSceneMessages(pinSequencePin.Narrative.PostRaceWinSceneID);

                        Debug.Log(pinSequencePin.ID +
                                  "_PostRaceLoseSceneID................................................");
                        PrintSceneMessages(pinSequencePin.Narrative.PostRaceLoseSceneID);
                    }
                }
            }
        }
    }

    private void PrintSceneMessages(string sceneID)
    {
        NarrativeScene scene;
        if (TierXConfiguration.NarrativeSceneConfiguration.GetScene(sceneID, out scene))
        {
            for (int i = 0; i < scene.Lines.Count; i++)
            {
                Debug.Log("Scene Line " + i);
                PrintLineMessages(scene, scene.Lines[i]);
            }
        }
    }

    private void PrintLineMessages(NarrativeScene scene, NarrativeSceneLine line)
    {
        //if (LanguageSource!=null && !string.IsNullOrEmpty(line.StateData.StateDetails.Message))
        //{
        //    if (!LanguageSource.ContainsTerm(line.StateData.StateDetails.Message))
        //    {
        //        LanguageSource.AddTerm(line.StateData.StateDetails.Message);
        //    }
        //}
        Debug.Log("Message : " + line.StateData.StateDetails.Message);

        for (int i = 0; i < line.Responses.Count; i++)
        {
            var responseLine = scene.Lines.FirstOrDefault(s =>
                s.SceneLineID == line.Responses[i].SceneLineID);
            Debug.Log("Response " + i);
            PrintLineMessages(scene, responseLine);
        }
    }

    public void CopyThemeLayoutToConfiguration(bool setDirty = true)
    {
        if (ThemeLayoutAsset != null)
        {
            var themeLayout = LoadFromBinary<ThemeLayout>(ThemeLayoutAsset.bytes);
            themeLayout.DoSerializationStaff();
            TierXConfiguration.ThemeLayout = themeLayout;
            if (setDirty)
                EditorUtility.SetDirty(TierXConfiguration);
        }
    }

    public void CopyPinScheduleToConfiguration(bool setDirty = true)
    {
        if (PinScheduleConfigurationAsset != null)
        {
            var pinScheduleConfiguration =
                LoadFromBinary<PinScheduleConfiguration>(PinScheduleConfigurationAsset.bytes);
            TierXConfiguration.PinScheduleConfiguration = pinScheduleConfiguration;
            if (setDirty)
                EditorUtility.SetDirty(TierXConfiguration);
        }
    }

    public void CopyPopupToConfiguration(bool setDirty = true)
    {
        if (PopUpConfigurationAsset != null)
        {
            var popUpConfiguration = LoadFromBinary<PopUpConfiguration>(PopUpConfigurationAsset.bytes);
            TierXConfiguration.PopUpConfiguration = popUpConfiguration;
            TierXConfiguration.PopUpConfiguration.DoSerializationStaff();
            if (setDirty)
                EditorUtility.SetDirty(TierXConfiguration);
        }
    }

    public void CopyAnimationThemeToConfiguration(bool setDirty = true)
    {
        if (ThemeAnimationsConfigurationAsset != null)
        {
            var themeAnimationsConfiguration =
                LoadFromBinary<ThemeAnimationsConfiguration>(ThemeAnimationsConfigurationAsset.bytes);
            TierXConfiguration.ThemeAnimationsConfiguration = themeAnimationsConfiguration;
        }

        if (setDirty)
            EditorUtility.SetDirty(TierXConfiguration);
    }

    public void CopyNarrativeToConfiguration(bool setDirty = true)
    {
        if (NarrativeAsset == null ||
            string.IsNullOrEmpty(NarrativeAsset.text))
        {
            TierXConfiguration.NarrativeSceneConfiguration = new NarrativeSceneConfiguration();
            this.AddNarrativeIdsFromFilenames(ref TierXConfiguration.NarrativeSceneConfiguration);
        }
        else
        {
            TierXConfiguration.NarrativeSceneConfiguration =
                JsonConverter.DeserializeObject<NarrativeSceneConfiguration>(this.NarrativeAsset.text);
        }

        TextAsset[] narrativeScenes = this.NarrativeScenesAssets;
        for (int i = 0; i < narrativeScenes.Length; i++)
        {
            TextAsset textAsset = narrativeScenes[i];
            if (textAsset != null)
            {
                TierXConfiguration.NarrativeSceneConfiguration.RegisterScene(textAsset);
            }
        }

        TierXConfiguration.NarrativeSceneConfiguration.DoSerializatuionStaff();
        if (setDirty)
            EditorUtility.SetDirty(TierXConfiguration);
    }

    private void AddNarrativeIdsFromFilenames(ref NarrativeSceneConfiguration config)
    {
        config.NarrativeScenes = new List<string>();
        TextAsset[] narrativeScenes = NarrativeScenesAssets;
        for (int i = 0; i < narrativeScenes.Length; i++)
        {
            TextAsset textAsset = narrativeScenes[i];
            config.NarrativeScenes.Add(textAsset.name);
        }
    }

    void CopyAll()
    {
        CopyThemeLayoutToConfiguration(false);
        CopyPinScheduleToConfiguration(false);
        CopyPopupToConfiguration(false);
        CopyAnimationThemeToConfiguration(false);
        CopyNarrativeToConfiguration(false);
        EditorUtility.SetDirty(TierXConfiguration);
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
            catch (Exception exception)
            {
                GTDebug.LogError(GTLogChannel.TierX, exception.Message);
            }
        }
        return (T)((object)null);
    }

    private void SaveDialog(object obj,string fileName)
    {
        var tierXPath = EditorPrefs.GetString("TierXDirectory");
        var path = EditorUtility.SaveFilePanel("Save Configuration", tierXPath, fileName, "bytes");
        if (!string.IsNullOrEmpty(path))
        {
            SaveToBinary(obj, path);
            EditorPrefs.SetString("TierXDirectory", Path.GetDirectoryName(path));
        }
    }


    public void SaveToBinary(object obj,string path)
    {
        TierXConfigurationSerializer tierXConfigurationSerializer = new TierXConfigurationSerializer();
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            try
            {
                tierXConfigurationSerializer.Serialize(fs, obj);

                if (EditorUtility.DisplayDialog("Save", "Save success.Show file?", "Yes", "No"))
                {
                    EditorUtility.RevealInFinder(path);
                }
            }
            catch (Exception exception)
            {
                GTDebug.LogError(GTLogChannel.TierX, exception.Message);
            }
        }
    }
}
