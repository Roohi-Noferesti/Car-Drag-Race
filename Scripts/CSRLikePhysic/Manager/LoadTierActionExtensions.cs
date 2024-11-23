using DataSerialization;
using System;
using System.Linq;

public static class LoadTierActionExtensions
{
	public static void Initialise(this LoadTierAction lta)
	{
		lta.entryPopups.ForEach(delegate(PopupData p)
		{
			p.Initialise();
		});
	}

	public static PopUp GetPopUp(this LoadTierAction lta, IGameState gameState)
	{
		PopupData popupData = lta.entryPopups.FirstOrDefault((PopupData p) => p.IsEligible(gameState));
		if (popupData != null)
		{
			return popupData.GetPopup(null, null);
		}
		return null;
	}
}
