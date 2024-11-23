using LitJson;
using System;

public class GoldEarnedTransaction : PlayerProfileTransactionBase
{
	private int m_GoldEarned;

	private EGoldEarnedReason m_Reason;

	public GoldEarnedTransaction() : base(ETransactionAction.GoldEarned)
	{
	}

	public GoldEarnedTransaction(int goldEarned, EGoldEarnedReason reason) : base(ETransactionAction.GoldEarned)
	{
		this.m_GoldEarned = goldEarned;
		this.m_Reason = reason;
	}

	protected override void SerializeSpecificDataToJson(JsonDict dict)
	{
		dict.Set("increase", this.m_GoldEarned);
		dict.SetEnum<EGoldEarnedReason>("reason", this.m_Reason);
	}

	protected override void SerializeSpecificDataFromJson(JsonDict dict)
	{
		this.m_GoldEarned = dict.GetInt("increase");
		this.m_Reason = dict.GetEnum<EGoldEarnedReason>("reason");
	}

	public override string ToString()
	{
		return string.Format("Gold earned {0}, Reason {1}, Action {2}, Timestamp {3}", new object[]
		{
			this.m_GoldEarned,
			this.m_Reason,
			this.m_Action,
			this.m_TimeStamp
		});
	}
}
