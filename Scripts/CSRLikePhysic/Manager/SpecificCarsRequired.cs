using DataSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;

namespace EventPaneRestriction
{
	public class SpecificCarsRequired : IRestriction
	{
		private readonly RaceEventData eventData;

		private readonly List<eRaceEventRestrictionType> carSpecificRestrictions = new List<eRaceEventRestrictionType>
		{
			eRaceEventRestrictionType.CAR_MANUFACTURER,
			eRaceEventRestrictionType.CAR_DRIVE_WHEELS,
			eRaceEventRestrictionType.CAR_MODEL,
			eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN,
			eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN,
			eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN,
			eRaceEventRestrictionType.CAR_HORSEPOWER_MORE_THAN,
			eRaceEventRestrictionType.CAR_PP_LESS_THAN,
			eRaceEventRestrictionType.CAR_PP_MORE_THAN,
			eRaceEventRestrictionType.CAR_PRO
		};

		public SpecificCarsRequired(RaceEventData currentEventData)
		{
			this.eventData = currentEventData;
		}

		private IEnumerable<RaceEventRestriction> GetFailedRestrictions()
		{
			IEnumerable<RaceEventRestriction> source = from q in this.eventData.Restrictions
			where this.carSpecificRestrictions.Contains(q.RestrictionType)
			select q;
			string currentlySelectedCarDBKey = PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey;
			CarInfo currentCarInfo = CarDatabase.Instance.GetCar(currentlySelectedCarDBKey);
			return from q in source
			where q.DoesMeetRestrictionNaive(currentCarInfo) == RaceEventRestriction.RestrictionMet.FALSE
			select q;
		}

		public bool IsRestrictionActive()
		{
			if (CarDataDefaults.IsBossCar(PlayerProfileManager.Instance.ActiveProfile.CurrentlySelectedCarDBKey))
			{
				CarPhysicsSetupCreator playerPhysicsSetup = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup;
				return !this.eventData.DoesMeetRestrictions(playerPhysicsSetup);
			}
			if (this.eventData.ForceUserInCar)
			{
				IEnumerable<CarGarageInstance> compatibleCars = this.eventData.GetCompatibleCars();
				if (compatibleCars.Any<CarGarageInstance>())
				{
					return false;
				}
			}
			return this.GetFailedRestrictions().Any<RaceEventRestriction>();
		}

		public void RestrictionButtonPressed()
		{
			IEnumerable<CarGarageInstance> compatibleCars = this.eventData.GetCompatibleCars();
			if (compatibleCars.Any<CarGarageInstance>())
			{
				IOrderedEnumerable<CarGarageInstance> source = from q in compatibleCars
				orderby q.CurrentPPIndex descending
				select q;
				SpecificCarsRequired.ShowCompatibleCarOwnedPopUp(source.First<CarGarageInstance>());
				return;
			}
			IEnumerable<CarInfo> selectedTierCars;
			if (this.eventData.IsWorldTourRace())// || MultiplayerUtils.SelectedMultiplayerMode != MultiplayerMode.NONE)
			{
				selectedTierCars = CarDatabase.Instance.GetAllCars();
			}
			else
			{
				eCarTier carTier = this.eventData.Parent.GetTierEvents().GetCarTier();
				selectedTierCars = CarDatabase.Instance.GetCarsOfTier(carTier);
			}
			IEnumerable<CarInfo> availableCars = (from q in selectedTierCars
			where ShowroomScreen.VisibleInShowroom(q)
			select q).ToList();
			IEnumerable<CarInfo> aproavedCars = (from q in availableCars
			where this.eventData.DoesMeetRestrictions(q) == RaceEventRestriction.RestrictionMet.TRUE
			select q).ToList();
			if (!aproavedCars.Any<CarInfo>())
			{
				aproavedCars = from q in availableCars
				where this.eventData.DoesMeetRestrictions(q) != RaceEventRestriction.RestrictionMet.FALSE
				select q;
			}
			if (aproavedCars.Any<CarInfo>())
			{
				if (this.eventData.ForceUserInCar)
				{
					ShowroomScreen.ShowScreenWithPresetCarList(aproavedCars);
				}
				else
				{
				    SpecificCarsRequired.ShowCompatibleCarShowroomPopUp(aproavedCars, null);// TierXManager.Instance.GetRestrictionHelpOverride(this.eventData.EventID));
				}
			}
			else
			{
				SpecificCarsRequired.ShowWinProCarsInRTWPopup();
			}
		}

		public string RestrictionButtonText()
		{
			return "TEXT_BUTTON_NEXT";
		}

        public void AddRestrictionBubbleGraphics(EventPaneRestrictionBubble bubble)
        {
            CarPhysicsSetupCreator playerPhysicsSetup = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup;
            foreach (RaceEventRestriction current in this.eventData.Restrictions)
            {
                bool active = !current.DoesMeetRestriction(playerPhysicsSetup);
                string translatedBubbleMessage = current.ToString(playerPhysicsSetup, string.Empty);
                bubble.AddRestriction(translatedBubbleMessage, current.GetTextureName(), active);
            }
        }

		private static void ShowCompatibleCarShowroomPopUp(IEnumerable<CarInfo> cars, RestrictionRaceHelperOverride special)
		{
			string bodyText = "TEXT_POPUPS_RESTRICTION_CAR_SHOWROOM_BODY";
			string bundledGraphicPath = PopUpManager.Instance.graphics_agentPrefab;
			string imageCaption = "TEXT_NAME_AGENT";
			bool flag = false;
			if (special != null)
			{
				if (!string.IsNullOrEmpty(special.BodyText))
				{
					bodyText = special.BodyText;
				}
				if (!string.IsNullOrEmpty(special.BundledGraphicPath))
				{
					bundledGraphicPath = special.BundledGraphicPath;
					flag = special.IsCrewLeader;
				}
				if (!string.IsNullOrEmpty(special.ImageCaption))
				{
					imageCaption = special.ImageCaption;
				}
			}
			PopUp popup = new PopUp
			{
				Title = "TEXT_POPUPS_RESTRICTION_CAR_SHOWROOM_TITLE",
				BodyText = bodyText,
				ConfirmText = "TEXT_BUTTON_OK",
				CancelText = "TEXT_BUTTON_NO_THANKS",
				ConfirmAction = delegate
				{
					ShowroomScreen.ShowScreenWithPresetCarList(cars);
				},
                GraphicPath = bundledGraphicPath,
				ImageCaption = imageCaption,
				IsCrewLeader = flag,
				UseImageCaptionForCrewLeader = flag
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		}

		private static void ShowCompatibleCarOwnedPopUp(CarGarageInstance car)
		{
			string arg = LocalizationManager.GetTranslation(CarDatabase.Instance.GetCar(car.CarDBKey).ShortName);
			string bodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_RESTRICTION_CAR_OWNED_BODY"), arg);
			PopUp popup = new PopUp
			{
				Title = "TEXT_POPUPS_RESTRICTION_CAR_OWNED_TITLE",
				BodyText = bodyText,
				BodyAlreadyTranslated = true,
				ConfirmText = "TEXT_BUTTON_OK",
				CancelText = "TEXT_BUTTON_NO_THANKS",
				ConfirmAction = delegate
				{
                    MyCarScreen.OnLoadCar = car.CarDBKey;
				    ScreenManager.Instance.PushScreen(ScreenID.CarSelect);
                    //ScreenManager.Instance.PushScreen(ScreenID.CarSelect);
				},
                GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
				ImageCaption = "TEXT_NAME_RACE_OFFICIAL"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		}

		private static void ShowWinProCarsInRTWPopup()
		{
			PopUp popUp = new PopUp();
			popUp.Title = "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PRIZE_LIST_TITLE";
			popUp.BodyText = "TEXT_POPUPS_MULTIPLAYER_GET_PRO";
			popUp.ConfirmText = "TEXT_BUTTON_OK";
			popUp.CancelText = "TEXT_BUTTON_NO_THANKS";
			popUp.ConfirmAction = delegate
			{
                ScreenManager.Instance.PopScreen();
                //ScreenManager.Instance.PopScreen();
			};
            popUp.GraphicPath = PopUpManager.Instance.graphics_agentPrefab;
			popUp.ImageCaption = "TEXT_NAME_AGENT";
			PopUp popup = popUp;
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		}
	}
}
