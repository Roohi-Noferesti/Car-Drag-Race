using System.Collections;
using UnityEngine;

public class TestTrigger : TriggerBase
{
    //[SerializeField] private float m_distanceRatio = 0.5F;
#if(UNITY_EDITOR)
    IEnumerator Start()
    {
        yield return 0;
        //var pos = LevelController.getPlayerPosition(0);
        //pos.z = (LevelController.getPlayerPosition(0).z + LevelController.matchDistance) * m_distanceRatio;
        //transform.position = pos;

        //        var path = "file:///"+Application.dataPath + "/../Bundle/CarMetadata.ETC2.67";
        //var path2 = Application.dataPath + "/../Bundle/458SpecialeEngine1.txt";
        //var www = WWW.LoadFromCacheOrDownload (path, 0);
        //yield return www;
        //var objs = www.assetBundle.LoadAll ();
        //foreach (Object ob in objs) {
        //    if(ob is ScriptableObject){
        //        var so = ob as ScriptableObject;
        //        var ins = so.CreateInstance();
        //        AssetDatabase.CreateAsset(ins, "Assets/CarMetaData/"+ob.name+".asset");
        //        AssetDatabase.SaveAssets();
        //        break;
        //    }
        //}
    }


#endif
}
