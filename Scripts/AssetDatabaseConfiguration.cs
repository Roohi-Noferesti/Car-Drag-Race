#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AssetDatabaseConfiguration : ScriptableSingleton<AssetDatabaseConfiguration>
{

    
    public int version;

    //public string product_version;
    public string minimum_version;
    [HideInInspector] public int base_version;
    public List<AssetDatabaseGroup> AssetGroups;

    public AssetDatabaseGroup DefaultGroup
    {
        get { return AssetGroups.FirstOrDefault(a => a.IsDefault); }
    }


    public List<string> SimilarAssetInEachGroup
    {
        get
        {
            List<string> similarAssetsInEachGroup = new List<string>();
            foreach (var assetGroup in AssetGroups.Where(g => !g.IsDefault))
            {
                foreach (var asset in assetGroup.AssetDatabaseAssets)
                {
                    if (!similarAssetsInEachGroup.Contains(asset.code))
                    {
                        similarAssetsInEachGroup.Add(asset.code);
                    }
                }
            }

            return similarAssetsInEachGroup;
        }
    }


    public List<string> AllObjectsInAsset(string assetCode)
    {
        //List<string> allObjects = new List<string>();
        //foreach(var g in AssetGroups)
        //{
        //    var asset = g.AssetDatabaseAssets.FirstOrDefault(a => a.code == code);
        //    if (asset != null)
        //    {
        //        foreach(var obj in asset.ObjectsPaths)
        //        {
        //            if (!allObjects.Contains(obj))
        //            {
        //                allObjects.Add(obj);
        //            }
        //        }
        //    }
        //}

        List<AssetDatabaseGroup> groups = new List<AssetDatabaseGroup>();
        foreach (var g in AssetGroups)
        {
            var asset = g.AssetDatabaseAssets.FirstOrDefault(a => a.code == assetCode);
            if (asset != null)
            {
                groups.Add(g);
            }
        }
        return groups.SelectMany
            (a => a.AssetDatabaseAssets.FirstOrDefault(b => b.code == assetCode).ObjectsPaths)
            .Distinct<string>().ToList();
        //allObjects.Except(AssetGroups[0].AssetDatabaseAssets.FirstOrDefault(a => a.code == code).ObjectsPaths);
    }

    public List<string> ObjectPaths(string group, string asset)
    {
        return AssetGroups.FirstOrDefault(a => a.GroupName == group).AssetDatabaseAssets.FirstOrDefault(b => b.code == asset).ObjectsPaths;
    }


    [Button("Update Object List")]
    private void UpdateAssetDatabaseObjectList()
    {
        AssetDatabaseUtility.UpdateAssetDatabaseObjectList(this);
    }


    [Button("Restore Bundle labels")]
    private void RestoreLabels()
    {
        AssetDatabaseUtility.RestoreAllGroupLabels(this);
    }


    [Button("Build Database Only")]
    private void BuildDatabase()
    {
        this.CreateAssetDatabase(BuildTarget.Android);
        AssetDatabaseUtility.RevealDatabaseInFinder();
    }

    [Button("Build Android")]
    public void BuildAndroid()
    {
        this.Build(BuildTarget.Android);
    }

    [Button("Build iOS")]
    public void BuildIOS()
    {
        this.Build(BuildTarget.iOS);
    }


    [Button("Increase All Asset Version")]
    public void IncreaseAssetVersion()
    {
        this.IncreaseAllAssetVersion();
    }
    
    
    [Button("Increase Asset Databse Version")]
    public void IncreaseDatabaseVersion()
    {
        this.IncreaseAssetDatabaseVersion();
    }
    
    [ContextMenu("ClearVehicles")]
    private void ClearVehicles()
    {
        AssetGroups[3].AssetDatabaseAssets =
            AssetGroups[3].AssetDatabaseAssets.Where(x => x.type != GTAssetTypes.vehicle).ToList();
    }

    [ContextMenu("Add Vehicles From Selected Car Prefabs")]
    private void AddVehicles()
    {
        var selection = Selection.objects;
        var result = new List<AssetDatabaseAsset>();
        foreach (var obj in selection)
        {
            string code = obj.name.ToLower();
            code = code.Replace("car_", "");
            code = code.Replace("_facelift", "");
            code = code.Replace("_", "");
            int oldVersion = 0;
            try {
                oldVersion = AssetGroups[0].AssetDatabaseAssets.Where(x => x.code == code).ToArray()[0].version;
            } catch {
                Debug.LogError("Error on " + obj.name.ToLower());
            }
            List<string> list = new List<string>(1);
            list.Add(AssetDatabase.GetAssetPath(obj));
            AssetDatabaseAsset dbAsset = new AssetDatabaseAsset(code, oldVersion+1, GTAssetTypes.vehicle);
            dbAsset.ObjectsPaths = list;
            result.Add(dbAsset);
        }
        result.AddRange(AssetGroups[3].AssetDatabaseAssets);
        AssetGroups[3].AssetDatabaseAssets = result;
    }
    
    
    [ContextMenu("ClearLiveries")]
    private void ClearLiveries()
    {
        AssetGroups[0].AssetDatabaseAssets =
            AssetGroups[0].AssetDatabaseAssets.Where(x => x.type != GTAssetTypes.livery).ToList();
    }

    [ContextMenu("Add Liveries From Folder")]
    private void AddLiveries()
    {
        Debug.LogWarning("project path should set corrrectly");
       /* var dirPath = @"C:/Projects/GT Club 1/Assets/Classic/art/bundles/liveries";
        var rootPath = @"C:/Projects/GT Club 1/";
        var files = Directory.GetFiles(dirPath, "*.prefab", SearchOption.AllDirectories);
        files = files.Select(x => x.Remove(0, rootPath.Length).Replace('\\','/').ToLowerInvariant()).ToArray();
            
        var result = new List<AssetDatabaseAsset>();

        foreach (var filePath in files)
        {
            var segments = filePath.Split('/');
            var objName = segments[segments.Length-1].Replace(".prefab","");
            var carName = segments[segments.Length-2].Replace(".prefab","");
            
            var bundleName = carName+"_livery_" + objName;
            AssetImporter.GetAtPath(filePath).SetAssetBundleNameAndVariant(bundleName,"");

            result.Add(new AssetDatabaseAsset(bundleName, 1, GTAssetTypes.livery));
        }
        result.AddRange(AssetGroups[0].AssetDatabaseAssets);
        AssetGroups[0].AssetDatabaseAssets = result;*/
    }
}


#endif