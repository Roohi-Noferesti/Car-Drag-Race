using System;
using I2.Loc;
using KingKodeStudio;

namespace EventPaneRestriction
{
	public class GasRequired : IRestriction
	{
		private readonly RaceEventData raceData;

		public GasRequired(RaceEventData currentRaceData)
		{
			this.raceData = currentRaceData;
		}

		public bool IsRestrictionActive()
		{
            int fuel = FuelManager.Instance.GetFuel();
            int fuelCostForEvent = GameDatabase.Instance.Currencies.GetFuelCostForEvent(raceData);
            return fuel < fuelCostForEvent;
		}

		public void RestrictionButtonPressed()
		{
			PopUp moreFuelPopup = GasRequired.GetMoreFuelPopup();
			PopUpManager.Instance.TryShowPopUp(moreFuelPopup, PopUpManager.ePriority.Default, null);
		}

		public string RestrictionButtonText()
		{
			return "TEXT_BUTTON_NEXT";
		}

        public void AddRestrictionBubbleGraphics(EventPaneRestrictionBubble bubble)
        {
            bubble.AddRestriction(LocalizationManager.GetTranslation("TEXT_EVENT_PANE_FUEL_EMPTY"), "map_restriction_fuel", true);
        }

		private static PopUp GetMoreFuelPopup()
		{
			PopUp popUp = new PopUp();
			popUp.Title = "TEXT_GET_MORE_FUEL_TITLE";
			popUp.BodyText = "TEXT_GET_MORE_FUEL_BODY";
			popUp.ConfirmText = "TEXT_BUTTON_OK";
			popUp.CancelText = "TEXT_BUTTON_NO_THANKS";
		    popUp.IsBig = true;
			popUp.ConfirmAction = delegate
			{
                ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
			};
            popUp.GraphicPath = PopUpManager.Instance.graphics_mechanicPrefab;
			popUp.ImageCaption = "TEXT_NAME_MECHANIC";
			return popUp;
		}
	}
}
