using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;
using ThreadPriority = System.Threading.ThreadPriority;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResourceManager :MonoSingleton<ResourceManager>
{
    //[SerializeField]
    //private ItemAssets m_itemAssets;

    private static ResourceRequest GetAssetAsync<T>(string path, string postfix = "")
    where T : Object
    {
        return Resources.LoadAsync<T>(path + postfix);
    }

    public static T GetAsset<T>(string path, string postfix = "")
    where T : Object
    {
        return Resources.Load<T>(path + postfix);
    }

    public static T[] GetAssetAll<T>(string path)
where T : Object
    {
        return Resources.LoadAll<T>(path);
    }

    public static T GetCarAsset<T>(string carID, ServerItemBase.AssetType assetType, string subItemID = null, string postfix = "")
        where T : Object
    {
        var path = GetCarAssetPath(carID, assetType, subItemID);
        return GetAsset<T>(path, postfix);
    }

    //public static T GetSharedCarAsset<T>(string itemID,string carID, ServerItemBase.AssetType assetType)
    //where T : Object
    //{
    //    var path = GetSharedCarAssetPath(itemID,carID, assetType);
    //    return GetAsset<T>(path);
    //}

    public static ResourceRequest GetCarAssetAsync<T>(string carID, ServerItemBase.AssetType assetType,
        string subItemID = null, string postfix = "")
        where T : Object
    {
        var path = GetCarAssetPath(carID, assetType, subItemID);
        return GetAssetAsync<T>(path, postfix);
    }

    //public static ResourceRequest GetSharedCarAssetAsync<T>(string itemID,string carID, ServerItemBase.AssetType assetType,
    //string subItemID = null)
    //where T : Object
    //{
    //    var path = GetSharedCarAssetPath(itemID,carID, assetType);
    //    return GetAssetAsync<T>(path);
    //}

    public static T GetSharedAsset<T>(string itemID, ServerItemBase.AssetType assetType, string postFix = "")
    where T : Object
    {
        var path = GetSharedAssetPath(itemID, assetType);
        return GetAsset<T>(path, postFix);
    }

    public static T[] GetSharedAssetAll<T>(string itemID, ServerItemBase.AssetType assetType)
        where T : Object
    {
        var path = GetSharedAssetPath(itemID, assetType);
        return GetAssetAll<T>(path);
    }

    public static ResourceRequest GetSharedAssetAsync<T>(string itemID, ServerItemBase.AssetType assetType, string postFix = "")
        where T : Object
    {
        var path = GetSharedAssetPath(itemID, assetType);
        return GetAssetAsync<T>(path);
    }

    private static string GetCarAssetPath(string carID, ServerItemBase.AssetType assetType, string subItemID = null)
    {
        var path =  "cars/" + carID + "/" + assetType + "/" + (String.IsNullOrEmpty(subItemID) ? carID : subItemID);
        //Debug.Log(path);
        return path;
    }

    private static string GetSharedAssetPath(string itemID, ServerItemBase.AssetType assetType)
    {
        return "shared_assets/" + assetType + "/" + itemID;
    }

    //private static string GetSharedCarAssetPath(string itemID,string carItemID, ServerItemBase.AssetType assetType)
    //{
    //    var index = itemID.IndexOf(carItemID);
    //    var finalPath = "shared_assets/" + assetType + "/" + itemID.Substring(0, index-1);
    //    return "shared_assets/" + assetType + "/" + itemID.Substring(0, index-1);
    //}


    private const float TIMEOUT = 40;
    public static void GetCarsThumbnails(string[] ids, Action<Texture2D[], bool> loadCompleted, bool customThumbnail)
    {
        Instance.StartCoroutine(_getCarsThumbnails(ids, loadCompleted, customThumbnail));
    }
    private static IEnumerator _getCarsThumbnails(string[] ids, Action<Texture2D[], bool> loadCompleted, bool customThumbnail)
    {
        var t = Time.realtimeSinceStartup;
        var thumbnails = new List<Texture2D>();
        foreach (string id in ids)
        {
            GetCarThumbnail(id, customThumbnail, texture =>
            {
                thumbnails.Add(texture);
            });
        }

        while (thumbnails.Count < ids.Length && Time.realtimeSinceStartup - t < TIMEOUT)
        {
            yield return 0;
        }

        loadCompleted(thumbnails.ToArray(), thumbnails.Count == ids.Length);
    }

    public static void GetCarThumbnail(string itemID, bool customThumbnail, Action<Texture2D> loadCompleted)
    {
        if (customThumbnail && ExternalThumbnailExists(itemID))
        {
            GetExternalThumbnail(itemID, texture =>
            {
                if (loadCompleted != null)
                {
                    loadCompleted(texture);
                }
            });
        }
        else
        {
            Instance.StartCoroutine(_loadAssetAsync(itemID, ServerItemBase.AssetType.thumbnail, false, loadCompleted, GameDatabase.Instance.CarsConfiguration.postfix));
        }
    }

    public static void LoadAssetAsync<T>(string itemID, ServerItemBase.AssetType type, bool shared, Action<T> loadCompleted) where T : Object
    {
        Instance.StartCoroutine(_loadAssetAsync(itemID, type, shared, loadCompleted));
    }

    public static void LoadAssetAsync<T>(string path, Action<T> loadCompleted) where T : Object
    {
        Instance.StartCoroutine(_loadAssetAsync(path, loadCompleted));
    }

    private static IEnumerator _loadAssetAsync<T>(string itemID, ServerItemBase.AssetType type, bool shared, Action<T> loadCompleted, string postfix = "") where T : Object
    {
        var rr = shared
            ? GetSharedAssetAsync<T>(itemID, type, postfix)
            : GetCarAssetAsync<T>(itemID, type, null, postfix);
        while (!rr.isDone)
        {
            yield return 0;
        }

        if (loadCompleted != null)
        {
            loadCompleted((T)rr.asset);
        }
    }


    private static IEnumerator _loadAssetAsync<T>(string path, Action<T> loadCompleted) where T : Object
    {
        var rr = GetAssetAsync<T>(path);
        while (!rr.isDone)
        {
            yield return 0;
        }

        if (loadCompleted != null)
        {
            loadCompleted((T)rr.asset);
        }
    }

    public static void SaveScreenshot(Texture2D virtualPhoto, string fileName)
    {
        var bytes = virtualPhoto.EncodeToPNG();
        var path = OurTempSquareImageLocation(fileName);
        File.WriteAllBytes(path
            , bytes);
        // virtualCam.SetActive(false); ... no great need for this.
        //LogUtility.Log("Saved to : " + OurTempSquareImageLocation(fileName));
    }

    public static void SaveScreenshotInAssets(Texture2D virtualPhoto, string filePath)
    {
#if UNITY_EDITOR

        //AssetDatabase.CreateAsset(virtualPhoto, filePath);
        var bytes = virtualPhoto.EncodeToPNG();
        //var path =  OurTempSquareImageLocation(fileName);
        File.WriteAllBytes(filePath
            , bytes);
        //AssetDatabase.Refresh();
        // virtualCam.SetActive(false); ... no great need for this.

        filePath = filePath.Replace(Application.dataPath, "Assets");
        TextureImporter tImporter = AssetImporter.GetAtPath(filePath) as TextureImporter;
        if (tImporter != null)
        {
            tImporter.mipmapEnabled = false;
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.Default);
        }
        Debug.Log("Saved to : " + filePath);
#endif

    }

    private static string OurTempSquareImageLocation(string carID)
    {
        return Application.persistentDataPath + "/" + carID + ".png";
    }

    public static void GetExternalThumbnail(string carID, Action<Texture2D> loadCompleted)
    {
        Instance.StartCoroutine(Instance.LoadExternalImage(carID, loadCompleted));
    }

    public static bool ExternalThumbnailExists(string carID)
    {
        var path = OurTempSquareImageLocation(carID);
        return File.Exists(path);
    }

    IEnumerator LoadExternalImage(string carID, Action<Texture2D> loadCompleted)
    {
        var path = OurTempSquareImageLocation(carID);
        //Debug.Log("Loading thumbnails at : " + path);
        if (!File.Exists(path)) yield break;
        //Debug.Log("thumbnails exists at : " + path);
        //Debug.Log(path);
        //var textureSize = GetTextureSize(512, 1.337f);
        //var texture2 = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.ARGB32, false);
        //var loadImageWWW = new WWW(path);
        //yield return loadImageWWW;

        //byte[] bytes = null;
        //bool done = false;
        //Thread thread = new Thread(() =>
        //{
        //    using (FileStream stream = File.Open(path, FileMode.Open))
        //    {
        //        bytes = new byte[stream.Length];
        //        stream.BeginRead(bytes, 0, (int) stream.Length, (callback) =>
        //        {
        //            if (callback.IsCompleted)
        //            {
        //                done = true;
        //            }
        //        }, null);
        //    }
        //});
        //thread.Priority = ThreadPriority.Lowest;
        //thread.Start();

        //while (!done)
        //{
        //    yield return new WaitForEndOfFrame();
        //}
        ////Texture2D texture = new Texture2D(width, height);
        ////texture.filterMode = FilterMode.Trilinear;
        //if (bytes != null)
        //{
        //    Debug.Log("loading image by size : " + bytes.Length);
        //    texture2.LoadImage(bytes);
        //}

        var fullPath = "file://" + path;
        WWW www = new WWW(fullPath);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("error loading image : "+www.error);
        }


        if (loadCompleted != null)
            loadCompleted(www.texture);

        //loadImageWWW.LoadImageIntoTexture(texture2);
        //if (loadCompleted != null)
        //    loadCompleted(texture2);
        //yield return 0;
    }

    private static Vector2 GetTextureSize(float size, float aspect)
    {
        int height = (int)(size / aspect);
        return new Vector2(size, height);
    }

    public static void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
    }
}
