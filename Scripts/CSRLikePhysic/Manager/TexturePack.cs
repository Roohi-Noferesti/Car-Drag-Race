using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TexturePack : MonoBehaviour
{
	private class BundledTextureLoader : IBundleOwner
	{
		private List<KeyValuePair<TexturePack.TextureLoadedDelegate, string>> Callbacks;

		public BundledTextureLoader(string bundleID)
		{
			AssetProviderClient.Instance.RequestAsset(bundleID, new BundleLoadedDelegate(this.AssetLoaded), this);
			this.Callbacks = new List<KeyValuePair<TexturePack.TextureLoadedDelegate, string>>();
		}

		public void RequestCallback(string textureID, TexturePack.TextureLoadedDelegate action)
		{
			this.Callbacks.Add(new KeyValuePair<TexturePack.TextureLoadedDelegate, string>(action, textureID));
		}

		public void AssetLoaded(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
		{
		    var names = zAssetBundle.GetAllAssetNames();
            GameObject gameObject = zAssetBundle.LoadAsset("TexturePack", typeof(GameObject)) as GameObject;
			TexturePack component = gameObject.GetComponent<TexturePack>();
			TexturePack.AddPackToCache(zAssetID, component);
			foreach (KeyValuePair<TexturePack.TextureLoadedDelegate, string> current in this.Callbacks)
			{
				current.Key(component.GetTexture(current.Value));
			}
			AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
			TexturePack.BundleLoadsInFlight.Remove(zAssetID);
		}
	}

	public delegate void TextureLoadedDelegate(Texture2D texture);

	private const int CachedPackLimit = 2;

	public List<string> Keys = new List<string>();

	public List<Texture2D> Textures = new List<Texture2D>();

	private DateTime LastAccessed = DateTime.MinValue;

	private static Dictionary<string, TexturePack> CachedPacks = new Dictionary<string, TexturePack>();

	private static Dictionary<string, TexturePack.BundledTextureLoader> BundleLoadsInFlight = new Dictionary<string, TexturePack.BundledTextureLoader>();

	public Texture2D GetTexture(string key)
	{
		if (BuildType.IsAppTuttiBuild && key.ToLower() == "italia")
			key += "apptutti";
		int num = this.Keys.FindIndex((string q) => q == key);
		return (num != -1) ? this.Textures[num] : null;
	}

	public static void RequestTextureFromBundle(string bundleID, string textureID, TexturePack.TextureLoadedDelegate action)
	{
	    bundleID = bundleID.ToLower();
	    textureID = textureID.ToLower();

        TexturePack texturePack;
		TexturePack.BundledTextureLoader bundledTextureLoader;
		if (TexturePack.CachedPacks.TryGetValue(bundleID, out texturePack))
		{
			action(texturePack.GetTexture(textureID));
			texturePack.LastAccessed = GTDateTime.Now;
		}
		else if (TexturePack.BundleLoadsInFlight.TryGetValue(bundleID, out bundledTextureLoader))
		{
			bundledTextureLoader.RequestCallback(textureID, action);
		}
		else
		{
			TexturePack.ReduceCache();
			bundledTextureLoader = new TexturePack.BundledTextureLoader(bundleID);
			bundledTextureLoader.RequestCallback(textureID, action);
			TexturePack.BundleLoadsInFlight.Add(bundleID, bundledTextureLoader);
		}
	}

	public static void RequestTextureFromBundle(string textureAddress, TexturePack.TextureLoadedDelegate action)
	{
		int num = textureAddress.IndexOf('.');
		string bundleID = textureAddress.Substring(0, num);
		string textureID = textureAddress.Substring(num + 1, textureAddress.Length - num - 1);

#if UNITY_EDITOR && !USE_ASSET_BUNDLE
        var path = "Assets/TexturePacker/" + bundleID + "/" + textureID + ".psd";
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        action(texture);
#else
        TexturePack.RequestTextureFromBundle(bundleID, textureID, action);
#endif
    }

    public static void PrecacheBundle(string bundleID)
	{
		if (!TexturePack.CachedPacks.ContainsKey(bundleID) && !TexturePack.BundleLoadsInFlight.ContainsKey(bundleID))
		{
			TexturePack.ReduceCache();
			TexturePack.BundledTextureLoader value = new TexturePack.BundledTextureLoader(bundleID);
			TexturePack.BundleLoadsInFlight.Add(bundleID, value);
		}
	}

	private static void AddPackToCache(string bundleID, TexturePack pack)
	{
		TexturePack.CachedPacks.Add(bundleID, pack);
		pack.LastAccessed = GTDateTime.Now;
	}

	public static void ClearCache()
	{
		TexturePack.CachedPacks.Clear();
	}

	private static void ReduceCache()
	{
		while (TexturePack.CachedPacks.Count >= 2)
		{
			DateTime t = DateTime.MaxValue;
			string key = null;
			foreach (KeyValuePair<string, TexturePack> current in TexturePack.CachedPacks)
			{
				if (current.Value.LastAccessed < t)
				{
					t = current.Value.LastAccessed;
					key = current.Key;
				}
			}
			TexturePack.CachedPacks.Remove(key);
		}
	}
}
