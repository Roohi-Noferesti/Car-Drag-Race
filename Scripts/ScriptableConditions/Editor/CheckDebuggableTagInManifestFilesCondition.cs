using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/" + nameof(CheckDebuggableTagInManifestFilesCondition),
        fileName = nameof(CheckDebuggableTagInManifestFilesCondition))]
    public class CheckDebuggableTagInManifestFilesCondition : ScriptableCondition
    {
        public override bool IsEligible()
        {
            string[] files = System.IO.Directory.GetFiles(Application.dataPath, "AndroidManifest.xml", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    string contents = sr.ReadToEnd().ToLower().Replace(" ", "");
                    if (contents.Contains("android:debuggable=\"true\""))
                    {
                        Debug.LogError("android:debuggable=\"true\" attribute detected in " + file + ". Make sure to set this attribute to false in AndroidManifest files.");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}