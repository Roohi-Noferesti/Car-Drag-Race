using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAudio : MonoBehaviour
{
    private Button m_button;
    [SerializeField]
    private AudioSfx m_menuBleeps = AudioSfx.MenuClickBeep;

    void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(() =>
        {
            MenuAudio.Instance.playSound(m_menuBleeps);
        });
    }
}
