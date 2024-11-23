using System;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameOverScreen : HUDScreen
{
    private float m_playerTime;
    private float m_opponentTime;
    [SerializeField]
    private Slider _playerProgress;
    [SerializeField]
    private Slider _opponentProgress;

    [SerializeField]
    private Text _playerTimeText;
    [SerializeField]
    private Text _opponentTimeText;

    [SerializeField]
    private Text _playerSpeedText;
    [SerializeField]
    private Text _opponentSpeedText;


    public float playerProgress
    {
        get
        {
            if (Mathf.Approximately(m_opponentTime, 0))
                return 1;
            return m_playerTime > m_opponentTime ? m_opponentTime / m_playerTime : 1;
        }
    }

    public float opponentProgress
    {
        get
        {
            if (Mathf.Approximately(m_playerTime,0))
                return 1;
            return m_opponentTime > m_playerTime ? m_playerTime / m_opponentTime : 1;
        }
    }

    public float playerTime
    {
        set
        {
            m_playerTime = value;
            _playerTimeText.text = String.Format("{0:00.000}", value);
            updateProgress();
        }
    }

    public float opponentTime
    {
        set
        {
            m_opponentTime = value;
            _opponentTimeText.text = String.Format("{0:00.000}", value);
            updateProgress();
        }
    }

    public float playerSpeed
    {
        set { _playerSpeedText.text = String.Format("{0:00.0 kmh}", value * 3.6); }
    }

    public float opponentSpeed
    {
        set { _opponentSpeedText.text = String.Format("{0:00.0 kmh}", value * 3.6); }
    }

    private void updateProgress()
    {
        if (Mathf.Approximately(m_playerTime, 0) || Mathf.Approximately(m_opponentTime, 0))
            return;
        _opponentProgress.value = opponentProgress;
        _playerProgress.value = playerProgress;
    }

}
