using UnityEngine;
using UnityEngine.UI;

public class AvatarCellView : SelectableScrollerCellView
{
    [SerializeField] protected RawImage m_image;
    private string m_avatarID;

    public Texture2D Icon
    {
        set { m_image.texture = value; }
    }

    public string AvatarID
    {
        set
        {
            m_avatarID = value;
        }
    }

    public virtual void SetActive(bool value)
    {
        m_image.gameObject.SetActive(value);
    }

    public void ReloadImage()
    {
        ResourceManager.LoadAssetAsync<Texture2D>(m_avatarID, ServerItemBase.AssetType.avatar,true
            , tex =>
            {
                Icon = tex;
            });
    }
}