using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using Metrics;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChooseNameScreen : ZHUDScreen
{
    [SerializeField] private TMP_InputField m_inputNameGlobal;
    [SerializeField] private TMP_InputField m_inputNameOnlyEnglish;
    [SerializeField] private TMP_InputField m_invitorIdInput;
    [SerializeField] private AvatarScroller m_avatarScroller;
    [SerializeField] private RuntimeTextButton m_changeNameButton;
    [SerializeField] private GameObject m_invitorTitle;
    private string selectedName;
    private string selectedAvatarID;
    private int m_numberOfAttemps;
    private string SelectedInvitorId;
    private bool _previouslySetInvitor;
    public override ScreenID ID
    {
        get { return ScreenID.ChooseName; }
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);

        /*if (BasePlatform.ActivePlatform.IsAppTuttiBuild) {
            m_inputNameGlobal.gameObject.SetActive(false);
            m_inputNameOnlyEnglish.gameObject.SetActive(true);
        } else {
            m_inputNameGlobal.gameObject.SetActive(true);
            m_inputNameOnlyEnglish.gameObject.SetActive(false);
        }*/
        m_inputNameGlobal.gameObject.SetActive(true);
        m_inputNameOnlyEnglish.gameObject.SetActive(false);
        
        
        MenuAudio.Instance.playSound(AudioSfx.Popup);
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        m_avatarScroller.SetAvatars(AvatarIDs.IDs);
        m_avatarScroller.Reload();
        if (string.IsNullOrEmpty(PlayerProfileManager.Instance.ActiveProfile.AvatarID))
        {
            m_avatarScroller.SelectedIndex = Random.Range(0, AvatarIDs.IDs.Count());
        }
        else
        {
            m_avatarScroller.SelectedID = activeProfile.AvatarID;
        }

        /*if (BasePlatform.ActivePlatform.IsAppTuttiBuild) {
            m_inputNameGlobal.text = string.IsNullOrEmpty(activeProfile.DisplayName) ? string.Empty : activeProfile.DisplayName;
            m_inputNameOnlyEnglish.text = string.IsNullOrEmpty(activeProfile.DisplayName) ? string.Empty : activeProfile.DisplayName;
        } else {
            m_inputNameGlobal.text = string.IsNullOrEmpty(activeProfile.DisplayName) ? string.Empty : activeProfile.DisplayName;
            m_inputNameOnlyEnglish.text = string.IsNullOrEmpty(activeProfile.DisplayName) ? string.Empty : activeProfile.DisplayName;
        }*/
        m_inputNameGlobal.text = string.IsNullOrEmpty(activeProfile.DisplayName) ? string.Empty : activeProfile.DisplayName;
        m_inputNameOnlyEnglish.text = string.IsNullOrEmpty(activeProfile.DisplayName) ? string.Empty : activeProfile.DisplayName;
        
        CheckPreviouslyEnterInvitor();
        
    }


    public void SetName()
    {
        /*if (BasePlatform.ActivePlatform.IsAppTuttiBuild) {
            selectedName = m_inputNameOnlyEnglish.text;
        } else {
            selectedName = m_inputNameGlobal.text;
        }*/
        selectedName = m_inputNameGlobal.text;
        selectedAvatarID = m_avatarScroller.SelectedID;
        if (string.IsNullOrEmpty(selectedName) || selectedName.Length == 0)
        {
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUPS_INVALID_NAME_TITLE",
                BodyText = "TEXT_POPUPS_INVALID_NAME_BODY",
                IsBig = false,
                ConfirmAction = new PopUpButtonAction(this.RecordMetricsEvent1),
                ConfirmText = "TEXT_BUTTON_OK"
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
        }
        else
        {
            if (_previouslySetInvitor)
                SaveNameAndInvitation();
            else
                m_animator.Play("Next");
        }
    }

    public void SaveNameAndInvitation()
    {
        SelectedInvitorId = m_invitorIdInput.text;

        m_numberOfAttemps++;
        if (!PolledNetworkState.IsNetworkConnected
            || !ServerSynchronisedTime.Instance.ServerTimeValid || m_numberOfAttemps >= 2)
        {
            var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            activeProfile.AvatarID = selectedAvatarID;
            activeProfile.HasChoosePlayerName = true;
            activeProfile.DisplayName = selectedName;
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
            Close();
            return;
        }

        JsonDict parameters = new JsonDict();
        parameters.Set("username", UserManager.Instance.currentAccount.Username);
        parameters.Set("display_name", selectedName);
        parameters.Set("avatar_id", selectedAvatarID);
        parameters.Set("selected_invitor_id", SelectedInvitorId);
        WebRequestQueue.Instance.StartCall("acc_change_name", "Change user display name", parameters,
            ChangeUserDisplayNameResponse, null, NameHashCode(selectedName));
        Log.AnEvent(Events.ChooseYourName, new Dictionary<Parameters, string>()
        {
            {Parameters.PName, selectedName},
            {Parameters.PAvtr, selectedAvatarID},
        });
        m_changeNameButton.CurrentState = BaseRuntimeControl.State.Disabled;
    }

    private string NameHashCode(string value)
    {
        return BasePlatform.ActivePlatform.HMACSHA1_Hash(value, BasePlatform.eSigningType.Server_Accounts);
    }


    private void ChangeUserDisplayNameResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        //  if (zstatus == 200 && string.IsNullOrEmpty(zerror))
        //  {
        JsonDict jsonDict = new JsonDict();
        if (jsonDict.Read(zhttpcontent))
        {
            var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            activeProfile.AvatarID = selectedAvatarID;
            activeProfile.HasChoosePlayerName = true;
            activeProfile.DisplayName = selectedName;
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();


            bool RequestedForInvitation;
            if (jsonDict.TryGetValue("current_invitation_used", out RequestedForInvitation))
            {
                if (RequestedForInvitation)
                {
                    int result;
                    jsonDict.TryGetValue("invitation_result", out result);

                    if (result == 3)
                    {
                        int winningGold = 10;
                        var winningGoldWithIcon = CurrencyUtils.GetGoldStringWithIcon(winningGold);
                        UserManager.Instance.currentAccount.IAPGold += winningGold;
                        UserManager.Instance.currentAccount.HasUsedInvitation = true;
                        UserManager.Instance.SaveCurrentAccount();
                

                        PopUpManager.Instance.TryShowPopUp(new PopUp
                        {
                            Title = "TEXT_PRIZE_GOLD",
                            BodyText =String.Format(LocalizationManager.GetTranslation("TEXT_PRIZE_GOLD_BODY"),winningGoldWithIcon),
                            BodyAlreadyTranslated = true,
                            IsBig = true,
                            ConfirmText = "TEXT_OK",
                            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                            ImageCaption = "TEXT_NAME_AGENT",
                            ConfirmAction = () =>
                            {
                       
                            }
                        }, PopUpManager.ePriority.Default, null);

                    }

                }
            }
            Close();
        }
        // }
        else
        {
            m_changeNameButton.CurrentState = BaseRuntimeControl.State.Active;
            GTDebug.LogWarning(GTLogChannel.Account, zerror);
        }
    }


    public override void RequestBackup()
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (!activeProfile.HasChoosePlayerName)
        {
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUPS_CHOOSE_NAME_FORCE_TITLE",
                BodyText = "TEXT_POPUPS_CHOOSE_NAME_FORCE_BODY",
                IsBig = false,
                ConfirmAction = new PopUpButtonAction(this.RecordMetricsEvent2),
                ConfirmText = "TEXT_BUTTON_OK",
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
        }
        else
        {
            base.RequestBackup();
        }
    }

    private void RecordMetricsEvent1()
    {
        Log.AnEvent(Events.InvalidName);
    }

    private void RecordMetricsEvent2()
    {
        Log.AnEvent(Events.YouShouldChooseAName);
    }

    private void CheckPreviouslyEnterInvitor()
    {
        _previouslySetInvitor = false;
        if (UserManager.Instance.currentAccount != null)
        {
            if (UserManager.Instance.currentAccount.HasUsedInvitation)
            {
                _previouslySetInvitor = true;
                //m_invitorTitle.SetActive(false);
                //m_invitorIdInput.transform.parent.gameObject.SetActive(false);
            }
        }
    }

}
