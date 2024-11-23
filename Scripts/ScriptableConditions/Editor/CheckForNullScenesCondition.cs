using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomBuildWindow;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/" + nameof(CheckForNullScenesCondition),
        fileName = nameof(CheckForNullScenesCondition))]
    public class CheckForNullScenesCondition : ScriptableCondition
    {
        
        public BuildWindowScenes DevScenes;
        public BuildWindowScenes MainScenes;
        
        public override bool IsEligible()
        {
            int i = 0;
            foreach (var scene in DevScenes.Scenes)
            {

          
                if (scene == null)
                {
                    Debug.LogError($"There is a null scene in DevScenes configuration. Scene index:{i}");
                    return false;
                }
                i++;
            }
            
            foreach (var scene in MainScenes.Scenes)
            {
                if (scene == null)
                {
                    Debug.LogError("There is a null scene in MainScenes configuration.");
                    return false;
                }
            }
            
            return true;
        }
    }
}