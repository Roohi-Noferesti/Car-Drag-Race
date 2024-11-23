using LitJson;
using System;

public class CashSpentTransaction : PlayerProfileTransactionBase
{
	private int m_CashSpent;

	private ECashSpentReason m_Reason;

	public CashSpentTransaction() : base(ETransactionAction.CashSpent)
	{
	}

	public CashSpentTransaction(int cashSpent, ECashSpentReason reason) : base(ETransactionAction.CashSpent)
	{
		this.m_CashSpent = cashSpent;
		this.m_Reason = reason;
	}

	protected override void SerializeSpecificDataToJson(JsonDict dict)
	{
		dict.Set("increase", this.m_CashSpent);
		dict.SetEnum<ECashSpentReason>("reason", this.m_Reason);
	}

	protected override void SerializeSpecificDataFromJson(JsonDict dict)
	{
		this.m_CashSpent = dict.GetInt("increase");
		this.m_Reason = dict.GetEnum<ECashSpentReason>("reason");
	}

	public override string ToString()
	{
		return string.Format("Cash spent {0}, Reason {1}, Action {2}, Timestamp {3}", new object[]
		{
			this.m_CashSpent,
			this.m_Reason,
			this.m_Action,
			this.m_TimeStamp
		});
	}
}
