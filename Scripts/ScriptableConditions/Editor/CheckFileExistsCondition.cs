using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/" + nameof(CheckFileExistsCondition),
        fileName = nameof(CheckFileExistsCondition))]
    public class CheckFileExistsCondition : ScriptableCondition
    {

        [InfoBox("example: Plugin/Android/file.ext")]
        [SerializeField] private string[] LocalPathToFiles;


        public override bool IsEligible()
        {
            foreach (var localPathToFile in LocalPathToFiles)
            {
                if (File.Exists(Path.Combine(Application.dataPath, localPathToFile))) continue;
                Debug.LogError("File not found: " + localPathToFile);
                return false;

            }
            return true;
        }
    }
}