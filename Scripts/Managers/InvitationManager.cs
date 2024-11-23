using System;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public class InvitationManager : MonoBehaviour
{
    private bool _hasCheckedForInvitation;
    private bool _checkingForInvitation;
    private int _invitedUserCount;
    private bool _popupRewardShown;
    private int _goldReward;
    public static InvitationManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    void Update()
    {
        var currentScreen = ScreenManager.Instance.CurrentScreen;
        if (_invitedUserCount > 0 && !PopUpManager.Instance.isShowingPopUp && !_popupRewardShown
            && currentScreen == ScreenID.Workshop)
        {
            if (PopUpManager.Instance.TryShowPopUp(GetRewardPopup(_invitedUserCount)))
            {
                _popupRewardShown = true;
            }
        }


        if (!_hasCheckedForInvitation && !_checkingForInvitation  && SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend)
        {
            _checkingForInvitation = true;
            CheckInvitation();
        }


    }


    public void CheckInvitation()
    {
        var currentAccount = UserManager.Instance.currentAccount;
        if (currentAccount != null && currentAccount.UserID!=0)
        {
            JsonDict parameters = new JsonDict();
            var userName = UserManager.Instance.currentAccount.Username;
            parameters.Set("username", userName);
            WebRequestQueue.Instance.StartCall("acc_check_my_invitee", "Check Invitation Status", parameters, CheckInvitationResponse, null, NameHashCode(userName));
        }
    }

    private string NameHashCode(string value)
    {
        return BasePlatform.ActivePlatform.HMACSHA1_Hash(value, BasePlatform.eSigningType.Server_Accounts);
    }


    private void CheckInvitationResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        _checkingForInvitation = false;
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting invitation response : " + zerror);
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(zhttpcontent))
        {
            GTDebug.LogError(GTLogChannel.RPBonus,
                "error getting invitation response : server send malformed json in response");
            return;
        }


        parameters.TryGetValue("count", out _invitedUserCount);

        _hasCheckedForInvitation = true;
    }

    public void ConfirmInvitationReward()
    {
        var currentAccount = UserManager.Instance.currentAccount;
        if (currentAccount != null && currentAccount.UserID != 0)
        {
            JsonDict parameters = new JsonDict();
            var userName = UserManager.Instance.currentAccount.Username;
            parameters.Set("username", userName);
            WebRequestQueue.Instance.StartCall("acc_confirm_invitation_reward", "Confirm Invitation Reward", parameters, ConfirmInvitationRewardResponse, null, NameHashCode(userName));
        }
    }

    private void ConfirmInvitationRewardResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error confirming invitation reward response : " + zerror);
            return;
        }

        PlayerProfileManager.Instance.ActiveProfile.AddGold(_goldReward,"reward","invitation");
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    }

    public PopUp GetRewardPopup(int count)
    {
        _goldReward = GetGoldReward() * count;
        var goldCurrencyText = CurrencyUtils.GetGoldStringWithIcon(_goldReward);
        return new PopUp
        {
            Title = "FRIENDINVITATION_PRIZE_TITLE",
            BodyText = String.Format(LocalizationManager.GetTranslation("FRIENDINVITATION_PRIZE_BODY"), goldCurrencyText,
                count),
            BodyAlreadyTranslated = true,
            IsBig = true,
            ConfirmText = "TEXT_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT",
            ConfirmAction = () =>
            {
                _invitedUserCount = 0;
                ConfirmInvitationReward();
            }
        };
    }


    public int GetGoldReward()
    {
        var goldRewards = GameDatabase.Instance.SocialConfiguration.ReferralGoldRewards;
        var tier = RaceEventQuery.Instance.getHighestUnlockedClass();
        return goldRewards[(int)tier];
    }
}


