using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GearLightsDisplay : MonoBehaviour
{
    public GearChangeLogic.OutputState state;

    public List<GearChangeLight> lights = new List<GearChangeLight>();

    [SerializeField] private Image m_lightImage;
    [SerializeField] private Color[] m_lightColors;

    public float currentRevs;
    public float redlineRpm;
    public float idleRpm;

    public int numberOfBlueLights;

    public Animator paddleGlow;

    public Animator throttleGlow;

    public Animator greenLightGlow;

    //private int numberOfActiveLights;

    //private bool flashGreen;

    private GreenLightIndicator m_greenLightIndicator;

    private GreenLightIndicator m_goodLightIndicator;

    private void Start()
    {
        paddleGlow.gameObject.SetActive(false);
    }

    public bool GreenLightOn()
    {
        return state.normalisedLightsNumber > state.perfectGearStartNumber &&
               state.normalisedLightsNumber < state.perfectGearEndNumber;
    }

    public void Reset(bool shouldDisplay)
	{
		//this.numberOfActiveLights = 0;
		currentRevs = 0f;

	    if (!GetlighImage()) return;
	    m_lightImage.color = m_lightColors[0];
	    m_greenLightIndicator.Reset();
        //for (int i = 0; i < this.lights.Count; i++)
        //{
        //    this.lights[i].Reset();
        //}
		if (!shouldDisplay)
		{
			gameObject.SetActive(false);
		}

        CompetitorManager.Instance.LocalCompetitor.CarPhysics.GearChangeLogic.FireEnterRPMRegionEvent += EventDisplayChangeInfoMessage;
        CompetitorManager.Instance.LocalCompetitor.CarPhysics.GearChangeLogic.FireGearChangeUpEvent += GearChangeLogic_FireGearChangeUpEvent;
	}

    void OnDestroy()
    {
        //CompetitorManager.Instance.LocalCompetitor.CarPhysics.GearChangeLogic.FireEnterRPMRegionEvent -= this.EventDisplayChangeInfoMessage;
        //CompetitorManager.Instance.LocalCompetitor.CarPhysics.GearChangeLogic.FireGearChangeUpEvent -= GearChangeLogic_FireGearChangeUpEvent;
    }

    private void EventDisplayChangeInfoMessage(RPMRegion rpmRegion, bool areWeRacing)
    {
        switch (rpmRegion)
        {
            case RPMRegion.NotReady:
                m_lightImage.color = m_lightColors[0];
                break;
            case RPMRegion.NearlyReady:
                m_lightImage.color = m_lightColors[0];
                break;
            case RPMRegion.Ready:
                m_lightImage.color = m_lightColors[2];
                break;
            case RPMRegion.Now:
                m_lightImage.color = m_lightColors[3];
                break;
            case RPMRegion.Late:
                m_lightImage.color = m_lightColors[4];
                break;
        }
    }

    void GearChangeLogic_FireGearChangeUpEvent(GearChangeRating gearChangeRating)
    {
        m_lightImage.color = m_lightColors[0];
    }

    private bool GetlighImage()
    {
        if (m_lightImage == null)
        {
            m_greenLightIndicator = GameObject.Find("GreenLight").GetComponent<GreenLightIndicator>();
            m_goodLightIndicator = GameObject.Find("GoodLight").GetComponent<GreenLightIndicator>();
            var progress = transform.GetComponentInChildren<GaugeProgressBar>().transform.Find("Progress");
            if (progress != null)
            {
                m_lightImage = progress.GetComponent<Image>();
                return true;
            }
            return false;
        }
        return true;
    }

	private void Update()
	{
        if (!GetlighImage()) return;

		UpdateControlsGlow();
		if (PauseGame.isGamePaused)
		{
			throttleGlow.gameObject.SetActive(false);
			paddleGlow.gameObject.SetActive(false);
			return;
		}
		//this.numberOfActiveLights = 0;
		//this.flashGreen = false;
		UpdateNumLightsOn();
		SetLightStates();
	}

	private void UpdateNumLightsOn()
	{
		//this.flashGreen = false;
		if (state.normalisedLightsNumber < 0f)
		{
			//this.numberOfActiveLights = 0;
		}
		else if (state.normalisedLightsNumber > state.lateGearStartNumber)
		{
			//this.numberOfActiveLights = this.numberOfBlueLights + 2;
		}
		else if (state.normalisedLightsNumber > state.goodGearStartNumber)
		{
			//this.numberOfActiveLights = this.numberOfBlueLights + 1;
			bool flag = state.normalisedLightsNumber <= state.perfectGearEndNumber && state.normalisedLightsNumber >= state.perfectGearStartNumber;
			if (!state.inNeutralGear && flag)
			{
				//this.flashGreen = true;
			}
		}
		else if (state.normalisedLightsNumber > state.earlyGearStartNumber)
		{
			//float num = this.state.goodGearStartNumber - this.state.earlyGearStartNumber;
			//float num2 = num / (float)this.numberOfBlueLights;
			//this.numberOfActiveLights = Mathf.CeilToInt((this.state.normalisedLightsNumber - this.state.earlyGearStartNumber) / num2);
		}
		else
		{
			//this.numberOfActiveLights = 0;
		}

	    m_greenLightIndicator.MaxRevs = m_goodLightIndicator.MaxRevs = redlineRpm;
	    m_greenLightIndicator.IdleRpm = m_goodLightIndicator.IdleRpm = idleRpm;
        var minGoodRevs = RaceHUDController.Instance.hudTacho.minimumGoodRevs;
        var maxGoodRevs = RaceHUDController.Instance.hudTacho.maximumGoodRevs;
        var minPerfectRevs = RaceHUDController.Instance.hudTacho.minimumPerfectRevs;
        var maxPerfectRevs = RaceHUDController.Instance.hudTacho.maximumPerfectRevs;
        //Debug.Log(minPerfectRevs + "-" + maxPerfectRevs + "     "+state.perfectGearStartNumber+"    "+state.perfectGearEndNumber+"    "+state.normalisedLightsNumber);
        m_greenLightIndicator.UpdateIndicator(minPerfectRevs, maxPerfectRevs, state.inNeutralGear);
        m_goodLightIndicator.UpdateIndicator(minGoodRevs, maxGoodRevs, state.inNeutralGear);
	}

    private void SetLightStates()
    {
        //for (int i = 0; i < m_lightColors.Length; i++)
        //{
        //    bool flag = i < this.numberOfActiveLights;
        //    if (flag)
        //    {
        //        ////m_lightImage.color = m_lightColors[i];
        //        //if (this.numberOfBlueLights == i)
        //        //{
        //        //    m_lightImage.color = m_lightColors[3];

        //        //    //if (this.flashGreen)
        //        //    //{
        //        //    //    this.lights[i].shouldFlash = true;
        //        //    //}
        //        //    //else
        //        //    //{
        //        //    //    this.lights[i].shouldFlash = false;
        //        //    //}
        //        //}
        //        //else if (this.numberOfBlueLights > i)
        //        //{
        //        //    m_lightImage.color = m_lightColors[4];
        //        //}

        //        //if (this.lights[i].turnOffPreviousLight)
        //        //{
        //        //    this.lights[i - 1].isOn = false;
        //        //}
        //    }
        //    else if (i == 0)
        //    {
        //        m_lightImage.color = m_lightColors[0];
        //    }
        //    else
        //    {
        //        //this.lights[i].isOn = false;
        //    }
        //}


        //GearChangeLogic.OutputState state = RaceHUDController.Instance.hudGearLightsDisplay.state;
        //if (state.normalisedLightsNumber > state.lateGearStartNumber)
        //{
        //    m_lightImage.color = m_lightColors[4];
        //}
        //else if (state.normalisedLightsNumber > state.perfectGearEndNumber)
        //{
        //    m_lightImage.color = m_lightColors[2];
        //}
        //else if (state.normalisedLightsNumber > state.perfectGearStartNumber)
        //{
        //    m_lightImage.color = m_lightColors[3];
        //}
        //else if (state.normalisedLightsNumber > state.perfectGearStartNumber)
        //{
        //    m_lightImage.color = m_lightColors[3];
        //}
        //else if (state.normalisedLightsNumber > state.earlyGearStartNumber)
        //{
        //    m_lightImage.color = m_lightColors[2];
        //}
        //else
        //{
        //    m_lightImage.color = m_lightColors[1];
        //}
    }

    private void UpdateControlsGlow()
	{
		if (state.normalisedLightsNumber > state.perfectGearStartNumber)
		{
            if (!state.inNeutralGear && !paddleGlow.gameObject.activeSelf && Time.time-m_lastGearChangeTime>0.3F)
			{
                paddleGlow.gameObject.SetActive(true);
                //paddleGlow.Play("play");
                //m_called = true;
			    //if (PlayerProfileManager.Instance.ActiveProfile.RacesWon < 1 &&
			    //    this.state.normalisedLightsNumber < state.lateGearStartNumber)
			    //{
			    //    greenLightGlow.gameObject.SetActive(true);
			    //    greenLightGlow.Play("play");
			    //}
			}
		}
		else
		{
			throttleGlow.gameObject.SetActive(true);
            if (throttleGlow.transform.parent.gameObject.activeSelf)
                throttleGlow.Play("play");
        }
	}

    /// <summary>
    /// Call by UI Button
    /// </summary>
    public void StopPaddleGlow()
    {
        paddleGlow.gameObject.SetActive(false);
        m_lastGearChangeTime = Time.time;
        //paddleGlow.StopPlayback();
    }

    private float m_lastGearChangeTime;
}