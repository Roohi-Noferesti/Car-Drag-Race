using UnityEngine;

public class TestAssetBundleSystem : MonoBehaviour,IBundleOwner
{
    [SerializeField] private string m_assetName;
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AssetProviderClient.Instance.RequestAsset(m_assetName, BundleLoaded,this);
        }
    }

    private void BundleLoaded(string zassetid, AssetBundle zassetbundle, IBundleOwner zowner)
    {
        var myObj = zassetbundle.LoadAsset<GameObject>(zassetbundle.mainAsset.name);
        Debug.Log(myObj.name);
    }
}
