using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/" + nameof(CheckFirebaseAARIncludesArm64ArchCondition),
        fileName = nameof(CheckFirebaseAARIncludesArm64ArchCondition))]
    public class CheckFirebaseAARIncludesArm64ArchCondition : ScriptableCondition
    {
        [SerializeField] private long aarFileSizeMin = 2000000;
        public override bool IsEligible()
        {
            var aar = Directory.GetFiles(Application.dataPath + "\\Plugins\\Android", "com.google.firebase.firebase-app-unity*.aar")[0];
            var fileInfo = new System.IO.FileInfo(aar);
            if (fileInfo.Length > aarFileSizeMin)
            {
                return true;
            }
            else
            {
                Debug.LogError("com.google.firebase.firebase-app-unity AAR file size is less than " + aarFileSizeMin + ". This could mean that this AAR does not include Arm64 Architecture. You have to enable IL2CPP, Arm-7 and Arm-64 in player settings and then resolve to bypass this problem.");
                return false;
            }
        }
    }
    
    
}
