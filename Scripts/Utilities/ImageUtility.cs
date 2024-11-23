using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ImageUtility
{
    private static Queue<UserImageData> m_cachedImage = new Queue<UserImageData>();

    private static UserImageData GetImageData(string googleUrl, UserImageSize size)
    {
        return m_cachedImage.FirstOrDefault(i => i.ImageUrl == googleUrl && i.ImageSize == size);
    }
    public static void LoadImage(string googleUrl, UserImageSize imageSize, Action<bool,Texture> callback)
    {
        var imageData = GetImageData(googleUrl, imageSize);
        if (imageData == null)
        {
            if (!string.IsNullOrEmpty(googleUrl))
            {
                CoroutineManager.Instance.StartCoroutine(_loadImage(googleUrl,imageSize, callback));
            }
            else
            {
                callback(false, null);
            }
        }
        else
        {
            callback(true, imageData.ImageCache);
        }
    }

    private static IEnumerator _loadImage(string googleUrl, UserImageSize imageSize, Action<bool, Texture> callback)
    {
        var size = (int)imageSize;
        var url = googleUrl.Replace("sz=50", "sz=" + size);
        var www = new WWW(url);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            if (m_cachedImage.Count > 50)
            {
                m_cachedImage.Dequeue();
                m_cachedImage.Enqueue(new UserImageData(0, googleUrl, www.texture, imageSize));
            }
            callback(true,www.texture);
        }
        else
        {
            GTDebug.Log(GTLogChannel.Screens,"error loading image : " + www.error);
            callback(false, null);
        }
    }
}
