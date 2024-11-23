using System;
using System.Collections.Generic;
using System.Linq;

public class Accounts
{
	private const int MAX_ACCOUNTS = 8;

	private Dictionary<int, Account> mAccounts = new Dictionary<int, Account>();

	public int Count
	{
		get
		{
			return this.mAccounts.Count;
		}
	}

	public Accounts()
	{
		this.LoadAll();
	}

	public List<Account> GetAllAccounts()
	{
		return this.mAccounts.Values.ToList<Account>();
	}

	private void LoadAll()
	{
		if (this.LoadAccountsUsingIdent(Storage.IdentMode.Default, true))
		{
			return;
		}
		if (this.LoadAccountsUsingIdent(Storage.IdentMode.Legacy, false))
		{
			for (int i = 0; i < 8; i++)
			{
				if (this.mAccounts.ContainsKey(i))
				{
					Storage.SaveSlot(i, this.mAccounts[i]);
				}
				else
				{
					Storage.ClearSlot(i);
				}
			}
		}
	}

	private bool LoadAccountsUsingIdent(Storage.IdentMode ident, bool deleteInvalid)
	{
		bool result = false;
		for (int i = 0; i < 8; i++)
		{
			Account account = new Account();
			if (Storage.IsSlotValid(i, ident))
			{
				if (!Storage.LoadSlot(i, account, ident))
				{
					if (deleteInvalid)
					{
						Storage.ClearSlot(i);
					}
				}
				else
				{
					this.mAccounts[i] = account;
					result = true;
				}
			}
		}
		return result;
	}

	public Account NewTempAccount()
	{
		return this.NewAccount("temp");
	}

	public Account NewAccount(string username)
    {
		int key = this.FindFreeSlot();
		Account account = new Account();
		account.Username = username;
		account.CountryCode = GetCountryCode();
		mAccounts[key] = account;
		return account;
	}

	private string GetCountryCode()
	{
		return BasePlatform.ActivePlatform.GetCountryCode();
	}

	public Account Default()
	{
		return this.Default(false);
	}

	public Account Default(bool zExcludeGameCenterAccounts)
    {
		List<Account> list = new List<Account>(this.mAccounts.Values);
		list.RemoveAll((Account q) => q.IsTemporaryAccount);
		list.RemoveAll((Account q) => !string.IsNullOrEmpty(q.GCid) && zExcludeGameCenterAccounts);
		if (list.Count <= 0)
		{
			return null;
		}
		return list.MaxItem((Account q) => q.LastUsed);
	}

	public Account FindGameCenterAccount(string gcid, string[] alternatives)
    {
        List<Account> list = new List<Account>(this.mAccounts.Values);
		list.RemoveAll((Account q) => q.IsTemporaryAccount);
		list.RemoveAll((Account q) => string.IsNullOrEmpty(q.Username));
		if (BasePlatform.ActivePlatform.IsSupportedGooglePlayStore())
		{
			return this.FindGooglePlayGamesAccount(list, gcid, alternatives);
		}
		list.RemoveAll((Account q) => string.IsNullOrEmpty(q.GCid) || q.GCid != gcid);
		if (list.Count <= 0)
		{
			return null;
		}
		return list.MaxItem((Account q) => q.LastUsed);
	}

	public Account FindGooglePlayGamesAccount(List<Account> accounts, string gcid, string[] alternatives)
    {
        //Account account = accounts.Where((Account q) => !string.IsNullOrEmpty(q.GCid) && q.GCid == gcid)
        //    .OrderByDescending(q => q.LastUsed).FirstOrDefault();
        Account account = (from q in accounts
            where !string.IsNullOrEmpty(q.GCid) && q.GCid == gcid
            orderby q.LastUsed descending
            select q).FirstOrDefault();

		if (account != null)
		{
			return account;
		}
		List<Account> list = accounts.FindAll((Account q) => !string.IsNullOrEmpty(q.GCid)).ToList<Account>();
		list = (from q in list
		where alternatives.Contains(q.GCid)
		select q).ToList<Account>();
		if (list.Count <= 0)
		{
			return null;
		}
		list.Sort((Account x, Account y) => y.LastUsed.CompareTo(x.LastUsed));
		Account account2 = list.Find((Account q) => q.UserConverted);
		if (account2 != null)
		{
			return account2;
		}
		return list[0];
	}

	public void Clear(int slot)
	{
		Storage.ClearSlot(slot);
		this.mAccounts.Remove(slot);
	}

	public void ClearAll()
	{
		for (int i = 0; i < 8; i++)
		{
			this.Clear(i);
		}
	}

	public int FindFreeSlot()
	{
		for (int i = 0; i < 8; i++)
		{
			if (!this.mAccounts.ContainsKey(i))
			{
				return i;
			}
		}
		Account account = null;
		foreach (Account current in this.mAccounts.Values)
		{
			if (account == null || account.LastUsed >= current.LastUsed)
			{
				account = current;
			}
		}
		int num = this.FindSlot(account);
		this.Clear(num);
		return num;
	}

	public void Save(Account account)
	{
		int num = this.FindSlot(account);
		if (num < 0)
		{
			num = this.FindFreeSlot();
		}
		Storage.SaveSlot(num, account);
	}

	public int FindSlot(Account account)
	{
		foreach (int current in this.mAccounts.Keys)
		{
			if (account == this.mAccounts[current])
			{
				return current;
			}
		}
		return -1;
	}

	public int FindSlot(string username)
    {
		foreach (int current in this.mAccounts.Keys)
		{
			if (this.mAccounts[current].Username == username)
			{
				return current;
			}
		}
		return -1;
	}

	public Account Find(string zUsername)
	{
		int num = this.FindSlot(zUsername);
		return (num < 0) ? null : this.mAccounts[num];
	}

	public Account FindTemp()
	{
		return this.Find("temp");
	}
}
