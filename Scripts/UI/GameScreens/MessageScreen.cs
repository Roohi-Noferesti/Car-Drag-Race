using System;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MessageScreen : HUDScreen
{
    [SerializeField] private Button[] m_buttons;
    private int m_buttonsCount;


    public int ButtonsCount
    {
        get { return m_buttonsCount; }
        set
        {
            m_buttonsCount = value;
            for (int i = 0; i < m_buttons.Length; i++)
            {
                m_buttons[i].gameObject.SetActive(i <value);
            }
        }
    }

    public void SetButtonsText(string[] texts)
    {
        for (int i = 0; i < m_buttons.Length; i++)
        {
            m_buttons[i].GetComponentInChildren<Text>().text = texts[i];
        }
    }
}
