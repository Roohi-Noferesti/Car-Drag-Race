using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

public class Storage
{
	public enum IdentMode
	{
		Legacy,
		Default
	}

	private enum eField
	{
		VALUE_DATA
	}

	private static string sLegacyIdent = "gtuser";

	private static string sBaseIdent = "gtracinguser";

	private static string MakePath(int slot)
	{
		return string.Concat(new object[]
		{
			Application.persistentDataPath,
			"/",
			Storage.sBaseIdent,
			slot,
			".txt"
		});
	}

	public static bool IsSlotValid(int slot, Storage.IdentMode mode = Storage.IdentMode.Default)
	{
		JsonDict jsonDict = Storage.LoadSlotData(slot, mode);
		if (jsonDict == null)
		{
			return false;
		}
		string value;
		bool flag = jsonDict.TryGetValue("password", out value);
		return flag && !string.IsNullOrEmpty(value);
	}

	private static JsonDict LoadSlotData(int slot, Storage.IdentMode mode)
	{
		string text = null;
		if (Keychain.IsEnabled())
		{
			if (mode == Storage.IdentMode.Legacy)
			{
				text = Keychain.ReadAccount(Storage.sLegacyIdent + slot);
			}
			else
			{
				text = Keychain.ReadAccount(Storage.sBaseIdent + slot);
			}
		}
		else
		{
			string path = Storage.MakePath(slot);
			if (File.Exists(path))
			{
				StreamReader streamReader = File.OpenText(path);
				text = streamReader.ReadToEnd();
				streamReader.Dispose();
				if (!text.StartsWith("{"))
				{
					return null;
				}
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		JsonDict jsonDict = new JsonDict();
		if (!jsonDict.Read(text))
		{
			return null;
		}
		return jsonDict;
	}

	public static bool LoadSlot(int slot, Account account, Storage.IdentMode mode = Storage.IdentMode.Default)
	{
		JsonDict jsonDict = Storage.LoadSlotData(slot, mode);
		if (jsonDict == null)
		{
			return false;
		}
		bool flag = true;
		flag &= jsonDict.TryGetValue("username", out account.Username);
		flag &= jsonDict.TryGetValue("password", out account.Password);
		flag &= jsonDict.TryGetValue("lastused", out account.LastUsed);
		flag &= jsonDict.TryGetValue("gamecenterid", out account.GCid);
		flag &= jsonDict.TryGetValue("gold", out account.IAPGold);
		flag &= jsonDict.TryGetValue("cash", out account.IAPCash);
		if (!(flag & jsonDict.TryGetValue("supernitrous", out account.SuperNitrous)))
		{
			return false;
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
	    jsonDict.TryGetValue("language", out account.Language);
	    jsonDict.TryGetValue("country_code", out account.CountryCode);
        jsonDict.TryGetValue("has_chosen_base_or_fortumo", out account.HasChosenBaseStoreOrFortumo);
        jsonDict.TryGetValue("is_fortumo",out account.IsFortumo);
      //  jsonDict.TryGetValue("current_league_id", out account.CurrentLeague);
      //  jsonDict.TryGetValue("previous_league_id", out account.PreviousLeague);
        jsonDict.TryGetValue("has_used_invitation", out account.HasUsedInvitation);
        jsonDict.TryGetValue("fresh_id", out account.FreschatRestoreID);
        jsonDict.TryGetValue("BranchPostFix", out account.BranchPostfix);
        jsonDict.TryGetValue("BranchStores", out account.PossibleStoresForIAPABTest);
        return true;
	}

	public static bool SaveSlot(int slot, Account account)
	{
		JsonDict jsonDict = new JsonDict();
		jsonDict.Set("username", account.Username);
		jsonDict.Set("password", account.Password);
		jsonDict.Set("lastused", account.LastUsed);
		jsonDict.Set("gamecenterid", account.GCid);
		jsonDict.Set("gold", account.IAPGold);
		jsonDict.Set("cash", account.IAPCash);
		jsonDict.Set("supernitrous", account.SuperNitrous);
		jsonDict.Set("accesstokenfb", account.FBAccessToken);
		jsonDict.Set("expritydatefb", account.FBExpirationDate);
		jsonDict.Set("abtest_branch_name", account.AssetDatabaseBranch);
		jsonDict.Set("abtest_bucket_name", account.ABTestBucketName);
		jsonDict.Set("abtest_code", account.ABTestCode);
		jsonDict.Set("device_token", account.DeviceToken);
		jsonDict.Set("bam", account.BAM);
		jsonDict.Set("mpt", account.MPToken);
		jsonDict.Set("ryft", account.RYFToken);
		jsonDict.Set("last_time_diff", account.LastTimeDifference);
		jsonDict.Set("last_time_diff_set", account.LastTimeDifferenceSet);
		jsonDict.Set("last_time_diff_relevant", account.lastTimeDiffRelevant);
		jsonDict.Set("last_utc_time", account.LastUTCTime);
		jsonDict.Set("banned", account.IsBanned);
		jsonDict.Set("has_upgraded_fuel_tank", account.HasUpgradedFuelTank);
		jsonDict.Set("racecrew", account.RaceCrew);
	    jsonDict.Set("product", "gtclub1");
	    jsonDict.Set("ref_user_id",account.ReferralUserID);
	    jsonDict.Set("adjust_tracker_name", account.AdjustTrackerName);
	    jsonDict.Set("adjust_attribution_set", account.AdjustAttributionSet);
	    jsonDict.Set("language", account.Language);
	    jsonDict.Set("country_code", account.CountryCode);
		jsonDict.Set("has_chosen_base_or_fortumo",account.HasChosenBaseStoreOrFortumo);
		jsonDict.Set("is_fortumo",account.IsFortumo);
		//jsonDict.Set("current_league_id",account.CurrentLeagueProp);
		//jsonDict.Set("previous_league_id",account.PreviousLeagueProp);
		jsonDict.Set("has_used_invitation",account.HasUsedInvitation);
        jsonDict.Set("fresh_id", account.FreschatRestoreID);
        jsonDict.Set("fresh_id", account.FreschatRestoreID);
        jsonDict.Set("BranchPostFix", account.BranchPostfix);
        jsonDict.Set("BranchStores", account.PossibleStoresForIAPABTest);
        string text = jsonDict.ToString();
		if (Keychain.IsEnabled())
		{
			Keychain.WriteAccount(Storage.sBaseIdent + slot, text);
		}
		else
		{
			string path = Storage.MakePath(slot);
			StreamWriter streamWriter = File.CreateText(path);
			streamWriter.WriteLine(text);
			streamWriter.Dispose();
		}
		return true;
	}

	public static bool ClearSlot(int slot)
	{
		if (Keychain.IsEnabled())
		{
			Keychain.WriteAccount(Storage.sBaseIdent + slot, string.Empty);
		}
		else
		{
			string path = Storage.MakePath(slot);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		return true;
	}
}
