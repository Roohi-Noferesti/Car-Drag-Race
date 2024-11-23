using DataSerialization;
using Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;

[Serializable]
public class MultiplayerEventData
{
	public int ID;

	public DateTime StartTime = DateTime.MaxValue;

	public DateTime EndTime = DateTime.MaxValue;

	public float RPMultiplier = 1f;

	public List<RaceEventRestriction> Restrictions = new List<RaceEventRestriction>();

	public PrizeProgressionData PrizeProgression = new PrizeProgressionData();

	public List<SpotPrizeData> SpotPrizes = new List<SpotPrizeData>();

	public MultiplayerModeTheme Theme = new MultiplayerModeTheme();

	public PopupData IntroPopup = PopupData.CreateDontShowPopupData();

	public DifficultySettings EventDifficultySettings = new DifficultySettings();

	public string TexturePack
	{
		get
		{
			return "MultiplayerEvent" + this.ID;
		}
	}

	public void Initialise()
	{
		foreach (RaceEventRestriction current in this.Restrictions)
		{
			current.Initialise();
		}
		this.SpotPrizes.EnumerateEach(delegate(SpotPrizeData sp, int i)
		{
			sp.Initialise(this.ID, i);
		});
	}

	public string GetTitle()
	{
		return "TEXT_MULTIPLAYER_EVENT_TITLE_" + this.ID;
	}

	public string GetDescription()
	{
		return this.Theme.Description;
	}

	public string GetNarrative()
	{
		return "TEXT_MULTIPLAYER_EVENT_NARRATIVE_" + this.ID;
	}

	public bool IsActive()
	{
		if (ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			DateTime dateTime = ServerSynchronisedTime.Instance.GetDateTime();
			return dateTime >= this.StartTime && dateTime < this.EndTime;
		}
		return false;
	}

	private string GetServerTimeNetworkStatus()
	{
		if (ServerSynchronisedTime.Instance.RequestInProgress)
		{
			return LocalizationManager.GetTranslation("TEXT_SERVER_TIME_SYNCHRONISING");
		}
		return "--";
	}

	public string GetTimeRemainingString(bool addRemaining = false)
	{
		if (!ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			return this.GetServerTimeNetworkStatus();
		}
		DateTime dateTime = ServerSynchronisedTime.Instance.GetDateTime();
		if (dateTime < this.StartTime)
		{
			return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_EVENT_NOT_STARTED");
		}
		if (dateTime > this.EndTime)
		{
			return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_EVENT_HAS_ENDED");
		}
        return GetNiceTimeSpan(this.EndTime - dateTime, addRemaining);//LocalisationManager.GetNiceTimeSpan(this.EndTime - dateTime, addRemaining);
	}

    private string GetNiceTimeSpan(TimeSpan timeSpan,bool addRemaining)
    {
        return timeSpan.ToString();
    }

	public string GetTimeToStartString()
	{
		if (!ServerSynchronisedTime.Instance.ServerTimeValid)
		{
			return this.GetServerTimeNetworkStatus();
		}
		DateTime dateTime = ServerSynchronisedTime.Instance.GetDateTime();
		if (dateTime > this.StartTime)
		{
			return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_EVENT_STARTED");
		}
		if (dateTime > this.EndTime)
		{
			return LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_EVENT_HAS_ENDED");
		}
        return GetNiceTimeSpan(this.EndTime - dateTime, false);//LocalisationManager.GetNiceTimeSpan(this.StartTime - dateTime, false);
	}

	public void ShowIntroPopup(PopUpButtonAction callback)
	{
		PopUp popup = this.IntroPopup.GetPopup(null, null);
		popup.Title = "TEXT_MULTIPLAYER_EVENT_NARRATIVE_TITLE_" + this.ID;
		popup.BodyText = "TEXT_MULTIPLAYER_EVENT_NARRATIVE_BODY_" + this.ID;
		popup.BodyAlreadyTranslated = false;
		PopUp expr_4C = popup;
		expr_4C.ConfirmAction = (PopUpButtonAction)Delegate.Combine(expr_4C.ConfirmAction, callback);
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public float GetMaxMilestone()
	{
		return (from sp in this.SpotPrizes
		select sp.Milestone).DefaultIfEmpty(0f).Max();
	}

	public bool DoesMeetRestrictions(CarPhysicsSetupCreator zCarPhysicsSetup)
	{
		foreach (RaceEventRestriction current in this.Restrictions)
		{
			if (!current.DoesMeetRestriction(zCarPhysicsSetup))
			{
				return false;
			}
		}
		return true;
	}

	public RaceEventRestriction.RestrictionMet DoesMeetRestrictions(CarInfo info)
	{
		RaceEventRestriction.RestrictionMet restrictionMet = RaceEventRestriction.RestrictionMet.TRUE;
		foreach (RaceEventRestriction current in this.Restrictions)
		{
			RaceEventRestriction.RestrictionMet restrictionMet2 = current.DoesMeetRestrictionNaive(info);
			if (restrictionMet2 == RaceEventRestriction.RestrictionMet.FALSE)
			{
				restrictionMet = RaceEventRestriction.RestrictionMet.FALSE;
			}
			if (restrictionMet2 == RaceEventRestriction.RestrictionMet.UNKNOWN && restrictionMet != RaceEventRestriction.RestrictionMet.FALSE)
			{
				restrictionMet = RaceEventRestriction.RestrictionMet.UNKNOWN;
			}
		}
		return restrictionMet;
	}

	public void AwardNextSpotPrize(Queue<SpotPrizeData> queue)
	{
		if (queue.Count > 0)
		{
			SpotPrizeData spotPrize = queue.Dequeue();
			PopUp pop = new PopUp
			{
				Title = "TEXT_PRIZE_CONGRATULATIONS",
				BodyText = spotPrize.GetPopupBody(),
				BodyAlreadyTranslated = true,
				ConfirmText = "TEXT_BUTTON_GETPRIZE",
				IsSpotPrize = true,
				SpotPrizeType = spotPrize.PrizeTypeEnum,
				ConfirmAction = delegate
				{
					spotPrize.AwardPrize();
					PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
					this.AwardNextSpotPrize(queue);
				}
			};
			int num = this.SpotPrizes.Count((SpotPrizeData p) => p.PrizeAwarded) + 1;
			Dictionary<Parameters, string> data = new Dictionary<Parameters, string>
			{
				{
					Parameters.PrizeLvl,
					num.ToString()
				},
				{
					Parameters.MPEvtID,
					this.ID.ToString()
				},
				{
					Parameters.MPEvtRace,
					MultiplayerEvent.Saved.GetRacesCompleted().ToString()
				},
				{
					Parameters.SptPrize,
					spotPrize.PrizeType
				}
			};
			Log.AnEvent(Events.SpotPrize, data);
			PopUpManager.Instance.ForcePopup(pop, PopUpManager.ePriority.SystemUrgent, null);
		}
	}
}
