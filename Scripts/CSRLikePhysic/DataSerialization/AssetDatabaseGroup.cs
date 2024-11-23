using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AssetDatabaseGroup
{
    public string GroupName;
    public bool IsActive;
    public bool IsDefault;
    [Searchable]
    public List<AssetDatabaseAsset> AssetDatabaseAssets;
}
