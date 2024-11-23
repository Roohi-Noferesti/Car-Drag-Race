using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class AssetDatabaseAsset
{
    [SerializeField]
    private string m_code;
    [SerializeField]
    private int m_version;
    [SerializeField]
    private GTAssetTypes m_type;

    [SerializeField] private List<string> _objectsPaths;


    public string code
    {
        get { return m_code; }
    }

    public int version
    {
        get { return m_version; }
        set => m_version = value;
    }

    public GTAssetTypes type
    {
        get { return m_type; }
        set { m_type = value; }
    }


    public List<string> ObjectsPaths
    {
        get { return _objectsPaths; }
        set { _objectsPaths = value; }
    }

    public AssetDatabaseAsset(string inCode, int inVersion, GTAssetTypes inType)
	{
		this.m_code = inCode;
		this.m_version = inVersion;
        this.type = inType;
	}

	public static AssetDatabaseAsset MakeAssetDatabaseAssetFromJson(JsonDict assetAsJson)
	{
		int @int = assetAsJson.GetInt("Version");
		JsonDict jsonDict = assetAsJson.GetJsonDict("Asset");
		string @string = jsonDict.GetString("Code");
		int int2 = jsonDict.GetInt("TypeId");
		GTAssetTypes inType = (GTAssetTypes)int2;
		return new AssetDatabaseAsset(@string, @int, inType);
	}

	public static AssetDatabaseAsset MakeAssetDatabaseAssetFromJsonOldFormat(JsonDict assetAsJson)
	{
		string @string = assetAsJson.GetString("code");
		int inVersion = assetAsJson.GetInt("version");
		int num = assetAsJson.GetInt("type");
		GTAssetTypes inType = (GTAssetTypes)num;
		return new AssetDatabaseAsset(@string, inVersion, inType);
	}
}
