using DataSerialization;
using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Vector3 = UnityEngine.Vector3;

public class EventPin : MonoBehaviour,IMapPin
{
	public enum eState
	{
		Normal,
		Completed
	}

	public const float WorldTourPinGreyness = 0.85f;

	public RuntimeButton Button;

    public Image EventSprite;
    public Image GlowSprite;
    public Image GlowSmallSprite;
    public GameObject CircleObject;
    public GameObject ParticleObject;

    public RawImage OverlaySprite;

    public RawImage BossSprite;

    public Image CrossSprite;

    public Image CrossGlowSprite;

    public Image ReflectionSprite;

    public CompletionBar CompletionBar;

    public GameObject[] ActiveStaffs;

    [NonSerialized]
	public bool IsEventPin;
    [NonSerialized]
	public bool IsGroupPin;

    [NonSerialized]
	public RaceEventData EventData;
    [NonSerialized]
	public RaceEventGroup GroupData;

	public AudioSfx onClickSound = AudioSfx.MenuClickForward;

	private bool alwaysPlayClickSound;

	public Animation appearAnimation;

    public Animator HighlightAnimator;

	private BubbleMessage bubble;

	private BubbleMessageData worldTourBubbleData;

	private bool bubbleMessageShown;

	private EventPin.eState State;

    private CareerModeMapEventSelect zEventSelect;

	public EventPin.eState GetState
	{
		get
		{
			return this.State;
		}
	}

	public bool IsTapDisabled
	{
		get;
		private set;
	}

	public void KillBubble()
	{
		if (this.bubble != null)
		{
			this.bubble.KillNow();
			this.bubble = null;
		}
		this.worldTourBubbleData = null;
		this.bubbleMessageShown = false;
	}

	public void DismissBubble()
	{
		if (this.bubble != null)
		{
			this.bubble.Dismiss();
			this.bubble = null;
		}
		this.bubbleMessageShown = false;
	}

    public void ShowBubbleMessage(float DelayTime)
    {
        BubbleMessageData bubbleMessageData = null;
        if (this.worldTourBubbleData != null && this.worldTourBubbleData.ShouldBeDisplayed())
        {
            bubbleMessageData = this.worldTourBubbleData;
        }
        else if (this.EventData != null)
        {
            bubbleMessageData = this.EventData.PinBubbleMessage;
        }
        if (bubbleMessageData != null && bubbleMessageData.ShouldBeDisplayed())
        {
            Action<BubbleMessage> callback = delegate(BubbleMessage callbackBubble)
            {
                if (this.bubble != null || !this.bubbleMessageShown)
                {
                    callbackBubble.KillNow();
                }
                else
                {
                    this.bubble = callbackBubble;
                }
            };
            base.StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(DelayTime, bubbleMessageData.Text, false,
                base.gameObject.transform, new UnityEngine.Vector3(0f, bubbleMessageData.YOffset, -0.5f),
                BubbleMessage.NippleDir.UP, bubbleMessageData.NipplePos, BubbleMessageConfig.ThemeStyle.SMALL, false,
                callback));
            this.bubbleMessageShown = true;
        }
    }

    public bool IsBubbleMessageShown()
	{
		return this.bubbleMessageShown;
	}

	public void AddCallbacks(CareerModeMapEventSelect eventSelect)
	{
        this.zEventSelect = eventSelect;
        this.Button.AddValueChangedDelegate(this.onPlayButtonSound);
        this.Button.AddValueChangedDelegate(CallEventPressed);
	}

    private void CallEventPressed()
    {
        zEventSelect.OnEventPress(this);
    }

	public void RemoveCallbacks(CareerModeMapEventSelect eventSelect)
	{
        this.Button.RemoveValueChangedDelegate(this.onPlayButtonSound);
        this.Button.RemoveValueChangedDelegate(CallEventPressed);
		this.RemoveTierXCallbacks(zEventSelect);
	}


	public bool IsCompleted()
	{
		return this.State == EventPin.eState.Completed;
	}

	public void Setup(RaceEventData zEvent)
	{
		this.KillBubble();
		this.State = EventPin.eState.Normal;
		this.IsTapDisabled = false;
		this.GroupData = null;
		this.EventData = zEvent;
		this.IsEventPin = true;
		this.IsGroupPin = false;
        //string backgroundTextureName = zEvent.Group.Parent.GetBackgroundTextureName(zEvent);
        Color backgroundColor = zEvent.Group.Parent.GetBackgroundColor(zEvent);
		string overlayTextureName = zEvent.Group.Parent.GetOverlayTextureName(zEvent);
		string bossTextureName = zEvent.Group.Parent.GetBossTextureName(zEvent);
        this.SetupSprite(backgroundColor, overlayTextureName, bossTextureName, true);
		ProgressBarStyle style = ProgressBarStyle.Bar;
	    Fraction progress;
        //foreach (var activeStaff in ActiveStaffs)
        //{
        //    activeStaff.SetActive(zEvent.IsRegulationRace() || zEvent.IsDailyBattle());
        //}
        if (zEvent.IsRegulationRace() || zEvent.IsRaceTheWorldOrClubRaceEvent() || zEvent.IsFriendRaceEvent() || zEvent.IsDailyBattle()
            || zEvent.IsSMPRaceEvent())
		{
			style = ProgressBarStyle.None;
		}
        if (zEvent.GetWorldTourPinPinDetail() != null)
        {
            if (!zEvent.GetWorldTourPinPinDetail().HideTextBox)
            {
                //this.LoadCompletionBarPrefab(false);
                CompletionBar.gameObject.SetActive(true);
                progress = new Fraction
                {
                    Numerator = zEvent.Parent.NumEventsComplete(),
                    Denominator = zEvent.Parent.NumOfEvents()
                };
                this.CompletionBar.Setup(zEvent, style, progress);
            }
            else
            {
                CompletionBar.gameObject.SetActive(false);
                //this.DestroyCompletionBar();
            }
        }
        //<Added by me mojtaba
        progress = new Fraction
        {
            Numerator = zEvent.Parent.NumEventsComplete(),
            Denominator = zEvent.Parent.NumOfEvents()
        };
	    if (zEvent.IsSMPRaceEvent())
	    {
            this.CompletionBar.SetupForTitle(zEvent);
	    }
	    else
	    {
            this.CompletionBar.Setup(zEvent, style, progress);
	    }
        //Added by me mojtaba>

		this.ResetAlpha();
	}

	public float getSpriteWidth()
	{
	    //return this.EventSprite.width;
	    return 0;
	}

    public float getSpriteHeight()
    {
        //return this.EventSprite.height;
        return 0;
    }

    public void setDimensions(float width, float height)
	{
        //this.EventSprite.SetSize(width, height);
        //this.Button.SetSize(width, height);
	}

	public void SetupCompleted(bool zShowCross = true)
	{
		this.KillBubble();
		this.State = EventPin.eState.Completed;
		if (zShowCross)
		{
			this.SetupCross();
		}
		if (this.EventData != null)
		{
            //this.CrossGlowSprite.gameObject.transform.localPosition = new UnityEngine.Vector3(0f, this.EventData.Parent.GetOverlayOffset().y, 0.02f);
		}
		this.SetGreyness(1f);
        this.CompletionBar.Disable();
	}

	private void SetupForTimeline(TimelinePinDetails timelineDetails)
	{
		this.KillBubble();
		this.State = EventPin.eState.Completed;
        //this.CompletionBar.Disable();
		this.SetAlpha(timelineDetails.Alpha);
		this.SetGreyness(timelineDetails.Greyness);
		base.gameObject.transform.localScale = new UnityEngine.Vector3(timelineDetails.Scale.x, timelineDetails.Scale.y, 1f);
	}

	public void SetupForPreviousInTimeline(TimelinePinDetails timelineDetails)
	{
		this.SetupCross();
		this.SetupForTimeline(timelineDetails);
	}

	public void SetupForNextInTimeline(TimelinePinDetails timelineDetails)
	{
		this.SetupForTimeline(timelineDetails);
	}

	private void ResetAlpha()
	{
		this.SetAlpha(1f);
	}

	private void SetAlpha(float alpha)
	{
		this.SetPinAlpha(alpha);
		this.SetCrossAlpha(alpha);
	}

	public void SetCrossAlpha(float alpha)
	{
        //this.CrossSprite.SetAlpha(alpha);
        //this.CrossGlowSprite.SetAlpha(alpha);
	}

	public void SetPinAlpha(float alpha)
	{
        //this.EventSprite.SetAlpha(alpha);
        //this.BossSprite.SetAlpha(alpha);
        //this.OverlaySprite.SetAlpha(alpha);
        //if (this.CompletionBar != null)
        //{
        //    this.CompletionBar.SetAlpha(alpha);
        //}
	}

	public void SetGreyness(float greyness)
	{
        //this.EventSprite.GetComponent<Renderer>().material.SetFloat("_Greyness", greyness);
        //this.OverlaySprite.GetComponent<Renderer>().material.SetFloat("_Greyness", greyness);
        //this.BossSprite.GetComponent<Renderer>().material.SetFloat("_Greyness", greyness);
	}

	public void ResetScale()
	{
        //base.gameObject.transform.localScale = UnityEngine.Vector3.one;
        //this.CrossSprite.transform.localScale = UnityEngine.Vector3.one;
        //this.CrossGlowSprite.transform.localScale = UnityEngine.Vector3.one;
        //this.EventSprite.transform.localScale = UnityEngine.Vector3.one;
        //this.BossSprite.transform.localScale = UnityEngine.Vector3.one;
        //this.OverlaySprite.transform.localScale = UnityEngine.Vector3.one;
	}

	private void SetupCross()
	{
        //Texture2D texture2D = (Texture2D)Resources.Load("Career/Small_X");
        //this.CrossSprite.GetComponent<Renderer>().material.mainTexture = texture2D;
        //this.CrossSprite.Setup((float)texture2D.width / 200f, (float)texture2D.height / 200f, new UnityEngine.Vector2(0f, (float)(texture2D.height - 1)), new UnityEngine.Vector2((float)texture2D.width, (float)texture2D.height));
        //this.CrossSprite.GetComponent<Renderer>().material.SetFloat("_Greyness", 1f);
        if (CrossSprite != null)
	        this.CrossSprite.gameObject.SetActive(true);
	    GetComponent<CanvasGroup>().alpha = 0.5F;
	    foreach (var activeStaff in ActiveStaffs)
	    {
	        activeStaff.SetActive(false);
	    }
	    //this.CrossGlowSprite.gameObject.SetActive(true);
	}

    private void SetupSprite(Color zBackgroundColor, string zOverlay, string zBossSprite, bool useEventData = true)
	{
        //float num = 200f;
        //if (string.IsNullOrEmpty(zBackground))
        //{
        //    this.EventSprite.gameObject.SetActive(false);
        //}
        //else
        //{
        //    Texture2D texture2D = (Texture2D)Resources.Load("Career/HighResPins/" + zBackground);
        //    if (texture2D == null)
        //    {
        //        return;
        //    }
        //    this.EventSprite.gameObject.SetActive(true);
        //    this.EventSprite.GetComponent<Renderer>().material.mainTexture = texture2D;
        //    //this.EventSprite.Setup((float)texture2D.width / num, 0.5f * (float)texture2D.height / num, new UnityEngine.Vector2(0f, (float)(texture2D.height - 1)), new UnityEngine.Vector2((float)texture2D.width, (float)texture2D.height));
        //    this.EventSprite.GetComponent<Renderer>().material.SetFloat("_Greyness", 0f);
        //    this.EventSprite.gameObject.transform.localPosition = UnityEngine.Vector3.zero;
        //}
        if (EventSprite != null)
            this.EventSprite.color = zBackgroundColor;
        SetupGlow(zBackgroundColor);
		if (string.IsNullOrEmpty(zOverlay))
		{
		    if (OverlaySprite != null)
		        this.OverlaySprite.gameObject.SetActive(false);
		}
		else
		{
			Texture2D overlayTexture = (Texture2D)Resources.Load("Career/HighResPins/" + zOverlay);
			if (overlayTexture == null)
			{
				return;
			}
		    if (OverlaySprite != null)
		    {
		        this.OverlaySprite.gameObject.SetActive(true);
		        this.OverlaySprite.texture = overlayTexture;
		    }
		    //this.OverlaySprite.Setup(0.5f * (float)texture2D2.width / num, 0.5f * (float)texture2D2.height / num, new UnityEngine.Vector2(0f, (float)(texture2D2.height - 1)), new UnityEngine.Vector2((float)texture2D2.width, (float)texture2D2.height));
            //if (useEventData)
            //{
            //    this.OverlaySprite.gameObject.transform.localPosition = new UnityEngine.Vector3(0f, this.EventData.Parent.GetOverlayOffset().y, -0.01f);
            //}
            //this.OverlaySprite.GetComponent<Renderer>().material.SetFloat("_Greyness", 0f);
		}
        if (string.IsNullOrEmpty(zBossSprite))
        {
            if (this.BossSprite != null)
                this.BossSprite.gameObject.SetActive(false);
        }
        else
        {
            Texture2D bossTexture = (Texture2D)Resources.Load("CharacterCards/" + zBossSprite);
            if (bossTexture == null)
            {
                return;
            }
            this.BossSprite.gameObject.SetActive(true);
            this.BossSprite.texture = bossTexture;
            //this.BossSprite.Setup(0.5f * (float)texture2D3.width / num, 0.5f * (float)texture2D3.height / num, new UnityEngine.Vector2(0f, (float)(texture2D3.height - 1)), new UnityEngine.Vector2((float)texture2D3.width, (float)texture2D3.height));
            //if (useEventData)
            //{
            //    this.BossSprite.gameObject.transform.localPosition = new UnityEngine.Vector3(0f, this.EventData.Parent.GetOverlayOffset().y, -0.01f);
            //}
            //this.BossSprite.GetComponent<Renderer>().material.SetFloat("_Greyness", 0f);
        }
        this.DeActivateCross();
        //this.ReflectionSprite.gameObject.SetActive(false);
        //this.CrossSprite.gameObject.transform.localPosition = new UnityEngine.Vector3(0f, 0f, -0.13f);
	}

    private void SetupGlow(Color color)
    {
        var alpha = GlowSprite.color.a;
        color.a = alpha;
        GlowSprite.color = color;

        alpha = GlowSmallSprite.color.a;
        color.a = alpha;
        GlowSmallSprite.color = color;
    }

	public void Setup(RaceEventGroup zGroup)
	{
		this.KillBubble();
		this.State = EventPin.eState.Normal;
		this.IsTapDisabled = false;
		this.GroupData = zGroup;
		this.EventData = zGroup.RaceEvents[0];
		this.IsEventPin = false;
		this.IsGroupPin = true;
        //string backgroundTextureName = zGroup.Parent.GetBackgroundTextureName(null);
        Color backgroundColor = zGroup.Parent.GetBackgroundColor(null);
		string overlayTextureName = zGroup.Parent.GetOverlayTextureName(null);
        this.SetupSprite(backgroundColor, overlayTextureName, null, true);
        if (this.EventData.GetWorldTourPinPinDetail() != null)
        {
            if (!this.EventData.GetWorldTourPinPinDetail().HideTextBox)
            {
                if (zGroup.RaceEvents[0].IsRegulationRace())
                {
                    this.CompletionBar.Setup(zGroup.RaceEvents[0], ProgressBarStyle.None, default(Fraction));
                }
                else
                {
                    this.CompletionBar.Setup(zGroup.RaceEvents[0], ProgressBarStyle.Bar, default(Fraction));
                }
            }
            else
            {
                CompletionBar.Hide();
            }
        }
		this.onClickSound = AudioSfx.MenuClickForward;
		this.alwaysPlayClickSound = false;
		this.ResetPin();
	}

	public void onPlayButtonSound()
	{
		if ((!this.IsTapDisabled && !this.IsCompleted()) || this.alwaysPlayClickSound)
		{
			MenuAudio.Instance.playSound(this.onClickSound);
		}
	}

	public virtual void SetupTierX(PinDetail pin,RaceEventData eventData, bool eventPin, Texture2D backgroundTex, Texture2D overlayTex, Texture2D Sprite, string Title, ProgressBarStyle progressStyle, bool isUnlocked, BubbleMessageData bubbleMessageData, Fraction progression, bool isHyperEvent = false)
	{
		this.KillBubble();
		this.worldTourBubbleData = bubbleMessageData;
		this.State = EventPin.eState.Normal;
		this.IsTapDisabled = !pin.IsSelectable;
		this.IsEventPin = eventPin;
		this.IsGroupPin = !this.IsEventPin;
		this.SetupTierXSprite(pin, backgroundTex, overlayTex, Sprite, isUnlocked, isHyperEvent);
		if (!pin.HideTextBox)
		{
			if (!pin.IsSelectable && TierXManager.Instance.IsOverviewThemeActive())
			{
				if (pin.PinID == "WorldTour_Hyper_Event")
				{
                    Title = LocalizationManager.GetTranslation("TEXT_WORLDTOUR_HYPER_TITLE");
                }
				else
				{
                    Title = LocalizationManager.GetTranslation("TEXT_COMING_SOON");
                }
			}
            this.CompletionBar.SetupTierX(Title,eventData, progressStyle, progression);
        }
		else
		{
            this.CompletionBar.Hide();
		}
		if (pin.PinID.ToLower().StartsWith("car"))
		{
			this.onClickSound = AudioSfx.CarArrived;
			this.alwaysPlayClickSound = true;
		}
		else
		{
			this.onClickSound = AudioSfx.MenuClickForward;
			this.alwaysPlayClickSound = false;
		}
		this.ResetPin();
	}

	private void SetupTierXSprite(PinDetail pin, Texture2D zBackground, Texture2D zOverlay, Texture2D zBossSprite, bool isUnlocked, bool isHyperEvent)
	{
		//float num = 200f;
		if (zBackground == null)
		{
		    //if (this.EventSprite != null)
		    //    this.EventSprite.gameObject.SetActive(false);
		}
		else
		{
            //this.EventSprite.gameObject.SetActive(true);
            //this.EventSprite.renderer.material.mainTexture = zBackground;
            //this.EventSprite.Setup((float)zBackground.width * pin.GetPinTextureScale().x / num, 0.5f * (float)zBackground.height * pin.GetPinTextureScale().y / num, new UnityEngine.Vector2(0f, (float)(zBackground.height - 1)), new UnityEngine.Vector2((float)zBackground.width, (float)zBackground.height));
            //this.EventSprite.gameObject.transform.localPosition = pin.GetBackgroundOffset();
            //this.EventSprite.renderer.material.SetFloat("_Greyness", pin.Greyness);
		}
		//this.ReflectionSprite.gameObject.SetActive(false);
		if (zOverlay == null)
		{
			this.OverlaySprite.gameObject.SetActive(false);
		}
		else
		{
			this.OverlaySprite.gameObject.SetActive(true);
		    this.OverlaySprite.texture = zOverlay;
            //this.OverlaySprite.renderer.material.mainTexture = zOverlay;
            if (!isUnlocked)
			{
                //this.OverlaySprite.Setup((float)zOverlay.width / num, (float)zOverlay.height / num, new UnityEngine.Vector2(0f, (float)(zOverlay.height - 1)), new UnityEngine.Vector2((float)zOverlay.width, (float)zOverlay.height));
			}
			else
			{
                //this.OverlaySprite.Setup(0.5f * (float)zOverlay.width * pin.GetPinTextureScale().x / num, 0.5f * (float)zOverlay.height * pin.GetPinTextureScale().y / num, new UnityEngine.Vector2(0f, (float)(zOverlay.height - 1)), new UnityEngine.Vector2((float)zOverlay.width, (float)zOverlay.height));
			}
            //this.OverlaySprite.gameObject.transform.localPosition = new UnityEngine.Vector3(pin.GetOverlayOffset().x, pin.GetOverlayOffset().y, -0.1f);
            //this.OverlaySprite.renderer.material.SetFloat("_Greyness", pin.Greyness);
		}
		if (zBossSprite == null)
		{
		    if (this.BossSprite != null)
		        this.BossSprite.gameObject.SetActive(false);
		}
		else
		{
			this.BossSprite.gameObject.SetActive(true);
		    this.BossSprite.texture = zBossSprite;
            //this.BossSprite.renderer.material.mainTexture = zBossSprite;
            //this.BossSprite.Setup((float)zBossSprite.width * pin.GetBossScale().x / num, (float)zBossSprite.height * pin.GetBossScale().y / num, new UnityEngine.Vector2(0f, (float)(zBossSprite.height - 1)), new UnityEngine.Vector2((float)zBossSprite.width, (float)zBossSprite.height));
            //this.BossSprite.gameObject.transform.localPosition = new UnityEngine.Vector3(pin.GetBossOffset().x, pin.GetBossOffset().y, -0.1f);
			//float value = isUnlocked ? pin.Greyness : 0.85f;
            //this.BossSprite.renderer.material.SetFloat("_Greyness", value);
			//if (pin.PinID == "WorldTour_UK_Event" || pin.PinID == "WorldTour_USA_Event")
			//{
   //             //this.ReflectionSprite.transform.localScale = pin.GetBossScale();
			//	string key = (!(pin.PinID == "WorldTour_UK_Event")) ? "us-flag-blur" : "uk-flag-blur";
			//	Texture2D texture2D = TierXManager.Instance.PinTextures[key];
   //             //this.ReflectionSprite.SetTexture(texture2D);
   //             //this.ReflectionSprite.Setup((float)texture2D.width / num, (float)texture2D.height / num, new UnityEngine.Vector2(0f, (float)texture2D.height), new UnityEngine.Vector2((float)texture2D.width, (float)texture2D.height));
   //             //this.ReflectionSprite.renderer.material.SetFloat("_Greyness", value);
			//	this.ReflectionSprite.gameObject.SetActive(true);
			//}
		}
        //UnityEngine.Vector2 crossOffset = pin.GetCrossOffset();
        //this.CrossSprite.gameObject.transform.localPosition = new UnityEngine.Vector3(crossOffset.x, crossOffset.y, -0.13f);
        //this.CrossGlowSprite.gameObject.transform.localPosition = new UnityEngine.Vector3(crossOffset.x, crossOffset.y, -0.12f);
		this.DeActivateCross();
	}

	public void GreyOutBossSprite()
	{
        //this.BossSprite.renderer.material.SetFloat("_Greyness", 1f);
		this.DisablePin();
	}

	public void CrossAnimation()
	{
		this.SetupCross();
        //this.CrossSprite.renderer.material.SetFloat("_Greyness", 0f);
		Animation component = this.CrossSprite.GetComponent<Animation>();
		component.Play();
	}

	public void DisablePin()
	{
		if (this.EventData != null)
		{
			this.CrossGlowSprite.gameObject.transform.localPosition = new UnityEngine.Vector3(0f, this.EventData.Parent.GetOverlayOffset().y, 0.02f);
		}
        //this.EventSprite.renderer.material.SetFloat("_Greyness", 1f);
        //this.OverlaySprite.renderer.material.SetFloat("_Greyness", 1f);
        //this.CompletionBar.Disable();
	}

	public void ResetPin()
	{
		this.DeActivateCross();
		this.ResetAlpha();
		this.ResetScale();
	}

	public void AddTierXCallbacks(CareerModeMapEventSelect zEventSelect)
	{
	    this.zEventSelect = zEventSelect;
        this.Button.AddValueChangedDelegate(this.onPlayButtonSound);
	    this.Button.AddValueChangedDelegate(OnTierXPinPress);
	}

	public void RemoveTierXCallbacks(CareerModeMapEventSelect zEventSelect)
	{
        this.Button.RemoveValueChangedDelegate(OnTierXPinPress);
    }

    private void OnTierXPinPress()
    {
        zEventSelect.OnTierXPinPress(this);
    }


    private void DeActivateCross()
	{
        if(CrossSprite!=null)
        this.CrossSprite.gameObject.SetActive(false);
        //this.CrossGlowSprite.gameObject.SetActive(false);
	}

    public Vector3 position
    {
        get { return transform.position; }
    }
    public ProgressionMapPinEventType type { get; set; }
    public eCarTier tier { get; set; }

    public bool interactable
    {
        get { return !IsTapDisabled; }
        set { IsTapDisabled = !value; }
    }

    public string Name { get; set; }

    public void SetHightlight(bool value)
    {
        CircleObject.SetActive(value);
        ParticleObject.SetActive(value);
        HighlightAnimator.Play(value ? "Arrow" : "disable");
    }
}
