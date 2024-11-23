using UnityEngine;
using UnityEngine.UI;

public class ImageButtonCellView : SelectableScrollerCellView 
{
    [SerializeField] protected Image m_iconImage;
    [SerializeField] protected bool m_disableImageOnNull = false;
    public virtual Sprite Icon
    {
        set
        {
            m_iconImage.sprite = value;
            m_iconImage.gameObject.SetActive(value != null || !m_disableImageOnNull);
            //m_iconImage.SetNativeSize();
        }
    }

    public Color Color
    {
        set { m_iconImage.color = value; }
    }
}
