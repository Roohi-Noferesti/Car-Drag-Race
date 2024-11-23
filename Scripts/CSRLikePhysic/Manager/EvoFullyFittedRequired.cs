using System;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

namespace EventPaneRestriction
{
	public class EvoFullyFittedRequired : IRestriction
	{
		private readonly RaceEventData eventData;

		public EvoFullyFittedRequired(RaceEventData currentEventData)
		{
			this.eventData = currentEventData;
		}

		public bool IsRestrictionActive()
		{
			CarGarageInstance carFromID = PlayerProfileManager.Instance.ActiveProfile.GetCarFromID(this.eventData.GetHumanCar());
		    var carinfo = CarDatabase.Instance.GetCar(carFromID.ID);
            return carFromID != null && carinfo && CarDatabase.Instance.GetCar(carFromID.CarDBKey).UsesEvoUpgrades() && !carFromID.GetIsAsFittedAsPossible(carinfo);
		}

		public void RestrictionButtonPressed()
		{
			PopUp popUp = new PopUp();
			popUp.Title = "TEXT_POPUPS_RESTRICTION_EVO_REQUIRED_BODY";
			popUp.BodyText = "TEXT_POPUPS_RESTRICTION_EVO_REQUIRED";
			popUp.ConfirmText = "TEXT_BUTTON_OK";
			popUp.CancelText = "TEXT_BUTTON_NO_THANKS";
			popUp.ConfirmAction = delegate
			{
                ScreenManager.Instance.PushScreen(ScreenID.Tuning);
			};
            popUp.GraphicPath = "InternationalPortraits.mechanic";
			popUp.ImageCaption = "TEXT_NAME_MECHANIC";
			popUp.IsCrewLeader = true;
			popUp.UseImageCaptionForCrewLeader = true;
			PopUp popup = popUp;
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		}

		public string RestrictionButtonText()
		{
			return "TEXT_BUTTON_NEXT";
		}

        public void AddRestrictionBubbleGraphics(EventPaneRestrictionBubble bubble)
        {
            string translatedBubbleMessage = LocalizationManager.GetTranslation("TEXT_RACE_EVENT_RESTRICTION_TYPE_EVO_REQUIRED");
            string textureName = "map_restriction_evo";
            bubble.AddRestriction(translatedBubbleMessage, textureName, true);
        }
	}
}
