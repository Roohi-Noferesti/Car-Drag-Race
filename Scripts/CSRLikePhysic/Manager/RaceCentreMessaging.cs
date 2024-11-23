using System;
using Fabric;
using I2.Loc;
using TMPro;
using UnityEngine;

public class RaceCentreMessaging : MonoBehaviour
{
	public enum TextAnimPhases
	{
		None,
		FadeIn,
		Hold,
		FadeOut
	}

	private const float TEXT_TIME = 0.5f;

	private const float TextAnimSpeed = 15f;

	private const float TextAnimSpeed2 = 20f;

	private const float TextScaleStart = 6f;

	private const float TextScaleHold = 1f;

    public bool runInDebug;

	public MainRaceCountdown MainRaceCountdown;

	public HeadstartRaceCountdown HeadstartRaceCountdown;

	public Color countdownNumberColour;

	public Color countdownGoColour;

	public Color winColour;

	public Color loseColour;

	public float LeadInTime;

	public float BetweenLightsTime;

	public float GreenTime;

	public float FadeOutTime;

	private float timer;

	public float textFadeDelta;

	public int countdownFromNumber;

	private int countdownValue;

	private TextMeshProUGUI centreMessageText;

	private AudioSource audioSource;

	private static float textTimer;

	private string cachedGOTranslation;

	private TextAnimPhases centreMessageTextAnimPhase;

	private float centreMessageTextAnimScale;

	private Color lastCol = new Color(0f, 0f, 0f, 0f);

    public event Action FireStartRaceFromTrafficLightsEvent;

	public void Reset()
	{
        //if (this.audioSource == null)
        //{
        //    this.audioSource = base.gameObject.AddComponent<AudioSource>();
        //}
        //this.audioSource.volume = 0.4f;
        //this.audioSource.Stop();
		this.centreMessageText = base.gameObject.GetComponentInChildren<TextMeshProUGUI>();
	    this.centreMessageText.color = countdownNumberColour;
		this.centreMessageText.enabled = false;
		this.centreMessageText.text = string.Empty;
		textTimer = 0.5f;
		this.centreMessageTextAnimPhase = TextAnimPhases.None;
		this.cachedGOTranslation = LocalizationManager.GetTranslation("TEXT_COUNTDOWN_GO");
		this.HeadstartRaceCountdown = new HeadstartRaceCountdown(this);
		this.MainRaceCountdown = new MainRaceCountdown(this, runInDebug);
	}

	public void StartCountdown()
	{
		this.MainRaceCountdown.Reset();
		this.MainRaceCountdown.State = RaceCountdown.eState.LEAD_IN;
		if (RelayManager.IsCurrentEventRelay())
		{
			this.MainRaceCountdown.showCountdownGraphics = (RelayManager.GetTimeDifference() >= 0f);
		}
		else if (RaceEventInfo.Instance.CurrentEvent.AutoHeadstart)
		{
			this.MainRaceCountdown.showCountdownGraphics = (RaceEventInfo.Instance.CurrentEvent.AutoHeadstartTimeDifference() >= 0f);
		}
		if (!this.MainRaceCountdown.showCountdownGraphics)
		{
			this.HeadstartRaceCountdown.AICountdownStarted();
		}
	}

	public void CentreMessageTextEnabled(bool enabled)
	{
		this.centreMessageText.enabled = enabled;
	}

	public void ShowGo()
	{
		this.centreMessageText.color =this.countdownGoColour;
		this.ShowText(this.cachedGOTranslation);
		EventManager.Instance.PostEvent("HUD/Go",Camera.main.gameObject);
		RaceHUDController.Instance.HUDAnimator.DismissThrottle();
	}

	public void ShowCountdownNumber(int number)
	{
		this.centreMessageText.color = this.countdownNumberColour;
		this.ShowText(LocalizationManager.GetTranslation("TEXT_COUNTDOWN_NUMBER_"+number));
	    EventManager.Instance.PostEvent("HUD/Countdown", Camera.main.gameObject);//base.gameObject);
	}

	private void FixedUpdate()
	{
		if (PauseGame.isGamePaused)
		{
			return;
		}
		this.MainRaceCountdown.Update();
		this.HeadstartRaceCountdown.Update();
	}

	public void ShowText(string text)
	{
		this.centreMessageText.text = text.ToNativeNumber();
		this.centreMessageTextAnimScale = 6f;
		textTimer = 0.5f + -this.HeadstartRaceCountdown.timeDifference;
		this.centreMessageTextAnimPhase = this.StartCentreTextAnimation();
	}

	public void TriggerRaceStartEvent()
	{
		if (this.FireStartRaceFromTrafficLightsEvent != null)
		{
			this.FireStartRaceFromTrafficLightsEvent();
		}
	}

	public void ShowFinishText(string text, bool positive = true)
	{
		string text2;
		if (positive)
		{
			text2 = text;
			this.centreMessageText.color =  this.winColour;
		}
		else
		{
			text2 = text;
			this.centreMessageText.color = this.loseColour;
		}
		this.MainRaceCountdown.State = RaceCountdown.eState.WIN_LOSE;
		this.centreMessageText.enabled = true;
		this.ShowText(text2);
	}

	public void HideWinLoseText()
	{
		this.centreMessageText.enabled = false;
	}

	public TextAnimPhases StartCentreTextAnimation()
	{
		return TextAnimPhases.FadeIn;
	}

	public void DoTextAnimation()
	{
		RaceHUDController.Instance.hudRaceCentreMessage.updateTextAnimation(ref this.centreMessageTextAnimPhase, 15f, 20f, ref this.centreMessageTextAnimScale, ref textTimer, ref this.centreMessageText, 6f, 1f);
		if (this.centreMessageText.color != this.lastCol)
		{
		    Color color = this.centreMessageText.color;
			color.a = this.centreMessageText.color.a;
		    this.centreMessageText.color = color;
			this.lastCol = this.centreMessageText.color;
		}
	}

	public void updateTextAnimation(ref TextAnimPhases AnimPhase, float AnimSpeed, float AnimSpeed2, ref float AnimScale, ref float Timer, ref TextMeshProUGUI TextMeshProUGUI, float ScaleStart, float ScaleHold)
	{
		switch (AnimPhase)
		{
		case TextAnimPhases.None:
			if (TextMeshProUGUI != null)
			{
				Color color = new Color(0f, 0f, 0f, 0f);
				if (TextMeshProUGUI.color != color)
				{
                    TextMeshProUGUI.color = color;
				}
			}
			break;
		case TextAnimPhases.FadeIn:
			if (AnimScale > ScaleHold)
			{
				AnimScale -= Time.deltaTime * AnimSpeed;
				if (AnimScale < ScaleHold)
				{
					AnimScale = ScaleHold;
				}
				TextMeshProUGUI.transform.localScale = new Vector3(AnimScale, AnimScale, 1f);
				Color color2 = default(Color);
                color2 = TextMeshProUGUI.color;
				color2.a = 1f - (AnimScale - 1f) / (ScaleStart - 1f);
                if (TextMeshProUGUI.color != color2)
				{
                    TextMeshProUGUI.color = color2;
				}
			}
			else
			{
				AnimPhase = TextAnimPhases.Hold;
			}
			break;
		case TextAnimPhases.Hold:
			if (Timer > 0f)
			{
				Timer -= Time.deltaTime;
				if (Timer <= 0f)
				{
					AnimPhase = TextAnimPhases.FadeOut;
				}
			}
			break;
		case TextAnimPhases.FadeOut:
			if (AnimScale < ScaleStart)
			{
				AnimScale += Time.deltaTime * AnimSpeed2;
				if (AnimScale > ScaleStart)
				{
					AnimScale = ScaleStart;
				}
				TextMeshProUGUI.transform.localScale = new Vector3(AnimScale, 1f, 1f);
				Color color3 = default(Color);
                color3 = TextMeshProUGUI.color;
				color3.a = Mathf.Pow(Mathf.Clamp(1f - (AnimScale - 1f) / (ScaleStart - 1f), 0f, 1f), 2f);
                if (TextMeshProUGUI.color != color3)
				{
                    TextMeshProUGUI.color = color3;
				}
			}
			else
			{
				TextMeshProUGUI.enabled = false;
				AnimPhase = TextAnimPhases.None;
			}
			break;
		}
	}
}
