using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public abstract class ConfigurationAssetLoader : IBundleOwner
{
	protected GTAssetTypes _assetType;

	protected string _assetID = string.Empty;

	public bool IsValid
	{
		get; protected set;
	}

	public GTAssetTypes GetAssetType
	{
		get
		{
			return this._assetType;
		}
	}

	public string AssetID
	{
		get
		{
			return this._assetID;
		}
	}

	protected ConfigurationAssetLoader(GTAssetTypes assetType, string assetID)
	{
		this._assetType = assetType;
		this._assetID = assetID;
	}

	private ConfigurationAssetLoader()
	{
	}

	public void Initialise()
	{
        this.LoadMetaData();
	}
    private void LoadMetaData()
	{
		if (string.IsNullOrEmpty(this._assetID))
		{
			return;
		}

#if UNITY_EDITOR && !USE_ASSET_BUNDLE
        //var scriptableObject =
        //    AssetDatabase.LoadAssetAtPath<ScriptableObject>(
        //        "Assets/configuration/" + _assetID + ".asset");
        var objects = AssetProviderClient.Instance.RequestAssets<ScriptableObject>(this._assetID.ToLower());
        if (objects.Length > 0)
        {
            ProcessEventAssetDataString(objects[0]);
            this.IsValid = true;
        }
        else
        {
            Debug.Log("no config found for " + _assetID.ToLower());
        }
#else
        AssetProviderClient.Instance.RequestAsset(this._assetID.ToLower(), this.BundleLoaded, this);
#endif
    }


    private void BundleLoaded(string assetID, AssetBundle assetBundle, IBundleOwner owner)
    {
        var mainAsset = assetBundle.LoadAsset<ScriptableObject>(assetBundle.mainAsset());
        if (!this.LoadAsset(mainAsset))
        {
            this.IsValid = false;
            return;
        }
        AssetProviderClient.Instance.ReleaseAsset(assetID, owner);
        this.IsValid = true;
    }

    //private void BundleLoaded(ScriptableObject scriptableObject)
    //{
    //    //UnityEngine.Object mainAsset = assetBundle.mainAsset;
    //    //if (!this.LoadAsset(mainAsset))
    //    //{
    //    //    this.IsValid = false;
    //    //    return;
    //    //}
    //    //AssetProviderClient.Instance.ReleaseAsset(assetID, owner);
    //    this.IsValid = true;
    //    ProcessEventAssetDataString(scriptableObject);
    //}

    protected bool LoadAsset(ScriptableObject scriptableObject)
	{
        //if (textasset == null)
        //{
        //    return false;
        //}
        //TextAsset textAsset = textasset as TextAsset;
        //string text = textAsset.text;
        //List<string> list = JsonConverter.DeserializeObject<List<string>>(text);
        //string headerString = list[0];
        //string text2 = list[1];
        //if (!this.ValidateAsset(headerString, text2))
        //{
        //    AssetDatabaseClient.Instance.CorruptedDataPopUp();
        //    return false;
        //}


        this.ProcessEventAssetDataString(scriptableObject);
		return true;
	}

	private bool ValidateAsset(string headerString, string bodyString)
	{
		GameDataHeader gameDataHeader = JsonConverter.DeserializeObject<GameDataHeader>(headerString);
		string platformDependentChecksum = gameDataHeader.GetPlatformDependentChecksum();
		string b = BasePlatform.ActivePlatform.HMACSHA1_Hash(bodyString, BasePlatform.eSigningType.Client_Everything);
		return platformDependentChecksum == b;
	}

	public void Shutdown()
	{
		this.IsValid = false;
		this.OnShutdown();
	}

	protected abstract void ProcessEventAssetDataString(ScriptableObject scriptableObject);

	protected virtual void OnShutdown()
	{
	}
}
