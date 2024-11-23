using System;
using System.Collections;
using UnityEngine;

public class VSDummy : ZHUDScreen
{
	public enum eVSMode
	{
		Multiplayer,
		International,
		None
	}

	private class MultiplayerVSLauncher : MonoBehaviour
	{
        //[DebuggerHidden]
		public IEnumerator WaitForFuelSpend()
		{
		    yield return 0;
		    //VSDummy.MultiplayerVSLauncher.<WaitForFuelSpend>c__Iterator38 <WaitForFuelSpend>c__Iterator = new VSDummy.MultiplayerVSLauncher.<WaitForFuelSpend>c__Iterator38();
		    //<WaitForFuelSpend>c__Iterator.<>f__this = this;
		    //return <WaitForFuelSpend>c__Iterator;
		}
	}

	public static eVSMode VSMode;

    public static PlayerReplay ReplayData;

	private static bool LaunchingRace;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.VSDummy;
		}
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		base.OnActivate(zAlreadyOnStack);
		BaseVersusScreen.StartAsFinished = zAlreadyOnStack;
		LoadingScreenManager.Instance.PreShowVSLoading(VSMode, this);
        //UIManager.instance.blockInput = false;
		base.BlockTutorialBubbles();
	}

	public static void BeginRace(CachedOpponentInfo replayData, eVSMode vsMode = eVSMode.Multiplayer)
	{
		VSMode = vsMode;
		if (!PreRaceConditionsMet(VSMode))
		{
			return;
		}
		TakeFuelAndStartRace(replayData.PlayerReplay);
	}

	public static void BeginRace(PlayerReplay replay, eVSMode vsMode = eVSMode.Multiplayer)
	{
		VSMode = vsMode;
		if (!PreRaceConditionsMet(VSMode))
		{
			return;
		}
        TakeFuelAndStartRace(replay);
	}

	private static void ShowNotEnoughFuelPopup(int currentFuel, int requiredFuel)
	{
		PopUp popUp = new PopUp();
		popUp.Title = "TEXT_GET_MORE_FUEL_TITLE";
	    popUp.BodyText = "need fuel";//string.Format(LocalizationManager.GetTranslation("TEXT_GET_MORE_FUEL_RELAY_BODY"), requiredFuel, requiredFuel - currentFuel);
		popUp.ConfirmText = "TEXT_BUTTON_OK";
		popUp.CancelText = "TEXT_BUTTON_NO_THANKS";
		popUp.ConfirmAction = delegate
		{
            //ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
		};
        popUp.GraphicPath = PopUpManager.Instance.graphics_mechanicPrefab;
		popUp.ImageCaption = "TEXT_NAME_MECHANIC";
		popUp.BodyAlreadyTranslated = true;
		PopUp popup = popUp;
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
	}

	public static bool PreRaceConditionsMet(eVSMode mode)
	{
		if (LaunchingRace)
		{
			return false;
		}
		int fuel = FuelManager.Instance.GetFuel();
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		int num = (!currentEvent.IsRandomRelay() && !currentEvent.AutoHeadstart) ? GameDatabase.Instance.Currencies.GetFuelCostForEvent(RaceEventInfo.Instance.CurrentEvent) : 0;
		if (fuel < num)
		{
			if (mode == eVSMode.International)
			{
				ShowNotEnoughFuelPopup(fuel, num);
			}
			else
			{
                //ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
			}
			return false;
		}
		return true;
	}

	private static void TakeFuelAndStartRace(PlayerReplay replay)
	{
        //UIManager.instance.blockInput = true;
        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
		ReplayData = replay;
		LaunchingRace = true;
		GameObject gameObject = new GameObject("MultiplayerLauncher", new Type[]
		{
			typeof(MultiplayerVSLauncher)
		});
		MultiplayerVSLauncher component = gameObject.GetComponent<MultiplayerVSLauncher>();
		component.StartCoroutine(component.WaitForFuelSpend());
	}

	private static void StartRace()
	{
        //ScreenManager.Instance.PushScreen(ScreenID.VSDummy);
        //CleanDownManager.Instance.OnEnterMultiplayerVersus();
		LaunchingRace = false;
	}

	public static void CancelRace()
	{
		LoadingScreenManager.Instance.StopLoadingEffects();
		int fuelCostForEvent = GameDatabase.Instance.Currencies.GetFuelCostForEvent(RaceEventInfo.Instance.CurrentEvent);
		FuelManager.Instance.AddFuel(fuelCostForEvent, FuelReplenishTimeUpdateAction.KEEP, FuelAnimationLockAction.DONTCARE);
		LaunchingRace = false;
        //ScreenManager.Instance.PopToScreen(ScreenID.Workshop);
        //ScreenManager.Instance.UpdateImmediately();
	}

    //[DebuggerHidden]
	private static IEnumerator WaitForFuelSpend()
	{
	    yield return 0;
	    //return new VSDummy.<WaitForFuelSpend>c__Iterator37();
	}

	public override bool HasBackButton()
	{
		return VSMode == eVSMode.International && !RaceEventInfo.Instance.CurrentEvent.IsRelay && !RaceEventInfo.Instance.CurrentEvent.AutoHeadstart;
	}

    //public override bool ForceVisualBackButton()
    //{
    //    return true;
    //}

	protected override void OnEnterPressed()
	{
		if (VSMode == eVSMode.Multiplayer)
		{
            //MultiplayerVersusScreen.Instance.OnRaceButton();
		}
		else if (VSMode == eVSMode.International)
		{
			InternationalVersusScreen.Instance.OnRaceButton();
		}
	}

	public override void RequestBackup()
	{
		LoadingScreenManager.Instance.SnapCloseVSScreen();
        //CommonUI.Instance.NavBar.HideBackButton();
        //ScreenManager.Instance.PopScreen();
        //ScreenManager.Instance.UpdateImmediately();
	}
}
