using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;

namespace EventPaneRestriction
{
	public class UpgradeRequired : IRestriction
	{
		private class RestrictionHintPopup
		{
			public eRaceEventRestrictionType RestrictionType;

			public string Body;

			public eUpgradeType UpgradeToShow;
		}

		private static readonly List<UpgradeRequired.RestrictionHintPopup> RestrictionPopUps = new List<UpgradeRequired.RestrictionHintPopup>
		{
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_HORSEPOWER_MORE_THAN,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_ADD_HORSEPOWER_BODY"
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_HORSEPOWER_LESS_THAN,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_REMOVE_HORSEPOWER_BODY"
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_NITROUS_NEEDED,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_ADD_NITROUS_BODY",
				UpgradeToShow = eUpgradeType.NITROUS
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_NO_NITROUS_ALLOWED,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_REMOVE_NITROUS_BODY",
				UpgradeToShow = eUpgradeType.NITROUS
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_TYRES_NEEDED,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_ADD_TYRES_BODY",
				UpgradeToShow = eUpgradeType.TYRES
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_NO_TYRES_ALLOWED,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_REMOVE_TYRES_BODY",
				UpgradeToShow = eUpgradeType.TYRES
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_WEIGHT_LESS_THAN,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_REMOVE_WEIGHT_BODY",
				UpgradeToShow = eUpgradeType.BODY
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_WEIGHT_MORE_THAN,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_ADD_WEIGHT_BODY",
				UpgradeToShow = eUpgradeType.BODY
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_NO_TURBO_ALLOWED,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_REMOVE_TURBO_BODY",
				UpgradeToShow = eUpgradeType.TURBO
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_NO_UPGRADES_ALLOWED,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_REMOVE_ALL_BODY",
				UpgradeToShow = eUpgradeType.INVALID
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_PP_LESS_THAN,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_LESS_PP",
				UpgradeToShow = eUpgradeType.INVALID
			},
			new UpgradeRequired.RestrictionHintPopup
			{
				RestrictionType = eRaceEventRestrictionType.CAR_PP_MORE_THAN,
				Body = "TEXT_POPUPS_RESTRICTION_UPGRADE_MORE_PP",
				UpgradeToShow = eUpgradeType.INVALID
			}
		};

		private readonly RaceEventData raceData;

		public UpgradeRequired(RaceEventData currentRaceData)
		{
			this.raceData = currentRaceData;
		}

		public bool IsRestrictionActive()
		{
			CarPhysicsSetupCreator playerPhysicsSetup = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup;
			if (this.raceData.ForceUserInCar)
			{
				IEnumerable<CarGarageInstance> compatibleCars = this.raceData.GetCompatibleCars();
				if (compatibleCars.Any<CarGarageInstance>())
				{
					return false;
				}
			}
			return !this.raceData.DoesMeetRestrictions(playerPhysicsSetup);
		}

		public void RestrictionButtonPressed()
		{
			CarPhysicsSetupCreator currentCarPhys = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup;
			RaceEventRestriction firstFailedRestriction = this.raceData.Restrictions.FirstOrDefault((RaceEventRestriction q) => !q.DoesMeetRestriction(currentCarPhys));
			if (firstFailedRestriction == null)
			{
				return;
			}
			UpgradeRequired.RestrictionHintPopup restrictionHintPopup = UpgradeRequired.RestrictionPopUps.FirstOrDefault((UpgradeRequired.RestrictionHintPopup q) => q.RestrictionType == firstFailedRestriction.RestrictionType);
			if (restrictionHintPopup == null)
			{
				return;
			}
			UpgradeRequired.ShowUpgradeChangeNeededPopup(restrictionHintPopup, (int)firstFailedRestriction.Horsepower);
		}

		private static void ShowUpgradeChangeNeededPopup(UpgradeRequired.RestrictionHintPopup popupType, int pp)
		{
			PopUp popup = new PopUp
			{
				Title = "TEXT_POPUPS_RESTRICTION_UPGRADE_CHANGE_TITLE",
				BodyText = string.Format(LocalizationManager.GetTranslation(popupType.Body), pp),
				BodyAlreadyTranslated = true,
				ConfirmText = "TEXT_BUTTON_OK",
				CancelText = "TEXT_BUTTON_NO_THANKS",
				ConfirmAction = delegate
				{
					TuningScreen.ExternalStartScreenOn = popupType.UpgradeToShow;
                    ScreenManager.Instance.PushScreen(ScreenID.Tuning);
				},
                GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
				ImageCaption = "TEXT_NAME_RACE_OFFICIAL"
			};
			PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
		}

		public string RestrictionButtonText()
		{
			return "TEXT_BUTTON_NEXT";
		}

        public void AddRestrictionBubbleGraphics(EventPaneRestrictionBubble bubble)
        {
            CarPhysicsSetupCreator playerPhysicsSetup = PlayerProfileManager.Instance.ActiveProfile.PlayerPhysicsSetup;
            foreach (RaceEventRestriction current in this.raceData.Restrictions)
            {
                bool flag = current.DoesMeetRestriction(playerPhysicsSetup);
                bubble.AddRestriction(current.ToString(playerPhysicsSetup, string.Empty), current.GetTextureName(), !flag);
            }
        }
	}
}
