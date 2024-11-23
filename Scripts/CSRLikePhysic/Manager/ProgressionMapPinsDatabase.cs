using System;
using System.Collections.Generic;
using KingKodeStudio;
using UnityEngine;

public class ProgressionMapPinsDatabase : ConfigurationAssetLoader
{
	public ProgressionMapPinsConfiguration Configuration
	{
		get;
		private set;
	}

	public ProgressionMapPinsDatabase() : base(GTAssetTypes.configuration_file, "ProgressionMapPinsConfiguration")
	{
		this.Configuration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject obj)
	{
	    this.Configuration = (ProgressionMapPinsConfiguration) obj;//JsonConverter.DeserializeObject<ProgressionMapPinsConfiguration>(assetDataString);
		foreach (ProgressionMapPinsData current in this.Configuration.MapPins)
		{
			current.Initialise();
		}
	}

	public void Populate(CareerModeMapEventSelect EventSelect)
	{
		EventSelect.RemoveHighlight();
		IGameState gs = new GameStateFacade();
		//CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;

	    var eligiblePins = this.Configuration.MapPins.FindAll(p => p.ShowingRequirements.IsEligible(gs));
#if UNITY_EDITOR
	    if (GameDatabase.Instance.EventDebugConfiguration.ShowAllMapPins)
	    {
	        eligiblePins = this.Configuration.MapPins;
        }
#else

        //  eligiblePins = this.Configuration.MapPins;

#endif
        foreach (ProgressionMapPinsData current in eligiblePins)
	    {
	        switch (current.EventType)
	        {
	            case ProgressionMapPinEventType.REGULATION_RACES:
                    EventSelect.PopulateRegulationRaces(current.Tier);
	                break;
	            case ProgressionMapPinEventType.CREW_RACES:
                    EventSelect.PopulateCrewRaces(current.Tier, current.OverrideHighlightRequirements.IsEligible(gs));
	                break;
	            case ProgressionMapPinEventType.LADDER_RACES:
                    EventSelect.PopulateTutorialLadderRaces(current.Tier, true);
	                //if (careerModeMapScreen != null)
	                //{
	                //    careerModeMapScreen.CheckForMapPinsBubble();
	                //}
	                break;
                case ProgressionMapPinEventType.MECHANIC:
                    EventSelect.CreateCustomPin(CustomPin.CustomType.Mechanic, current.EventType,current.Name);
	                //if (careerModeMapScreen != null)
	                //{
	                //    careerModeMapScreen.CheckForMapPinsBubble();
	                //}
	                break;
                case ProgressionMapPinEventType.DAILY_BATTLE_RACES:
                    EventSelect.PopulateDailyBattle(current.Tier, true);
	                //if (careerModeMapScreen != null)
	                //{
	                //    careerModeMapScreen.CheckForMapPinsBubble();
	                //}
	                break;
	            case ProgressionMapPinEventType.FRIENDS_RACES:
                    EventSelect.PopulateFriendRaceEvent(current.Tier, true);
	                break;
                case ProgressionMapPinEventType.SMP_RACES:
                    EventSelect.PopulateSMPEvents(current.Tier, true);
                    break;
                case ProgressionMapPinEventType.RESTRICTION_RACES:
                    if (current.Tier >= eCarTier.TIER_1 || BoostNitrous.TierBossChallengeFinished(eCarTier.TIER_5))
	                {
                        EventSelect.PopulateTutorialRestrictionRaces(current.Tier, true);
	                }
	                break;
	            case ProgressionMapPinEventType.CAR_SPECIFIC_RACES:
                    //if (current.Tier >= eCarTier.TIER_3 || BoostNitrous.TierBossChallengeFinished(eCarTier.TIER_5))
                    {
                        EventSelect.PopulateTutorialCarSpecificEvents(current.Tier, true);
                    }
	                break;
	            case ProgressionMapPinEventType.MANUFACTURER_SPECIFIC_RACES:
                    //if (current.Tier >= eCarTier.TIER_4 || BoostNitrous.TierBossChallengeFinished(eCarTier.TIER_5))
	                {
                        EventSelect.PopulateTutorialManufacturerSpecificEvents(current.Tier, true);
	                }
	                break;
	            case ProgressionMapPinEventType.WORLD_TOURS:
	                //if (current.Tier >= eCarTier.TIER_4 || BoostNitrous.TierBossChallengeFinished(eCarTier.TIER_5))
	            {
	                EventSelect.PopulateWorldTourFakeEvents(current.Name);
	            }
	                break;
            }
	    }
	}
}
