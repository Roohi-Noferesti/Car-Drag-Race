using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AssetBundleExtension 
{
    public static string mainAsset(this AssetBundle assetBundle)
    {
        return assetBundle.GetAllAssetNames()[0];
    }
}
