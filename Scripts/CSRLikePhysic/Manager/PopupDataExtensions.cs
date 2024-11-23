using System.Linq;
using DataSerialization;
using UnityEngine;

public static class PopupDataExtensions
{
	public static void Initialise(this PopupData pd)
	{
		pd.OkButtonRequirement.Initialise();
		pd.CancelButtonRequirement.Initialise();
		pd.PopupRequirements.Initialise();
		foreach (PopupDataButtonAction current in pd.ConfirmActions)
		{
			current.Initialise();
		}
		foreach (PopupDataButtonAction current2 in pd.CancelActions)
		{
			current2.Initialise();
		}
	}

	public static bool IsEligible(this PopupData pd, IGameState gs)
	{
	    return pd.PopupRequirements.IsEligible(gs);
	}

	public static PopUp GetPopup(this PopupData pd, PopUpButtonAction confirmCallback = null, PopUpButtonAction cancelCallback = null)
	{
		PopUp popUp = new PopUp
		{
			Title = pd.TitleText,
			BodyText = pd.GetPopupBodyText(),
            Dialogs = pd.GetPopupBodyTextAll(),
            BodyAlreadyTranslated = true,//pd.IsBodyTranslated,
			IsCrewLeader = pd.SetupForCrewLeader,
            IsBig = pd.IsBig,
			UseImageCaptionForCrewLeader = pd.SetupForCrewLeader,
            GraphicPath = pd.CharacterTexture,
			ImageCaption = pd.CharacterName
		};
		return pd.GetPopupActions(ref popUp, confirmCallback, cancelCallback);
	}

	public static PopUp GetPopupActions(this PopupData pd, ref PopUp thePopup, PopUpButtonAction confirmCallback = null, PopUpButtonAction cancelCallback = null)
	{
		IGameState gameState = new GameStateFacade();
		if (pd.OkButtonRequirement.IsEligible(gameState))
		{
			thePopup.ConfirmText = pd.ConfirmButtonText;
			thePopup.ConfirmAction = delegate
			{
				if (confirmCallback != null)
				{
					confirmCallback();
				}
				pd.ExecutePopupConfirmActions();
			};
		}
		if (pd.CancelButtonRequirement.IsEligible(gameState))
		{
			thePopup.CancelText = pd.CancelButtonText;
			thePopup.CancelAction = delegate
			{
				if (cancelCallback != null)
				{
					cancelCallback();
				}
				pd.ExecutePopupCancelActions();
			};
		}
		return thePopup;
	}

	private static string GetPopupBodyText(this PopupData pd)
	{
		if (pd.BodyText.Count == 0)
		{
			return string.Empty;
		}
		if (pd.BodyText.Count == 1)
		{
			return pd.BodyText[0].GetFormatString();
		}
		int index = 0;
		if (pd.BodyText.Count > 1)
		{
			index = Random.Range(0, pd.BodyText.Count);
		}
		return pd.BodyText[index].GetFormatString();
	}

    private static string[] GetPopupBodyTextAll(this PopupData pd)
    {
        return pd.BodyText.Select(s=>s.GetFormatString()).ToArray();
    }

	private static void ExecutePopupConfirmActions(this PopupData pd)
	{
		foreach (PopupDataButtonAction current in pd.ConfirmActions)
		{
			current.Execute();
		}
	}

	private static void ExecutePopupCancelActions(this PopupData pd)
	{
		foreach (PopupDataButtonAction current in pd.CancelActions)
		{
			current.Execute();
		}
	}
}
