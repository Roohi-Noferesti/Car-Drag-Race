using System;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LobbyScreen : HUDScreen
{
    [SerializeField] private Text[] m_PlayerTexts;
    [SerializeField] private Button m_start_Button;
    [SerializeField] private Button m_maniMenu_Button;
    [SerializeField] private Text m_captionText;

    public int playerCount
    {
        set
        {
            for (int i = 0; i < value && i<m_PlayerTexts.Length; i++)
            {
                m_PlayerTexts[i].gameObject.SetActive(true);
            }

            for (int i = value; i < m_PlayerTexts.Length; i++)
            {
                m_PlayerTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public string caption
    {
        set { m_captionText.text = value; }
    }

    public void setPlayerName(string[] values,int localPlayerIndex)
    {
        for (int i = 0; i < values.Length; i++)
        {
            m_PlayerTexts[i].text = values[i] + (localPlayerIndex == i ? "(you)" : "");
        }
    }

    public bool startButtonVisible
    {
        set
        {
            m_start_Button.gameObject.SetActive(value);
        }
    }
}
