using DataSerialization;
using System;
using System.Collections.Generic;
using KingKodeStudio;

public class CrewStateDismissScreenWithSuperNitrousCheck : BaseCrewState
{
	public CrewStateDismissScreenWithSuperNitrousCheck(CrewProgressionScreen zParentScreen) : base(zParentScreen)
	{
	}

	public CrewStateDismissScreenWithSuperNitrousCheck(NarrativeSceneStateConfiguration config) : base(config)
	{
	}

	public override void OnEnter()
	{
	}

	public override bool OnMain()
	{
		base.OnMain();
		return true;
	}

	public override void OnExit()
	{
		Dictionary<string, PlayerProfileData.DeferredNarrativeScene> worldTourDeferredNarrativeScenes = PlayerProfileManager.Instance.ActiveProfile.WorldTourDeferredNarrativeScenes;
		string currentThemeName = TierXManager.Instance.CurrentThemeName;
		string text = string.Empty;
		if (worldTourDeferredNarrativeScenes.ContainsKey(currentThemeName))
		{
			text = worldTourDeferredNarrativeScenes[currentThemeName].SequenceID;
			worldTourDeferredNarrativeScenes.Remove(currentThemeName);
			PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
		}
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent != null && string.IsNullOrEmpty(text))
		{
            PinDetail worldTourPinPinDetail = currentEvent.GetWorldTourPinPinDetail();
            if (worldTourPinPinDetail == null)
            {
                ScreenManager.Instance.PopScreen();
                return;
            }
            text = worldTourPinPinDetail.WorldTourScheduledPinInfo.ParentSequence.ID;
        }
		GameStateFacade gameStateFacade = new GameStateFacade();
		gameStateFacade.CanOfferSuperNitrous = true;
		gameStateFacade.CurrentWorldTourSequenceID = text;
        RaceEventData firstAvailableEvent = TierXManager.Instance.GetFirstAvailableEvent(gameStateFacade);
        if (true)//firstAvailableEvent == null || !firstAvailableEvent.IsHighStakesEvent())
        {
            ScreenManager.Instance.PopScreen();
            return;
        }
        //HighStakesScreenBase.IsSetupForWorldTourHighStakes = true;
        //HighStakesScreenBase.WorldTourHighStakesRaceEvent = firstAvailableEvent;
        //PlayerProfileManager.Instance.ActiveProfile.WorldTourBoostNitrous.SetRaceBegun(firstAvailableEvent.EventID, firstAvailableEvent.GetWorldTourPinPinDetail().PinID);
        //PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
        //if (firstAvailableEvent.EventID == 150029)
        //{
        //    //InternationalCarAwardScreen.PrepareScreen("FerrariFXXK", true);
        //    ScreenManager.Instance.PushScreen(ScreenID.InternationalCarAward);
        //    return;
        //}
        //ScreenManager.Instance.SwapScreen(ScreenID.HighStakesChallenge);
    }
}
