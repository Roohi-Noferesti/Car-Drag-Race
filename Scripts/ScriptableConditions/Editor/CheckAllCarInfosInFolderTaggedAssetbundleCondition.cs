using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/"+nameof(CheckAllCarInfosInFolderTaggedAssetbundleCondition),fileName = nameof(CheckAllCarInfosInFolderTaggedAssetbundleCondition))]
    public class CheckAllCarInfosInFolderTaggedAssetbundleCondition : ScriptableCondition
    {

        [SerializeField] private List<string> excludedCarInfoPathes;
        
        public override bool IsEligible()
        {
            var files = Directory.GetFiles(Application.dataPath + "/CarInfo", "*.asset", SearchOption.TopDirectoryOnly);
            var result1 = new List<AssetImporter>();
            var result2 = new List<AssetImporter>();
            foreach (var file in files)
            {
                var fixedPath  = file.Replace('\\', '/');
                fixedPath = fixedPath.Replace(Application.dataPath, "Assets");
                var asset = AssetImporter.GetAtPath(fixedPath);
                if (!string.Equals(asset.assetBundleName,"carmetadata",StringComparison.InvariantCultureIgnoreCase) && !IsExcluded(asset.assetPath))
                    result1.Add(asset);
                if(string.Equals(asset.assetBundleName,"carmetadata",StringComparison.InvariantCultureIgnoreCase) && IsExcluded(asset.assetPath))
                    result2.Add(asset);
            }

            if (!result1.Any() && !result2.Any()) return true;
            foreach (var assetImporter in result1) Debug.Log("This Asset is Not Tagged Properly: " + assetImporter.assetPath);
            foreach (var assetImporter in result2) Debug.Log("This Asset Should Be Excluded: " + assetImporter.assetPath);
            return false;

        }

        // [MenuItem("ClassicTools/Test")]
        // public static void Test()
        // {
        //     var files = Directory.GetFiles(Application.dataPath + "/CarInfo", "*.asset", SearchOption.TopDirectoryOnly);
        //     var result = new List<AssetImporter>();
        //     foreach (var file in files)
        //     {
        //         var fixedPath  = file.Replace('\\', '/');
        //         fixedPath = fixedPath.Replace(Application.dataPath, "Assets");
        //         var asset = AssetImporter.GetAtPath(fixedPath);
        //         if (!string.Equals(asset.assetBundleName,"carmetadata",StringComparison.InvariantCultureIgnoreCase))
        //             result.Add(asset);
        //     }
        //     foreach (var assetImporter in result) Debug.Log(assetImporter.assetPath);
        // }

        private bool IsExcluded(string path)
        {
            foreach (var excl in excludedCarInfoPathes)
            {
                if (excl == path)
                    return true;
            }
            return false;
        }
    }
}