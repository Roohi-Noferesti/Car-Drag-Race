using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/" + nameof(CheckMetrixAppIDExistsInManifestCondition),
        fileName = nameof(CheckMetrixAppIDExistsInManifestCondition))]
    public class CheckMetrixAppIDExistsInManifestCondition : ScriptableCondition
    {
        public override bool IsEligible()
        {
            using (StreamReader sr = new StreamReader(Application.dataPath + "\\Plugins\\Android\\AndroidManifest.xml"))
            {
                string contents = sr.ReadToEnd().ToLower().Replace(" ", "");
                if (!contents.Contains("metrix_appId".ToLower()))
                {
                    Debug.LogError("\"metrix_appId\" metadata is no present in the main android manifest file. Make sure to add this tag to Android Manifest file.");
                    Debug.LogError("Add this line to application tag in android manifest file: <meta-data android:name=\"metrix_appId\" android:value=\"jzbrsoaengvmeju\" />");
                    return false;
                }
            }
            return true;
        }
    }
}