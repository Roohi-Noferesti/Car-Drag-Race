using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/" + nameof(CheckStandardMaterialCondition),
        fileName = nameof(CheckStandardMaterialCondition))]
    public class CheckStandardMaterialCondition : ScriptableCondition
    {
        public override bool IsEligible()
        {
            var materialGUIDs = AssetDatabase.FindAssets("t:material");
            var result = new List<Material>();
            foreach (var guid in materialGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat.shader.name == "Standard (Specular setup)" || mat.shader.name == "Standard")
                {
                    result.Add(mat);
                }

            }

            if (result.Count > 0) {
                Debug.LogError("There " + (result.Count==1?"is a Standard Material":"are " + result.Count + " Standard Materials") 
                     + " present in the project. This would cause the Standard Shader to be present in the build and occupy memory resources. " +
                     "Consider finding and removing the Standard Materials with \'EditorTools>FindAllStandardMaterials\' tool.");
                return false;
            } else {
                return true;
            }
        }
    }
}