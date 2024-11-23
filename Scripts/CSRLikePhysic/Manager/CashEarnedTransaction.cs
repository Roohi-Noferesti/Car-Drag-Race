using LitJson;
using System;

public class CashEarnedTransaction : PlayerProfileTransactionBase
{
	private int m_CashEarned;

	private int m_CashBonusEarned;

	private ECashEarnedReason m_Reason;

	public CashEarnedTransaction() : base(ETransactionAction.CashEarned)
	{
	}

	public CashEarnedTransaction(int cashEarned, int cashBonusEarned, ECashEarnedReason reason) : base(ETransactionAction.CashEarned)
	{
		this.m_CashEarned = cashEarned;
		this.m_CashBonusEarned = cashBonusEarned;
		this.m_Reason = reason;
	}

	protected override void SerializeSpecificDataToJson(JsonDict dict)
	{
		dict.Set("increase", this.m_CashEarned);
		if (this.m_CashBonusEarned > 0)
		{
			dict.Set("boost", this.m_CashBonusEarned);
		}
		dict.SetEnum<ECashEarnedReason>("reason", this.m_Reason);
	}

	protected override void SerializeSpecificDataFromJson(JsonDict dict)
	{
		this.m_CashEarned = dict.GetInt("increase");
		if (dict.ContainsKey("boost"))
		{
			this.m_CashBonusEarned = dict.GetInt("boost");
		}
		else
		{
			this.m_CashBonusEarned = 0;
		}
		this.m_Reason = dict.GetEnum<ECashEarnedReason>("reason");
	}

	public override string ToString()
	{
		return string.Format("Cash earned {0}, Cash earned Bonus {1}, Reason {2}, Action {3}, Timestamp {4}", new object[]
		{
			this.m_CashEarned,
			this.m_CashBonusEarned,
			this.m_Reason,
			this.m_Action,
			this.m_TimeStamp
		});
	}
}
