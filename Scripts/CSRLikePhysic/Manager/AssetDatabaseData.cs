using System;
using System.Collections;
using System.Collections.Generic;

public class AssetDatabaseData
{
	private List<AssetDatabaseAsset> assets = new List<AssetDatabaseAsset>();

	private JsonDict header = new JsonDict();

	private Dictionary<string, AssetDatabaseAsset> assetCodeMap = new Dictionary<string, AssetDatabaseAsset>();

	private Dictionary<GTAssetTypes, List<AssetDatabaseAsset>> assetTypeMap = new Dictionary<GTAssetTypes, List<AssetDatabaseAsset>>();

	private Dictionary<GTAssetTypes, Dictionary<string, AssetDatabaseAsset>> assetTypeMapMap = new Dictionary<GTAssetTypes, Dictionary<string, AssetDatabaseAsset>>();

	private bool isValid;

	public bool IsValid
	{
		get
		{
			return this.isValid;
		}
	}

	private void Clear()
	{
		this.assets.Clear();
		this.assetCodeMap.Clear();
		this.assetTypeMap.Clear();
		this.assetTypeMapMap.Clear();
		this.isValid = false;
	}

	public bool AssetExists(string assetCode)
	{
		return this.assetCodeMap.ContainsKey(assetCode);
	}

	public AssetDatabaseAsset GetAsset(string assetCode, bool okToFail = false)
	{
		AssetDatabaseAsset result;
		if (!this.assetCodeMap.TryGetValue(assetCode, out result))
		{
			if (!okToFail)
			{
			}
			return null;
		}
		return result;
	}

	public Dictionary<string, AssetDatabaseAsset> GetAssets()
	{
		return this.assetCodeMap;
	}

	public List<AssetDatabaseAsset> GetAssetsOfType(GTAssetTypes inType)
	{
		return this.assetTypeMap[inType];
	}

	public Dictionary<string, AssetDatabaseAsset> GetAssetsOfTypeDict(GTAssetTypes inType)
	{
		return this.assetTypeMapMap[inType];
	}

	public int GetAssetVersion(string assetCode, bool okToFail = false)
	{
		AssetDatabaseAsset asset = this.GetAsset(assetCode, okToFail);
		if (asset == null)
		{
			return 0;
		}
		return asset.version;
	}

	public bool ValidateAndLoadAssetDatabase(string databaseString)
	{
		JsonReader jsonReader = JsonConverter.Reader(databaseString);
		this.Clear();
		if (!jsonReader.Read())
		{
			return false;
		}
		if (!jsonReader.Read())
		{
			return false;
		}
		this.header = new JsonDict();
		if (!this.header.Read(jsonReader))
		{
			return false;
		}
		if (!jsonReader.Read())
		{
			return false;
		}
		JsonList jsonList = new JsonList();
		if (!jsonList.Read(jsonReader))
		{
			return false;
		}
		string text = jsonList.ToString();
		text.Remove(0, 1);
		string @string = this.header.GetString("cusanu_fy_ass_metel_gloyw");
		string b = @string;
		string a = string.Empty;
		if (BasePlatform.ActivePlatform != null)
		{
			a = BasePlatform.ActivePlatform.HMACSHA1_Hash(text, BasePlatform.eSigningType.Client_Everything);
		}
		if (a != b)
		{
			return false;
		}
		this.ConvertJsonToAssets(jsonList);
		if (!this.BuildIndexDict())
		{
			return false;
		}
		if (!this.BuildTypesListsDict())
		{
			return false;
		}
		this.isValid = true;
		return true;
	}

	private void ConvertJsonToAssets(JsonList assetsAsJson)
	{
		this.assets.Clear();
		for (int i = 0; i < assetsAsJson.Count; i++)
		{
			JsonDict jsonDict = assetsAsJson.GetJsonDict(i);
			AssetDatabaseAsset item = AssetDatabaseAsset.MakeAssetDatabaseAssetFromJsonOldFormat(jsonDict);
			this.assets.Add(item);
		}
	}

	private bool BuildIndexDict()
	{
		foreach (AssetDatabaseAsset current in this.assets)
		{
			this.assetCodeMap[current.code] = current;
		}
		return true;
	}

	private bool BuildTypesListsDict()
	{
		IEnumerator enumerator = Enum.GetValues(typeof(GTAssetTypes)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				GTAssetTypes key = (GTAssetTypes)((int)enumerator.Current);
				List<AssetDatabaseAsset> value = new List<AssetDatabaseAsset>();
				Dictionary<string, AssetDatabaseAsset> value2 = new Dictionary<string, AssetDatabaseAsset>();
				this.assetTypeMap[key] = value;
				this.assetTypeMapMap[key] = value2;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		foreach (AssetDatabaseAsset current in this.assets)
		{
			if (this.assetTypeMap.ContainsKey(current.type))
			{
				List<AssetDatabaseAsset> list = this.assetTypeMap[current.type];
				list.Add(current);
			}
		}
		foreach (GTAssetTypes current2 in this.assetTypeMap.Keys)
		{
			List<AssetDatabaseAsset> list2 = this.assetTypeMap[current2];
			Dictionary<string, AssetDatabaseAsset> dictionary = this.assetTypeMapMap[current2];
			foreach (AssetDatabaseAsset current3 in list2)
			{
				dictionary[current3.code] = current3;
			}
		}
		return true;
	}

	public int GetVersion()
	{
		int num = 0;
		string s;
		if (this.header.TryGetValue("version", out s))
		{
			int.TryParse(s, out num);
		}
		if (num == 0)
		{
		}
		return num;
	}

	public int GetBaseVersion()
	{
		int result = -1;
		this.header.TryGetValue("base_version", out result);
		return result;
	}

    public string GetMinimumVersion()
    {
        string empty = string.Empty;
        if (this.header.TryGetValue("minimum_version", out empty))
        {
            return empty;
        }
        return string.Empty;
    }

    public string GetAppVersion()
	{
		string empty = string.Empty;
		if (this.header.TryGetValue("product_version", out empty))
		{
			return empty;
		}
		return string.Empty;
	}

	public string GetBranch()
	{
		string empty = string.Empty;
		if (this.header.TryGetValue("branch_name", out empty))
		{
			return empty;
		}
		return string.Empty;
	}
}
