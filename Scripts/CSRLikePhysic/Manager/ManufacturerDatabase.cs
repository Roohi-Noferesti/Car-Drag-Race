using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ManufacturerDatabase : MonoBehaviour, IBundleOwner
{
	public static ManufacturerDatabase Instance;

	public bool isReady
	{
		get;
		private set;
	}

    public Dictionary<string, Manufacturer> Manufacturers
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
        this.Manufacturers = new Dictionary<string, Manufacturer>();
		this.isReady = false;
	}

	public void Shutdown()
	{
		this.isReady = false;
		this.Manufacturers.Clear();
	}

	public void Load()
	{
		this.isReady = false;
#if UNITY_EDITOR
        var manufactures = AssetDatabase.LoadAssetAtPath<ManufacturesConfiguration>("Assets/configuration/Manufactures.asset");
        InitialiseManufactures(manufactures);
#else
		AssetProviderClient.Instance.RequestAsset("manufactures".ToLower(), new BundleLoadedDelegate(this.ManufacturersBundleReady), this);
#endif
    }



    private void ManufacturersBundleReady(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
        var configuration = zAssetBundle.LoadAsset<ManufacturesConfiguration>(zAssetBundle.mainAsset());
        InitialiseManufactures(configuration);
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
		this.isReady = true;
	}


    private void InitialiseManufactures(ManufacturesConfiguration manufactures)
    {
        this.Manufacturers.Clear();
        foreach (Manufacturer current in manufactures.Manufactures)
        {
            this.Manufacturers[current.id] = current;
        }
        this.isReady = true;
    }

    public Manufacturer Get(string manuId)
	{
		return this.Manufacturers[manuId];
	}
}
