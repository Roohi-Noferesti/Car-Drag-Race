using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UICarItem : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] protected TextMeshProUGUI m_rateText;
    [SerializeField] protected TextMeshProUGUI m_nameText;
    [SerializeField] protected TextMeshProUGUI m_tierText;
    [SerializeField] protected RawImage m_image;
    [SerializeField] protected GameObject m_detailsPanel;
    [SerializeField] protected GameObject m_newBadge;
    private Toggle m_toggle;
    private string m_carID;
    private bool m_isActive;
    public int ItemIndex;
    void Awake()
    {
        m_toggle = GetComponent<Toggle>();
    }


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

    public string Tier
    {
        set { m_tierText.text = string.Format("{0}", value); }
    }

    public Texture2D Icon
    {
        set { m_image.texture = value; }
    }

    public virtual void SetActive(bool value)
    {
        m_toggle.isOn = false;
        m_toggle.interactable = value;
        m_image.gameObject.SetActive(value);
        //m_nameText.gameObject.SetActive(value);
        //m_rateText.gameObject.SetActive(value);
        m_detailsPanel.SetActive(value);
        m_tierText.gameObject.SetActive(value);
        m_isActive = value;
        m_newBadge.SetActive(false);
    }

    public void ReloadImage(bool custom)
    {
        if(m_newBadge!=null)
            m_newBadge.SetActive(PlayerProfileManager.Instance.ActiveProfile.HasSeenCar(m_carID));
        ResourceManager.GetCarThumbnail(m_carID, custom, (t) =>
        {
            Icon = t;
        });
    }

    public bool IsSelected
    {
        get
        {
            if (m_toggle != null)
            {
                return m_toggle.isOn;
            }
            return false;
        }
        set
        {
            if (m_toggle != null)
            {
                m_toggle.isOn = value;
            }
        }
    }

    public ToggleGroup ToggleGroup
    {
        get { return m_toggle.group; }
        set { m_toggle.group = value; }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var groupCarCellVeiw = GetComponentInParent<GroupCarCellView>();

        if (groupCarCellVeiw != null && m_isActive)
        {
            groupCarCellVeiw.OnSelected(ItemIndex);
        }

        if (eventData.dragging)
        {
            GetComponentInParent<ScrollRect>().OnInitializePotentialDrag(eventData);
        }
    }
}
