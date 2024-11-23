using Metrics;
using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrizeOMaticCard : MonoBehaviour, IBundleOwner
{
	public enum CardState
	{
		Waiting,
		Dealing,
		Dealt,
		Animating,
		PrizeReveal,
		CardFlipped,
		End
	}

	public enum CardAnimationState
	{
		None,
		Flip
	}

	public enum RevealAnimationState
	{
		None,
		GlowFade
	}

	private const float DEAL_SPEED = 1f;

	private const float DEAL_DURATION = 0.5f;

	private const int MAX_CARD_AUDIO_NUMBER = 5;

	private const float CAR_UV_ADD = 0.5f;

	private const float ADJUST_TO_FLIP_LENGTH = 0.5f;

	private PrizeOMaticCard.CardAnimationState _CurrentAnimationState;

	private PrizeOMaticCard.CardState _CurrentCardState;

	private PrizeOMaticCard.RevealAnimationState _CurrentRevealState;

	private Reward _CardReward;

	private int _CardNo;

	private string _CarCardDBKey;

	private bool _CardSelected;

	private bool _PrizeAlreadyAwarded;

	public GameObject CardGameObject;

	public GameObject CardHolder;

	public GameObject CardGlow;

    public float Card_In_Time = 0.5F;

	public float LENGTH_OF_GLOW_REVEAL = 2.5f;

	//private float _TimeThroughGlowAnim;

	//private Texture2D _CardTexture;

	public ParticleSystem TurnOverParticle;

	public AnimationCurve AnimCurve;

	public AnimationCurve GlowFadeCurve;

	public AnimationCurve CameraShake;

	public Animator CardAnimation;

    public TextMeshProUGUI PrizeText;

	private float _TimeThroughCardFlip;

	private float _LengthOfCardFlipAnim;

	private Vector3 _StartPos;

	private Vector3 _EndPos;

	private float _RandomRotation;

	private float _TimingValue;

    private Animator m_animator;

    public GameObject adIcon;

	public PrizeOMaticCard.CardAnimationState CurrentAnimationState
	{
		get
		{
			return this._CurrentAnimationState;
		}
		set
		{
			this._CurrentAnimationState = value;
		}
	}

	public PrizeOMaticCard.CardState CurrentCardState
	{
		get
		{
			return this._CurrentCardState;
		}
		set
		{
			this._CurrentCardState = value;
		}
	}

	public Reward CardReward
	{
		get
		{
			return this._CardReward;
		}
		set
		{
			this._CardReward = value;
		}
	}

	public int CardNo
	{
		get
		{
			return this._CardNo;
		}
		set
		{
			this._CardNo = value;
		}
	}

	public string CarCardDBKey
	{
		get
		{
			return this._CarCardDBKey;
		}
		set
		{
			this._CarCardDBKey = value;
		}
	}

	public Vector3 StartPos
	{
		get
		{
			return this._StartPos;
		}
		set
		{
			this._StartPos = value;
		}
	}

	public Vector3 EndPos
	{
		get
		{
			return this._EndPos;
		}
		set
		{
			this._EndPos = value;
		}
	}

	public float RandomRotation
	{
		get
		{
			return this._RandomRotation;
		}
		set
		{
			this._RandomRotation = value;
		}
	}

	public bool CardSelected
	{
		get
		{
			return this._CardSelected;
		}
		set
		{
			this._CardSelected = value;
		}
	}

    public PrizeOMaticCard.CardState ChangeState(PrizeOMaticCard.CardState newState)
    {
        this.CurrentCardState = newState;
        switch (newState)
        {
            case PrizeOMaticCard.CardState.Dealing:
                //this.PlayCardSound();
                if (m_animator == null)
                {
                    m_animator = GetComponent<Animator>();
                }
                m_animator.Play("Card_in");
                break;
            case PrizeOMaticCard.CardState.PrizeReveal:
                this.PlayTurnCelebrationAnim();
                //this.CardGameObject.GetComponent<Renderer>().materials[0].SetTexture("_MainTex", this._CardTexture);
                //this.CardGameObject.GetComponent<Renderer>().materials[1].SetTexture("_MainTex", this._CardTexture);
                break;
        }
        return newState;
    }

    private void Start()
	{
        //this._LengthOfCardFlipAnim = this.CardAnimation["CardTurn"].length - 0.5f;
		this._CardSelected = false;
        //this.CardGlow.SetActive(false);
		//this._TimeThroughGlowAnim = this.LENGTH_OF_GLOW_REVEAL;
		this._CurrentRevealState = PrizeOMaticCard.RevealAnimationState.None;
	}


	public void SetupCard()
	{
		adIcon.SetActive(false);
		string textureForReward = PrizeomaticCardsTextureManager.GetTextureForReward(this._CardReward);
		this.LoadCardFrontFaceTexture(textureForReward);
        AwardPrizeBase awardPrizeBase = PrizeomaticAwarding.CreatePrize(this.CardReward);
	    PrizeText.text = awardPrizeBase.GetPrizeString();
		if (PrizeOMaticRewardsManager.IsRewardACarReward(this._CardReward))
		{
			this.SetCarPartTexture(true, this._CarCardDBKey);
		}
		else
		{
			this.SetCarPartTexture(false, string.Empty);
		}
	}

	public void LoadCardFrontFaceTexture(string texturePath)
	{
		//this._CardTexture = (Resources.Load(texturePath) as Texture2D);
	}

	public void LoadedAsset(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
		Texture2D texture = zAssetBundle.mainAsset as Texture2D;
		Color color = new Color(1f, 1f, 1f, 1f);
		this.CardGameObject.GetComponent<Renderer>().materials[2].SetColor("_Tint", color);
		this.CardGameObject.GetComponent<Renderer>().materials[2].SetTexture("_MainTex", texture);
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
	}

	public void SetCarPartTexture(bool BeingUsed, string CarDBKey)
	{
        //Color color = new Color(0f, 0f, 0f, 0f);
        //this.CardGameObject.GetComponent<Renderer>().materials[2].SetColor("_Tint", color);
        //if (BeingUsed)
        //{
        //    this.CardGameObject.GetComponent<Renderer>().materials[2].SetVector("_UVScale", new Vector3(1f, 0.53f, 0f));
        //    this.CardGameObject.GetComponent<Renderer>().materials[2].SetFloat("_YOffset", 0.45f);
        //AssetProviderClient.Instance.RequestAsset("PrizeTex_" + CarDBKey, new BundleLoadedDelegate(this.LoadedAsset), this);
        //}
	}

	public void QuickPrizeChange(Reward newReward)
	{
		this._CardReward = newReward;
		if (!PrizeOMaticScreen.IsTestRunning)
		{
			this.SetupCard();
		}
	}

	public void Update()
	{
	    switch (this._CurrentCardState)
	    {
	        case PrizeOMaticCard.CardState.Dealing:
	            this.DealingCardUpdate();
	            break;
	        case PrizeOMaticCard.CardState.Animating:
	            this.AnimationUpdate();
	            break;
	        case PrizeOMaticCard.CardState.PrizeReveal:
	            this.PrizeRevealUpdate();
	            break;
	    }
	    //Debug.Log(_CurrentCardState);
	}

	public void DealingCardUpdate()
	{
        //this.CardHolder.gameObject.transform.Rotate(0f, 0f, this.RandomRotation * Time.deltaTime);
        //this._TimingValue += Time.deltaTime*1f;
        //float time = this._TimingValue / 0.5f;
        //float num = Mathf.Clamp01(this.AnimCurve.Evaluate(time));
        ////Vector3 position = Vector3.Lerp(this.StartPos, this.EndPos, num);
        ////base.transform.position = position;
        //if (num == 1f || this._TimingValue >= 1f)
        //{
        //    this.CurrentCardState = PrizeOMaticCard.CardState.Dealt;
        //}



        if (m_animator.IsFinished("Card_in", Card_In_Time))
	    {
            this.CurrentCardState = PrizeOMaticCard.CardState.Dealt;
	    }
	}

	public void AnimationUpdate()
	{
		PrizeOMaticCard.CardAnimationState currentAnimationState = this._CurrentAnimationState;
	    if (currentAnimationState == PrizeOMaticCard.CardAnimationState.Flip)
	    {
	        this.Anim_CardFlipUpdate();
	    }
	}

	private void Anim_CardFlipUpdate()
	{
	    if (this.m_animator.IsFinished("Reveal"))
	    {
            this._CurrentRevealState = PrizeOMaticCard.RevealAnimationState.None;
            this._CurrentCardState = this.ChangeState(PrizeOMaticCard.CardState.CardFlipped);
            this._CurrentAnimationState = PrizeOMaticCard.CardAnimationState.Flip;
	    }
        //if (this._TimeThroughCardFlip >= this._LengthOfCardFlipAnim)
        //{
        //    this.CurrentAnimationState = PrizeOMaticCard.CardAnimationState.None;
        //    this.CurrentCardState = this.ChangeState(PrizeOMaticCard.CardState.PrizeReveal);
        //    this._CurrentRevealState = PrizeOMaticCard.RevealAnimationState.GlowFade;
        //    return;
        //}
        //this._TimeThroughCardFlip += Time.deltaTime;
	}

	private void PrizeRevealUpdate()
	{
		PrizeOMaticCard.RevealAnimationState currentRevealState = this._CurrentRevealState;
	    if (currentRevealState == PrizeOMaticCard.RevealAnimationState.GlowFade)
	    {
	        this.FadeOutGlow();
	    }
	}

	private void FadeOutGlow()
	{
		if (false)//this._TimeThroughGlowAnim > 0f)
		{
			//this._TimeThroughGlowAnim -= Time.deltaTime;
			//float time = this._TimeThroughGlowAnim / this.LENGTH_OF_GLOW_REVEAL;
			//float t = Mathf.Clamp01(this.GlowFadeCurve.Evaluate(time));
			//float num = Mathf.Lerp(0f, 1f, t);
			//Color color = new Color(num, num, num, num);
			//this.CardGlow.gameObject.GetComponent<Renderer>().material.SetColor("_Tint", color);
			//color = new Color(num, num, num, num);
		}
		else
		{
			this._CurrentRevealState = PrizeOMaticCard.RevealAnimationState.None;
			this._CurrentCardState = this.ChangeState(PrizeOMaticCard.CardState.CardFlipped);
			this._CurrentAnimationState = PrizeOMaticCard.CardAnimationState.Flip;
			this.Anim_HideGlow();
		}
	}

	public void PlayRevealAnimation()
	{
        m_animator.Play("Reveal");
        //AnimationUtils.PlayAnim(this.CardAnimation, "CardTurn");
        this.CurrentAnimationState = PrizeOMaticCard.CardAnimationState.Flip;
        this.CurrentCardState = PrizeOMaticCard.CardState.Animating;
	}

	private void PlayTurnCelebrationAnim()
	{
        //this.CardAnimation.Play("Reveal");
	    //AnimationUtils.PlayAnim(this.TurnAnimation, "PrizeomaticTurnCelebration");
	}

	private void Anim_StartParticleSystem()
	{
		this.TurnOverParticle.Play();
	}

	private void Anim_StartGlow()
	{
		this.CardGlow.SetActive(true);
	}

	private void Anim_HideGlow()
	{
        //this.CardGlow.SetActive(false);
	}

	public void SetPrizeAlreadyAwarded()
	{
		this._PrizeAlreadyAwarded = true;
	}

	public void AwardPrize()
	{
		AwardPrizeBase awardPrizeBase = PrizeomaticAwarding.CreatePrize(this.CardReward);
		awardPrizeBase.AwardPrize();
		awardPrizeBase.SendMetricsEvent();
		if (!PrizeOMaticScriptingManager.IsScriptedMoment(PlayerProfileManager.Instance.ActiveProfile.NumberOfStargazerMoments))
		{
			awardPrizeBase.TakePrizeAwayFromProfile();
		}
		PrizeProgression.AddProgress(PrizeProgressionType.GachaCards, 1f);
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}

	public void TakePrizeAwayFromPrizePool()
	{
		if (PrizeOMaticScriptingManager.IsScriptedMoment(PlayerProfileManager.Instance.ActiveProfile.NumberOfStargazerMoments) || PrizeOMaticRewardsManager.PrizeIsReplacement())
		{
			return;
		}
		switch (this.CardReward)
		{
		case Reward.SportCarPart:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfSportsCarPiecesRemaining--;
			break;
		case Reward.DesiribleCarPart1:
		case Reward.DesiribleCarPart2:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfDesiribleCarPiecesRemaining--;
			break;
		case Reward.CommonCarPart1:
		case Reward.CommonCarPart2:
		case Reward.CommonCarPart3:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfCommonCarPiecesRemaining--;
			break;
		case Reward.FuelRefill:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfFuelRefillsRemaining--;
			break;
		case Reward.CashTiny:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfTinyCashRewardsRemaining--;
			break;
		case Reward.CashSmall:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfSmallCashRewardsRemaining--;
			break;
		case Reward.CashMedium:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfMediumCashRewardsRemaining--;
			break;
		case Reward.CashLarge:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfLargeCashRewardsRemaining--;
			break;
		case Reward.CashHuge:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfHugeCashRewardsRemaining--;
			break;
		case Reward.GoldTiny:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfTinyGoldRewardsRemaining--;
			break;
		case Reward.GoldSmall:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfSmallGoldRewardsRemaining--;
			break;
		case Reward.GoldMedium:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfMediumGoldRewardsRemaining--;
			break;
		case Reward.GoldLarge:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfLargeGoldRewardsRemaining--;
			break;
		case Reward.GoldHuge:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfHugeGoldRewardsRemaining--;
			break;
		case Reward.PipsOfFuel:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfFuelPipsRewardsRemaining--;
			break;
		case Reward.FreeUpgrade:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfUpgradeRewardsRemaining--;
			break;
		case Reward.RPTiny:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfTinyRPRewardsRemaining--;
			break;
		case Reward.RPSmall:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfSmallRPRewardsRemaining--;
			break;
		case Reward.RPMedium:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfMediumRPRewardsRemaining--;
			break;
		case Reward.RPLarge:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfLargeRPRewardsRemaining--;
			break;
		case Reward.RPHuge:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfHugeRPRewardsReamining--;
			break;
		case Reward.ProTuner:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfProTunerRewardsRemaining--;
			break;
		case Reward.N20Maniac:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfN20ManiacRewardsRemaining--;
			break;
		case Reward.TiresCrew:
			PlayerProfileManager.Instance.ActiveProfile.NumberOfTireCrewRewardsRemaining--;
			break;
		}
	}

    public void OnCardPress()
	{
		if (CurrentCardState == CardState.Dealt && !PrizeOMaticScreen.IsAwardingPrize && !this._PrizeAlreadyAwarded)
		{
			if (PrizeomaticController.Instance.IsBonusRewardsTurn)
			{
				PopUpManager.Instance.TryShowPopUp(new PopUp
				{
					Title = "TEXT_POPUP_PRIZEOMATIC_BONUS_TITLE",
					BodyText = "TEXT_POPUP_PRIZEOMATIC_BONUS_BODY",
					IsBig = true,
					BodyAlreadyTranslated = false,
					CancelAction = delegate
					{
						/*PrizeomaticController.Instance.numOfAttempts = 0;
						PrizeomaticController.Instance.numOfBonusAttempts = 0;
						PrizeomaticController.Instance.IsBonusRewardsTurn = false;
						PrizeOMaticScreen.screenState = PrizeOMaticScreen.ScreenState.Complete;*/
					},
					ConfirmAction = delegate
					{
						VideoForRewardsManager.Instance.SetExtraRewardResult(new VideoForRewardsManager.ExtraRewardResult()
						{
							ActionOnVideoFail = () =>
							{
								PrizeomaticController.Instance.numOfAttempts = 0;
								PrizeomaticController.Instance.numOfBonusAttempts = 0;
								PrizeOMaticScreen.screenState = PrizeOMaticScreen.ScreenState.Complete;
							},
							ActionOnVideoSuccess = ()=>GiveCardReward(),
							ActionOnVideoOfferReject = () =>
							{
								PrizeomaticController.Instance.numOfAttempts = 0;
								PrizeomaticController.Instance.numOfBonusAttempts = 0;
								PrizeOMaticScreen.screenState = PrizeOMaticScreen.ScreenState.Complete;
							},
							VideoFailRewardText = "",
							VideoSuccessRewardText = ""
						});
						VideoForRewardsManager.Instance.StartFlow(VideoForRewardConfiguration.eRewardID.VideoForBonusDailyPrize);
					},
					CancelText = "TEXT_POPUP_PRIZEOMATIC_BONUS_CANCEL",
					ConfirmText = "TEXT_POPUP_PRIZEOMATIC_BONUS_CONFIRM",
					GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
					ImageCaption = "TEXT_NAME_AGENT"
				}, PopUpManager.ePriority.Default, null);
			} else {
				GiveCardReward();
			}
		}
	}

    private void GiveCardReward()
    {
	    PrizeOMaticScreen.ScreenHasBeenTapped = true;
	    this.CardSelected = true;
	    if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_PrizeScreenCompleted)
	    {
		    Log.AnEvent(Events.TapPrizeCard);
	    }
    }

	private void PlayCardSound()
	{
        int num = UnityEngine.Random.Range(1, 5);
        string soundName = "PrizeomaticCard" + num;
        AudioManager.Instance.PlaySound(soundName, null);
	}

	public void SetSprite(Sprite sprite)
	{
		//GetComponent<Image>().sprite = sprite;
	}
	
	public void SetAdIcon(bool active)
	{
		if (CurrentCardState == CardState.CardFlipped)
			return;
			
		adIcon.SetActive(active);
	}

	public void SetColor(bool fadeedout)
	{
		if (fadeedout) {
			GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
			adIcon.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
		} else {
			GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			adIcon.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		}
	}
}
