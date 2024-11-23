using System.Collections;
using UnityEngine;

public class TestAssetBundle : MonoBehaviour 
{
    IEnumerator Start()
    {
        WWW www = new WWW("File:///D:/CarMetaDataUnrar");
        yield return www;

        // Get the designated main asset and instantiate it.
        //var cars = www.assetBundle.LoadAllAssets<CarInfo>();
    }
}
