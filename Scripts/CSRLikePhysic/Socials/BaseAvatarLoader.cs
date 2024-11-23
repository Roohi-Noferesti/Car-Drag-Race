using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class BaseAvatarLoader
{
	public delegate void RequestDelegate(string userID);

	private int FriendPicSize = 256;

	private string CacheSuffix;

	protected Dictionary<string, List<PersonaComponent>> activeRequests = new Dictionary<string, List<PersonaComponent>>();

	public BaseAvatarLoader(string CacheSuffix)
	{
		this.CacheSuffix = CacheSuffix;
	}

	public string GetCachePicPath(string userID)
	{
		string path = string.Format("{0}_large_{1}.png", userID, this.CacheSuffix);
		return Path.Combine(SocialController.Instance.FriendsPicsPath, path);
	}

	protected bool IsAvatarInCache(string userID)
	{
		return File.Exists(this.GetCachePicPath(userID));
	}

	public void WritePicToCache(Texture2D pic, string userID)
	{
		if (!Directory.Exists(SocialController.Instance.FriendsPicsPath))
		{
			Directory.CreateDirectory(SocialController.Instance.FriendsPicsPath);
		}
		string cachePicPath = this.GetCachePicPath(userID);
		byte[] bytes = pic.EncodeToPNG();
		File.WriteAllBytes(cachePicPath, bytes);
	}

	protected Texture2D LoadAvatarFromCache(string userID)
	{
		Texture2D result;
		try
		{
			string cachePicPath = this.GetCachePicPath(userID);
			byte[] data = File.ReadAllBytes(cachePicPath);
			Texture2D texture2D = new Texture2D(this.FriendPicSize, this.FriendPicSize, TextureFormat.ARGB32, false);
			texture2D.LoadImage(data);
			texture2D.filterMode = FilterMode.Trilinear;
			result = texture2D;
		}
		catch (Exception var_3_3F)
		{
			result = null;
		}
		return result;
	}

	public void RequestProfilePictureFromUserID(string userID, PersonaComponent persona)
	{
		this.CheckCacheOrDoRequest(userID, persona, delegate
		{
			this.DoRequestProfilePictureFromUserID(userID);
		});
	}

	public void CheckCacheOrDoRequest(string userID, PersonaComponent persona, Action requestAction)
	{
		if (this.activeRequests.ContainsKey(userID))
		{
			this.activeRequests[userID].Add(persona);
			return;
		}
		if (this.IsAvatarInCache(userID))
		{
			Texture2D texture = this.LoadAvatarFromCache(userID);
			persona.OnAvatarLoaded(texture);
			return;
		}
		this.activeRequests.Add(userID, new List<PersonaComponent>
		{
			persona
		});
		requestAction();
	}

	protected abstract void DoRequestProfilePictureFromUserID(string userID);

	protected void RequestComplete(string userID, Texture2D texture)
	{
		List<PersonaComponent> list;
		if (!this.activeRequests.TryGetValue(userID, out list))
		{
			return;
		}
		foreach (PersonaComponent current in list)
		{
			if (texture != null)
			{
				current.OnAvatarLoaded(texture);
			}
			else
			{
				current.OnAvatarLoadFailed();
			}
		}
		this.activeRequests.Remove(userID);
		SocialController.Instance.CheckAndShrinkProfilePicCache();
	}
}
