using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;
using Z2HSharedLibrary.DatabaseEntity;

public class DifficultyScreen : ZHUDScreen
{
    [SerializeField]
    private Toggle m_easyToggle;
    [SerializeField]
    private Toggle m_normalToggles;
    [SerializeField]
    private Toggle m_hardToggles;


    public void UpdateDifficulty()
    {
        //RaceEventInfo.Instance.DifficultyRating = m_easyToggle.isOn
        //    ? AutoDifficulty.DifficultyRating.Easy
        //    : (m_normalToggles.isOn
        //        ? AutoDifficulty.DifficultyRating.Challenging
        //        : AutoDifficulty.DifficultyRating.Difficult);
    }

    public void Continue()
    {
        //var playerCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        //var zRaceEventData = MapScreenMap.Instance.GetSelectedEvent(RaceEventInfo.Instance.DifficultyRating);
        //AutoDifficulty.GetRandomOpponentForCarAtDifficulty(RaceEventInfo.Instance.DifficultyRating,
        //    ref zRaceEventData,
        //    playerCar);

        //if (!zRaceEventData.ForceUserInCar)
        //{
        //    if (!FuelManager.Instance.SpendFuel(GameDatabase.Instance.Currencies.GetFuelCostForEvent(zRaceEventData)))
        //    {
        //        ScreenManager.Active.pushScreen("Hacker_BG");
        //        return;
        //    }
        //}
        ////this.GoToRaceAnimationTimer = 0f;
        ////this.GoToRaceAnimating = true;
        ////this.StopUserInput.gameObject.SetActive(true);
        //TouchManager.Instance.GesturesEnabled = false;
        //MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
        //RaceEventInfo.Instance.PopulateFromRaceEvent(zRaceEventData);
        ////this.bIgnoreHardwareBackButton = true;
        //ScreenManager.Active.pushScreen("VS_Page");
    }
}
