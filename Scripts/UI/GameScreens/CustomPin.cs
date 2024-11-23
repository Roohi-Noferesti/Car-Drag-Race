using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class CustomPin : MonoBehaviour,IMapPin
{
	public enum CustomType
	{
		Mechanic,
		RaceTheWorld,
		RaceTheWorldWorldTour,
		PrizeList,
		RematchList,
		ClubRacing,
		Leaderboards,
		SeasonInfo,
		FriendRace,
        SMPRace,
        WorldTourFake
	}

	public RuntimeButton Button;

    public RawImage CustomSprite;
    public Image BackgroundSprite;
    public Image GlowSprite;
    public Image GlowSmallSprite;
    public GameObject CircleObject;
    public GameObject ParticleObject;

    public Animator HighlightAnimator;

    public CompletionBar NameBar;

    //public PackedSprite LeftCircleHolder;

    //public PackedSprite RightCircleHolder;

    //public PackedSprite CentreCircleHolder;

    //public SpriteText CircleText;

    private CareerModeMapEventSelect zEventSelect;

	public AudioSfx onClickSound = AudioSfx.MenuClickForward;

	public Animation appearAnimation;

	public CustomPin.CustomType Type
	{
		get;
		private set;
	}

	public Vector2 UIPosition
	{
		get;
		set;
	}

	public void Setup(CustomPin.CustomType zType,string pinID)
	{
		this.Type = zType;
		string zFilename = string.Empty;
		string zName = string.Empty;
		string empty = string.Empty;
		bool shouldUse = false;
		switch (zType)
		{
		case CustomPin.CustomType.Mechanic:
		        zFilename = "HighResPins/Pin_mechanic";
			zName = LocalizationManager.GetTranslation("TEXT_PIN_MECHANIC");
			shouldUse = this.UpdatePinMechanic(true, out empty);
			break;
		case CustomPin.CustomType.PrizeList:
			zFilename = "HighResPins/Multiplayer/circle_pin_prizes";
			zName = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_PRIZELIST");
			this.ShowCircledValue(false);
			break;
		case CustomPin.CustomType.RematchList:
			zFilename = "HighResPins/Multiplayer/circle_pin_prizes";
			zName = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_REMATCHLIST");
			this.ShowCircledValue(false);
			break;
		case CustomPin.CustomType.Leaderboards:
			zFilename = "HighResPins/Multiplayer/circle_pin_leaderboard";
			zName = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_LEADERBOARDS");
			this.ShowCircledValue(false);
			break;
		case CustomPin.CustomType.SeasonInfo:
			zFilename = "HighResPins/Multiplayer/circle_pin_season_info";
			zName = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_SEASONINFO");
			this.ShowCircledValue(false);
		    break;
		    case CustomPin.CustomType.WorldTourFake:
		        zFilename = "HighResPins/"+pinID;
		        zName = "";
		        this.ShowCircledValue(false);
		        break;
        }

	    var color = GetBackgroundColor(Type);
        this.SetupSprite(zFilename, color);
        //name = zType.ToString();
        if (this.NameBar != null)
            this.NameBar.Setup(zName, empty, shouldUse, default(Fraction));
    }

    public Color GetBackgroundColor(CustomType type)
    {
        Color color;
        string colorString = null;
        switch (type)
        {
            case CustomType.Mechanic:
                colorString = "#00FFE9FF";
                break;
        }
        ColorUtility.TryParseHtmlString(colorString, out color);
        return color;
    }

    public bool UpdatePinMechanic(bool firstTime, out string flasher)
	{
		bool flag = false;
		if (PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining == 0)
		{
			this.ShowCircledValue(false);
		}
		else if (PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining < 0)
		{
			this.ShowCircledValue(false);
			flag = true;
		}
		else
		{
			this.ShowCircledValue(true);
			this.SetCircledValue(string.Empty + PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining);
			flag = true;
		}
		if (!firstTime)
		{
            //this.NameBar.SetFlashText(flag);
		}
		flasher = string.Format(LocalizationManager.GetTranslation("TEXT_PIN_RACES_REMAINING"), PlayerProfileManager.Instance.ActiveProfile.MechanicTuningRacesRemaining);
		return flag;
	}

	private void SetupSprite(string zFilename,Color color)
	{
		if (BuildType.IsAppTuttiBuild && zFilename.ToLower().Contains("italia_event") && !zFilename.Contains("apptutti"))
			zFilename += "_apptutti";
        Texture2D texture2D = (Texture2D)Resources.Load("Career/" + zFilename);
        if (texture2D == null)
        {
        }
        ////float num = 200f;
        this.CustomSprite.texture = texture2D;
        //this.BackgroundSprite.color = color;
        //SetupGlow(color);
        //this.CustomSprite.Setup(128f / num, 128f / num, new Vector2(0f, 127f), new Vector2(128f, 128f));
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

	public void onPlayButtonSound()
	{
        if (!interactable)
        {
            return;
        }
		MenuAudio.Instance.playSound(this.onClickSound);
	}

	public void ShowCircledValue(bool show)
	{
		show = false;
        //this.LeftCircleHolder.Hide(!show);
        //this.RightCircleHolder.Hide(!show);
        //this.CentreCircleHolder.Hide(!show);
        //this.CircleText.Hide(!show);
        //this.LeftCircleHolder.gameObject.SetActive(show);
        //this.RightCircleHolder.gameObject.SetActive(show);
        //this.CentreCircleHolder.gameObject.SetActive(show);
        //this.CircleText.gameObject.SetActive(show);
	}

	public void SetCircledValue(string text)
	{
        //this.CircleText.Text = text;
        //float num = 0f;
        //float totalWidth = this.CircleText.TotalWidth;
        //if (totalWidth + 0.02f > this.LeftCircleHolder.width)
        //{
        //    float num2 = this.LeftCircleHolder.width - (totalWidth + 0.02f);
        //    num = Mathf.Ceil(num2 / 0.01f) * 0.01f / 2f;
        //}
        //this.LeftCircleHolder.transform.localPosition = new Vector3(num, 0f, 0f);
        //this.RightCircleHolder.transform.localPosition = new Vector3(-num, 0f, 0f);
        //this.CentreCircleHolder.SetSize(num * 2f, this.CentreCircleHolder.height);
	}

	private void Update()
	{
	}

    public void AddCallbacks(CareerModeMapEventSelect eventSelect)
    {
        this.zEventSelect = eventSelect;
        this.Button.AddValueChangedDelegate(this.onPlayButtonSound);
        this.Button.AddValueChangedDelegate(CallCustomPressed);
    }

    public void RemoveCallbacks(CareerModeMapEventSelect eventSelect)
    {
        this.Button.RemoveValueChangedDelegate(this.onPlayButtonSound);
        this.Button.RemoveValueChangedDelegate(CallCustomPressed);
    }

    private void CallCustomPressed()
    {
        if (!interactable)
        {
            return;
        }
        zEventSelect.OnCustomPress(this);
 
    }

    public Vector3 position
    {
        get { return transform.position; }
    }
    public ProgressionMapPinEventType type { get; set; }

    public eCarTier tier
    {
        get
        {
            return eCarTier.BASE_EVENT_TIER;
        }
    }

    public bool interactable
    {
        get { return true; }
        set {  }
    }

    public string Name { get; set; }

    public void SetHightlight(bool value)
    {
        if (CircleObject != null)
            CircleObject.SetActive(value);
        if (ParticleObject != null)
            ParticleObject.SetActive(value);
        HighlightAnimator.Play(value ? "Arrow" : "disable");

    }
}
