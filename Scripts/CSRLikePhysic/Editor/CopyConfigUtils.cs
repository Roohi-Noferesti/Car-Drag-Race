using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using I2;
using LitJson;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CopyConfigUtils
{
    private static CoroutineManager m_coroutineManager;
    public static void Copy(this ScriptableObject source, ref ScriptableObject dest)
    {
        var fields = dest.GetType().GetFields().Where(f => Attribute.IsDefined(f, typeof(SerializeField))
                 || (f.IsPublic && !Attribute.IsDefined(f, typeof(NonSerializedAttribute))));

        foreach (var fieldInfo in fields)
        {
            var value = fieldInfo.GetValue(source);

            dest.GetType().GetField(fieldInfo.Name).SetValue(dest, value);
        }
    }
    
    
    public static T LoadFromFile<T>() where T:ScriptableObject
    {
        var lastDirectory = EditorPrefs.GetString("cfg_path", Application.persistentDataPath);
        var filePath = EditorUtility.OpenFilePanel("Load ", lastDirectory, "txt");
        if (!string.IsNullOrEmpty(filePath))
        {
            JsonMapper.Clear();
            var json = File.ReadAllText(filePath);
            var career = JsonConverter.DeserializeObject<T>(json);
            EditorPrefs.SetString("cfg_path", filePath);
            return career;
        }

        return null;
    }


    public static void CopyFromFile<T>(Object target,Action<Object> onCompleted=null) where T:ScriptableObject
    {
        var lastDirectory = EditorPrefs.GetString("cfg_path", Application.persistentDataPath);
        var filePath = EditorUtility.OpenFilePanel("Load " + target.name, lastDirectory, "txt");
        if (!string.IsNullOrEmpty(filePath))
        {
            JsonMapper.Clear();
            var json = File.ReadAllText(filePath);
            var career = JsonConverter.DeserializeObject<T>(json);
            var localCareer = (target as ScriptableObject);
            career.Copy(ref localCareer);

            EditorUtility.SetDirty(localCareer);
            EditorPrefs.SetString("cfg_path", filePath);
            EditorUtility.DisplayDialog("Load", "Loading to " + target.name + " completed", "ok");
            if (onCompleted != null)
            {
                onCompleted(localCareer);
            }
        }
    }


    public static void CopyFromAssetBundle<T>(Object target) where T : ScriptableObject
    {
        var lastDirectory = EditorPrefs.GetString("cfg_path", Application.persistentDataPath);
        var filePath = EditorUtility.OpenFilePanel("Load " + target.name, lastDirectory, "");
        if (!string.IsNullOrEmpty(filePath))
        {
            //var www1 = AssetBundle.LoadFromFile("file:///" + filePath);
            var coroutineManager = GetCoroutineManager();
            coroutineManager.StartCoroutine(LoadBundleAsync("file:///"+filePath, www =>
            {
                var obj = www.assetBundle.LoadAsset<T>(www.assetBundle.GetAllAssetNames()[0]);
                var localCareer = (target as ScriptableObject);
                obj.Copy(ref localCareer);
                EditorUtility.SetDirty(localCareer);
                EditorPrefs.SetString("cfg_path", filePath);
                EditorUtility.DisplayDialog("Load", "Loading to " + target.name + " completed", "ok");
                DestroyCoroutineManager();
                www.assetBundle.Unload(true);
            }));
        }
    }

    private static IEnumerator LoadBundleAsync(string filePath,Action<WWW> callback)
    {
        WWW www = new WWW(filePath);
        yield return www;
        callback(www);
    }

    public static void SaveToFile<T>(T target) where T : ScriptableObject
    {
        var lastDirectory = EditorPrefs.GetString("cfg_path", Application.persistentDataPath);
        var filePath = EditorUtility.SaveFilePanel("Save "+target.name, lastDirectory, target.name, "txt");
        if (!string.IsNullOrEmpty(filePath))
        {
            JsonMapper.Clear();
            var jsonValue = JsonConverter.SerializeObject(target);
            File.WriteAllText(filePath,jsonValue);
            EditorPrefs.SetString("cfg_path", filePath);
            EditorUtility.DisplayDialog("Save", "Saveing to " + target.name + " completed", "ok");
        }
    }


    public static T ReadFromText<T>(string jsonConfig) where T : ScriptableObject
    {
        if (!string.IsNullOrEmpty(jsonConfig))
        {
            return JsonConverter.DeserializeObject<T>(jsonConfig);
        }
        return null;
    }

    public static CoroutineManager GetCoroutineManager()
    {
        if (m_coroutineManager == null)
        {
            m_coroutineManager = new GameObject("Coroutine Manager Editor").AddComponent<CoroutineManager>();
        }
        return m_coroutineManager;
    }

    public static void DestroyCoroutineManager()
    {
        if (m_coroutineManager != null)
        {
            Object.DestroyImmediate(m_coroutineManager.gameObject);
        }
    }
}
