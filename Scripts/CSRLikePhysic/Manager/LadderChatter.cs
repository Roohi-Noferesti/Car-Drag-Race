using System;
using I2.Loc;
using UnityEngine;

public class LadderChatter
{
	private static int lastUsed = -1;

	private static int NumOfConsolations = 6;

	public static bool PostRace(PopUpButtonAction methodToInvoke, RaceEventData raceEvent, bool playerWon)
	{
		if (raceEvent == null)
		{
			return false;
		}
		if (raceEvent.Parent == null)
		{
			return false;
		}
		eCarTier carTier = raceEvent.Parent.GetTierEvents().GetCarTier();
		if (!raceEvent.IsLadderSemiFinal() && !raceEvent.IsLadderFinal())
		{
			return false;
		}
		if (playerWon)
		{
			LadderChatter.ShowPopUp(LadderChatter.GetWinRace((int)(carTier + 1), raceEvent.IsLadderFinal(), true), LadderChatter.GetWinRace((int)(carTier + 1), raceEvent.IsLadderFinal(), false), methodToInvoke, carTier, false);
		}
		else
		{
			LadderChatter.ShowPopUp(LadderChatter.GetLoseRace((int)(carTier + 1), raceEvent.IsLadderFinal(), true), LadderChatter.GetLoseRace((int)(carTier + 1), raceEvent.IsLadderFinal(), false), methodToInvoke, carTier, false);
		}
		return true;
	}

    public static bool PreRace(PopUpButtonAction methodToInvoke, RaceEventData raceEvent)
    {
        if (raceEvent == null)
        {
            return false;
        }
        if (raceEvent.Parent == null)
        {
            return false;
        }
        if (RaceController.Instance.RaceIsRestart)
        {
            return false;
        }
        eCarTier carTier = raceEvent.Parent.GetTierEvents().GetCarTier();
        if (raceEvent.IsLadderSemiFinal())
        {
            LadderChatter.ShowPopUp(LadderChatter.GetPreRace((int) (carTier + 1), false, true),
                LadderChatter.GetPreRace((int) (carTier + 1), false, false), methodToInvoke, carTier, true);
            PauseGame.Pause(false);
            return true;
        }
        if (raceEvent.IsLadderFinal())
        {
            LadderChatter.ShowPopUp(LadderChatter.GetWinRace((int) (carTier + 1), true, true),
                LadderChatter.GetPreRace((int) (carTier + 1), true, false), methodToInvoke, carTier, true);
            PauseGame.Pause(false);
            return true;
        }
        return true;
    }

    public static string GetPreRace(int zTier, bool IsFinal, bool IsTitle)
	{
		string text = (!IsFinal) ? "SEMI" : "FINAL";
		if (IsTitle)
		{
			return LocalizationManager.GetTranslation("TEXT_CHATTER_LADDER_" + text + "_PRERACE_TITLE");
		}
        return LocalizationManager.GetTranslation(string.Concat(new object[]
		{
			"TEXT_CHATTER_LADDER_",
			text,
			"_PRERACE_T",
			zTier
		}));
	}

	public static string GetLoseRace(int zTier, bool IsFinal, bool IsTitle)
	{
		string text = (!IsFinal) ? "SEMI" : "FINAL";
		if (IsTitle)
		{
            return LocalizationManager.GetTranslation("TEXT_CHATTER_LADDER_" + text + "_PRERACE_TITLE");
		}
		int num = UnityEngine.Random.Range(1, LadderChatter.NumOfConsolations);
		if (num == LadderChatter.lastUsed)
		{
			num = num + 1 % LadderChatter.NumOfConsolations + 1;
		}
        return LocalizationManager.GetTranslation(string.Concat(new object[]
		{
			"TEXT_CHATTER_LADDER_",
			text,
			"_LOSE_",
			num
		}));
	}

	public static string GetWinRace(int zTier, bool IsFinal, bool IsTitle)
	{
		string text = (!IsFinal) ? "SEMI" : "FINAL";
		if (IsTitle)
		{
            return LocalizationManager.GetTranslation("TEXT_CHATTER_LADDER_" + text + "_PRERACE_TITLE");
		}
        string text2 = LocalizationManager.GetTranslation(string.Concat(new object[]
		{
			"TEXT_CHATTER_LADDER_",
			text,
			"_WIN_T",
			zTier
		}));
        //if (zTier == 3)
        //{
        //    string carDBKey = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().CarDBKey;
        //    string arg = LocalizationManager.GetTranslation(CarDatabase.Instance.GetCar(carDBKey).ShortName);
        //    return string.Format(text2, arg);
        //}
		return text2;
	}

	public static void ShowPopUp(string title, string body, PopUpButtonAction methodToInvoke, eCarTier carTier, bool disableBG = false)
	{
		PopUp popup = new PopUp
		{
			Title = title,
			TitleAlreadyTranslated = true,
			BodyText = body,
			BodyAlreadyTranslated = true,
			IsBig = true,
			ConfirmAction = methodToInvoke,
			ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
			ImageCaption = "TEXT_NAME_RACE_OFFICIAL",
			ShouldCoverNavBar = true
		};
		Action onShown = null;
		if (disableBG)
		{
			onShown = delegate
			{
				PopUpManager.Instance.PopUpScreenInstance.DisableBackground();
			};
		}
		PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, onShown);
	}
}
