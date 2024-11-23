using UnityEngine;
using UnityEngine.UI;

public class MapEventPane : MonoBehaviour
{
    [SerializeField] private GameObject m_particle;
    [SerializeField] private GameObject m_cirlce;
    private Button m_button;

    public bool IsActive { get; private set; }

    public bool IsHighlight { get; private set; }


    void Awake()
    {
        m_button = GetComponent<Button>();
        SetEventActive(false,false);
    }

    public void SetEventActive(bool value,bool highlight)
    {
        IsActive = value;
        m_cirlce.SetActive(value);
        m_button.interactable = value;
        m_particle.SetActive(value && highlight);
    }

    public void SetHighlight(bool value)
    {
        m_particle.SetActive(IsActive && value);
    }
}
