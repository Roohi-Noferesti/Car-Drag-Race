using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class CarNameStats : MonoBehaviour, IPersistentUI
{
	private enum AnimState
	{
		STILL,
		GOINGIN,
		GOINGOUT,
		NAMEIN,
		NAMEOUT
	}

	private const float TimeToAnimIn = 0.2f;

	private const float TimeToAnimOut = 0.8f;


	public TextMeshProUGUI TxtCarName;

	public CarStatsElem CarStatsElem;

	public AnimationCurve AnimCurveIn;

	public AnimationCurve AnimCurveOut;

	private bool _haveACar;

	private float CurrentAnimPos;

	//private Vector3 StatsPosIn = new Vector3(0f, 0.04f, 0f);

	//private Vector3 StatsPosOut = new Vector3(0f, -0.1f, 0f);

	//private Vector3 NamePosIn = new Vector3(0f, -0.1f, -0.06f);

	//private Vector3 NamePosOut = new Vector3(0f, 0.05f, -0.06f);

	private string CarName_TARGET;

	private eCarTier Class_TARGET;

	private int PerfIndex_TARGET;

	private string CarName_Current;

	//private eCarTier Class_Current;

	//private int PerfIndex_Current;

	private HUDScreen CurrentScreen;

	private bool _dontResetTillPerformanceCalculated;

	private AnimState _currentAnimState;
    [SerializeField]
    private bool m_isForGarage;

	//private bool isItTuningUpgrade;

	private AnimState CurrentAnimState
	{
		get
		{
			return this._currentAnimState;
		}
		set
		{
			this._currentAnimState = value;
		}
	}

	private void Start()
	{
        //float y = GUICamera.Instance.PixelToScreenCoords(new Vector2(0f, 40f)).y;
        //this.TxtCarName.renderer.material.SetFloat("_ClipPos", y);
        //float y2 = GUICamera.Instance.PixelToScreenCoords(new Vector2(0f, 40f)).y;
        //this.CarStatsElem.SetClipPos(y2);
        //NavBarAnimationManager.Instance.Subscribe(base.gameObject);
		CarStatsCalculator.Instance.NewPPIndexCalculated += this.CallbackFromCarStats;
		this._dontResetTillPerformanceCalculated = false;
	}

    public void OnScreenChanged(ScreenID screen)
	{
        //if (newScreen == ScreenID.Splash)
        //{
        //    if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
        //    {
        //        this._haveACar = false;
        //    }
        //}
        if (screen == ScreenID.CareerModeMap || screen == ScreenID.Tuning)
        {
            if (PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
            {
                this._haveACar = true;
            }
        }
        //else if (newScreen == ScreenID.SportsPack)
        //{
        //    this._dontResetTillPerformanceCalculated = true;
        //    this._haveACar = false;
        //}
        var zhudScreen = ScreenManager.Instance.ActiveScreen as ZHUDScreen;
        this.Show(zhudScreen != null && zhudScreen.ShowCarName);
        this.CurrentScreen = zhudScreen;
	}

    public void Show(bool zShow)
	{
		base.gameObject.SetActive(zShow && this._haveACar);
	}

	public void CallbackFromCarStats(string carName, eCarTier carTier, int newPP, float newQMTime)
	{
		if (this._dontResetTillPerformanceCalculated)
		{
			this._dontResetTillPerformanceCalculated = false;
		}
		else
		{
			//this.isItTuningUpgrade = true;
		}
		if (this.CurrentAnimState == AnimState.GOINGOUT || this.CurrentAnimState == AnimState.NAMEOUT)
		{
			this.CurrentAnimPos = (0.8f - this.CurrentAnimPos) / 0.8f * 0.2f;
		}
		this.CurrentAnimState = AnimState.GOINGIN;
		this.Reset(carName, carTier, newPP);
	}

	private void ShowTheCorrectColoursForClass(eCarTier zClass)
	{
        //Color tierColour = GameDatabase.Instance.Colours.GetTierColour(zClass);
        //foreach (PackedSprite current in this.SpritesToColour)
        //{
        //    current.SetColor(tierColour);
        //}
	}

	public void SetForLongName(string zCarName)
	{
		this.CarName_TARGET = zCarName;
		this.DoAnimCheck();
	}

	public void Reset(string zCarName, eCarTier zClass, int zPerformanceIndex)
	{
		if (this._dontResetTillPerformanceCalculated)
		{
			if (CarStatsCalculator.Instance.IsCalculatingPerformance)
			{
				return;
			}
			this._dontResetTillPerformanceCalculated = false;
		}
		this.PerfIndex_TARGET = zPerformanceIndex;
		this.Class_TARGET = zClass;
		this.CarName_TARGET = zCarName;
		this.DoAnimCheck();
        var isZScreen = ScreenManager.Instance.ActiveScreen is ZHUDScreen;
        if(!m_isForGarage)
            this.Show(isZScreen && ((ZHUDScreen) ScreenManager.Instance.ActiveScreen).ShowCarName);
	}

	private void RevealCarName()
	{
		this.TriggerValueSwap();
		this.CurrentAnimState = AnimState.GOINGOUT;
		this.CurrentAnimPos = 0f;
		this._haveACar = true;
        //this.TxtCarName.gameObject.transform.localPosition = this.NamePosIn;
        //this.CarStatsElem.gameObject.transform.localPosition = this.StatsPosIn;
	}

	public void ResetHaveCar()
	{
		this._haveACar = false;
	}

	public void DoAnimCheck()
	{
		if (!this._haveACar)
		{
			if (PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
			{
				this.RevealCarName();
			}
		    if (ScreenManager.Instance.ActiveScreen is ShowroomScreen)
		    {
		        this.RevealCarName();
		    }
		    return;
		}
        //if (this.CurrentAnimState != AnimState.STILL)
        //{
        //    return;
        //}
        //bool sameName = string.Equals(LocalizationManager.GetTranslation(this.CarName_Current), LocalizationManager.GetTranslation(this.CarName_TARGET));
        //bool sameTier = Equals(this.Class_Current, this.Class_TARGET);
        //bool samePPIndex = Equals(this.PerfIndex_Current, this.PerfIndex_TARGET);
        //if (!sameName || !sameTier)
        //{
        //    if (!sameName && sameTier && samePPIndex)
        //    {
        //        this.CurrentAnimState = AnimState.NAMEIN;
        //    }
        //    else
        //    {
        //        this.CurrentAnimState = AnimState.GOINGIN;
        //    }
        //}
        //else
        //{
			this.TriggerValueSwap();
        //}
	}

	public void SetShortStatBarTextForTuning(string zCarName, eCarTier zCarTier, int zPerfIndex)
	{
		//this.isItTuningUpgrade = true;
		if (this.CurrentAnimState == AnimState.GOINGOUT || this.CurrentAnimState == AnimState.NAMEOUT)
		{
			this.CurrentAnimPos = (0.8f - this.CurrentAnimPos) / 0.8f * 0.2f;
		}
		this.CurrentAnimState = AnimState.GOINGIN;
		this.Reset(zCarName, zCarTier, zPerfIndex);
	}

	public void SetShortStatBarText(string zCarName, eCarTier zCarTier, int zPerfIndex)
	{
		this.Reset(zCarName, zCarTier, zPerfIndex);
	}

	public void TriggerValueSwap()
	{
		this.TxtCarName.text = LocalizationManager.GetTranslation(this.CarName_TARGET);
		this.CarStatsElem.Set(this.Class_TARGET, this.PerfIndex_TARGET);
		this.ShowTheCorrectColoursForClass(this.Class_TARGET);
		if (this.CurrentScreen is MyCarScreen && this.CarName_Current != this.CarName_TARGET)
		{
			MenuAudio.Instance.playSound(AudioSfx.CarArrived);
		}
		this.CarName_Current = this.CarName_TARGET;
		//this.Class_Current = this.Class_TARGET;
		//this.PerfIndex_Current = this.PerfIndex_TARGET;
	}

    //public void Update()
    //{
    //    if (this.CurrentAnimState == AnimState.STILL)
    //    {
    //        return;
    //    }
    //    this.CurrentAnimPos += Time.deltaTime;
    //    if (this.CurrentAnimState == AnimState.GOINGIN)
    //    {
    //        float num = this.AnimCurveIn.Evaluate(this.CurrentAnimPos / 0.2f);
    //        float d = 1f - num;
    //        if (!this.isItTuningUpgrade)
    //        {
    //            this.TxtCarName.gameObject.transform.localPosition = num * this.NamePosIn + d * this.NamePosOut;
    //        }
    //        this.CarStatsElem.gameObject.transform.localPosition = num * this.StatsPosIn + d * this.StatsPosOut;
    //        if (this.CurrentAnimPos >= 0.2f)
    //        {
    //            this.CurrentAnimPos = 0f;
    //            this.CurrentAnimState = AnimState.GOINGOUT;
    //            this.TriggerValueSwap();
    //        }
    //    }
    //    else if (this.CurrentAnimState == AnimState.GOINGOUT)
    //    {
    //        float num2 = this.AnimCurveOut.Evaluate(this.CurrentAnimPos / 0.8f);
    //        float d2 = 1f - num2;
    //        if (!this.isItTuningUpgrade)
    //        {
    //            this.TxtCarName.gameObject.transform.localPosition = d2 * this.NamePosIn + num2 * this.NamePosOut;
    //        }
    //        this.CarStatsElem.gameObject.transform.localPosition = d2 * this.StatsPosIn + num2 * this.StatsPosOut;
    //        if (this.CurrentAnimPos >= 0.8f)
    //        {
    //            this.CurrentAnimPos = 0f;
    //            this.CurrentAnimState = AnimState.STILL;
    //            if (this.isItTuningUpgrade)
    //            {
    //                this.isItTuningUpgrade = false;
    //            }
    //            this.DoAnimCheck();
    //        }
    //    }
    //    else if (this.CurrentAnimState == AnimState.NAMEIN)
    //    {
    //        float num3 = this.AnimCurveIn.Evaluate(this.CurrentAnimPos / 0.2f);
    //        float d3 = 1f - num3;
    //        this.TxtCarName.gameObject.transform.localPosition = num3 * this.NamePosIn + d3 * this.NamePosOut;
    //        if (this.CurrentAnimPos >= 0.2f)
    //        {
    //            this.CurrentAnimPos = 0f;
    //            this.CurrentAnimState = AnimState.NAMEOUT;
    //            this.TriggerValueSwap();
    //        }
    //    }
    //    else if (this.CurrentAnimState == AnimState.NAMEOUT)
    //    {
    //        float num4 = this.AnimCurveOut.Evaluate(this.CurrentAnimPos / 0.8f);
    //        float d4 = 1f - num4;
    //        this.TxtCarName.gameObject.transform.localPosition = d4 * this.NamePosIn + num4 * this.NamePosOut;
    //        if (this.CurrentAnimPos >= 0.8f)
    //        {
    //            this.CurrentAnimPos = 0f;
    //            this.CurrentAnimState = AnimState.STILL;
    //            this.DoAnimCheck();
    //        }
    //    }
    //}
}
