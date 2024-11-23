using LitJson;
using System;

public class GoldSpentTransaction : PlayerProfileTransactionBase
{
	private int m_GoldSpent;

	private EGoldSpentReason m_Reason;

	public GoldSpentTransaction() : base(ETransactionAction.GoldSpent)
	{
	}

	public GoldSpentTransaction(int GoldSpent, EGoldSpentReason reason) : base(ETransactionAction.GoldSpent)
	{
		this.m_GoldSpent = GoldSpent;
		this.m_Reason = reason;
	}

	protected override void SerializeSpecificDataToJson(JsonDict dict)
	{
		dict.Set("increase", this.m_GoldSpent);
		dict.SetEnum<EGoldSpentReason>("reason", this.m_Reason);
	}

	protected override void SerializeSpecificDataFromJson(JsonDict dict)
	{
		this.m_GoldSpent = dict.GetInt("increase");
		this.m_Reason = dict.GetEnum<EGoldSpentReason>("reason");
	}

	public override string ToString()
	{
		return string.Format("Gold spent {0}, Reason {1}, Action {2}, Timestamp {3}", new object[]
		{
			this.m_GoldSpent,
			this.m_Reason,
			this.m_Action,
			this.m_TimeStamp
		});
	}
}
