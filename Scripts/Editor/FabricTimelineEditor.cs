using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fabric;
using Fabric.TimelineComponent;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class FabricTimelineEditor 
{
    private static string m_defaultPath;

    [MenuItem("Fabric/GT/Load TimelineCompinent")]
    private static void LoadTimelineComponent()
    {
        if (Selection.activeGameObject == null)
            return;
        var timeline = Selection.activeGameObject.GetComponent<TimelineComponent>();

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


            var json = File.ReadAllText(path);
            JsonReader jsonReader = JsonConverter.Reader(json);
            jsonReader.Read();
            JsonList jsonList = new JsonList();
            if (!jsonList.Read(jsonReader))
            {
                return;
            }

            Undo.RecordObject(timeline,"Load timeline");
            var layers = timeline.GetComponentsInChildren<TimelineLayer>();

            for (int i = 0; i < jsonList.Count; i++)
            {
                var dic = jsonList.GetJsonDict(i);

                var regionsList = dic.GetJsonList("regions");
                var timelineRegions = layers[i].GetComponentsInChildren<TimelineRegion>();
                for (int j = 0; j < regionsList.Count; j++)
                {
                    var regionDic = regionsList.GetJsonDict(j);
                    var timelineRegion = timelineRegions[j];
                    var fields = timelineRegion.GetType().GetFields();
                    Debug.Log(timelineRegion.name + "     " + regionDic.GetString("name"));
                    foreach (var fieldInfo in fields.Where(
                        f => f.IsDefined(typeof(SerializeField), true)
                            || f.IsPublic))
                    {
                        if(!regionDic.ContainsKey(fieldInfo.Name))
                            continue;
                        object value = null;
                        if (fieldInfo.FieldType == typeof (int))
                        {
                            Debug.Log(fieldInfo.Name + "   " + regionDic.GetString(fieldInfo.Name));
                            value = Convert.ToInt32(regionDic.GetString(fieldInfo.Name));
                        }
                        else if (fieldInfo.FieldType == typeof(float))
                        {
                            value = Convert.ToSingle(regionDic.GetString(fieldInfo.Name));
                        }
                        else if (fieldInfo.FieldType == typeof(string))
                        {
                            value = regionDic.GetString(fieldInfo.Name);
                        }
                        else if (fieldInfo.FieldType == typeof(long))
                        {
                            value = Convert.ToInt64(regionDic.GetString(fieldInfo.Name));
                        }
                        else if (fieldInfo.FieldType == typeof(bool))
                        {
                            value = Convert.ToBoolean(regionDic.GetString(fieldInfo.Name));
                        }
                        //else
                        //{
                        //    value = regionDic.GetString(fieldInfo.Name);
                        //}

                        if (value != null)
                        {
                            Debug.Log("Set value of '" + fieldInfo.Name+"' to : "+value);
                            fieldInfo.SetValue(timelineRegion, value);
                        }
                    }
                    EditorUtility.SetDirty(timelineRegion);
                }
            }
        }
    }


    [MenuItem("Fabric/GT/Save Fabric")]
    private static void SaveFabric()
    {
        if (Selection.activeGameObject == null)
            return;

        if (string.IsNullOrEmpty(m_defaultPath))
        {
            m_defaultPath = EditorPrefs.GetString("OpenFilePath");
            if (string.IsNullOrEmpty(m_defaultPath))
                m_defaultPath = Application.persistentDataPath;
        }
        var path = EditorUtility.SaveFilePanel(
            "load fabric",m_defaultPath,"fabric", "txt");

        if (path.Length != 0)
        {
            //Save Directory
            var pathDir = Path.GetDirectoryName(path);
            EditorPrefs.SetString("OpenFilePath", pathDir);
            m_defaultPath = pathDir;

            JsonSerializer.Serialize(Selection.activeGameObject.transform, path);
            EditorUtility.RevealInFinder(path);
        }
    }


    [MenuItem("Fabric/GT/Load Fabric")]
    private static void LoadFabric()
    {
        if (Selection.activeGameObject == null)
            return;
        var fabricManager = Selection.activeGameObject.GetComponent<FabricManager>();

        if (string.IsNullOrEmpty(m_defaultPath))
        {
            m_defaultPath = EditorPrefs.GetString("OpenFilePath");
            if (string.IsNullOrEmpty(m_defaultPath))
                m_defaultPath = Application.persistentDataPath;
        }
        var path = EditorUtility.OpenFilePanel(
            "load fabric", m_defaultPath, "txt");

        if (path.Length != 0)
        {
            //Save Directory
            var pathDir = Path.GetDirectoryName(path);
            EditorPrefs.SetString("OpenFilePath", pathDir);
            m_defaultPath = pathDir;


            var json = File.ReadAllText(path);
            JsonDict jsonDict = new JsonDict();
            if (jsonDict.Read(json))
            {
                Undo.RecordObject(fabricManager.transform, "Load Fabric");
                JsonSerializer.DeserializeComponent(jsonDict, fabricManager.transform);
                EditorUtility.SetDirty(fabricManager.transform);
            }
        }
    }


    [MenuItem("Fabric/GT/Fix fabric layers")]
    private static void FixFabricLayers()
    {
        if (Selection.activeGameObject == null)
            return;
        var timelines = Selection.activeGameObject.GetComponentsInChildren<TimelineComponent>();

        foreach (TimelineComponent timeline in timelines
            )
        {
            var layers = timeline.GetComponentsInChildren<TimelineLayer>();

            foreach (var timelineLayer in layers)
            {
                var regions = timelineLayer.GetComponentsInChildren<TimelineRegion>();
                timelineLayer._regions = new TimelineRegion[0];
                timelineLayer._regions = regions;

                foreach (var timelineRegion in regions)
                {
                    timelineRegion._volumeEnvelope._points = null;
                    timelineRegion.ResetVolumeEnvelope();
                }
            }
        }
        //Undo.RecordObject(fabricManager, "Fix Fabric Layers");
        //EditorUtility.SetDirty(fabricManager);
    }
}
