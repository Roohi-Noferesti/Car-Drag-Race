using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using Metrics;
using TMPro;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class ReferralScreen : ZHUDScreen
{
    private List<ReferralItemUI> m_referralItemUIs = new List<ReferralItemUI>();

    public GameObject m_referralItemPrefab;
    public RectTransform m_itemsContainer;
    public TextMeshProUGUI m_referralDescText;
    public TextMeshProUGUI m_referralCountText;
    private bool m_loadingReferrals;
    private bool m_showingprofile;

    public override ScreenID ID
    {
        get { return ScreenID.Referral; }
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        zAlreadyOnStack = false;
        base.OnCreated(zAlreadyOnStack);
        if (!zAlreadyOnStack)
        {
            m_referralCountText.text = string.Format(LocalizationManager.GetTranslation("TEXT_REFERRAL_SCREEN_SHARE_COUNT"), 0);
            var goldReward = GetGoldReward();
            var goldText = CurrencyUtils.GetGoldStringWithIcon(goldReward);
            m_referralDescText.text =
                string.Format(LocalizationManager.GetTranslation("TEXT_REFERRAL_SCREEN_SHARE_BODY"), goldText, UserManager.Instance.currentAccount.UserID);
            var username = UserManager.Instance.currentAccount.Username;
            if (PolledNetworkState.IsNetworkConnected && ServerSynchronisedTime.Instance.ServerTimeValid)
            {
                ReferralManager.Instance.GetReferrals(username, ReferralCallback);
                PopUpDatabase.Common.ShowWaitSpinnerPopup();
                m_loadingReferrals = true;
                CoroutineManager.Instance.StartCoroutineDelegate(_timeoutOperation);
            }
            else
            {
                ReferralCallback(false, new ReferralItem[0]);
            }
        }
    }


    private int GetGoldReward()
    {
        var goldRewards = GameDatabase.Instance.SocialConfiguration.ReferralGoldRewards;
        var tier = RaceEventQuery.Instance.getHighestUnlockedClass();
        return goldRewards[(int)tier];
    }

    private void ReferralCallback(bool success, ReferralItem[] records)
    {
        m_loadingReferrals = false;
        PopUpManager.Instance.KillPopUp();

        ClearItems();

        if (!success)
        {
            var popup = new PopUp()
            {
                Title = "TEXT_POPUP_REFERRAL_GET_LIST_ERROR_TITLE",
                BodyText = "TEXT_POPUP_REFERRAL_GET_LIST_ERROR_BODY",
                ConfirmAction = () => { ScreenManager.Instance.PopScreen(); },
                ConfirmText = "TEXT_BUTTON_OK"
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
            return;
        }

        var goldReward = GetGoldReward();

        if (records != null)
        {
            m_referralCountText.text = string.Format(LocalizationManager.GetTranslation("TEXT_REFERRAL_SCREEN_SHARE_COUNT"), records.Length);
            foreach (var referralItem in records)
            {
                var instance = (GameObject) Instantiate(m_referralItemPrefab);

                instance.transform.SetParent(m_itemsContainer, false);
                var referralItemUI = instance.GetComponent<ReferralItemUI>();
                //referralItemUI.Setup(referralItem, goldReward , () =>
                //    CollectReward(referralItem));
                referralItemUI.Setup(referralItem, goldReward, () =>
                   ShowProfile(referralItem.AssociatedUserID));
                m_referralItemUIs.Add(referralItemUI);
            }
        }
    }

    private void CollectReward(ReferralItem referralItem)
    {
        PopUpDatabase.Common.ShowWaitSpinnerPopup();
        var username = UserManager.Instance.currentAccount.Username;
        ReferralManager.Instance.CollectReward(username, referralItem.ID, status =>
        {
            PopUpManager.Instance.KillPopUp();
            if (status)
            {
                var goldReward = GetGoldReward();

                var popup = new PopUp()
                {
                    Title = "TEXT_POPUP_REFERRAL_COLLECT_TITLE",
                    BodyText = string.Format(LocalizationManager.GetTranslation("TEXT_POPUP_REFERRAL_COLLECT_BODY"),
                        CurrencyUtils.GetGoldStringWithIcon(goldReward)),
                    BodyAlreadyTranslated = true,
                    GraphicPath = PopUpManager.Instance.graphics_raceOfficialPrefab,
                    ConfirmAction = () => { OnCollectButtonConfirmed(referralItem, goldReward); },
                    ConfirmText = "TEXT_BUTTON_COLLECT"
                };
                PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Default, null);
            }
            else
            {
                PopUpDatabase.Common.ShowErrorPopup();
            }
        });

    }

    private void OnCollectButtonConfirmed(ReferralItem referralItem, int goldReward)
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var currentGold = activeProfile.GetCurrentGold();
        activeProfile.AddGold(goldReward,"reward","referral");
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();

        Log.AnEvent(Events.ReferralReward, new Dictionary<Parameters, string>()
        {
            {Parameters.DGld, (activeProfile.GetCurrentGold() - currentGold).ToString()}
        });

        var itemUI = m_referralItemUIs.FirstOrDefault(i => i.ReferralItem == referralItem);

        if (itemUI != null)
        {
            itemUI.ReferralItem.HasCollectedReward = true;
            itemUI.UpdateUI(null);
        }
    }

    private void ClearItems()
    {
        foreach (var mReferralItemUI in m_referralItemUIs)
        {
            Destroy(mReferralItemUI.gameObject);
        }

        m_referralItemUIs.Clear();
    }

    public void OnCloseButton()
    {
        ScreenManager.Instance.PopScreen();
    }


    private IEnumerator _timeoutOperation()
    {
        float timout;
        try
        {
            timout = GameDatabase.Instance.OnlineConfiguration.LeaderboardTimeout;
        }
        catch (Exception)
        {
            timout = 30;
        }

        yield return new WaitForSeconds(timout);
        if (m_loadingReferrals)
        {
            PopUpManager.Instance.KillPopUp();
            PopUpDatabase.Common.ShowTimeoutPopop(Close);
        }
    }

    public void ShowProfile(long userID)
    {
        if (UserManager.Instance.currentAccount.UserID == userID)
        {
            ShowProfile(PlayerProfileManager.Instance.ActiveProfile.GetProfileData());
        }
        else
        {
            var username = Account.GetUsername(userID);
            PopUpDatabase.Common.ShowWaitSpinnerPopup();
            m_showingprofile = true;
            PlayerProfileWeb.GetPlayerProfileData(username, false, ProfileDataResponseCallback);
            //Instance.StopCoroutine("_timeoutOperation");
            //Instance.StartCoroutine("_timeoutOperation");
        }
    }

    private void ShowProfile(PlayerProfileData user)
    {
        ProfileScreen.SetUser(user);
        ScreenManager.Instance.PushScreen(ScreenID.Profile);
    }

    private void ProfileDataResponseCallback(string content, string zerror, int zstatus, object zuserdata)
    {
        PopUpManager.Instance.KillPopUp();

        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            m_showingprofile = false;
            GTDebug.LogError(GTLogChannel.Leaderboards, "error getting profile data : " + zerror);
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(content))
        {
            m_showingprofile = false;
            GTDebug.LogError(GTLogChannel.Leaderboards, "error getting profile data : server send malformed json in response");
            return;
        }

        var username = parameters.GetString("username");
        var profileDataJson = parameters.GetString("profile_data");
        if (string.IsNullOrEmpty(profileDataJson))
        {
            m_showingprofile = false;
            GTDebug.LogError(GTLogChannel.Leaderboards, "error getting profile data : profile data json is empty or null");
            return;
        }

        var profileData = PlayerProfileMapper.FromJson(profileDataJson);
        var userID = Account.GetUserID(username);

        if (m_showingprofile)
        {
            GTDebug.Log(GTLogChannel.Leaderboards, "showing profile for userid : " + userID);
            ShowProfile(profileData);
            m_showingprofile = false;
        }
    }
}
