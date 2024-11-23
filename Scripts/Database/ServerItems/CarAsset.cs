using UnityEngine;

public class CarAsset : ScriptableObject
{
    [SerializeField] private string m_imagePath;

    [SerializeField] private string m_prefabPath;

    public string ImagePath
    {
        get { return m_imagePath; }
    }

    public string PrefabPath
    {
        get { return m_prefabPath; }
    }
}
