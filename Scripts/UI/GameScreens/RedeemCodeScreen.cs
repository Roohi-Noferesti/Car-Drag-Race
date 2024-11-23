using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Metrics;
using TMPro;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class RedeemCodeScreen : ZHUDScreen 
{
    [SerializeField]
    private TMP_InputField m_inputCode;

    private bool m_applying;
    private Redeem m_redeem;

    public override ScreenID ID
    {
        get { return ScreenID.RedeemCode; }
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        MenuAudio.Instance.playSound(AudioSfx.Popup);
        m_inputCode.text = string.Empty;
    }

    private void ApplyRewardInProfile(Redeem redeem)
    {
        CommonUI.Instance.CashStats.GoldLockedState(false);
        CommonUI.Instance.CashStats.CashLockedState(false);
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        activeProfile.AddGold(redeem.Gold,"redeem","redeem");
        activeProfile.AddCash(redeem.Cash,"redeem","redeem");
        activeProfile.Save();
        Close();
    }


    public void ConsumeCode()
    {
        var redeemCode = m_inputCode.text;
        if (string.IsNullOrEmpty(redeemCode) || redeemCode.Length == 0)
        {
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUPS_INVALID_REDEEM_CODE_TITLE",
                BodyText = "TEXT_POPUPS_INVALID_REDEEM_CODE_BODY",
                IsBig = false,
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
        }
        else
        {
            //var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
            //ClientConnectionManager.GetRedeemCodeDetails(redeemCode.Trim(), activeProfile.ID);
            JsonDict parameters = new JsonDict();
            parameters.Set("username", UserManager.Instance.currentAccount.Username);
            parameters.Set("redeem_code", redeemCode.Trim());
            WebRequestQueue.Instance.StartCall("acc_get_redeem", "get redeem code details", parameters, GetRedeemCodeResponse,
                null, null);
            m_applying = true;
            StopCoroutine("_timeoutOperation");
            StartCoroutine("_timeoutOperation");
            //Log.AnEvent(Events.ApplyRedeemCode, new Dictionary<Parameters, string>()
            //{
            //    {Parameters.Code,redeemCode},
            //});

            PopUpDatabase.Common.ShowWaitSpinnerPopup();
        }
    }

    private void GetRedeemCodeResponse(string content, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            return;
        }

        JsonDict response = new JsonDict();
        if (!response.Read(content))
        {
            return;
        }

        Redeem redeem = response.ContainsKey("redeem") ? response.GetObject<Redeem>("redeem", DeserialiseRedeem) : null;

        m_applying = false;
        m_redeem = redeem;
        var playerID = UserManager.Instance.currentAccount.UserID;
        var isvalidRedeem = IsRedeemValid(redeem);
        //string title;
        //string body;
        int errorCode = 0;
        if (redeem == null)
        {
            errorCode = 4001;
        }
        else if (redeem.UserID != 0 && redeem.UserID != playerID)
        {
            //title = "TEXT_POPUPS_INVALID_REDEEM_TITLE";
            //body = "TEXT_POPUPS_INVALID_REDEEM_BODY";
            errorCode = 4002;
        }
        else if (redeem.Consumed)
        {
            errorCode = 4003;
        }
        else if (redeem.ExpireDate < ServerSynchronisedTime.Instance.GetDateTime())
        {
            errorCode = 4004;
        }

        var body = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_INVALID_REDEEM_BODY"), errorCode);
        if (redeem==null || !isvalidRedeem)
        {
            PopUpManager.Instance.KillPopUp();
            PopUp popup = new PopUp
            {
                Title = "TEXT_POPUPS_INVALID_REDEEM_TITLE",
                BodyText = body,
                BodyAlreadyTranslated = true,
                IsBig = false,
                ConfirmText = "TEXT_BUTTON_OK",
                GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
                ImageCaption = "TEXT_NAME_AGENT"
            };
            PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
        }
        else
        {
            CommonUI.Instance.CashStats.GoldLockedState(true);
            CommonUI.Instance.CashStats.CashLockedState(true);
            //ClientConnectionManager.ConsumeRedeemCode(redeem.Code, playerID);
            JsonDict parameters = new JsonDict();
            parameters.Set("username", playerID);
            parameters.Set("redeem_code", redeem.Code);
            WebRequestQueue.Instance.StartCall("acc_consume_redeem", "consume redeem code", parameters, ConsumeRedeemResponse,
                null, null);
        }
    }

    private void DeserialiseRedeem(JsonDict jsondict, ref Redeem redeem)
    {
        redeem.ID = jsondict.GetLong("id");
        redeem.Cash = jsondict.GetInt("cash");
        redeem.Gold = jsondict.GetInt("gold");
        redeem.Code = jsondict.GetString("code");
        redeem.Consumed = jsondict.GetBool("cons");
        redeem.ConsumedDate = jsondict.GetDateTime("cmdt");
        redeem.CreatedDate = jsondict.GetDateTime("crdt");
        redeem.ExpireDate = jsondict.GetDateTime("exdt");
        redeem.UserID = jsondict.GetLong("usid");
    }

    private void ConsumeRedeemResponse(string content, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            return;
        }

        m_applying = false;
        PopUpManager.Instance.KillPopUp();
        var goldString = m_redeem.Gold > 0 ? CurrencyUtils.GetGoldStringWithIcon(m_redeem.Gold) : string.Empty;
        var cashString = m_redeem.Cash > 0 ? CurrencyUtils.GetCashString(m_redeem.Cash) : string.Empty;
        var body = string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_REDEEM_CONSUMED_BODY"), goldString,
            cashString);
        PopUp popup = new PopUp
        {
            Title = "TEXT_POPUPS_REDEEM_CONSUMED_TITLE",
            BodyText = body,
            BodyAlreadyTranslated = true,
            IsBig = false,
            ConfirmAction = () => ApplyRewardInProfile(m_redeem),
            ConfirmText = "TEXT_BUTTON_OK",
            GraphicPath = PopUpManager.Instance.graphics_agentPrefab,
            ImageCaption = "TEXT_NAME_AGENT"
        };
        PopUpManager.Instance.TryShowPopUp(popup, PopUpManager.ePriority.Objective, null);
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
        if (m_applying)
        {
            PopUpManager.Instance.KillPopUp();
            PopUpDatabase.Common.ShowTimeoutPopop(Close);
        }
    }

    private bool IsRedeemValid(Redeem redeem)
    {
        var playerID = UserManager.Instance.currentAccount.UserID;
        return redeem!=null  && (redeem.UserID == 0 || redeem.UserID == playerID)
               && !redeem.Consumed
               && ServerSynchronisedTime.Instance.GetDateTime() < redeem.ExpireDate;
    }
}
