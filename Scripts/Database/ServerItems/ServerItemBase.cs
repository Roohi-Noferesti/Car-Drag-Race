using System;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

[Serializable]
public class ServerItemBase
{
    public string Name
    {
        get { return m_item.ItemID; }
    }

    [SerializeField] protected AssetPath[] m_assets;
    [SerializeField] protected VirtualItem m_item;

    public VirtualItem Item
    {
        get { return m_item; }
        set { m_item = value; }
    }

    public AssetPath[] Assets
    {
        get { return m_assets; }
    }


    [Serializable]
    public class AssetPath
    {
        [SerializeField] private AssetType m_assetType;
        [SerializeField] private string m_resourcePath;

        public AssetType AssetType
        {
            get { return m_assetType; }
        }

        public string ResourcePath
        {
            get { return m_resourcePath; }
        }
    }

    public enum AssetType
    {
        none,
        thumbnail,
        image,
        prefab,
        audio,
        body_shader,
        headlight_shader,
        ring_shader,
        logo,
        garage_model,
        race_model,
        lightmap_garage,
        lightmap_street,
        sticker,
        spoiler,
        configuration,
        spec,
        car_shared_shader,
        avatar
    }
}


