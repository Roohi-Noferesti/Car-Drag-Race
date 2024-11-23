using UnityEngine;
using System.Collections;
using TMPro;

public class VersionNumber : MonoBehaviour
{
    private TextMeshProUGUI m_text;

    void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();

        m_text.text = "Version: " + Application.version;
    }
}
