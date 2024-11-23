using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceUtility : MonoSingleton<ResourceUtility>
{
    private const float TIMEOUT = 5;
    public static void GetCarsThumbnails(string[] ids, Action<Texture2D[],bool> loadCompleted, bool customThumbnail)
    {
        Instance.StartCoroutine(_getCarsThumbnails(ids, loadCompleted, customThumbnail));
    }
    private static IEnumerator _getCarsThumbnails(string[] ids, Action<Texture2D[],bool> loadCompleted, bool customThumbnail)
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

        while (thumbnails.Count < ids.Length && Time.realtimeSinceStartup-t<TIMEOUT)
        {
            yield return 0;
        }

        loadCompleted(thumbnails.ToArray(), thumbnails.Count==ids.Length);
    }

    public static void GetCarThumbnail(string itemID,bool customThumbnail,Action<Texture2D> loadCompleted )
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
            Instance.StartCoroutine(_loadAssetAsync(itemID,ServerItemBase.AssetType.thumbnail,false,loadCompleted));
        }
    }

    public static void LoadAssetAsync<T>(string itemID, ServerItemBase.AssetType type,bool shared, Action<T> loadCompleted) where T : Object
    {
        Instance.StartCoroutine(_loadAssetAsync(itemID, type,shared, loadCompleted));
    }

    private static IEnumerator _loadAssetAsync<T>(string itemID,ServerItemBase.AssetType type,bool shared,Action<T> loadCompleted) where T : Object
    {
        var rr = shared
            ? ResourceManager.GetSharedAssetAsync<T>(itemID, type)
            : ResourceManager.GetCarAssetAsync<T>(itemID, type);
        while (!rr.isDone)
        {
            yield return 0;
        }

        if (loadCompleted != null)
        {
            loadCompleted((T) rr.asset);
        }
    }

    public static void SaveScreenshot(Texture2D virtualPhoto, string fileName)
    {
        var bytes = virtualPhoto.EncodeToPNG();
        var path = OurTempSquareImageLocation(fileName);
        File.WriteAllBytes(path
            , bytes);
        // virtualCam.SetActive(false); ... no great need for this.
        GTDebug.Log(GTLogChannel.CarSnapshot, "Saved to : " + OurTempSquareImageLocation(fileName));
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

        if (!File.Exists(path)) yield break;
        //Debug.Log(path);
        var textureSize = GetTextureSize(512, 1.337f);
        var texture2 = new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.ARGB32, false);
        //var loadImageWWW = new WWW(path);
        //yield return loadImageWWW;
        byte[] bytes = File.ReadAllBytes(path);
        //Texture2D texture = new Texture2D(width, height);
        //texture.filterMode = FilterMode.Trilinear;
        texture2.LoadImage(bytes);

        //loadImageWWW.LoadImageIntoTexture(texture2);
        if (loadCompleted != null)
            loadCompleted(texture2);
        yield return 0;
    }

    private static Vector2 GetTextureSize(float size,float aspect)
    {
        int height = (int)(size / aspect);
        return new Vector2(size, height);
    }
}
