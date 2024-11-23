using System;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;

[Serializable]
public class ResumeScreen:HUDScreen
{
    [SerializeField]
    private Text _countDownText;
    [SerializeField]
    private GameObject _pedal;
    [SerializeField]
    private Text _gearText;
    //[SerializeField]
    //private Toggle[] _lightsToggle;
    [SerializeField] private Image m_rpmGuideImage;
    [SerializeField] private Color[] m_rpmColors;
    [SerializeField] private Color m_redRpmColor;

    [SerializeField] private Slider _playerProgressbar;
    [SerializeField] private Slider _opponentProgressbar;
    [SerializeField] private Image m_nitrousImage;
    [SerializeField] private Text m_timeText;
    [SerializeField] private Text m_speed;
    private Range[] m_speeds;
    private int[] m_rpms;
    private float[] m_dividedSpeeds;

    void turnOffAllLights()
    {
        //foreach (var toggle in _lightsToggle)
        //{
        //    toggle.isOn = false;
        //}
        m_rpmGuideImage.color = Color.white;//m_rpmColors[0]
    }


    void _car_GearChanged(int obj, int i)
    {
        turnOffAllLights();
        //Debug.Log(Time.time + "   " + m_rpmGuideImage.color);
    }

    public float RaceTime
    {
        set { m_timeText.text = String.Format("{0:00.00}", value); }
    }

    public float PlayerSpeed
    {
        set { m_speed.text = String.Format("{0:000}", value); }
    }

    public void CountDown(int obj)
    {
        if (obj > 0)
        {
            _countDownText.gameObject.SetActive(true);
            _countDownText.text = obj.ToString();
        }
        else
        {
            _countDownText.gameObject.SetActive(false);
            _pedal.SetActive(false);
        }
    }
}
