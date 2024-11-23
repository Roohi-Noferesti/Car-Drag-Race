using System;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class StartScreen : HUDScreen
{
    [SerializeField] private InputField m_nameField;

    public new string name
    {
        set
        {
            if (m_nameField.text != value)
            {
                m_nameField.text = value;
            }
        }
        get { return m_nameField.text; }
    }

    public event UnityAction<string> nameChanged
    {
        add { m_nameField.onEndEdit.AddListener(value); }
        remove
        {
            m_nameField.onEndEdit.RemoveListener(value);
        }
    }

    private int i;
}
