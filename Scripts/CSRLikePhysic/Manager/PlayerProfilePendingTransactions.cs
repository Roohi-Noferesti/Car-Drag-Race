using LitJson;
using System;
using System.Collections.Generic;

public class PlayerProfilePendingTransactions
{
	private List<PlayerProfileTransactionBase> m_TransactionList = new List<PlayerProfileTransactionBase>();

	public void AddTransaction(PlayerProfileTransactionBase transaction)
	{
		this.m_TransactionList.Add(transaction);
	}

	public void Save()
	{
		if (this.m_TransactionList.Count > 0)
		{
			JsonDict jsonDict = new JsonDict();
			this.SerializeToJsonDict(ref jsonDict, true);
			PlayerProfileFile.WriteActiveProfileFile(jsonDict.ToString(), EProfileFileType.transaction);
		}
		else
		{
			PlayerProfileFile.EraseTransactionFile();
		}
	}

	public void Load()
	{
		this.m_TransactionList.Clear();
		string empty = string.Empty;
		if (PlayerProfileFile.ReadActiveProfileFile(ref empty, EProfileFileType.transaction))
		{
			this.SerializeFromJsonStr(empty);
		}
	}

	public void ClearTransactions()
	{
		this.m_TransactionList.Clear();
		PlayerProfileFile.EraseTransactionFile();
	}

	public void ClearUploadedTransactions()
	{
		List<PlayerProfileTransactionBase> list = new List<PlayerProfileTransactionBase>();
		for (int i = 0; i < this.m_TransactionList.Count; i++)
		{
			if (this.m_TransactionList[i].IsUploading)
			{
				list.Add(this.m_TransactionList[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			this.m_TransactionList.Remove(list[j]);
		}
		this.Save();
	}

	public int GetTransactionCount()
	{
		return this.m_TransactionList.Count;
	}

	public void SerializeToJsonDict(ref JsonDict dict, bool includeUploadingState)
	{
		JsonList jsonList = new JsonList();
		for (int i = 0; i < this.m_TransactionList.Count; i++)
		{
			JsonDict jsonDict = new JsonDict();
			this.m_TransactionList[i].SerializeToJsonDict(jsonDict, includeUploadingState);
			jsonList.Add(jsonDict);
		}
		dict.Set("transactions", jsonList);
	}

	public void PrepareForTransactionUpload(ref JsonDict dict)
	{
		for (int i = 0; i < this.m_TransactionList.Count; i++)
		{
			this.m_TransactionList[i].IsUploading = true;
		}
		this.SerializeToJsonDict(ref dict, false);
		this.Save();
	}

	private void SerializeFromJsonStr(string jsonStr)
	{
		this.m_TransactionList.Clear();
		JsonDict jsonDict = new JsonDict();
		if (jsonDict.Read(jsonStr))
		{
			this.SerializeFromJsonDict(jsonDict);
		}
	}

	public void SerializeFromJsonDict(JsonDict dict)
	{
		this.m_TransactionList.Clear();
		JsonList jsonList = dict.GetJsonList("transactions");
		for (int i = 0; i < jsonList.Count; i++)
		{
			JsonDict jsonDict = jsonList.GetJsonDict(i);
			PlayerProfileTransactionBase playerProfileTransactionBase = PlayerProfileTransactionBase.CreateObjectFromJson(jsonDict);
			if (playerProfileTransactionBase != null)
			{
				this.m_TransactionList.Add(playerProfileTransactionBase);
			}
		}
	}

	public override string ToString()
	{
		string text = "Transactions:\n";
		for (int i = 0; i < this.m_TransactionList.Count; i++)
		{
			text = text + "{" + this.m_TransactionList[i].ToString() + "}\n";
		}
		return text;
	}
}
