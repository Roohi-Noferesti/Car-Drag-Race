using DataSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BundleOfferPopupData
{
	public string TitleText = string.Empty;

	public string BundleOfferItem;

	public bool TimerActive = true;

    public string BundleOfferValidityDuration;

    public TimeSpan BundleOfferValidityDurationTimeSpan;

	public List<IBundleOfferWidgetInfo> WidgetInfo = new List<IBundleOfferWidgetInfo>();

	public string ConfirmButtonText = "TEXT_BUTTON_OK";

	public EligibilityRequirements OkButtonRequirement = EligibilityRequirements.CreateAlwaysEligible();

	public string CancelButtonText = "TEXT_BUTTON_CANCEL";

	public EligibilityRequirements CancelButtonRequirement = EligibilityRequirements.CreateNeverEligible();

	public bool CheckForShowOnlyOnce;

	public bool IsBodyTranslated = true;

	public EligibilityRequirements PopupRequirements = EligibilityRequirements.CreateAlwaysEligible();

	public List<PopupDataButtonAction> ConfirmActions = new List<PopupDataButtonAction>();

	public List<PopupDataButtonAction> CancelActions = new List<PopupDataButtonAction>();

	public bool UseStarterPackPopup;

	//public string StarterPackItem1 = string.Empty;

	//public string StarterPackItem2 = string.Empty;
	
	public string[] _StarterPackItem1;

	public string[] _StarterPackItem2;

	public string StarterPackItem1
	{
		get
		{
			if(ProductManager.Instance.discount < _StarterPackItem1.Length)
				return _StarterPackItem1[ProductManager.Instance.discount];
			else
				return String.Empty;
		}
		set { _StarterPackItem1[ProductManager.Instance.discount] = value; }
	}
	
	public string StarterPackItem2
	{
		get
		{
			if(ProductManager.Instance.discount < _StarterPackItem2.Length)
				return _StarterPackItem2[ProductManager.Instance.discount];
			else
				return String.Empty;
		}
		set { _StarterPackItem2[ProductManager.Instance.discount] = value; }
	}

	public bool HasBeenShown
	{
		get;
		set;
	}

	public void Initialise()
	{
        TimeSpan.TryParse(BundleOfferValidityDuration, out BundleOfferValidityDurationTimeSpan);
		this.OkButtonRequirement.Initialise();
		this.CancelButtonRequirement.Initialise();
		this.PopupRequirements.Initialise();
		foreach (PopupDataButtonAction current in this.ConfirmActions)
		{
			current.Initialise();
		}
		foreach (PopupDataButtonAction current2 in this.CancelActions)
		{
			current2.Initialise();
		}
	}

	public virtual bool IsEligible(IGameState gs)
	{
		return ((this.CheckForShowOnlyOnce && !this.HasBeenShown) || !this.CheckForShowOnlyOnce) && this.PopupRequirements.IsEligible(gs);
	}

	public PopUp GetPopUp(int ID, PopUpButtonAction confirmCallback = null, PopUpButtonAction cancelCallback = null)
	{
		PlayerPrefs.SetString("mode", "normal");
		PopUp popUp = null;
		if (this.UseStarterPackPopup)
		{
			popUp = this.GetStarterPackPopup();
		}
		else
		{
			popUp = this.GetBundleOfferPopup();
		}
        BundleOfferController.Instance.SetupOffer(ID, BundleOfferValidityDurationTimeSpan, this.BundleOfferItem);
		return this.GetPopupActions(ref popUp, confirmCallback, cancelCallback);
	}

	public PopUp GetPopUpByID(int ID, PopUpButtonAction confirmCallback = null, PopUpButtonAction cancelCallback = null)
	{
		PlayerPrefs.SetString("mode", "cheat");
		PopUp popUp = null;
		if (this.UseStarterPackPopup)
		{
			popUp = this.GetStarterPackPopupCheat();
		}
		else
		{
			popUp = this.GetBundleOfferPopCheat();
		}
		return this.GetPopupAction(ref popUp, confirmCallback, cancelCallback);
	}

	private PopUp GetBundleOfferPopup()
	{
		return new PopUp
		{
			Title = this.TitleText,
			BodyText = string.Empty,
			BodyAlreadyTranslated = this.IsBodyTranslated,
			IsIAPBundle = true,
			ShouldCentreIfOverflowsScreenBottom = true
		};
	}

	private PopUp GetBundleOfferPopCheat()
	{
		PopUp popup = new PopUp
		{
			Title = this.TitleText,
			BodyText = string.Empty,
			BodyAlreadyTranslated = this.IsBodyTranslated,
			IsIAPBundle = true,
			ShouldCentreIfOverflowsScreenBottom = true,
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		return popup;
	}

	private PopUp GetStarterPackPopup()
	{
		return new PopUp
		{
			Title = this.TitleText,
			BodyText = string.Empty,
			BodyAlreadyTranslated = this.IsBodyTranslated,
			IsStarterPack = true,
            //BundledGraphicPath = string.Empty,
			StarterPackItem1 = this.StarterPackItem1,
			StarterPackItem2 = this.StarterPackItem2,
			StarterPackOfferItem = this.BundleOfferItem,
            StarterPackValidityDuration = BundleOfferValidityDurationTimeSpan
		};
	}

	private PopUp GetStarterPackPopupCheat()
	{
		PopUp popup = new PopUp
		{
			Title = this.TitleText,
			BodyText = string.Empty,
			BodyAlreadyTranslated = this.IsBodyTranslated,
			IsStarterPack = true,
			IsIAPBundle = true,
			ShouldCentreIfOverflowsScreenBottom = true,
			StarterPackItem1 = this.StarterPackItem1,
			StarterPackItem2 = this.StarterPackItem2,
			StarterPackOfferItem = this.BundleOfferItem,
		};
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		return popup;
	}

	public PopUp GetPopupActions(ref PopUp thePopup, PopUpButtonAction confirmCallback = null, PopUpButtonAction cancelCallback = null)
	{
		IGameState gameState = new GameStateFacade();
		if (this.OkButtonRequirement.IsEligible(gameState))
		{
			thePopup.ConfirmText = this.ConfirmButtonText;
			thePopup.ConfirmAction = confirmCallback;
		}
		if (this.CancelButtonRequirement.IsEligible(gameState))
		{
			thePopup.CancelText = this.CancelButtonText;
			thePopup.CancelAction = cancelCallback;
		}
		if (thePopup.ConfirmAction == null)
		{
			thePopup.ConfirmAction = new PopUpButtonAction(this.GetPopupConfirmAction);
		}
		if (thePopup.CancelAction == null)
		{
			thePopup.CancelAction = new PopUpButtonAction(this.GetPopupCancelAction);
		}
		return thePopup;
	}

	public PopUp GetPopupAction(ref PopUp thePopup, PopUpButtonAction confirmCallback = null,
		PopUpButtonAction cancelCallback = null)
	{
		thePopup.ConfirmText = this.ConfirmButtonText;
		thePopup.ConfirmAction = confirmCallback;
		thePopup.CancelText = this.CancelButtonText;
		thePopup.CancelAction = cancelCallback;
		
		if (thePopup.ConfirmAction == null)
		{
			thePopup.ConfirmAction = new PopUpButtonAction(this.GetPopupConfirmAction);
		}
		if (thePopup.CancelAction == null)
		{
			thePopup.CancelAction = new PopUpButtonAction(this.GetPopupCancelAction);
		}
		return thePopup;
	}

	private void GetPopupConfirmAction()
	{
		foreach (PopupDataButtonAction current in this.ConfirmActions)
		{
			current.Execute();
		}
	}

	private void GetPopupCancelAction()
	{
		foreach (PopupDataButtonAction current in this.CancelActions)
		{
			current.Execute();
		}
	}
}
