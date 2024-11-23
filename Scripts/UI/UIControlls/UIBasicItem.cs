using UnityEngine;
using UnityEngine.UI;

public class UIBasicItem : MonoBehaviour 
{
    [SerializeField]
    protected bool m_isToggle;

    public string ID;

    public virtual void SetActive(bool value)
    {
        if (m_isToggle)
            Toggle.interactable = value;
    }

    protected Toggle Toggle
    {
        get
        {
            return m_isToggle ? GetComponent<Toggle>() : null;
        }
    }

    public bool IsSelected
    {
        get
        {
            return Toggle != null && Toggle.isOn;
        }
        set
        {
            if (Toggle != null)
                Toggle.isOn = value;
        }
    }
}
