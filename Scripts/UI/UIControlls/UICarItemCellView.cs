using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICarItemCellView : SelectableScrollerCellView
{
    [SerializeField] protected TextMeshProUGUI m_rateText;
    [SerializeField] protected TextMeshProUGUI m_nameText;
    [SerializeField] protected RawImage m_image;
    [SerializeField] protected GameObject m_detailsPanel;
    private string m_carID;

    public string CarID
    {
        set
        {
            m_carID = value;
        }
    }
    public int Rate
    {
        set { m_rateText.text = value.ToNativeNumber(); }
    }

    public string Name
    {
        set { m_nameText.text = string.Format("{0}", value); }
    }

    public Texture2D Icon
    {
        set { m_image.texture = value; }
    }

    public virtual void SetActive(bool value)
    {
        m_image.gameObject.SetActive(value);
        //m_nameText.gameObject.SetActive(value);
        //m_rateText.gameObject.SetActive(value);
        m_detailsPanel.SetActive(value);
    }

    public void ReloadImage(bool custom)
    {
        ResourceManager.GetCarThumbnail(m_carID, custom, (t) =>
        {
            Icon = t;
        });
    }
}
