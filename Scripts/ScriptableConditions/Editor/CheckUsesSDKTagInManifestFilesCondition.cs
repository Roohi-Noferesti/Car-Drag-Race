using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/" + nameof(CheckUsesSDKTagInManifestFilesCondition),
        fileName = nameof(CheckUsesSDKTagInManifestFilesCondition))]
    public class CheckUsesSDKTagInManifestFilesCondition : ScriptableCondition
    {
        public override bool IsEligible()
        {
            string[] files = System.IO.Directory.GetFiles(Application.dataPath, "AndroidManifest.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string contents = sr.ReadToEnd().ToLower().Replace(" ", "");
                    if (contents.Contains("uses-sdk"))
                    {
                        Debug.LogError("\"uses-sdk\" tag detected in " + file + ". Make sure to remove this tag in AndroidManifest files or else you will have errors on build.");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}