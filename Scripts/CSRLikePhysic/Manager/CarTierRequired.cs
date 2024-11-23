using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;

namespace EventPaneRestriction
{
	public class CarTierRequired : IRestriction
	{
		private readonly RaceEventData eventData;

		private RaceEventRestriction specificTierRestriction;

		public CarTierRequired(RaceEventData currentEventData)
		{
			this.eventData = currentEventData;
		}

	    public bool IsRestrictionActive()
	    {
	        CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
	        eCarTier currentTier = CarDatabase.Instance.GetCar(currentCar.CarDBKey).BaseCarTier;
            //eCarTier currentTier = currentCar.CurrentTier;
	        this.specificTierRestriction =
	            this.eventData.Restrictions.Find(
	                (RaceEventRestriction q) => q.RestrictionType == eRaceEventRestrictionType.CAR_CLASS);
	        //if (MultiplayerUtils.SelectedMultiplayerMode == MultiplayerMode.EVENT && this.specificTierRestriction == null)
	        //{
	        //    return false;
	        //}
	        eCarTier tierRestriction = this.getTierRestriction();
	        return !this.eventData.IsDailyBattle() && !this.eventData.IsRegulationRace() &&
	               !this.eventData.IsRaceTheWorldOrClubRaceEvent() && !this.eventData.IsFriendRaceEvent() &&
	               (!this.eventData.IsWorldTourRace() || this.specificTierRestriction != null) &&
	               currentTier != tierRestriction;
	    }

	    public void RestrictionButtonPressed()
		{
			eCarTier eventTier = this.getTierRestriction();
			List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
			IEnumerable<CarGarageInstance> source = from q in carsOwned
			where CarDatabase.Instance.GetCar(q.ID).BaseCarTier == eventTier
			select q;
			if (source.Any<CarGarageInstance>())
			{
				CarGarageInstance bestCar = (from q in source
				orderby q.CurrentPPIndex descending
				select q).First<CarGarageInstance>();
				CarTierRequired.ShowPopUpGoToOwnedCar(bestCar);
			}
			else
			{
				CarTierRequired.ShowPopUpGoToTierShowroom(eventTier);
			}
		}

		public string RestrictionButtonText()
		{
			return "TEXT_BUTTON_NEXT";
		}

        public void AddRestrictionBubbleGraphics(EventPaneRestrictionBubble bubble)
        {
            eCarTier tierRestriction = this.getTierRestriction();
            string arg = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(tierRestriction));
            string translatedBubbleMessage = string.Format(LocalizationManager.GetTranslation("TEXT_INCORRECT_TIER_BODY"), arg);
            bubble.AddRestriction(translatedBubbleMessage, "map_restriction_manufacturer", true);
        }

		private eCarTier getTierRestriction()
		{
			if (this.specificTierRestriction != null)
			{
				var classes =  this.specificTierRestriction.Classes;
                return EnumHelper.FromString<eCarTier>(classes);
            }
			return this.eventData.Parent.GetTierEvents().GetCarTier();
		}

	    public static void ShowPopUpGoToOwnedCar(CarGarageInstance bestCar)
		{
		    var baseTier = CarDatabase.Instance.GetCar(bestCar.ID).BaseCarTier;
            string arg = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(baseTier));
			string format = LocalizationManager.GetTranslation("TEXT_POPUPS_RESTRICTION_TIER_CAR_OWNED_BODY");
			string bodyText = string.Format(format, arg);
			PopUp popup = new PopUp
			{
				Title = "TEXT_POPUPS_RESTRICTION_TIER_CAR_OWNED_TITLE",
				BodyText = bodyText,
                IsBig = true,
				BodyAlreadyTranslated = true,
				ConfirmText = "TEXT_BUTTON_OK",
				CancelText = "TEXT_BUTTON_NO_THANKS",
				ConfirmAction = delegate
				{
                    MyCarScreen.OnLoadCar = bestCar.CarDBKey;
                    ScreenManager.Instance.PushScreen(ScreenID.CarSelect);
				},
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
				ImageCaption = "TEXT_NAME_AGENT"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		}

	    public static void ShowPopUpGoToTierShowroom(eCarTier eventTier)
		{
			string arg = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToString(eventTier));
			string format = LocalizationManager.GetTranslation("TEXT_POPUPS_RESTRICTION_TIER_CAR_BUY_BODY");
			string bodyText = string.Format(format, arg);
			PopUp popup = new PopUp
			{
                IsBig = true,
				Title = "TEXT_POPUPS_RESTRICTION_TIER_CAR_BUY_TITLE",
				BodyText = bodyText,
				BodyAlreadyTranslated = true,
				ConfirmText = "TEXT_BUTTON_OK",
				CancelText = "TEXT_BUTTON_NO_THANKS",
				ConfirmAction = delegate
				{
					ShowroomScreen.ShowScreenWithTierCarList(eventTier, false);
				},
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
				ImageCaption = "TEXT_NAME_AGENT"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		}
	}
}
