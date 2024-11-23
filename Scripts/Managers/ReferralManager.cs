using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;

public class ReferralManager : MonoSingleton<ReferralManager>
{
    private bool m_loadingReferrals;
    private Action<bool, ReferralItem[]> GetReferralItemDelegate;
    private Action<bool> CollectRewardDelegate;
    public void GetReferrals(string username, Action<bool, ReferralItem[]> callbackDelegate)
    {
        if (m_loadingReferrals)
            return;

        GetReferralItemDelegate = callbackDelegate;
        JsonDict parameters = new JsonDict();
        parameters.Set("username", username);
        /*WebRequestQueue.Instance.StartCall("rtw_get_referral", "get all referrals related to this username",
            parameters, ReferralListCallback,
            null, null);*/
        WebRequestQueue.Instance.StartCall("rtw_get_invites", "get all referrals related to this username",
            parameters, ReferralListCallback,
            null, null);
        m_loadingReferrals = true;
    }


    private void ReferralListCallback(string content, string zerror, int zstatus, object zuserdata)
    {
        m_loadingReferrals = false;
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting referral list : " + zerror);
            if (GetReferralItemDelegate != null)
            {
                GetReferralItemDelegate(false, null);
            }
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(content))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting referral list : server send malformed json in response");
            if (GetReferralItemDelegate != null)
            {
                GetReferralItemDelegate(false, null);
            }
            return;
        }

        ReferralItem[] records = null;
        if (parameters.ContainsKey("items"))
        {
            records = parameters.GetObjectList<ReferralItem>("items", GetReferralRecord).ToArray();
        }

        if (GetReferralItemDelegate != null)
        {
            GetReferralItemDelegate(true, records);
        }
    }

    private void GetReferralRecord(JsonDict jsondict, ref ReferralItem referralItem)
    {
        referralItem.ID = jsondict.GetLong("id");
        referralItem.AssociatedUserID = jsondict.GetLong("rfid");
        referralItem.AssociatedUserName = jsondict.GetString("rfnm");
        referralItem.AssociatedAvatarID = jsondict.GetString("rfav");
        referralItem.HasCollectedReward = jsondict.GetBool("rfcr");
    }


    public void CollectReward(string username,long refID, Action<bool> callbackDelegate)
    {
        if (m_loadingReferrals)
            return;

        CollectRewardDelegate = callbackDelegate;
        JsonDict parameters = new JsonDict();
        parameters.Set("username", username);
        parameters.Set("id", refID.ToString());
        /*WebRequestQueue.Instance.StartCall("rtw_collect_referral", "collect referral reward by refID",
            parameters, CollectReferralCallback,
            null, null);*/
        WebRequestQueue.Instance.StartCall("rtw_collect_invite", "collect referral reward by refID",
            parameters, CollectReferralCallback,
            null, null);
        m_loadingReferrals = true;
    }


    private void CollectReferralCallback(string content, string zerror, int zstatus, object zuserdata)
    {
        m_loadingReferrals = false;
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting referral list : " + zerror);
            if (GetReferralItemDelegate != null)
            {
                GetReferralItemDelegate(false, null);
            }
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(content))
        {
            GTDebug.LogError(GTLogChannel.RPBonus, "error getting referral list : server send malformed json in response");
            if (GetReferralItemDelegate != null)
            {
                GetReferralItemDelegate(false, null);
            }
            return;
        }

        var status = parameters.GetBool("status");

        if (CollectRewardDelegate != null)
        {
            CollectRewardDelegate(status);
        }
    }
}
