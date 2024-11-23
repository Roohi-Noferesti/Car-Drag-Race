using System;
using System.Collections.Generic;
using UnityEngine;

public class PopUp
{
    public enum NarrativeType
    {
        None,
        Normal,
        First
    }
	public PopUpID ID = PopUpID.Default;

    public string TargetGameObjectName;
    public string[] TargetGameObjectNames;

	public string Title = string.Empty;

	public string BodyText = string.Empty;

	public string CancelText = string.Empty;

	public string ConfirmText = string.Empty;

    public string[] Dialogs;

	public PopUpButtonAction CancelAction;

	public PopUpButtonAction ConfirmAction;

	public PopUpButtonAction SocialAction;

	public PopUpButtonAction CloseAction;

	public PopUpButtonAction CloseActionHACKFORBILLBOARDMAP;

	public PopUpButtonAction KillAction;

	public float BackgroundOffsetZ;

	public bool hasCloseButton;

	public bool IsBig;

    public bool IsVeryBig;

	public bool IsCrewLeader;

	public bool IsSocial;

	public int BossTier = -1;

    public Texture2D Graphic;

    public Texture2D ItemGraphic;

    public string GraphicPath {
	    get
	    {
		    if (m_GraphicPath.ToLower()=="advisor_agent.agent") {
			    return m_GraphicPath + (BasePlatform.ActivePlatform.ShouldShowFemaleHair ? "_World" : "");
		    } else if (m_GraphicPath.ToLower().Contains("crewportraitstier2") && !m_GraphicPath.ToLower().Contains("_world")) {
			    return m_GraphicPath + (BasePlatform.ActivePlatform.ShouldShowFemaleHair ? "_World" : "");
		    } else {
			    return m_GraphicPath;
		    }
	    }
	    set
	    {
		    m_GraphicPath = value;
	    }
    }

    private string m_GraphicPath = string.Empty;

    public string ItemGraphicPath;

	public string ImageCaption = string.Empty;

	public bool UseImageCaptionForCrewLeader;

	public bool TitleAlreadyTranslated;

	public bool BodyAlreadyTranslated;

	public bool CancelTextAlreadyTranslated;

	public bool ConfirmTextAlreadyTranslated;

	public bool ShouldCoverNavBar;

	public bool ShouldCentreIfOverflowsScreenBottom;

	public string TwitterRewardText = string.Empty;

	public string FacebookRewardText = string.Empty;

	public string StarterPackItem1 = string.Empty;

	public string StarterPackItem2 = string.Empty;

	public string StarterPackOfferItem = string.Empty;

	public TimeSpan StarterPackValidityDuration = TimeSpan.Zero;

	public int DelayKillPopupByFrames;

    public bool DelayKillPopupByAnimator;

	public bool IsCard;

    public bool IsConnection;

	public Reward CardRewardType;

	public string CarRewardCarDBKey;

	public bool HidePopup;

	public string CustomPrefab;

	public bool IsLegal;

	public bool IsStarterPack;

	public string PrivacyPolicyURL = string.Empty;

	public string TermsOfServiceURL = string.Empty;

	public bool IsDynamicImage;

	public string DynamicImageAssetID = string.Empty;

	public bool AllowBackButton = true;

	public bool IsWaitSpinner;

	public bool IsMiniStore;

	public bool IsRatePopup;

    public MiniStoreLayout MiniStoreLayoutToShow;

	public int MiniStoreAffordableGold;

    public MiniStoreController.MetricData MiniStoreMetricData;

	public PopUpButtonAction MiniStoreUseGoldAction;

	public PopUpButtonAction MiniStoreBuyButtonAction;

	public PopUpButtonAction MiniStoreShopButtonAction;

	public PopUpButtonAction MiniStoreCloseButtonAction;

	public bool IsSpotPrize;

    public SpotPrizeType SpotPrizeType;

	public bool IsIAPBundle;

    public List<ButtonDetails> Buttons;

	public bool IsProfileRestore;

	public bool IsAgeVerification;
	
	public bool IsPrivacyPolicy;

    public PlayerProfile[] profiles;

    public Color ConfirmColor = new Color(1, 1, 1, 1);

    public bool UseConfirmColor;

    public bool UseCancelColor;

    public Color CancelColor = new Color(1, 1, 1, 1);
}
