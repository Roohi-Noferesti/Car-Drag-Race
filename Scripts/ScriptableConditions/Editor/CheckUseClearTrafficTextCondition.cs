using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScriptableConditions.Editor
{
    [CreateAssetMenu(menuName = "Conditions/" + nameof(CheckUseClearTrafficTextCondition),
        fileName = nameof(CheckUseClearTrafficTextCondition))]
    public class CheckUseClearTrafficTextCondition : ScriptableCondition
    {
        [InfoBox("example: Plugins/Android/AndroidManifest.xml")]
        [SerializeField] private string[] LocalPathToManifestFiles;

        private string LocalPathToNetworkSecurityConfigFile = "Plugins/Android/res/xml/network_security_config.xml";
        private string GTNetworkDomain = "gt-api-proxy.sarand.net";
        
        public override bool IsEligible()
        {
            foreach (var localPathToManifestFile in LocalPathToManifestFiles)
            {
                if (!CheckIfManifestUsesClearTrafficText(localPathToManifestFile))
                {
                    Debug.LogError(localPathToManifestFile + " does not contain \'android:usesCleartextTraffic=\"true\"\'. This would cause error in IAP inside the country.");
                    return false;
                }
            }

            if (!CheckIfNetworkConfigContainsGTDomain())
            {
                Debug.LogError(LocalPathToNetworkSecurityConfigFile + " does not contain \'" + GTNetworkDomain + "\' domain. This would cause error in IAP inside the country.");
                return false;
            }

            return true;
        }

        private bool CheckIfManifestUsesClearTrafficText(string path)
        {
            if (!File.Exists(Path.Combine(Application.dataPath, path)))
            {
                Debug.LogError(path + " file does not exist.");
                return false;
            }
            
            StreamReader reader = new StreamReader("Assets/" + path);
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.Contains("application") && line.Contains("android:usesCleartextTraffic=\"true\""))
                {
                    reader.Close();
                    return true;
                }
            }
            reader.Close();
            return false;
        }
        
        private bool CheckIfNetworkConfigContainsGTDomain()
        {
            if (!File.Exists(Path.Combine(Application.dataPath, LocalPathToNetworkSecurityConfigFile)))
            {
                Debug.LogError(LocalPathToNetworkSecurityConfigFile + " file does not exist.");
                return false;
            }
            
            StreamReader reader = new StreamReader("Assets/" + LocalPathToNetworkSecurityConfigFile);
            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.Contains("domain") && line.Contains(GTNetworkDomain))
                {
                    reader.Close();
                    return true;
                }
            }
            reader.Close();
            return false;
        }
    }
}