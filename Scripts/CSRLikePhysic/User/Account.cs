using System;
using UnityEngine;

public class Account
{
    public const string tempAccountName = "temp";

    public string Username;

    public string Password;

    public int LastUsed;

    public string FBAccessToken;

    public string FBExpirationDate;

    public DateTime LastUTCTime;

    public DateTime LastTimeDifference;

    public bool LastTimeDifferenceSet;

    public bool lastTimeDiffRelevant;

    public int IAPGold;

    public int IAPCash;

    public int SuperNitrous;

    public int RaceCrew;

    public string GCid;

    private string branchName = string.Empty;

    private string forcedBranchName = string.Empty;

    public string IntroMessageJson;

    public string ABTestBucketName;

    public string ABTestCode;

    public string DeviceToken;

    public int BAM;

    public string ServerLastSavedProfileProductVersion;

    public string MPToken;

    public string RYFToken;

    public bool IsBanned;

    public bool HasUpgradedFuelTank;

    public int ClientAction;

    public string ClientActionTextTitle;

    public string ClientActionTextBody;

    public bool AdjustAttributionSet;

    public long ReferralUserID;

    public string AdjustTrackerName;

    private int mCollectUsageData;

    private string mIPAddress;
    public int IAPKeys_Bronze;
    public int IAPKeys_Silver;
    public int IAPKeys_Gold;
    public bool HasChosenBaseStoreOrFortumo;// you have to check user have chosen between fortumo or base payment to show choice popup or not
    public bool IsFortumo;// you have to check previously user have chosen fortumo or not to choose which IStoreManager you have to instantiate
   // public string CurrentLeague;// which league now user is inside- all possible values are in league helper
   // public string PreviousLeague;// same. user last week league.
    public bool HasUsedInvitation;// a user can use invitaion prize once. it means you can only be invited one time. make it true when you set it on the server

    //public string CurrentLeagueProp
    //{
    //    get
    //    {
    //        if (string.IsNullOrEmpty(CurrentLeague))
    //        {
    //            CurrentLeague = LeaguesHelper.Leagues[0].ID;
    //        }
    //        return CurrentLeague;
    //    }
    //    set {this.CurrentLeague = value; }
    //}

    //public string PreviousLeagueProp
    //{
    //    get
    //    {
    //        if (string.IsNullOrEmpty(PreviousLeague))
    //        {
    //            PreviousLeague = LeaguesHelper.Leagues[0].ID;
    //        }
    //        return PreviousLeague;
    //    }
    //    set { this.PreviousLeague = value; }
    //}

    public string AssetDatabaseBranch
    {
        get
        {
            if (!string.IsNullOrEmpty(this.forcedBranchName))
            {
                return this.forcedBranchName;
            }
            return this.branchName;
        }
        set
        {
            this.branchName = value;
        }
    }

    public string ForcedBranchName
    {
        get
        {
            return this.forcedBranchName;
        }
    }

    public string GCAlias
    {
        get
        {
            string result = string.Empty;
            //if (this.GCid == GameCenterController.Instance.currentID())
            //{
            //	result = GameCenterController.Instance.currentAlias();
            //}
            return result;
        }
    }

    public bool UserConverted
    {
        get
        {
            return this.IAPGold != 0 || this.IAPCash != 0 || this.RaceCrew != 0;
        }
    }

    public bool UserBoughtConsumables
    {
        get
        {
            return this.SuperNitrous != 0 || this.UserConverted;
        }
    }

    public int CollectUsageData
    {
        get
        {
            return this.mCollectUsageData;
        }
        set
        {
            this.mCollectUsageData = value;
        }
    }

    public string IPAddress
    {
        get
        {
            return this.mIPAddress;
        }
        set
        {
            this.mIPAddress = value;
        }
    }


    public bool IsTemporaryAccount
    {
        get
        {
            return this.Username != null && this.Username.Length >= 4 && this.Username.Substring(0, 4) == "temp";
        }
    }



    public int UserID
	{
		get { return GetUserID(this.Username); }
	}

    public string FreschatRestoreID;

    public string Language;
	
	public string CountryCode;

    public string BranchPostfix;

    public string PossibleStoresForIAPABTest;

    public bool IsActiveIApABTestBranch;


    public Account()
    {
        this.Username = string.Empty;
        this.Password = string.Empty;
        this.LastUsed = 0;
        this.IAPGold = 0;
        this.SuperNitrous = 0;
        this.IAPCash = 0;
        this.GCid = string.Empty;
        this.FBAccessToken = string.Empty;
        this.FBExpirationDate = string.Empty;
        this.SetBranchToDefault();
        this.DeviceToken = string.Empty;
        this.LastTimeDifference = new DateTime(0L);
        this.LastUTCTime = GTDateTime.Now;
        this.LastTimeDifferenceSet = false;
        this.lastTimeDiffRelevant = false;
        this.mCollectUsageData = 0;
        this.BAM = 1;
        this.ServerLastSavedProfileProductVersion = string.Empty;
        this.MPToken = string.Empty;
        this.RYFToken = string.Empty;
        this.IsBanned = false;
        this.HasUpgradedFuelTank = false;
        this.ReferralUserID = 0;
        this.AdjustAttributionSet = false;
        this.CountryCode = "US";
        this.AdjustTrackerName = string.Empty;
        this.Language = string.Empty;
        BranchPostfix = string.Empty;
        PossibleStoresForIAPABTest = string.Empty;
        IsActiveIApABTestBranch = false;
    }

    public string DumpAccount()
    {
        return string.Empty;
    }

    public void SetBranchToDefault()
    {
        string defaultBranchName = BasePlatform.ActivePlatform.GetDefaultBranchName();
        this.branchName = defaultBranchName;
        this.ABTestCode = string.Empty;
        this.ABTestBucketName = string.Empty;
    }

    public static int GetUserID(string username)
    {
        if (username.Length > 4)
        {
            string s = username.Substring(4, username.Length - 4);
            int result = 0;
            if (int.TryParse(s, out result))
            {
                return result;
            }
        }
        return 0;
    }

    public static string GetUsername(long userID)
    {
        return "user" + userID;
    }

    public static Account LoadFromJson(string accountdata)
    {
        Account account = new Account();
        JsonDict jsonDict = new JsonDict();
        if (!jsonDict.Read(accountdata))
        {
            return null;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (!jsonDict.ContainsKey("lastused"))
            {
                TimeSpan timeSpan = GTDateTime.Now - new DateTime(1970, 1, 1);
                jsonDict.Set("lastused", (int)timeSpan.TotalSeconds);
            }

            if (!jsonDict.ContainsKey("gamecenterid"))
            {
                jsonDict.Set("gamecenterid", string.Empty);
            }
        }
#endif
        bool flag = true;
        flag &= jsonDict.TryGetValue("username", out account.Username);
        flag &= jsonDict.TryGetValue("password", out account.Password);
        flag &= jsonDict.TryGetValue("lastused", out account.LastUsed);
        flag &= jsonDict.TryGetValue("gamecenterid", out account.GCid);
        flag &= jsonDict.TryGetValue("gold", out account.IAPGold);
        flag &= jsonDict.TryGetValue("cash", out account.IAPCash);
        if (!(flag & jsonDict.TryGetValue("supernitrous", out account.SuperNitrous)))
        {
            return null;
        }
        jsonDict.TryGetValue("accesstokenfb", out account.FBAccessToken);
        jsonDict.TryGetValue("expritydatefb", out account.FBExpirationDate);
        string assetDatabaseBranch = account.AssetDatabaseBranch;
        jsonDict.TryGetValue("abtest_branch_name", out assetDatabaseBranch);
        if (assetDatabaseBranch != account.AssetDatabaseBranch)
        {
            account.AssetDatabaseBranch = assetDatabaseBranch;
        }
        jsonDict.TryGetValue("abtest_bucket_name", out account.ABTestBucketName);
        jsonDict.TryGetValue("abtest_code", out account.ABTestCode);
        jsonDict.TryGetValue("device_token", out account.DeviceToken);
        jsonDict.TryGetValue("bam", out account.BAM);
        jsonDict.TryGetValue("mpt", out account.MPToken);
        jsonDict.TryGetValue("ryft", out account.RYFToken);
        jsonDict.TryGetValue("last_time_diff", out account.LastTimeDifference);
        jsonDict.TryGetValue("last_time_diff_set", out account.LastTimeDifferenceSet);
        jsonDict.TryGetValue("last_time_diff_relevant", out account.lastTimeDiffRelevant);
        jsonDict.TryGetValue("last_utc_time", out account.LastUTCTime);
        jsonDict.TryGetValue("banned", out account.IsBanned);
        jsonDict.TryGetValue("has_upgraded_fuel_tank", out account.HasUpgradedFuelTank);
        jsonDict.TryGetValue("racecrew", out account.RaceCrew);
        jsonDict.TryGetValue("ref_user_id", out account.ReferralUserID);
        jsonDict.TryGetValue("adjust_tracker_name", out account.AdjustTrackerName);
        jsonDict.TryGetValue("adjust_attribution_set", out account.AdjustAttributionSet);
        jsonDict.TryGetValue("has_chosen_base_or_fortumo", out account.HasChosenBaseStoreOrFortumo);
        jsonDict.TryGetValue("is_fortumo",out account.IsFortumo);
      //  jsonDict.TryGetValue("current_league_id",out account.CurrentLeague);
       // jsonDict.TryGetValue("previous_league_id", out account.PreviousLeague);
        jsonDict.TryGetValue("has_used_invitation", out account.HasUsedInvitation);
        jsonDict.TryGetValue("fresh_id", out account.FreschatRestoreID);
        jsonDict.TryGetValue("BranchPostFix", out account.BranchPostfix);
        jsonDict.TryGetValue("BranchStores", out account.PossibleStoresForIAPABTest);
        return account;
    }
}
