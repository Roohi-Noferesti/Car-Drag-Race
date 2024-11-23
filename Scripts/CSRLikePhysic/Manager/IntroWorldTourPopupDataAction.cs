using DataSerialization;
using System;
using KingKodeStudio;

public class IntroWorldTourPopupDataAction : PopupDataActionBase
{
	private const string worldTourIntroNarrativeSceneID = "worldtour_intro";

	public override void Execute(EligibilityConditionDetails details)
	{
        //ScreenID currentScreen = ScreenManager.Instance.CurrentScreen;
        //ScreenID screenID = currentScreen;
        //ScreenID[] zFakeHistory;
        //if (screenID != ScreenID.CareerModeMap)
        //{
        //	if (screenID == ScreenID.Workshop)
        //	{
        //		zFakeHistory = new ScreenID[]
        //		{
        //			ScreenID.CareerModeMap
        //		};
        //		goto IL_5E;
        //	}
        //	if (screenID == ScreenID.Home)
        //	{
        //		zFakeHistory = new ScreenID[]
        //		{
        //			ScreenID.Workshop,
        //			ScreenID.CareerModeMap
        //		};
        //		goto IL_5E;
        //	}
        //}
        //zFakeHistory = new ScreenID[0];
        //IL_5E:
        //CareerModeMapScreen.mapPaneSelected = MapPaneType.SinglePlayer;
        //ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.CrewProgression, zFakeHistory);
        //   ScreenManager.Instance.UpdateImmediately();
        //CrewProgressionScreen crewProgressionScreen = ScreenManager.Instance.ActiveScreen as CrewProgressionScreen;
        //if (crewProgressionScreen == null)
        //{
        //	return;
        //}
	    //NarrativeScene scene = null;
	    //if (!TierXManager.Instance.GetNarrativeScene("worldtour_intro", out scene))
	    //{
	    //	return;
	    //}
	    //crewProgressionScreen.SetupForNarrativeScene(scene);
	    CareerModeMapScreen.Instance.FocusOnEvent(ProgressionMapPinEventType.WORLD_TOURS, details.Tier, details.FloatValue,true, details.StringValue);
        PlayerProfileManager.Instance.ActiveProfile.HasSeenWorldTourIntro = true;
		PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
	}
}
