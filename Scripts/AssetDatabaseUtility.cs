#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using Object = UnityEngine.Object;

public static class AssetDatabaseUtility
{
    private const string DATABASE_FILE_NAME = "asset_database";

    [MenuItem("Tools/Asset Bundle/Configuration")]
    private static void SelectConfiguration()
    {
        var config = (AssetDatabaseConfiguration)EditorGUIUtility.Load("AssetDatabaseConfiguration.asset");
        Selection.activeObject = config;
    }
    
    [MenuItem("Tools/Asset Bundle/Clear Cache")]
    private static void ClearCache()
    {
        Caching.ClearCache();
    }


    [MenuItem("Tools/Asset Bundle/Print File Size...")]
    private static void PrintFileSize()
    {
        var mainFilePath = EditorUtility.OpenFilePanel("Main File", "Select main file to compute size", "obb");
        if (!string.IsNullOrEmpty(mainFilePath))
        {
            var size = MathTools.ComputeSizeForFile(mainFilePath);
            Debug.Log(string.Format("size value for generated main obb file is (clipboard) :" + size));
            GUIUtility.systemCopyBuffer = size.ToString();
        }
    }


    [MenuItem("Tools/Asset Bundle/Print File Hash...")]
    private static void PrintFileHash()
    {
        var mainFilePath = EditorUtility.OpenFilePanel("Select File", "Select file to compute hash", "obb");
        if (!string.IsNullOrEmpty(mainFilePath))
        {
            var checksum = MathTools.ComputeHashForFile(mainFilePath);
            Debug.Log(string.Format("hash value for file is (clipboard) :" + checksum));
            GUIUtility.systemCopyBuffer = checksum;
        }
    }


    [MenuItem("Tools/Asset Bundle/Show Cache Folder")]
    private static void ShowCacheFolder()
    {
        EditorUtility.RevealInFinder(Application.temporaryCachePath);
    }


    [MenuItem("Tools/Asset Bundle/Show Build Folder")]
    private static void ShowBuildFolder()
    {
        RevealInFinder();
    }


    public static void RevealDatabaseInFinder()
    {
        RevealInFinder("/asset_database_Default_Android");
    }

    public static void RevealInFinder(string subPath=null)
    {
        EditorUtility.RevealInFinder(GetStreamingAssetBundlePath()+ subPath);
    }

    private static void DrawBuildAssetBundleButton(BuildTarget buildTarget)
    {
        if (GUILayout.Button("Build " + buildTarget))
        {
            //if(!m_buildJustDatabase)
            //    ArchivePreviousBundles(buildTarget);
            //CreateAssetDatabase(buildTarget);
            //if (!m_buildJustDatabase)
            //    CreateAssetBundles.CreateBundle(buildTarget);
            //if (!m_buildJustDatabase)
            //    CreateObb(buildTarget);
        }
    }


    public static void Build(this AssetDatabaseConfiguration configuration,BuildTarget buildTarget)
    {
        ArchivePreviousBundles(buildTarget);
        CreateAssetDatabase(configuration,buildTarget);
        CreateBundles(configuration,buildTarget);
        RevealInFinder();
    }


    private static void ArchivePreviousBundles(BuildTarget buildTarget)
    {
        var bundlePath = GetBundlePath(buildTarget);
        var buildPath = bundlePath + "/../";
        var buildRoot = buildPath + "../";

        //Delete StreamingAssets
        var streamingBundlePath = GetStreamingAssetBundlePath();
        if (Directory.Exists(streamingBundlePath))
        {
            Directory.Delete(streamingBundlePath, true);
        }

        //Comment because of unusabality
        //if (Directory.Exists(buildPath))
        //{
        //    AssetDatabaseData data = null;
        //    string defaultFile = bundlePath + "/asset_database_Default";
        //    string androidFile = bundlePath + "/asset_database_Default_" + buildTarget;
        //    if (File.Exists(defaultFile))
        //    {
        //        data = LoadAssetDatabase(defaultFile);
        //    }
        //    else if (File.Exists(androidFile))
        //    {
        //        data = LoadAssetDatabase(androidFile);
        //    }

        //    if (data != null)
        //    {
        //        var databaseVersion = data.GetAppVersion();
        //        var archiveDirectory = buildRoot + buildTarget + "_" + databaseVersion;
        //        if (Directory.Exists(archiveDirectory))
        //        {
        //            Directory.Delete(archiveDirectory,true);
        //            //Directory.CreateDirectory(archiveDirectory);
        //        }

        //        if(buildTarget != BuildTarget.iOS)
        //            Directory.Move(buildPath, archiveDirectory);
        //    }
        //}
    }

    public static void CreateAssetDatabase(this AssetDatabaseConfiguration assetDatabase, BuildTarget buildTarget)
    {
        var defaultDatabase = assetDatabase.DefaultGroup;

        if (defaultDatabase == null)
        {
            Debug.LogError("No Default Database Group found . Please set one of the asset groups as default");
            return;
        }
        //Getting sha1 code
        foreach (var assetDatabaseAssetGroup in assetDatabase.AssetGroups.Where(a=>a.IsActive))
        {
            StringBuilder tempSB = new StringBuilder();
            JsonWriter tempWriter = new JsonWriter(tempSB);
            WriteAssetList(ref tempWriter, assetDatabaseAssetGroup, defaultDatabase);
            var platform = new StandalonePlatform();
            var content = tempSB.ToString();
            var hash = platform.HMACSHA1_Hash(content, BasePlatform.eSigningType.Client_Everything);

            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);

            var version = assetDatabase.version;

            writer.WriteArrayStart();
            writer.WriteObjectStart();
            writer.WritePropertyName("version");
            writer.Write(version.ToString());
            writer.WritePropertyName("product_version");
            writer.Write(Application.version);
            writer.WritePropertyName("minimum_version");
            writer.Write(assetDatabase.minimum_version);
            writer.WritePropertyName("branch_name");
            var fileName = assetDatabaseAssetGroup.GroupName + "_" + buildTarget;
            writer.Write(fileName);
            writer.WritePropertyName("base_version");
            writer.Write(assetDatabase.base_version);
            writer.WritePropertyName("cusanu_fy_ass_metel_gloyw");
            writer.Write(hash);
            writer.WriteObjectEnd();
            WriteAssetList(ref writer, assetDatabaseAssetGroup,defaultDatabase);

            writer.WriteArrayEnd();

            WriteAssetDatabaseFile(buildTarget, fileName, sb.ToString(), version);
        }
    }


    private static void CreateBundles(AssetDatabaseConfiguration configuration,BuildTarget buildTarget)
    {
        var similarAssets = configuration.SimilarAssetInEachGroup;

        foreach (var assetGroup in configuration.AssetGroups.Where(a=>a.IsActive))
        {
            
            foreach (var searchAssetGroup in configuration.AssetGroups)
            {
                if (searchAssetGroup != assetGroup)
                {
                    ClearBundleLabels(searchAssetGroup, similarAssets, configuration);
                }
            }
            RestoreBundleLabels(assetGroup, similarAssets, configuration);

            CreateAssetBundles.CreateBundle(buildTarget);
            OrganizingBundles(assetGroup, buildTarget);
        }


        //After Build Restore all bundles in each group
        foreach (var searchAssetGroup in configuration.AssetGroups)
        {
            RestoreBundleLabels(searchAssetGroup, similarAssets, configuration);
        }
    }

    private static void RestoreBundleLabels(AssetDatabaseGroup assetGroup, List<string> assetNeedToBeRestored, AssetDatabaseConfiguration configuration)
    {
        for (var i = 0; i < assetGroup.AssetDatabaseAssets.Count; i++)
        {
            var assetDatabaseAsset = assetGroup.AssetDatabaseAssets[i];
            if (assetNeedToBeRestored == null || assetNeedToBeRestored.Contains(assetDatabaseAsset.code))
            {
                foreach (var objectPath in assetDatabaseAsset.ObjectsPaths)
                {
                    AssetImporter assetImporter = AssetImporter.GetAtPath(objectPath);
                    if (assetImporter != null)
                    {
                        assetImporter.assetBundleName = assetDatabaseAsset.code;
                        assetImporter.SaveAndReimport();
                    }
                }
            }
        }
    }

    private static void ClearBundleLabels(AssetDatabaseGroup assetGroup, List<string> assetNeedToBeCleared, AssetDatabaseConfiguration configuration)
    {
        for (var i = 0; i < assetGroup.AssetDatabaseAssets.Count; i++)
        {
            var assetDatabaseAsset = assetGroup.AssetDatabaseAssets[i];
            if (assetNeedToBeCleared == null || assetNeedToBeCleared.Contains(assetDatabaseAsset.code))
            {
                foreach (var objectPath in assetDatabaseAsset.ObjectsPaths)
                {
                    AssetImporter assetImporter = AssetImporter.GetAtPath(objectPath);
                    if (assetImporter != null)
                    {
                        assetImporter.assetBundleName = null;
                        assetImporter.SaveAndReimport();
                    }
                }
            }
            
            var allObjectinAssets = configuration.AllObjectsInAsset(assetDatabaseAsset.code);
            var objectsNeedToBeCleared =
                allObjectinAssets.Except(assetGroup.AssetDatabaseAssets.FirstOrDefault(a => a.code == assetDatabaseAsset.code).ObjectsPaths).ToList();

            foreach (var objectPath in objectsNeedToBeCleared)
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(objectPath);
                if (assetImporter != null)
                {
                    assetImporter.assetBundleName = null;
                    assetImporter.SaveAndReimport();
                }
            }
        }
    }


    public static void RestoreAllGroupLabels(AssetDatabaseConfiguration assetDatabaseConfiguration)
    {
        foreach (var assetDatabaseGroup in assetDatabaseConfiguration.AssetGroups)
        {
            RestoreBundleLabels(assetDatabaseGroup,null, assetDatabaseConfiguration);
        }
    }


    private static AssetDatabaseData LoadAssetDatabase(string path)
    {
        AssetDatabaseData data = new AssetDatabaseData();
        using (Stream zStream = File.OpenRead(path))
        {
            string databaseString = string.Empty;
            using (StreamReader streamReader = new StreamReader(zStream))
            {
                databaseString = streamReader.ReadToEnd();
            }
            if (!data.ValidateAndLoadAssetDatabase(databaseString))
            {
                Debug.LogError("Database file is corrupted");
                return null;
            }
            return data;
        }
    }

    private static void WriteAssetDatabaseFile(BuildTarget buildTarget,string branchName,string json,int version)
    {
        var bundlePath = GetBundlePath(buildTarget);
        var streamingBundlePath = GetStreamingAssetBundlePath();
        var buildPath = bundlePath + "/../";
        DirectoryInfo directoryInfo = new DirectoryInfo(buildPath);
        if (directoryInfo.Exists)
        {
            directoryInfo.Delete(true);
        }
        EnsureDirectoryExists(bundlePath);
        var path = bundlePath + "/" + DATABASE_FILE_NAME + "_" + branchName;
        WriteFile(json, path);

        //Write to StreamingAssets
        EnsureDirectoryExists(streamingBundlePath);
        path = streamingBundlePath + "/" + DATABASE_FILE_NAME + "_" + branchName;
        WriteFile(json, path);

        //Write ABTest Database file
        var abTestPath = GetABTestPath();
        var databasePath = abTestPath + "/Database/";
        EnsureDirectoryExists(databasePath);
        WriteFile(json, databasePath + "/" + DATABASE_FILE_NAME + "." + version);

        //Write ABTest Version file
        WriteABTestVerionFile(version);

    }

    private static void WriteFile(string json,string path)
    {
        var zBytes = Encoding.UTF8.GetBytes(json);
        using (Stream stream = File.Create(path))
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(stream))
            {
                binaryWriter.Write(zBytes);
            }
        }
    }

    private static string GetBundlePath(BuildTarget buildTarget)
    {
        return GetBuildRoot()+"/" + buildTarget + "/AppDataRoot";
    }

    private static string GetStreamingAssetBundlePath()
    {
        return Application.streamingAssetsPath + "/AppDataRoot";
    }

    private static string GetBuildRoot()
    {
        return Application.dataPath + "/../BuiltBundles";
    }

    private static string GetABTestPath()
    {
        return GetBuildRoot() + "/ABTest";
    }


    private static void WriteAssetList(ref JsonWriter writer, AssetDatabaseGroup assetDatabaseGroup, AssetDatabaseGroup defaultDatabaseGroup)
    {
        List<AssetDatabaseAsset> assetsList = new List<AssetDatabaseAsset>(assetDatabaseGroup.AssetDatabaseAssets);
        if (assetDatabaseGroup != defaultDatabaseGroup)
        {
            foreach (var assetDatabaseAsset in defaultDatabaseGroup.AssetDatabaseAssets)
            {
                if(assetsList.All(a => a.code != assetDatabaseAsset.code))
                {
                    assetsList.Add(assetDatabaseAsset);
                }
            }
        }
        writer.WriteArrayStart();
        foreach (var assetDatabaseAssetDatabaseAsset in assetsList.OrderBy(a => a.code))
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("code");
            writer.Write(assetDatabaseAssetDatabaseAsset.code);
            writer.WritePropertyName("type");
            writer.Write((int)assetDatabaseAssetDatabaseAsset.type);
            writer.WritePropertyName("version");
            writer.Write(assetDatabaseAssetDatabaseAsset.version);
            writer.WriteObjectEnd();
        }
        writer.WriteArrayEnd();
    }


    private static void OrganizingBundles(AssetDatabaseGroup group , BuildTarget buildTarget)
    {
        var bundlePath = GetBundlePath(buildTarget);
        var streamingBundlePath = GetStreamingAssetBundlePath();

        var assetbundlePath = bundlePath + "/AssetBundles/";
        var abTestPath = GetABTestPath();


        var eAssetQuality =
            AssetQuality.High.ToString()
                .ToLower(); //Remove this line if you want to create bundle for each quality and then uncommonet foreach snippet above
        var path = assetbundlePath + eAssetQuality;
        streamingBundlePath = streamingBundlePath + "/AssetBundles/" + eAssetQuality;
        EnsureDirectoryExists(path);
        EnsureDirectoryExists(streamingBundlePath);


        foreach (var asset in group.AssetDatabaseAssets)
        {
            var origAssetPath = assetbundlePath + asset.code;
            if (File.Exists(origAssetPath))
            {
                File.Copy(origAssetPath,
                    streamingBundlePath + "/" + asset.code + "." + eAssetQuality + "." +
                    asset.version, true);
            }
            else
            {
                Debug.Log(
                    string.Format(
                        "asset {0} does not exists at path : {1} , please remove it from configuration asset list",
                        asset.code, origAssetPath));
            }
        }


        var infoDirectory = bundlePath + "/../info";
        EnsureDirectoryExists(infoDirectory);

        //Delete default created bundle files
        File.Delete(assetbundlePath + "AssetBundles");
        File.Delete(assetbundlePath + "AssetBundles.manifest");


        foreach (var asset in group.AssetDatabaseAssets)
        {
            if (File.Exists(assetbundlePath + asset.code))
            {
                File.Delete(assetbundlePath + asset.code);
                //File.Delete(assetbundlePath + asset.code + ".manifest");
                if (File.Exists(assetbundlePath + asset.code + ".manifest"))
                {
                    if (!File.Exists(infoDirectory + "/" + asset.code + ".manifest"))
                    {
                        File.Move(assetbundlePath + asset.code + ".manifest",
                            infoDirectory + "/" + asset.code + ".manifest");
                    }
                }
            }
        }



        //var destObbFile = bundlePath + "/../patch." + PlayerSettings.Android.bundleVersionCode + "." +
        //                  PlayerSettings.applicationIdentifier + ".obb";
        //if (File.Exists(destObbFile))
        //{
        //    File.Delete(destObbFile);
        //}

        //if (buildTarget == BuildTarget.Android)
        //{
        //    //Zip And Create obb
        //    using (ZipFile zip = new ZipFile())
        //    {
        //        //add directory, give it a name
        //        zip.AddDirectory(bundlePath+"/../");
        //        zip.CompressionMethod = CompressionMethod.None;
        //        zip.Save(destObbFile);
        //    }
        //}
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private static void WriteABTestVerionFile(int version)
    {
        //if (m_branchSelected == 0)
        //    return;
        //var databasePath = GetABTestPath() + "/Database/asset_database_" + branches[m_branchSelected] + ".version";
        //JsonDict jsonDict = new JsonDict();
        //jsonDict.Set("version", version);

        //File.WriteAllText(databasePath, jsonDict.ToString());
    }

    public static void UpdateAssetDatabaseObjectList(this AssetDatabaseConfiguration assetDatabase)
    {
        if (!EditorUtility.DisplayDialog("Update bundle objects",
            "Are you sure you want to update all bundles object list ?", "Yes", "No"))
        {
            return;
        }

        Undo.RegisterCompleteObjectUndo(assetDatabase, "Update object list");
        //Dictionary<string,AssetDatabaseAsset> existsAssets = new Dictionary<string, AssetDatabaseAsset>();
        //var bundlesToDelete = new List<string>();
        var bundles = AssetDatabase.GetAllAssetBundleNames();

        foreach (var bundle in bundles)
        {
            foreach (var group in assetDatabase.AssetGroups)
            {
                var assetDatabaseAssets =
                    group.AssetDatabaseAssets.FirstOrDefault(a => a.code == bundle);
                if (assetDatabaseAssets == null)
                {
                    //Just add new bundles to default group because other group should be manipulated manually
                    if (group.IsDefault)
                    {
                        assetDatabaseAssets = new AssetDatabaseAsset(bundle, 0, GTAssetTypes.invalid);
                        group.AssetDatabaseAssets.Add(assetDatabaseAssets);
                    }
                    else
                    {
                        continue;
                    }

                }

                var assetsPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);
                assetDatabaseAssets.ObjectsPaths = new List<string>();
                assetDatabaseAssets.ObjectsPaths.AddRange(assetsPaths);
            }

        }
    }


    public static void IncreaseAllAssetVersion(this AssetDatabaseConfiguration assetDatabase)
    {
        foreach (var assetDatabaseAssetGroup in assetDatabase.AssetGroups)
        {
            foreach (var assetDatabaseAsset in assetDatabaseAssetGroup.AssetDatabaseAssets)
            {
                assetDatabaseAsset.version++;
            }
        }

        EditorUtility.SetDirty(assetDatabase);
        AssetDatabase.SaveAssets();
    }
    
    public static void IncreaseAssetDatabaseVersion(this AssetDatabaseConfiguration assetDatabase)
    {
        assetDatabase.version++;

        EditorUtility.SetDirty(assetDatabase);
        AssetDatabase.SaveAssets();
    }
}
#endif
