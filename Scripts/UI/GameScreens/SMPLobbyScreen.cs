using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

public class SMPLobbyScreen : ZHUDScreen
{
    [SerializeField] private OnlineRaceSelectButton m_onlineRaceButtonPrefab;
    [SerializeField] private Transform m_buttonsLayout;
    public SMPWinStreakUI SMPWinStreakGO;

    public Sprite[] BetSprites;

    private List<OnlineRaceSelectButton> m_racesButtons = new List<OnlineRaceSelectButton>();
    private ScrollRect m_scrollRect;
    private bool m_updated;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.SMPLobby;
        }
    }

    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);
        m_scrollRect = m_buttonsLayout.GetComponentInParent<ScrollRect>();
        SetupOnlineRacingEvents();

        if (!PolledNetworkState.IsNetworkConnected)
        {
            PopUpDatabase.Common.ShowNoInternetConnectionPopup(Close);
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!m_updated)
        {
            SMPWinStreakGO.TriggerWinStreakUpdate();
            m_updated = true;
        }
    }

    private void ClearButtons()
    {
        foreach (var onlineRaceSelectButton in m_racesButtons)
        {
            DestroyImmediate(onlineRaceSelectButton.gameObject);
        }
        m_racesButtons.Clear();
    }

    private void SetupOnlineRacingEvents()
    {
        var onlineRacesMatch = GameDatabase.Instance.Online.GetOnlineRacesMatch();
        var car = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        var carinfo = CarDatabase.Instance.GetCar(car.CarDBKey);
        ClearButtons();
        for (int i = 0; i < onlineRacesMatch.Length; i++)
        {
            OnlineRace onlineRace = onlineRacesMatch[i];
            var raceButtonInstance = Instantiate(m_onlineRaceButtonPrefab.gameObject);
            raceButtonInstance.transform.SetParent(m_buttonsLayout, false);
            var raceButton = raceButtonInstance.GetComponent<OnlineRaceSelectButton>();
            m_racesButtons.Add(raceButton);
            var title = LocalizationManager.GetTranslation(onlineRace.LocalizationNameText);
            int raceIndex = i;
            raceButton.Setup(title, onlineRace, carinfo.BaseCarTier, () => { SelectRace(raceIndex); }, BetSprites[raceIndex]);
        }
        m_scrollRect.horizontalNormalizedPosition = 0;
    }

    private void SelectRace(int raceIndex)
    {
        MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
        var car = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
        var carinfo = CarDatabase.Instance.GetCar(car.CarDBKey);
        var selectedRace = GameDatabase.Instance.Online.GetOnlineRacesMatch()[raceIndex];
        var onlineRacingEvent = GameDatabase.Instance.CareerConfiguration.CareerRaceEvents.SMPRaceEvents;
        var raceEventData = RaceEventQuery.Instance.GetOnlineRaceEvent(onlineRacingEvent,
            selectedRace.EventIndex);

        //var activeRestriction = RestrictionHelper.GetActiveRestriction(raceEventData);

        //if (activeRestriction != null)
        //{
        //    activeRestriction.RestrictionButtonPressed();
        //    return;
        //}

        CommonUI.Instance.FuelStats.FuelLockedState(true);
        if (!FuelManager.Instance.SpendFuel(GameDatabase.Instance.Currencies.GetFuelCostForEvent(raceEventData)))
        {
            CommonUI.Instance.FuelStats.FuelLockedState(false);
            ScreenManager.Instance.PushScreen(ScreenID.GetMoreFuel);
            return;
        }

        if (selectedRace.GetStake(carinfo.BaseCarTier)>PlayerProfileManager.Instance.ActiveProfile.GetCurrentCash())
        {
            PopUpDatabase.Common.ShowNotEnoughFundPopup(ShopScreen.ItemType.Cash,
                "OnlineRace", "OnlineRace", () =>
                {
                    ShopScreen.InitialiseForPurchase(ShopScreen.ItemType.Cash, ShopScreen.PurchaseType.Buy,
                        ShopScreen.PurchaseSelectionType.Select);
                    ScreenManager.Instance.PushScreen(ScreenID.Shop);
                });
            return;
        }

        OnlineRaceMatchMakingScreen.SelectedOnlineRace = selectedRace;
        ScreenManager.Instance.PushScreen(ScreenID.OnlineRaceMatchMaking);
    }
}
