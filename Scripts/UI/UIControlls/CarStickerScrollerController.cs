using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarStickerScrollerController : CarPropertyScrollerController 
{
    [Serializable]
    public class StickerNameDictionary
    {
        public string Name;
        public Sprite Sprite;
    }

    [SerializeField] protected StickerNameDictionary[] m_stickerNameDictionaries;

    private Dictionary<string, Sprite> m_dic = new Dictionary<string, Sprite>();

    protected override void Awake()
    {
        base.Awake();
        m_dic = m_stickerNameDictionaries.ToDictionary(s => s.Name, s => s.Sprite);
    }


    protected override void SetSprite(ImageButtonCellView cellView, int dataIndex)
    {
        var id = IDs[dataIndex];
        if (m_dic.ContainsKey(id))
        {
            cellView.Icon = m_dic[id];
        }
    }

}
